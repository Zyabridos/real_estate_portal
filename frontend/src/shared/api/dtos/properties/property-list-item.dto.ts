export type PropertyListItemDto = {
  id: string
  title: string
  city: string
  price: number
  propertyType: string
  propertyStatus: string
  mainImageUrl?: string
  createdAt?: string
  // TODO: add brokerID(?) and updatedAt at backend and here
}
