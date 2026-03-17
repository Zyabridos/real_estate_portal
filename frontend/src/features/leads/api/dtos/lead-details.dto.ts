import type {LeadStatus} from "@/entities/leads/model/types";

export type LeadDetailsDto = {
  id: number;
  propertyId: number;
  fullName: string;
  email?: string;
  phoneNumber?: string;
  message?: string;
  status: LeadStatus;
  createdAt: Date;
  updatedAt: Date;
};
