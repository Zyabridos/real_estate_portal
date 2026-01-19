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
  sortDirection?: string;
}
