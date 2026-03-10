export type BrokerListItemDto = {
  id: number
  firstName: string
  lastName: string
  email?: string // on backend either phone, either email validation
  phoneNumber?: string
  photoUrl?: string
  createdAt: string
}
