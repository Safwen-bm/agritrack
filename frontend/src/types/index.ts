// Built with help from Claude
export interface AuthResponse {
  token: string
  expiresAt: string
  email: string
  fullName: string
}

export interface Farm {
  id: number
  name: string
  location: string | null
  fieldCount: number
  totalAreaHectares: number
}

export interface FieldItem {
  id: number
  farmId: number
  name: string
  areaHectares: number
  soilType: string | null
  latitude: number | null
  longitude: number | null
}

export interface CropType {
  id: number
  name: string
  seasonWaterNeedMm: number
  growthDays: number
  typicalYieldKgPerHectare: number
}

export interface Planting {
  id: number
  fieldId: number
  cropTypeId: number
  cropName: string
  plantingDate: string
  expectedHarvestDate: string | null
  status: string
}

export interface Harvest { id: number; plantingId: number; harvestDate: string; quantityKg: number }
export interface Irrigation { id: number; plantingId: number; date: string; volumeM3: number }
export interface Revenue { id: number; plantingId: number; date: string; amount: number; description: string | null }
export interface Expense {
  id: number; farmId: number; plantingId: number | null
  category: string; amount: number; date: string; description: string | null
}

export interface Profitability {
  totalExpenses: number
  totalRevenue: number
  netProfit: number
  totalHarvestKg: number
  costPerHectare: number
  costPerKg: number | null
  profitMarginPercent: number | null
}

export interface YieldResponse {
  estimate: {
    estimatedYieldKgPerHectare: number
    estimatedTotalKg: number
    historyCount: number
    historyWeight: number
  }
  actualKg: number
  comparison: { differenceKg: number; differencePercent: number | null }
}

export interface IrrigationEstimate {
  netRequirementMm: number
  recommendedVolumeM3: number
  appliedVolumeM3: number
  differenceM3: number
}

export interface ProfitGroup { label: string; expenses: number; revenue: number; profit: number }

export interface DashboardData {
  summary: {
    farmCount: number
    fieldCount: number
    totalAreaHectares: number
    totalExpenses: number
    totalRevenue: number
    netProfit: number
    totalWaterM3: number
    totalHarvestKg: number
  }
  profitByCrop: ProfitGroup[]
  profitBySeason: ProfitGroup[]
  expensesByCategory: { category: string; amount: number }[]
  waterByMonth: { month: string; volumeM3: number }[]
  yields: { label: string; yieldKgPerHectare: number; totalKg: number }[]
}

export interface UpcomingTask {
  date: string
  type: string
  title: string
  stage: string
  plantingId: number
  cropName: string
  fieldName: string
}

export interface MapPoint {
  id: number
  name: string
  farmName: string
  areaHectares: number
  latitude: number
  longitude: number
  currentCrops: string[]
}

export interface DailyForecast {
  date: string
  tempMaxC: number
  tempMinC: number
  precipitationMm: number
  windMaxKmh: number
}

export interface WeatherWarning { date: string; severity: string; message: string }

export interface WeatherResponse {
  fieldName: string
  forecast: DailyForecast[]
  warnings: WeatherWarning[]
}