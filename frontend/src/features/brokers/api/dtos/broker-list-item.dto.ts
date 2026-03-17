import type {BrokerGender} from "@/features/brokers/api/dtos/broker-details.dto.ts";

export type BrokerListItemDto = {
  id: number
  firstName: string
  lastName: string
  email?: string // on backend either phone, either email validation
  phoneNumber?: string
  photoUrl?: string
  createdAt: string
  gender?: BrokerGender
}
