// Built with help from Claude
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const modules = [
  {
    icon: "🌾",
    title: "Farms and fields",
    text: "Organize your farms into fields with area, soil type and GPS location, and see them on a map.",
  },
  {
    icon: "🌱",
    title: "Crops and plantings",
    text: "Record every planting, keep the history of each field and track expected harvest dates.",
  },
  {
    icon: "💧",
    title: "Irrigation",
    text: "Log water use and compare it with the volume your crop actually needs.",
  },
  {
    icon: "💰",
    title: "Finance",
    text: "Record expenses and revenue and see profit per crop and per season.",
  },
  {
    icon: "📊",
    title: "Analytics",
    text: "Charts for profit, expenses, water consumption and yield per hectare.",
  },
  {
    icon: "🌦️",
    title: "Weather and tasks",
    text: "A 7-day forecast and suggested tasks, with warnings when the weather could affect them.",
  },
];

const engines = [
  {
    title: "Yield estimate",
    text: "Blends the typical yield of a crop with your own harvest history, trusting your history more as it grows.",
  },
  {
    title: "Irrigation calculator",
    text: "Crop water need minus rainfall, adjusted for the efficiency of your irrigation system.",
  },
  {
    title: "Profitability engine",
    text: "Cost per hectare, cost per kg, net profit and margin for every planting.",
  },
  {
    title: "Task scheduler",
    text: "Suggested fertilizing, irrigation and inspection dates based on the crop cycle.",
  },
];

const steps = [
  {
    title: "Set up your farm",
    text: "Create a farm and add its fields with their area and location.",
  },
  {
    title: "Record what happens",
    text: "Add plantings, harvests, irrigation, expenses and revenue as they happen.",
  },
  {
    title: "Read the numbers",
    text: "The dashboard shows what is profitable, how much water you used and what to do next.",
  },
];

export default function Landing() {
  const { user } = useAuth();

  const primaryButton =
    "rounded bg-white px-3 py-1 text-sm font-medium text-green-800 hover:bg-green-100";

  return (
    <div className="min-h-screen bg-green-50">
      <header className="bg-green-800 text-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
          <Link to="/" className="text-xl font-bold">
            AgriTrack
          </Link>
          <div className="flex items-center gap-4 text-sm">
            {user ? (
              <Link to="/dashboard" className={primaryButton}>
                Open dashboard
              </Link>
            ) : (
              <>
                <Link to="/login" className="hover:underline">
                  Log in
                </Link>
                <Link to="/register" className={primaryButton}>
                  Get started
                </Link>
              </>
            )}
          </div>
        </div>
      </header>

      {/* Hero */}
      <section className="bg-green-800 pb-16 pt-12 text-center text-white">
        <div className="mx-auto max-w-3xl px-4">
          <h1 className="text-4xl font-bold sm:text-5xl">
            Run your whole farm from one place
          </h1>
          <p className="mt-4 text-lg text-green-100">
            AgriTrack helps farmers manage fields, crops, irrigation and
            finances, and turns those records into clear numbers: what to
            expect, what it costs and what you earn.
          </p>
          <div className="mt-8 flex flex-wrap justify-center gap-3">
            {user ? (
              <Link
                to="/dashboard"
                className="rounded bg-white px-6 py-3 font-semibold text-green-800 hover:bg-green-100"
              >
                Open dashboard
              </Link>
            ) : (
              <>
                <Link
                  to="/register"
                  className="rounded bg-white px-6 py-3 font-semibold text-green-800 hover:bg-green-100"
                >
                  Create a free account
                </Link>
                <Link
                  to="/login"
                  className="rounded border border-white px-6 py-3 font-semibold hover:bg-green-700"
                >
                  Log in
                </Link>
              </>
            )}
          </div>
        </div>
      </section>

      {/* Modules */}
      <section className="mx-auto max-w-5xl px-4 py-14">
        <h2 className="mb-8 text-center text-2xl font-bold text-green-800">
          Everything a farm needs
        </h2>
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {modules.map((m) => (
            <div key={m.title} className="rounded-lg bg-white p-5 shadow">
              <div className="mb-2 text-3xl">{m.icon}</div>
              <h3 className="font-semibold text-green-800">{m.title}</h3>
              <p className="mt-1 text-sm text-gray-600">{m.text}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Calculations */}
      <section className="bg-white py-14">
        <div className="mx-auto max-w-5xl px-4">
          <h2 className="text-center text-2xl font-bold text-green-800">
            More than a record book
          </h2>
          <p className="mx-auto mt-2 max-w-2xl text-center text-sm text-gray-600">
            AgriTrack applies simple, documented formulas to your data. They are
            transparent estimates, not black-box predictions.
          </p>
          <div className="mt-8 grid gap-4 sm:grid-cols-2">
            {engines.map((e) => (
              <div
                key={e.title}
                className="rounded-lg border-l-4 border-green-700 bg-green-50 p-4"
              >
                <h3 className="font-semibold text-green-800">{e.title}</h3>
                <p className="mt-1 text-sm text-gray-600">{e.text}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* How it works */}
      <section className="mx-auto max-w-5xl px-4 py-14">
        <h2 className="mb-8 text-center text-2xl font-bold text-green-800">
          How it works
        </h2>
        <div className="grid gap-6 sm:grid-cols-3">
          {steps.map((s, i) => (
            <div key={s.title} className="text-center">
              <div className="mx-auto mb-3 flex h-10 w-10 items-center justify-center rounded-full bg-green-700 font-bold text-white">
                {i + 1}
              </div>
              <h3 className="font-semibold text-green-800">{s.title}</h3>
              <p className="mt-1 text-sm text-gray-600">{s.text}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Final call to action */}
      {!user && (
        <section className="bg-green-800 py-12 text-center text-white">
          <h2 className="text-2xl font-bold">
            Ready to see your farm in numbers?
          </h2>
          <Link
            to="/register"
            className="mt-5 inline-block rounded bg-white px-6 py-3 font-semibold text-green-800 hover:bg-green-100"
          >
            Get started
          </Link>
        </section>
      )}

      <footer className="py-6 text-center text-sm text-gray-600">
        AgriTrack · by{" "}
        <a
          href="https://safwen-portfolio.vercel.app/"
          target="_blank"
          rel="noopener noreferrer"
          className="text-green-700 underline"
        >
          Safwen Benelmabrouk
        </a>
      </footer>
    </div>
  );
}
