import {string} from "zod";

export type SortDirection = "asc" | "desc";
export interface PropertiesListQuery {
  id?: string
  city?: string;
  type?: string;
  status?: string;
  minPrice?: number;
  maxPrice?: number;
  page?: number;
  pageSize?: number;
  sort?: string;
  // TODO: add BrokerId?
}

export interface BrokersListQuery {
  brokerId?: string;
  firstName?: string;
  lastName?: string;
  agencyId?: string;
  email?: string;

  phoneNumber?: string;
  page?: number;
  pageSize?: number;

  sortBy?: string;
  sortDirection?: SortDirection | null;
}

export type LeadsListQuery = {
  id?: string | null;
  propertyId?: string | null;
  fullName?: string | null;
  email?: string | null;
  phoneNumber?: string | null;

  page?: number | null;
  pageSize?: number | null;

  sortBy?: string | null;
  sortDirection?: SortDirection | null;
};

export type AgenciesListQuery = {
  id?: string | null;
  orgNumber?: string | null;
  name?: string | null;
  phoneNumber?: string | null;
  city?: string | null;
  street?: string | null;
  zipCode?: string | null;

  page?: number | null;
  pageSize?: number | null;
};
