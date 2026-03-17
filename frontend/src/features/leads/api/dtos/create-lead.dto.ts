export type CreateLeadRequestDto = {
  propertyId: number;
  fullName: string;
  email?: string | null;
  phoneNumber?: string | null;
  message?: string | null;
};
