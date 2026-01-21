export type LeadFormStatus = "idle" | "loading" | "error" | "success";

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
