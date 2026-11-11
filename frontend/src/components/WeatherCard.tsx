// Built with help from Claude
import { useEffect, useState } from "react";
import { api } from "../services/api";
import type { WeatherResponse } from "../types";

export default function WeatherCard({ fieldId }: { fieldId: string }) {
  const [data, setData] = useState<WeatherResponse | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    setData(null);
    setError("");
    api<WeatherResponse>(`/api/fields/${fieldId}/weather`)
      .then(setData)
      .catch((err) => setError(err.message));
  }, [fieldId]);

  return (
    <section className="rounded-lg bg-white p-4 shadow">
      <h2 className="mb-2 text-lg font-semibold text-green-800">
        Weather forecast
      </h2>
      {error && <p className="text-sm text-gray-600">{error}</p>}
      {!data && !error && <p className="text-sm">Loading...</p>}

      {data && (
        <>
          {data.warnings.length === 0 ? (
            <p className="mb-3 rounded bg-green-50 p-2 text-sm text-green-800">
              No weather warnings for your scheduled activities.
            </p>
          ) : (
            <ul className="mb-3 space-y-1">
              {data.warnings.map((w, i) => (
                <li
                  key={i}
                  className={`rounded p-2 text-sm ${
                    w.severity === "High"
                      ? "bg-red-100 text-red-800"
                      : "bg-orange-100 text-orange-800"
                  }`}
                >
                  <strong>{w.date}:</strong> {w.message}
                </li>
              ))}
            </ul>
          )}

          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead>
                <tr className="border-b text-gray-600">
                  <th className="py-1 pr-3">Date</th>
                  <th className="py-1 pr-3">Max °C</th>
                  <th className="py-1 pr-3">Min °C</th>
                  <th className="py-1 pr-3">Rain (mm)</th>
                  <th className="py-1">Wind (km/h)</th>
                </tr>
              </thead>
              <tbody>
                {data.forecast.map((d) => (
                  <tr key={d.date} className="border-b last:border-0">
                    <td className="py-1 pr-3">{d.date}</td>
                    <td className="py-1 pr-3">{d.tempMaxC}</td>
                    <td className="py-1 pr-3">{d.tempMinC}</td>
                    <td className="py-1 pr-3">{d.precipitationMm}</td>
                    <td className="py-1">{d.windMaxKmh}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="mt-2 text-xs text-gray-500">
            Forecast data: Open-Meteo.com
          </p>
        </>
      )}
    </section>
  );
}
