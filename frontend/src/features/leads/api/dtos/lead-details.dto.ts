import type {LeadStatus} from "@/entities/leads/model/types";

export type LeadDetailsDto = {
  id: string;
  propertyId: string;
  fullName: string;
  email?: string;
  phoneNumber?: string;
  message?: string;
  status: LeadStatus;
  createdAt: Date;
  updatedAt: Date;
};
