export type PropertyType = "Apartment" | "House" | "Commercial";
export type PropertyStatus = "Active" | "Sold";

export type PropertyListItem = {
  id: string;
  title: string;
  city: string;
  type: string;
  status: string;
  price: number;
  imageUrl?: string | null;
  brokerId?: number;
};

export type PropertyFiltersValue = {
  city?: string;
  type?: PropertyType;
  status?: PropertyStatus;
  minPrice?: number;
  maxPrice?: number;
};

export const emptyPropertyFilters: PropertyFiltersValue = {};
