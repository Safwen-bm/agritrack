// Built with help from Claude
import { useEffect, useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { api } from "../services/api";
import ChartCard from "../components/ChartCard";
import type { DashboardData, ProfitGroup } from "../types";
import TasksCard from "../components/TasksCard";

const COLORS = [
  "#15803d",
  "#ca8a04",
  "#2563eb",
  "#dc2626",
  "#7c3aed",
  "#0891b2",
  "#ea580c",
];

const fmt = (n: number) =>
  n.toLocaleString(undefined, { maximumFractionDigits: 2 });

function StatCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg bg-white p-4 shadow">
      <p className="text-xs text-gray-600">{label}</p>
      <p className="text-xl font-bold text-green-800">{value}</p>
    </div>
  );
}

// Used twice: profit by crop and profit by season
function ProfitChart({ data }: { data: ProfitGroup[] }) {
  return (
    <ResponsiveContainer width="100%" height="100%">
      <BarChart data={data}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="label" />
        <YAxis />
        <Tooltip />
        <Legend />
        <Bar dataKey="expenses" name="Expenses" fill="#dc2626" />
        <Bar dataKey="revenue" name="Revenue" fill="#2563eb" />
        <Bar dataKey="profit" name="Profit" fill="#15803d" />
      </BarChart>
    </ResponsiveContainer>
  );
}

export default function Dashboard() {
  const [data, setData] = useState<DashboardData | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    api<DashboardData>("/api/dashboard")
      .then(setData)
      .catch((err) => setError(err.message));
  }, []);

  if (error)
    return (
      <p className="rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>
    );
  if (!data) return <p>Loading...</p>;

  const s = data.summary;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-green-800">Dashboard</h1>

      <div className="grid grid-cols-2 gap-3 md:grid-cols-4">
        <StatCard label="Farms" value={String(s.farmCount)} />
        <StatCard label="Fields" value={String(s.fieldCount)} />
        <StatCard label="Total area" value={`${fmt(s.totalAreaHectares)} ha`} />
        <StatCard label="Harvested" value={`${fmt(s.totalHarvestKg)} kg`} />
        <StatCard label="Revenue" value={fmt(s.totalRevenue)} />
        <StatCard label="Expenses" value={fmt(s.totalExpenses)} />
        <StatCard label="Net profit" value={fmt(s.netProfit)} />
        <StatCard label="Water used" value={`${fmt(s.totalWaterM3)} m³`} />
      </div>

      <TasksCard />

      <div className="grid gap-4 md:grid-cols-2">
        <ChartCard
          title="Profit by crop"
          empty={data.profitByCrop.length === 0}
        >
          <ProfitChart data={data.profitByCrop} />
        </ChartCard>

        <ChartCard
          title="Profit by season (planting year)"
          empty={data.profitBySeason.length === 0}
        >
          <ProfitChart data={data.profitBySeason} />
        </ChartCard>

        <ChartCard
          title="Expenses by category"
          empty={data.expensesByCategory.length === 0}
        >
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                data={data.expensesByCategory}
                dataKey="amount"
                nameKey="category"
                outerRadius={90}
                label
              >
                {data.expensesByCategory.map((_, i) => (
                  <Cell key={i} fill={COLORS[i % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard
          title="Water consumption by month (m³)"
          empty={data.waterByMonth.length === 0}
        >
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={data.waterByMonth}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="month" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="volumeM3" name="Water (m³)" fill="#2563eb" />
            </BarChart>
          </ResponsiveContainer>
        </ChartCard>

        <div className="md:col-span-2">
          <ChartCard
            title="Yield per hectare (kg/ha)"
            empty={data.yields.length === 0}
          >
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={data.yields}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="label" />
                <YAxis />
                <Tooltip />
                <Bar
                  dataKey="yieldKgPerHectare"
                  name="kg / ha"
                  fill="#15803d"
                />
              </BarChart>
            </ResponsiveContainer>
          </ChartCard>
        </div>
      </div>
    </div>
  );
}
