# AgriTrack: Smart Farm Management Platform
#### Video Demo: <URL HERE>
#### Description:

AgriTrack is a web application that helps farmers manage their operations in one place. A farmer who grows olives, tomatoes and wheat on several fields records what happens on each field, and the application turns those records into numbers: how much to expect, what it costs, what it earns, how much water was used and what to do next. The live version is at <VERCEL URL>. The API runs on a free hosting tier, so the first request after a pause can take about a minute.

The stack is React, TypeScript, Vite and Tailwind CSS on the frontend, and C# with ASP.NET Core Web API and Entity Framework Core on the backend. The database is PostgreSQL, and authentication uses ASP.NET Core Identity with JWT. I chose this stack because I wanted a project I could also put in my portfolio. I had never used C# or ASP.NET before, so I used Claude as a helper, as the final project allows. Files written with its help start with a comment saying so, and I tested every part and can explain it.

From CS50 the project uses relational database design and SQL (through an ORM), HTTP requests and JSON, algorithms and business logic, and JavaScript for the interactive interface.

What the application does: a farmer creates farms and divides them into fields with area, soil type and GPS coordinates. Each field has plantings, meaning one crop in one field in one season, which keeps the history of the field. For each planting the farmer records harvests, irrigation, expenses and revenues. From these records the app calculates the yield estimate, the irrigation requirement, and profitability. It also shows a dashboard with charts, a map of the fields, suggested tasks, and a weather forecast with warnings.

## The calculations

The calculators are plain static C# classes with no database access, so they are easy to test and explain.

- Profitability: cost per hectare is expenses divided by area. Cost per kg is expenses divided by harvested kg. Net profit is revenue minus expenses, and margin is profit divided by revenue. When a division would be meaningless (no harvest, no revenue) the result is null instead of a fake zero.
- Yield estimate: a weighted average of the crop's typical yield and the farmer's own past yields of that crop, with weight n / (n + 2) for n past harvests. With no history it uses the typical value, and the more history exists the more it is trusted. It is a simple formula, not machine learning.
- Irrigation: net need in mm is the season water need minus rainfall, never below zero. The volume in m3 is net need x 10 x area / efficiency, because 1 mm of water on 1 hectare is exactly 10 m3.
- Task scheduler: suggests fertilizer doses at 15% and 45% of the crop cycle, weekly irrigation, fortnightly inspections and harvest preparation.
- Weather advisor: compares a 7-day Open-Meteo forecast with the scheduled tasks and warns about frost, heat, rain on irrigation or fertilizer days, strong wind, and rain at harvest.

## Backend files (backend/AgriTrack.Api)

- Program.cs configures everything: the PostgreSQL connection, Identity, JWT validation, CORS, the weather HTTP client, and the middleware order. It also has a /health endpoint for the host.
- Dockerfile builds the API into a container image for deployment.
- Data/AppDbContext.cs defines the tables for EF Core and seeds three crop types (olive, tomato, wheat) with their water need, growth days and typical yield.
- Migrations/ holds the generated history of the database schema.
- Models/ contains one class per table: ApplicationUser, Farm, Field, CropType, Planting, Harvest, IrrigationLog, Expense and Revenue.
- Dtos/ contains the request and response shapes: AuthDtos.cs, FarmDtos.cs (farms, fields, crop types, plantings), RecordDtos.cs (harvests, irrigation, revenues, expenses), DashboardDtos.cs and ExtraDtos.cs (tasks, map points, weather).
- Extensions/ClaimsExtensions.cs reads the logged-in user's id from the token.
- Services/TokenService.cs creates the JWT. Services/WeatherService.cs calls the Open-Meteo API.
- Controllers/AuthController.cs handles register, login and the /me endpoint.
- Controllers/FarmsController.cs, FieldsController.cs, CropTypesController.cs and PlantingsController.cs manage the farm structure. PlantingsController also applies a rule: if no harvest date is given, it is the planting date plus the crop's growth days.
- Controllers/HarvestsController.cs, IrrigationController.cs, RevenuesController.cs and ExpensesController.cs manage the records and validate them, for example no record dated before planting.
- Controllers/AnalyticsController.cs exposes the profitability, yield and irrigation calculators for a planting. DashboardController.cs aggregates totals and chart data. TasksController.cs, MapController.cs and WeatherController.cs serve tasks, field map points and the forecast with warnings.
- Calculations/ has ProfitabilityCalculator.cs, YieldEstimator.cs, IrrigationCalculator.cs, MonthlyAggregator.cs, FarmTaskScheduler.cs and WeatherAdvisor.cs, described above.

## Test files (backend/AgriTrack.Tests)

One xUnit file per calculator: ProfitabilityCalculatorTests.cs, YieldEstimatorTests.cs, IrrigationCalculatorTests.cs, MonthlyAggregatorTests.cs, FarmTaskSchedulerTests.cs and WeatherAdvisorTests.cs. The expected values are numbers I calculated by hand. All 26 tests pass.

## Frontend files (frontend/src)

- main.tsx and App.tsx start the app and define the routes. index.css loads Tailwind.
- services/api.ts is the single function every request goes through. It adds the token, and sends the user back to login if the token expired.
- context/AuthContext.tsx keeps the logged-in user available to all pages.
- types/index.ts holds the TypeScript types that match the API.
- components/Layout.tsx is the navigation bar, and ProtectedRoute.tsx blocks pages for logged-out users. RecordSection.tsx is one reusable list, form and delete component used for harvests, irrigation, revenues and expenses. Calculators.tsx shows the three calculators. ChartCard.tsx, TasksCard.tsx and WeatherCard.tsx show a chart frame, suggested tasks and the forecast.
- pages/Landing.tsx is the public home page. Login.tsx and Register.tsx handle accounts. Dashboard.tsx shows the charts. Farms.tsx, FarmDetail.tsx, FieldDetail.tsx and PlantingDetail.tsx are the farm, field and planting screens, and MapPage.tsx shows fields on a Leaflet map.
- vercel.json makes page refreshes work on the hosted site.

compose.yaml starts the local PostgreSQL in Docker, and .env.example shows the one variable it needs.

## Design decisions

- One backend and one database with separated modules (a modular monolith) instead of microservices, which would add complexity this project does not need.
- Every query is filtered by the owner through planting, field, farm and user. Another user's record returns 404, so ids cannot be probed.
- DTOs instead of entities, so the API controls exactly what goes in and out, and the owner id always comes from the token, never the request.
- Calculators are kept separate from the database so they can be unit tested.
- The JWT is stored in localStorage, which is simple but exposed if there is an XSS bug. httpOnly cookies would be safer in a larger system.
- Secrets live in user secrets locally and environment variables in production, never in the repository.

## Limitations

The crop values are rough starting values, not agronomic data. The task rules are generic and tasks cannot be marked as done. Profit per crop counts only expenses linked to a planting. The inventory module from my original plan was not built, to leave time for deployment.