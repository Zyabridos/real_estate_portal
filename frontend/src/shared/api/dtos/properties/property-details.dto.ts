export type PropertyDetailsDto = {
  id: string
  title: string
  description: string
  address: string
  city: string
  price: number
  propertyType: string
  bedrooms: number
  bathrooms: number
  area: number
  propertyStatus: string
  mainImageUrl?: string
  brokerId: string
  createdAt?: string
  // TODO: add UpdatedAt
  // TODO: add imagesUrls: string[] - both for front and back
}
