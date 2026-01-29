import { computed, reactive, ref, watch, type Ref } from "vue";
import type { LeadFormStatus, LeadFormValues } from "@/entities/leads/model/types";
import type { ProblemDetails } from "@/shared/types/errors";
import {
  leadSchema,
  FULL_NAME_MIN,
  FULL_NAME_MAX,
  EMAIL_MIN,
  EMAIL_MAX,
  PHONE_MIN,
  PHONE_MAX,
  MESSAGE_MAX,
  PHONE_SEPARATORS_MAX,
} from "@/features/leads/create/validation/leadSchema";

// TODO: in React I normally would keep it in /hooks - find out where it is correct to keep "hooks" in Vue

type FieldKey = keyof LeadFormValues;

const serverFieldMap: Record<string, FieldKey> = {
  FullName: "fullName",
  Email: "email",
  PhoneNumber: "phoneNumber",
  Message: "message",
};

export type FieldError = {
  key: string; // i18n key (e.g. errors:validation.lead.email.required)
  params?: Record<string, string | number>;
};

type UseLeadFormValidationArgs = {
  state: Ref<LeadFormStatus>;
  disabled: Ref<boolean>;
  initialValues: Ref<Partial<LeadFormValues> | undefined>;
  onSubmit: (values: LeadFormValues) => void;
};

const FIELDS: FieldKey[] = ["fullName", "email", "phoneNumber", "message"];

const MIN_BY_FIELD: Partial<Record<FieldKey, number>> = {
  fullName: FULL_NAME_MIN,
  email: EMAIL_MIN,
  phoneNumber: PHONE_MIN,
};

const MAX_BY_FIELD: Record<FieldKey, number> = {
  fullName: FULL_NAME_MAX,
  email: EMAIL_MAX,
  phoneNumber: PHONE_MAX,
  message: MESSAGE_MAX,
};

const ERROR_KEY = {
  fixErrors: "errors:validation.lead.form.fixErrors",
  eitherEmailOrPhone: "errors:validation.lead.form.eitherEmailOrPhone",
} as const;

