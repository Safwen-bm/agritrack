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