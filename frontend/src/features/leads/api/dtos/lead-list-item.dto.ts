export type LeadListItemDto = {
  id: number;
  propertyId: number;
  fullName: string;
  email: string | null;
  phoneNumber: string | null;
  status: string;
  createdAt: string;
  updatedAt: string | null;
};
