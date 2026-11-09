// Built with help from Claude
import { useEffect, useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { api } from "../api";
import type { Farm } from "../types";

export default function Farms() {
  const [farms, setFarms] = useState<Farm[] | null>(null); // null = still loading
  const [error, setError] = useState("");
  const [name, setName] = useState("");
  const [location, setLocation] = useState("");

  // Load the farms once, when the page opens
  useEffect(() => {
    api<Farm[]>("/api/farms")
      .then(setFarms)
      .catch((err) => setError(err.message));
  }, []);

  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setError("");
    try {
      const created = await api<Farm>("/api/farms", {
        method: "POST",
        body: JSON.stringify({ name, location: location || null }),
      });
      setFarms([...(farms ?? []), created]);
      setName("");
      setLocation("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not create farm");
    }
  }

  async function handleDelete(farm: Farm) {
    if (!confirm(`Delete "${farm.name}" and everything inside it?`)) return;
    try {
      await api<void>(`/api/farms/${farm.id}`, { method: "DELETE" });
      setFarms((farms ?? []).filter((f) => f.id !== farm.id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not delete farm");
    }
  }

  return (
    <div>
      <h1 className="mb-4 text-2xl font-bold text-green-800">My farms</h1>
      {error && (
        <p className="mb-3 rounded bg-red-100 p-2 text-sm text-red-700">
          {error}
        </p>
      )}

      <form
        onSubmit={handleCreate}
        className="mb-6 flex flex-col gap-2 rounded-lg bg-white p-4 shadow sm:flex-row"
      >
        <input
          required
          maxLength={100}
          placeholder="Farm name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="flex-1 rounded border p-2"
        />
        <input
          maxLength={200}
          placeholder="Location (optional)"
          value={location}
          onChange={(e) => setLocation(e.target.value)}
          className="flex-1 rounded border p-2"
        />
        <button className="rounded bg-green-700 px-4 py-2 text-white hover:bg-green-600">
          Add farm
        </button>
      </form>

      {farms === null && !error && <p>Loading...</p>}
      {farms?.length === 0 && (
        <p className="text-gray-600">No farms yet. Add your first one above.</p>
      )}

      <div className="grid gap-4 sm:grid-cols-2">
        {farms?.map((farm) => (
          <div key={farm.id} className="rounded-lg bg-white p-4 shadow">
            <h2 className="text-lg font-semibold">
              <Link
                to={`/farms/${farm.id}`}
                className="text-green-800 hover:underline"
              >
                {farm.name}
              </Link>
            </h2>
            <p className="text-sm text-gray-600">
              {farm.location ?? "No location"}
            </p>
            <p className="mt-2 text-sm">
              {farm.fieldCount} field(s) · {farm.totalAreaHectares} ha
            </p>
            <button
              onClick={() => handleDelete(farm)}
              className="mt-3 text-sm text-red-600 hover:underline"
            >
              Delete
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}
