import { z } from "zod";

// TODO: move to defaults?
export const FULL_NAME_MIN = 5;
export const FULL_NAME_MAX = 50;

export const EMAIL_MIN = 5;
export const EMAIL_MAX = 100;
export const PHONE_MIN = 7;
export const PHONE_MAX = 20;
export const MESSAGE_MAX = 2000;
export const PHONE_SEPARATORS_MAX = 3;

const emptyToUndefined = (v: unknown) => {
  if (typeof v !== "string") return v;
  const trimmed = v.trim();
  return trimmed.length === 0 ? undefined : trimmed;
};

const normalizeSpaces = (v: unknown) => {
  if (typeof v !== "string") return v;
  return v.trim().replace(/\s+/g, " ");
};

const isValidFullNameTokens = (fullName: string) => {
  const tokens = fullName.split(" ").filter(Boolean);
  // allow single token ("asfd") as valid; just ensure each token length >= 2
  return tokens.length > 0 && tokens.every((t) => t.length >= 2);
};

// Email: simple, pragmatic (client-side), backend will validate too
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

// Phone - same validation as on backend:
// - only digits, spaces, '-' and optional '+' at start
// - '+' only once and only at beginning
// - after optional '+', must start with digit
const phoneAllowedRegex = /^\+?\d[\d -]*$/;

const countSeparators = (phone: string) => (phone.match(/[ -]/g) ?? []).length;

export const leadSchema = z
  .object({
    fullName: z.preprocess(
      normalizeSpaces,
      z
        .string()
        .min(1, { message: "required" })
        .min(FULL_NAME_MIN, { message: "min" })
        .max(FULL_NAME_MAX, { message: "max" })
        .refine(isValidFullNameTokens, { message: "tokenMin" })
    ),

    // optional; validate only when provided
    email: z.preprocess(
      emptyToUndefined,
      z
        .string()
        .min(EMAIL_MIN, { message: "min" })
        .max(EMAIL_MAX, { message: "max" })
        .refine((v) => emailRegex.test(v), { message: "invalid" })
        .optional()
    ),

    // optional; validate only when provided
    phoneNumber: z.preprocess(
      emptyToUndefined,
      z
        .string()
        .min(PHONE_MIN, { message: "min" })
        .max(PHONE_MAX, { message: "max" })
        .refine((v) => phoneAllowedRegex.test(v), { message: "invalid" })
        .refine((v) => countSeparators(v) <= PHONE_SEPARATORS_MAX, { message: "separatorsMax" })
        .optional()
    ),

    message: z.preprocess(
      emptyToUndefined,
      z.string().max(MESSAGE_MAX, { message: "max" }).optional()
    ),
  })
  .superRefine((val, ctx) => {
    const hasEmail = !!val.email?.trim();
    const hasPhone = !!val.phoneNumber?.trim();

    // for better UI - show only after both empty
    if (!hasEmail && !hasPhone) {
      ctx.addIssue({
        code: "custom",
        path: [],
        message: "eitherEmailOrPhone",
      });
    }
  });
