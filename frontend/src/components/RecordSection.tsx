// Built with help from Claude
import { useEffect, useState, type FormEvent, type ReactNode } from "react";
import { api } from "../services/api";

export interface FieldDef {
  name: string;
  label: string;
  type: "number" | "date" | "text";
  required?: boolean;
  step?: string;
}

export interface Column<T> {
  label: string;
  render: (item: T) => ReactNode;
}

interface Props<T extends { id: number }> {
  title: string;
  listPath: string;
  createPath: string;
  deletePath: (id: number) => string;
  fields: FieldDef[];
  columns: Column<T>[];
  extraBody?: Record<string, unknown>; // fixed values sent with every new record
  onChange?: () => void; // called after add/delete so calculators can refresh
}

export default function RecordSection<T extends { id: number }>({
  title,
  listPath,
  createPath,
  deletePath,
  fields,
  columns,
  extraBody,
  onChange,
}: Props<T>) {
  const [items, setItems] = useState<T[] | null>(null);
  const [values, setValues] = useState<Record<string, string>>({});
  const [error, setError] = useState("");

  useEffect(() => {
    api<T[]>(listPath)
      .then(setItems)
      .catch((err) => setError(err.message));
  }, [listPath]);

  async function handleAdd(e: FormEvent) {
    e.preventDefault();
    setError("");

    // Build the JSON body: numbers as numbers, empty optional text as null
    const body: Record<string, unknown> = { ...extraBody };
    for (const f of fields) {
      const v = values[f.name] ?? "";
      body[f.name] = f.type === "number" ? Number(v) : v === "" ? null : v;
    }

    try {
      const created = await api<T>(createPath, {
        method: "POST",
        body: JSON.stringify(body),
      });
      setItems([created, ...(items ?? [])]);
      setValues({});
      onChange?.();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not save");
    }
  }

  async function handleDelete(id: number) {
    if (!confirm("Delete this record?")) return;
    try {
      await api<void>(deletePath(id), { method: "DELETE" });
      setItems((items ?? []).filter((i) => i.id !== id));
      onChange?.();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not delete");
    }
  }

  return (
    <section className="rounded-lg bg-white p-4 shadow">
      <h2 className="mb-3 text-lg font-semibold text-green-800">{title}</h2>
      {error && (
        <p className="mb-3 rounded bg-red-100 p-2 text-sm text-red-700">
          {error}
        </p>
      )}

      <form
        onSubmit={handleAdd}
        className="mb-4 flex flex-wrap items-end gap-2"
      >
        {fields.map((f) => (
          <label key={f.name} className="flex flex-col text-xs text-gray-600">
            {f.label}
            <input
              type={f.type}
              step={f.step}
              required={f.required}
              value={values[f.name] ?? ""}
              onChange={(e) =>
                setValues({ ...values, [f.name]: e.target.value })
              }
              className="w-36 rounded border p-1.5 text-sm text-black"
            />
          </label>
        ))}
        <button className="rounded bg-green-700 px-3 py-1.5 text-sm text-white hover:bg-green-600">
          Add
        </button>
      </form>

      {items === null && !error && <p className="text-sm">Loading...</p>}
      {items?.length === 0 && (
        <p className="text-sm text-gray-600">Nothing recorded yet.</p>
      )}

      {items && items.length > 0 && (
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b text-gray-600">
                {columns.map((c) => (
                  <th key={c.label} className="py-1 pr-3">
                    {c.label}
                  </th>
                ))}
                <th />
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id} className="border-b last:border-0">
                  {columns.map((c) => (
                    <td key={c.label} className="py-1 pr-3">
                      {c.render(item)}
                    </td>
                  ))}
                  <td className="py-1 text-right">
                    <button
                      onClick={() => handleDelete(item.id)}
                      className="text-red-600 hover:underline"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
