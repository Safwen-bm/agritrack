// Built with help from Claude
import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  CircleMarker,
  MapContainer,
  Popup,
  TileLayer,
  useMap,
} from "react-leaflet";
import "leaflet/dist/leaflet.css";
import { api } from "../api";
import type { MapPoint } from "../types";

// Zooms the map so every field is visible
function FitBounds({ points }: { points: [number, number][] }) {
  const map = useMap();
  useEffect(() => {
    if (points.length === 1) map.setView(points[0], 13);
    else if (points.length > 1) map.fitBounds(points, { padding: [40, 40] });
  }, [map, points]);
  return null;
}

export default function MapPage() {
  const [fields, setFields] = useState<MapPoint[] | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    api<MapPoint[]>("/api/map/fields")
      .then(setFields)
      .catch((err) => setError(err.message));
  }, []);

  // useMemo keeps the same array between renders, so FitBounds only runs when data changes
  const points = useMemo(
    () =>
      (fields ?? []).map((f) => [f.latitude, f.longitude] as [number, number]),
    [fields],
  );

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold text-green-800">Field map</h1>
      {error && (
        <p className="rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>
      )}
      {fields?.length === 0 && (
        <p className="rounded bg-white p-3 text-sm text-gray-600 shadow">
          No fields with coordinates yet. Add a latitude and longitude when you
          create a field.
        </p>
      )}

      <div className="h-112 overflow-hidden rounded-lg shadow">
        <MapContainer center={[34, 9]} zoom={6} className="h-full w-full">
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <FitBounds points={points} />
          {fields?.map((f) => (
            <CircleMarker
              key={f.id}
              center={[f.latitude, f.longitude]}
              radius={10}
              pathOptions={{
                color: "#166534",
                fillColor: "#16a34a",
                fillOpacity: 0.7,
              }}
            >
              <Popup>
                <Link to={`/fields/${f.id}`} className="font-semibold">
                  {f.name}
                </Link>
                <br />
                {f.farmName} · {f.areaHectares} ha
                <br />
                {f.currentCrops.length > 0
                  ? `Growing: ${f.currentCrops.join(", ")}`
                  : "Nothing growing"}
              </Popup>
            </CircleMarker>
          ))}
        </MapContainer>
      </div>
    </div>
  );
}
