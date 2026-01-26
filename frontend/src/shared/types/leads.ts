import type { PagedResult } from "@/shared/types/pagedResult.ts";
import type { SortDirection } from "@/shared/types/queries";

export type LeadFormStatus = "idle" | "loading" | "error" | "success";
export type LeadStatus = "New" | "Contacted" | "Closed";
export type LeadSortBy = "PropertyId" | "FullName" | "Email" | "PhoneNumber" | "CreatedAt";

export type LeadFormValues = {
  fullName: string;
  email: string;
  phoneNumber: string;
  message: string;
};

export type LeadFormProps = {
  state?: LeadFormStatus;
  disabled?: boolean;
  errorMessage?: string | null;
  successMessage?: string | null;
  testId?: string;
  initialValues?: Partial<LeadFormValues>;
};

export type LeadListItem = {
  id: string;
  propertyId: string;
  fullName: string;
  email: string | null;
  phoneNumber: string | null;
  status: LeadStatus;
  createdAt: string; // ISO
  updatedAt: string | null; // ISO
};

export type LeadListQuery = {
  id?: string | null;
  propertyId?: string | null;
  fullName?: string | null;
  email?: string | null;
  phoneNumber?: string | null;

  page?: number | null;
  pageSize?: number | null;

  sortBy?: LeadSortBy | null;
  sortDirection?: SortDirection | null;
};

export type LeadListQueryNormalized = {
  id?: string;
  propertyId?: string;
  fullName?: string;
  email?: string;
  phoneNumber?: string;

  page: number;
  pageSize: number;

  sortBy: LeadSortBy;
  sortDirection: SortDirection;
};

export type LeadsPagedResult = PagedResult<LeadListItem>;
