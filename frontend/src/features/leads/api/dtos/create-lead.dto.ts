export type CreateLeadRequestDto = {
  propertyId: string;
  fullName: string;
  email?: string | null;
  phoneNumber?: string | null;
  message?: string | null;
};
