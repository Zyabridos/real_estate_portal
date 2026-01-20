import type { PropertyType} from "@/shared/types/properties.ts";
import type { PropertyStatus } from "@/shared/types/properties.ts";

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
