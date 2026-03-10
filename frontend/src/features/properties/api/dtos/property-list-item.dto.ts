import type { PropertyType} from "@/entities/properties/model/types";
import type { PropertyStatus } from "@/entities/properties/model/types";

export type PropertyListItemDto = {
  id: number;
  title: string;
  description: string;
  city: string;
  price: number;
  type: PropertyType;
  status: PropertyStatus;
  mainImageUrl?: string;
  createdAt?: string;
  // TODO: add brokerID(?) and updatedAt at backend and here
}
