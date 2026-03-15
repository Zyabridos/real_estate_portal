export type AgencyListItem = {
  id: number;
  name: string;
  orgNumber: string;
  phoneNumber?: string;
  city?: string;
  street?: string;
  zipCode?: string;
  createdAt: string;
  updatedAt: string;
};

export type AgencyDetails = AgencyListItem;
