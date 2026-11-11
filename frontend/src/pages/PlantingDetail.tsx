// Built with help from Claude
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { api } from "../services/api";
import Calculators from "../components/Calculators";
import RecordSection from "../components/RecordSection";
import type {
  Expense,
  FieldItem,
  Harvest,
  Irrigation,
  Planting,
  Revenue,
} from "../types";

export default function PlantingDetail() {
  const { plantingId } = useParams();
  const [planting, setPlanting] = useState<Planting | null>(null);
  const [farmId, setFarmId] = useState<number | null>(null);
  const [error, setError] = useState("");
  const [refreshKey, setRefreshKey] = useState(0); // bump it to reload the calculators

  useEffect(() => {
    api<Planting>(`/api/plantings/${plantingId}`)
      .then((p) => {
        setPlanting(p);
        // The field tells us which farm this planting belongs to (needed for expenses)
        return api<FieldItem>(`/api/fields/${p.fieldId}`);
      })
      .then((field) => setFarmId(field.farmId))
      .catch((err) => setError(err.message));
  }, [plantingId]);

  const refresh = () => setRefreshKey((k) => k + 1);

  async function toggleStatus() {
    if (!planting) return;
    const status = planting.status === "Growing" ? "Harvested" : "Growing";
    try {
      await api<void>(`/api/plantings/${planting.id}`, {
        method: "PUT",
        body: JSON.stringify({
          cropTypeId: planting.cropTypeId,
          plantingDate: planting.plantingDate,
          expectedHarvestDate: planting.expectedHarvestDate,
          status,
        }),
      });
      setPlanting({ ...planting, status });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not update status");
    }
  }

  if (error && !planting)
    return (
      <p className="rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>
    );
  if (!planting) return <p>Loading...</p>;

  const id = planting.id;

  return (
    <div className="space-y-6">
      <div>
        <Link
          to={`/fields/${planting.fieldId}`}
          className="text-sm text-green-700 underline"
        >
          ← Back to field
        </Link>
        <div className="flex flex-wrap items-center gap-3">
          <h1 className="text-2xl font-bold text-green-800">
            {planting.cropName}
          </h1>
          <span className="rounded bg-green-100 px-2 py-0.5 text-sm">
            {planting.status}
          </span>
          <button
            onClick={toggleStatus}
            className="text-sm text-green-700 underline"
          >
            Mark as {planting.status === "Growing" ? "harvested" : "growing"}
          </button>
        </div>
        <p className="text-sm text-gray-600">
          Planted {planting.plantingDate} · expected harvest{" "}
          {planting.expectedHarvestDate ?? "-"}
        </p>
      </div>
      {error && (
        <p className="rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>
      )}

      <Calculators plantingId={id} refreshKey={refreshKey} />

      <RecordSection<Harvest>
        title="Harvests"
        listPath={`/api/plantings/${id}/harvests`}
        createPath={`/api/plantings/${id}/harvests`}
        deletePath={(hid) => `/api/harvests/${hid}`}
        fields={[
          { name: "harvestDate", label: "Date", type: "date", required: true },
          {
            name: "quantityKg",
            label: "Quantity (kg)",
            type: "number",
            required: true,
            step: "0.01",
          },
        ]}
        columns={[
          { label: "Date", render: (h) => h.harvestDate },
          { label: "Quantity (kg)", render: (h) => h.quantityKg },
        ]}
        onChange={refresh}
      />

      <RecordSection<Irrigation>
        title="Irrigation"
        listPath={`/api/plantings/${id}/irrigation`}
        createPath={`/api/plantings/${id}/irrigation`}
        deletePath={(iid) => `/api/irrigation/${iid}`}
        fields={[
          { name: "date", label: "Date", type: "date", required: true },
          {
            name: "volumeM3",
            label: "Volume (m³)",
            type: "number",
            required: true,
            step: "0.01",
          },
        ]}
        columns={[
          { label: "Date", render: (i) => i.date },
          { label: "Volume (m³)", render: (i) => i.volumeM3 },
        ]}
        onChange={refresh}
      />

      <RecordSection<Revenue>
        title="Revenues"
        listPath={`/api/plantings/${id}/revenues`}
        createPath={`/api/plantings/${id}/revenues`}
        deletePath={(rid) => `/api/revenues/${rid}`}
        fields={[
          { name: "date", label: "Date", type: "date", required: true },
          {
            name: "amount",
            label: "Amount",
            type: "number",
            required: true,
            step: "0.01",
          },
          { name: "description", label: "Description", type: "text" },
        ]}
        columns={[
          { label: "Date", render: (r) => r.date },
          { label: "Amount", render: (r) => r.amount },
          { label: "Description", render: (r) => r.description ?? "" },
        ]}
        onChange={refresh}
      />

      {farmId !== null && (
        <RecordSection<Expense>
          title="Expenses for this planting"
          listPath={`/api/farms/${farmId}/expenses?plantingId=${id}`}
          createPath={`/api/farms/${farmId}/expenses`}
          deletePath={(eid) => `/api/expenses/${eid}`}
          extraBody={{ plantingId: id }}
          fields={[
            {
              name: "category",
              label: "Category",
              type: "text",
              required: true,
            },
            {
              name: "amount",
              label: "Amount",
              type: "number",
              required: true,
              step: "0.01",
            },
            { name: "date", label: "Date", type: "date", required: true },
            { name: "description", label: "Description", type: "text" },
          ]}
          columns={[
            { label: "Date", render: (e) => e.date },
            { label: "Category", render: (e) => e.category },
            { label: "Amount", render: (e) => e.amount },
          ]}
          onChange={refresh}
        />
      )}
    </div>
  );
}
