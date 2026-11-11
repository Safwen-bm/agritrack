// Built with help from Claude
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../api";
import type { UpcomingTask } from "../types";

export default function TasksCard({ days = 14 }: { days?: number }) {
  const [tasks, setTasks] = useState<UpcomingTask[] | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    api<UpcomingTask[]>(`/api/tasks?days=${days}`)
      .then(setTasks)
      .catch((err) => setError(err.message));
  }, [days]);

  return (
    <section className="rounded-lg bg-white p-4 shadow">
      <h2 className="mb-2 font-semibold text-green-800">
        Suggested tasks (next {days} days)
      </h2>
      {error && <p className="text-sm text-red-700">{error}</p>}
      {tasks === null && !error && <p className="text-sm">Loading...</p>}
      {tasks?.length === 0 && (
        <p className="text-sm text-gray-600">
          No tasks. Tasks are suggested for plantings that are still growing.
        </p>
      )}
      {tasks && tasks.length > 0 && (
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b text-gray-600">
                <th className="py-1 pr-3">Date</th>
                <th className="py-1 pr-3">Task</th>
                <th className="py-1 pr-3">Crop</th>
                <th className="py-1">Stage</th>
              </tr>
            </thead>
            <tbody>
              {tasks.map((t, i) => (
                <tr
                  key={`${t.plantingId}-${t.date}-${i}`}
                  className="border-b last:border-0"
                >
                  <td className="py-1 pr-3">{t.date}</td>
                  <td className="py-1 pr-3">{t.title}</td>
                  <td className="py-1 pr-3">
                    <Link
                      to={`/plantings/${t.plantingId}`}
                      className="text-green-700 hover:underline"
                    >
                      {t.cropName}
                    </Link>{" "}
                    <span className="text-gray-500">({t.fieldName})</span>
                  </td>
                  <td className="py-1">{t.stage}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
