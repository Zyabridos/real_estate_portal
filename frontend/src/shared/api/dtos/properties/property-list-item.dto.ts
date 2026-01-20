export type PropertyListItemDto = {
  id: string
  title: string
  city: string
  price: number
  type: string
  status: string
  mainImageUrl?: string
  createdAt?: string
  // TODO: add brokerID(?) and updatedAt at backend and here
}
