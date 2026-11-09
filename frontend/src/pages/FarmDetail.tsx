// Built with help from Claude
import { useEffect, useState, type FormEvent } from "react";
import { Link, useParams } from "react-router-dom";
import { api } from "../api";
import RecordSection from "../components/RecordSection";
import type { Expense, Farm, FieldItem } from "../types";

export default function FarmDetail() {
  const { farmId } = useParams();
  const [farm, setFarm] = useState<Farm | null>(null);
  const [fields, setFields] = useState<FieldItem[]>([]);
  const [error, setError] = useState("");
  const [form, setForm] = useState({
    name: "",
    area: "",
    soil: "",
    lat: "",
    lng: "",
  });

  useEffect(() => {
    api<Farm>(`/api/farms/${farmId}`)
      .then(setFarm)
      .catch((err) => setError(err.message));
    api<FieldItem[]>(`/api/farms/${farmId}/fields`)
      .then(setFields)
      .catch((err) => setError(err.message));
  }, [farmId]);

  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setError("");
    try {
      const created = await api<FieldItem>(`/api/farms/${farmId}/fields`, {
        method: "POST",
        body: JSON.stringify({
          name: form.name,
          areaHectares: Number(form.area),
          soilType: form.soil || null,
          latitude: form.lat === "" ? null : Number(form.lat),
          longitude: form.lng === "" ? null : Number(form.lng),
        }),
      });
      setFields([...fields, created]);
      setForm({ name: "", area: "", soil: "", lat: "", lng: "" });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not create field");
    }
  }

  async function handleDelete(field: FieldItem) {
    if (!confirm(`Delete field "${field.name}" and its plantings?`)) return;
    try {
      await api<void>(`/api/fields/${field.id}`, { method: "DELETE" });
      setFields(fields.filter((f) => f.id !== field.id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not delete field");
    }
  }

  const input = "rounded border p-2";

  return (
    <div className="space-y-6">
      <div>
        <Link to="/farms" className="text-sm text-green-700 underline">
          ← All farms
        </Link>
        <h1 className="text-2xl font-bold text-green-800">
          {farm?.name ?? "Farm"}
        </h1>
        <p className="text-sm text-gray-600">{farm?.location}</p>
      </div>
      {error && (
        <p className="rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>
      )}

      <section className="rounded-lg bg-white p-4 shadow">
        <h2 className="mb-3 text-lg font-semibold text-green-800">Fields</h2>
        <form
          onSubmit={handleCreate}
          className="mb-4 grid gap-2 sm:grid-cols-6"
        >
          <input
            required
            placeholder="Name"
            value={form.name}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
            className={`${input} sm:col-span-2`}
          />
          <input
            required
            type="number"
            step="0.01"
            min="0.01"
            placeholder="Area (ha)"
            value={form.area}
            onChange={(e) => setForm({ ...form, area: e.target.value })}
            className={input}
          />
          <input
            placeholder="Soil type"
            value={form.soil}
            onChange={(e) => setForm({ ...form, soil: e.target.value })}
            className={input}
          />
          <input
            type="number"
            step="any"
            placeholder="Latitude"
            value={form.lat}
            onChange={(e) => setForm({ ...form, lat: e.target.value })}
            className={input}
          />
          <input
            type="number"
            step="any"
            placeholder="Longitude"
            value={form.lng}
            onChange={(e) => setForm({ ...form, lng: e.target.value })}
            className={input}
          />
          <button className="rounded bg-green-700 px-4 py-2 text-white hover:bg-green-600 sm:col-span-6">
            Add field
          </button>
        </form>

        {fields.length === 0 && (
          <p className="text-sm text-gray-600">No fields yet.</p>
        )}
        <div className="grid gap-3 sm:grid-cols-2">
          {fields.map((f) => (
            <div key={f.id} className="rounded border p-3">
              <Link
                to={`/fields/${f.id}`}
                className="font-semibold text-green-800 hover:underline"
              >
                {f.name}
              </Link>
              <p className="text-sm text-gray-600">
                {f.areaHectares} ha · {f.soilType ?? "soil not set"}
              </p>
              <button
                onClick={() => handleDelete(f)}
                className="mt-2 text-sm text-red-600 hover:underline"
              >
                Delete
              </button>
            </div>
          ))}
        </div>
      </section>

      <RecordSection<Expense>
        title="All farm expenses"
        listPath={`/api/farms/${farmId}/expenses`}
        createPath={`/api/farms/${farmId}/expenses`}
        deletePath={(id) => `/api/expenses/${id}`}
        fields={[
          { name: "category", label: "Category", type: "text", required: true },
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
          {
            label: "Planting",
            render: (e) => (e.plantingId ? `#${e.plantingId}` : "Farm-wide"),
          },
        ]}
      />
    </div>
  );
}
