export type BrokerListItemDto = {
  id: string
  firstName: string
  lastName: string
  email?: string // on backend either phone, either email validation
  phoneNumber?: string
  photoUrl?: string
  createdAt: string
  // TODO: consider adding city to back and front for brokers
}