export function useLeadFormValidation(args: UseLeadFormValidationArgs) {
  const values = reactive<LeadFormValues>({
    fullName: args.initialValues.value?.fullName ?? "",
    email: args.initialValues.value?.email ?? "",
    phoneNumber: args.initialValues.value?.phoneNumber ?? "",
    message: args.initialValues.value?.message ?? "",
  });

  watch(
    () => args.initialValues.value,
    (next) => {
      if (!next) return;
      if (next.fullName !== undefined) values.fullName = next.fullName;
      if (next.email !== undefined) values.email = next.email;
      if (next.phoneNumber !== undefined) values.phoneNumber = next.phoneNumber;
      if (next.message !== undefined) values.message = next.message;
    },
    { deep: true }
  );

  const touched = reactive<Record<FieldKey, boolean>>({
    fullName: false,
    email: false,
    phoneNumber: false,
    message: false,
  });

  const errors = reactive<Record<FieldKey, FieldError | null>>({
    fullName: null,
    email: null,
    phoneNumber: null,
    message: null,
  });

  const showFormError = ref(false);

  function applyServerErrors(pd?: ProblemDetails): void {
    if (!pd?.errors || typeof pd.errors !== "object") return;

    // reset existing field errors first (server-side)
    (Object.keys(errors) as FieldKey[]).forEach((k) => (errors[k] = null));

    for (const [key, val] of Object.entries(pd.errors)) {
      const field = serverFieldMap[key];
      const firstMsg =
        Array.isArray(val) ? (val.find((x) => typeof x === "string" && x.trim()) ?? "") : "";

      if (!field || !firstMsg) continue;

      touched[field] = true;
      errors[field] = {
        key: "errors:validation.lead.server",
        params: { message: firstMsg },
      };
    }
  }

  const isLoading = computed(() => args.state.value === "loading");
  const isSuccess = computed(() => args.state.value === "success");
  const isError = computed(() => args.state.value === "error");

  const isFormDisabled = computed(() => args.disabled.value || isLoading.value || isSuccess.value);

  const shouldShowContactRequired = computed(() => {
    const hasEmail = !!values.email?.trim();
    const hasPhone = !!values.phoneNumber?.trim();
    return touched.email && touched.phoneNumber && !hasEmail && !hasPhone;
  });

  function resetErrors(): void {
    FIELDS.forEach((f) => (errors[f] = null));
  }

  function markTouched(field: FieldKey): void {
    touched[field] = true;
  }

  function clearFieldError(field: FieldKey): void {
    errors[field] = null;
  }

  function setFieldError(field: FieldKey, code: string): void {
    // zod issue.message -> i18n key + params
    const base = `errors:validation.lead.${field}.`;

    const map: Record<string, () => FieldError> = {
      required: () => ({ key: `${base}required` }),
      invalid: () => ({ key: `${base}invalid` }),
      min: () => ({ key: `${base}min`, params: { min: MIN_BY_FIELD[field] ?? 3 } }),
      max: () => ({ key: `${base}max`, params: { max: MAX_BY_FIELD[field] } }),
      tokenMin: () => ({ key: `errors:validation.lead.fullName.tokenMin`, params: { min: FULL_NAME_MIN } }),
      separatorsMax: () => ({
        key: `errors:validation.lead.phoneNumber.separatorsMax`,
        params: { max: PHONE_SEPARATORS_MAX },
      }),
      eitherEmailOrPhone: () => ({ key: ERROR_KEY.eitherEmailOrPhone }),
    };

    errors[field] = map[code] ? map[code]() : { key: ERROR_KEY.fixErrors };
  }

  function applyContactRequiredIfAllowed(): void {
    if (!shouldShowContactRequired.value) return;

    showFormError.value = true;

    // Show under both fields
    if (!errors.email) setFieldError("email", "eitherEmailOrPhone");
    if (!errors.phoneNumber) setFieldError("phoneNumber", "eitherEmailOrPhone");
  }

  function validateAll(): boolean {
    resetErrors();
    showFormError.value = false;

    const parsed = leadSchema.safeParse(values);
    if (parsed.success) return true;

    // 1) handle issues
    for (const issue of parsed.error.issues) {
      if (!issue.path?.length) {
        if (issue.message === "eitherEmailOrPhone") {
          applyContactRequiredIfAllowed();
        } else {
          showFormError.value = true;
        }
        continue;
      }

      const field = issue.path[0] as FieldKey | undefined;
      if (!field) continue;

      if (!errors[field]) {
        setFieldError(field, issue.message);
      }
    }

    // 2) if there are field errors, show banner
    const hasFieldErrors = Object.values(errors).some(Boolean);
    if (hasFieldErrors) {
      const onlyContactRequired =
        (errors.email?.key === ERROR_KEY.eitherEmailOrPhone || errors.phoneNumber?.key === ERROR_KEY.eitherEmailOrPhone) &&
        !errors.fullName &&
        !errors.message;

      if (!onlyContactRequired) showFormError.value = true;
    }

    return false;
  }

  function validateField(_field: FieldKey): void {
    // cheap enough: validate all, then display relevant messages, evnt add fetures (?)
    validateAll();
  }

  const isValid = computed(() => leadSchema.safeParse(values).success);
  const isSubmitDisabled = computed(() => isFormDisabled.value || !isValid.value);

  function onBlur(field: FieldKey): void {
    markTouched(field);
    validateField(field);
  }

  function onInput(field: FieldKey): void {
    clearFieldError(field);
  }

  function submit(): void {
    showFormError.value = false;
    touched.fullName = true;

    const ok = validateAll();
    if (!ok) return;

    args.onSubmit({ ...values });
  }

  return {
    // state
    values,
    touched,
    errors,
    showFormError,

    // flags
    isLoading,
    isSuccess,
    isError,
    isFormDisabled,
    isValid,
    isSubmitDisabled,

    // handlers
    onBlur,
    onInput,
    validateAll,
    submit,
    applyServerErrors,
  };
}
