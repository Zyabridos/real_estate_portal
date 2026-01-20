export type PropertyType = "Apartment" | "House" | "Commercial";
export type PropertyStatus = "Active" | "Sold";

export type PropertyListItemDto = {
  id: string;
  title: string;
  city: string;
  price: number;
  type: PropertyType;
  status: PropertyStatus;
  mainImageUrl?: string;
  createdAt?: string;
  // TODO: add brokerID(?) and updatedAt at backend and here
}
