export type PropertyType = "Apartment" | "House" | "Commercial";
export type PropertyStatus = "Active" | "Sold";

export type PropertyDetailsDto = {
  id: string;
  title: string;
  description: string;
  address: string;
  city: string;
  price: number;
  type: PropertyType;
  status: PropertyStatus;
  bedrooms: number;
  bathrooms: number;
  area: number;
  mainImageUrl?: string;
  brokerId: string;
  createdAt?: string;
  // TODO: add UpdatedAt
  // TODO: add imagesUrls: string[] - both for front and back
}
