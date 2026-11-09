// Built with help from Claude
import { useEffect, useState } from "react";
import { api } from "../api";
import type {
  IrrigationEstimate,
  Profitability,
  YieldResponse,
} from "../types";

const fmt = (n: number | null | undefined) =>
  n === null || n === undefined
    ? "-"
    : n.toLocaleString(undefined, { maximumFractionDigits: 2 });

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex justify-between py-0.5 text-sm">
      <span className="text-gray-600">{label}</span>
      <span className="font-medium">{value}</span>
    </div>
  );
}

export default function Calculators({
  plantingId,
  refreshKey,
}: {
  plantingId: number;
  refreshKey: number;
}) {
  const [profit, setProfit] = useState<Profitability | null>(null);
  const [yieldData, setYieldData] = useState<YieldResponse | null>(null);
  const [irrigation, setIrrigation] = useState<IrrigationEstimate | null>(null);
  const [rainfall, setRainfall] = useState("0");
  const [efficiency, setEfficiency] = useState("0.75");
  const [error, setError] = useState("");

  useEffect(() => {
    api<Profitability>(`/api/plantings/${plantingId}/profitability`)
      .then(setProfit)
      .catch((err) => setError(err.message));
    api<YieldResponse>(`/api/plantings/${plantingId}/yield-estimate`)
      .then(setYieldData)
      .catch((err) => setError(err.message));
  }, [plantingId, refreshKey]);

  useEffect(() => {
    const rain = rainfall === "" ? 0 : Number(rainfall);
    api<IrrigationEstimate>(
      `/api/plantings/${plantingId}/irrigation-estimate?rainfallMm=${rain}&efficiency=${efficiency}`,
    )
      .then((data) => {
        setIrrigation(data);
        setError("");
      })
      .catch((err) => setError(err.message));
  }, [plantingId, refreshKey, rainfall, efficiency]);

  const diff = irrigation?.differenceM3 ?? 0;

  return (
    <div>
      {error && (
        <p className="mb-3 rounded bg-red-100 p-2 text-sm text-red-700">
          {error}
        </p>
      )}
      <div className="grid gap-4 md:grid-cols-3">
        <section className="rounded-lg bg-white p-4 shadow">
          <h2 className="mb-2 font-semibold text-green-800">Profitability</h2>
          {profit && (
            <>
              <Row label="Expenses" value={fmt(profit.totalExpenses)} />
              <Row label="Revenue" value={fmt(profit.totalRevenue)} />
              <Row label="Net profit" value={fmt(profit.netProfit)} />
              <Row
                label="Margin"
                value={
                  profit.profitMarginPercent === null
                    ? "-"
                    : `${fmt(profit.profitMarginPercent)} %`
                }
              />
              <Row label="Cost / hectare" value={fmt(profit.costPerHectare)} />
              <Row label="Cost / kg" value={fmt(profit.costPerKg)} />
            </>
          )}
        </section>

        <section className="rounded-lg bg-white p-4 shadow">
          <h2 className="mb-2 font-semibold text-green-800">Yield estimate</h2>
          {yieldData && (
            <>
              <Row
                label="Estimated / ha"
                value={`${fmt(yieldData.estimate.estimatedYieldKgPerHectare)} kg`}
              />
              <Row
                label="Estimated total"
                value={`${fmt(yieldData.estimate.estimatedTotalKg)} kg`}
              />
              <Row
                label="Actual harvest"
                value={`${fmt(yieldData.actualKg)} kg`}
              />
              <Row
                label="Difference"
                value={`${fmt(yieldData.comparison.differenceKg)} kg (${fmt(yieldData.comparison.differencePercent)} %)`}
              />
              <p className="mt-2 text-xs text-gray-500">
                Based on {yieldData.estimate.historyCount} past harvest(s) of
                this crop. Trust in your history:{" "}
                {Math.round(yieldData.estimate.historyWeight * 100)} %.
              </p>
            </>
          )}
        </section>

        <section className="rounded-lg bg-white p-4 shadow">
          <h2 className="mb-2 font-semibold text-green-800">Irrigation</h2>
          <div className="mb-2 flex gap-2 text-xs text-gray-600">
            <label className="flex flex-col">
              Rainfall (mm)
              <input
                type="number"
                min="0"
                value={rainfall}
                onChange={(e) => setRainfall(e.target.value)}
                className="w-24 rounded border p-1 text-sm text-black"
              />
            </label>
            <label className="flex flex-col">
              System
              <select
                value={efficiency}
                onChange={(e) => setEfficiency(e.target.value)}
                className="rounded border p-1 text-sm text-black"
              >
                <option value="0.9">Drip (90%)</option>
                <option value="0.75">Sprinkler (75%)</option>
                <option value="0.6">Surface (60%)</option>
              </select>
            </label>
          </div>
          {irrigation && (
            <>
              <Row
                label="Net need"
                value={`${fmt(irrigation.netRequirementMm)} mm`}
              />
              <Row
                label="Recommended"
                value={`${fmt(irrigation.recommendedVolumeM3)} m³`}
              />
              <Row
                label="Applied"
                value={`${fmt(irrigation.appliedVolumeM3)} m³`}
              />
              <p
                className={`mt-2 text-sm font-medium ${diff > 0 ? "text-orange-600" : "text-green-700"}`}
              >
                {diff > 0
                  ? `Over-irrigated by ${fmt(diff)} m³ (possible saving)`
                  : `Under the recommended volume by ${fmt(-diff)} m³`}
              </p>
            </>
          )}
        </section>
      </div>
    </div>
  );
}
