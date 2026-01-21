export type LeadFormStatus = "idle" | "loading" | "error" | "success";

export type LeadFormValues = {
  fullName: string;
  email: string;
  phoneNumber: string;
  message: string;
};

