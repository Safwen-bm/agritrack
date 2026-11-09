// Built with help from Claude
import type { ReactNode } from "react";

// A white card with a title, a fixed-height chart area and an empty state
export default function ChartCard({
  title,
  empty,
  children,
}: {
  title: string;
  empty: boolean;
  children: ReactNode;
}) {
  return (
    <section className="rounded-lg bg-white p-4 shadow">
      <h2 className="mb-2 font-semibold text-green-800">{title}</h2>
      {empty ? (
        <p className="py-10 text-center text-sm text-gray-500">No data yet.</p>
      ) : (
        <div className="h-64">{children}</div>
      )}
    </section>
  );
}
