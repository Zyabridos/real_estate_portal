import type { PropertyType} from "@/entities/properties/model/types";
import type { PropertyStatus } from "@/entities/properties/model/types";

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
  mainImageUrl?: string | null;
  brokerId: string;
  createdAt?: string;
  // TODO: add UpdatedAt
  // TODO: add imagesUrls: string[] - both for front and back
}
