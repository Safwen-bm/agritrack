// Built with help from Claude
import { useEffect, useState, type FormEvent } from "react";
import { Link, useParams } from "react-router-dom";
import { api } from "../services/api";
import type { CropType, FieldItem, Planting } from "../types";
import WeatherCard from "../components/WeatherCard";

export default function FieldDetail() {
  const { fieldId } = useParams();
  const [field, setField] = useState<FieldItem | null>(null);
  const [plantings, setPlantings] = useState<Planting[]>([]);
  const [crops, setCrops] = useState<CropType[]>([]);
  const [error, setError] = useState("");
  const [cropTypeId, setCropTypeId] = useState("");
  const [plantingDate, setPlantingDate] = useState("");
  const [harvestDate, setHarvestDate] = useState("");

  useEffect(() => {
    api<FieldItem>(`/api/fields/${fieldId}`)
      .then(setField)
      .catch((err) => setError(err.message));
    api<Planting[]>(`/api/fields/${fieldId}/plantings`)
      .then(setPlantings)
      .catch((err) => setError(err.message));
    api<CropType[]>("/api/crop-types")
      .then(setCrops)
      .catch((err) => setError(err.message));
  }, [fieldId]);

  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setError("");
    try {
      const created = await api<Planting>(`/api/fields/${fieldId}/plantings`, {
        method: "POST",
        body: JSON.stringify({
          cropTypeId: Number(cropTypeId),
          plantingDate,
          expectedHarvestDate: harvestDate || null, // empty: the server calculates it
        }),
      });
      setPlantings([created, ...plantings]);
      setPlantingDate("");
      setHarvestDate("");
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Could not create planting",
      );
    }
  }

  async function handleDelete(p: Planting) {
    if (!confirm(`Delete this ${p.cropName} planting and its records?`)) return;
    try {
      await api<void>(`/api/plantings/${p.id}`, { method: "DELETE" });
      setPlantings(plantings.filter((x) => x.id !== p.id));
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Could not delete planting",
      );
    }
  }

  return (
    <div className="space-y-6">
      <div>
        <Link
          to={field ? `/farms/${field.farmId}` : "/farms"}
          className="text-sm text-green-700 underline"
        >
          ← Back to farm
        </Link>
        <h1 className="text-2xl font-bold text-green-800">
          {field?.name ?? "Field"}
        </h1>
        <p className="text-sm text-gray-600">
          {field?.areaHectares} ha · {field?.soilType ?? "soil not set"}
        </p>
      </div>
      {error && (
        <p className="rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>
      )}

      <section className="rounded-lg bg-white p-4 shadow">
        <h2 className="mb-3 text-lg font-semibold text-green-800">
          Plant a crop
        </h2>
        <form
          onSubmit={handleCreate}
          className="flex flex-wrap items-end gap-2"
        >
          <label className="flex flex-col text-xs text-gray-600">
            Crop
            <select
              required
              value={cropTypeId}
              onChange={(e) => setCropTypeId(e.target.value)}
              className="rounded border p-2 text-sm text-black"
            >
              <option value="">Choose...</option>
              {crops.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
          <label className="flex flex-col text-xs text-gray-600">
            Planting date
            <input
              required
              type="date"
              value={plantingDate}
              onChange={(e) => setPlantingDate(e.target.value)}
              className="rounded border p-2 text-sm text-black"
            />
          </label>
          <label className="flex flex-col text-xs text-gray-600">
            Expected harvest (optional)
            <input
              type="date"
              value={harvestDate}
              onChange={(e) => setHarvestDate(e.target.value)}
              className="rounded border p-2 text-sm text-black"
            />
          </label>
          <button className="rounded bg-green-700 px-4 py-2 text-white hover:bg-green-600">
            Add planting
          </button>
        </form>
        <p className="mt-2 text-xs text-gray-500">
          If you leave the harvest date empty, it is planting date + the
          crop&apos;s growth days.
        </p>
      </section>

      <section>
        <h2 className="mb-3 text-lg font-semibold text-green-800">Plantings</h2>
        {plantings.length === 0 && (
          <p className="text-sm text-gray-600">No plantings yet.</p>
        )}
        <div className="grid gap-3 sm:grid-cols-2">
          {plantings.map((p) => (
            <div key={p.id} className="rounded-lg bg-white p-4 shadow">
              <Link
                to={`/plantings/${p.id}`}
                className="font-semibold text-green-800 hover:underline"
              >
                {p.cropName}
              </Link>
              <span className="ml-2 rounded bg-green-100 px-2 py-0.5 text-xs">
                {p.status}
              </span>
              <p className="text-sm text-gray-600">
                Planted {p.plantingDate} · harvest{" "}
                {p.expectedHarvestDate ?? "-"}
              </p>
              <button
                onClick={() => handleDelete(p)}
                className="mt-2 text-sm text-red-600 hover:underline"
              >
                Delete
              </button>
            </div>
          ))}
        </div>
      </section>
      {fieldId && <WeatherCard fieldId={fieldId} />}
    </div>
  );
}
