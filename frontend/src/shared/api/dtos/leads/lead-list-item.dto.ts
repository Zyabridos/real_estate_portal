export type LeadListItemDto = {
  id: string;
  propertyId: string;
  fullName: string;
  email: string | null;
  phoneNumber: string | null;
  status: string;
  createdAt: string; // ISO
  updatedAt: string | null; // ISO
};
