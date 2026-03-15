export type BrokerGender = "Unspecified" | "male" | "female" | "other";

export type BrokerDetailsDto = {
  id: number
  agencyId: number
  firstName: string
  lastName: string
  email?: string
  phoneNumber?: string
  photoUrl?: string
  createdAt: string
  updatedAt: string
  gender?: BrokerGender
}
