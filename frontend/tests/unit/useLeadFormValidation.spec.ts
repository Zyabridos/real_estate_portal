import { describe, it, expect, vi } from "vitest";
import { effectScope, nextTick, ref } from "vue";
import { useLeadFormValidation } from "../../src/features/leads/create/validation/useLeadFormValidation"
import type { LeadFormStatus, LeadFormValues } from "../../src/features/leads/create/validation/useLeadFormValidation.ts";
import type { ProblemDetails } from "../../src/shared/types/errors";


function createVm(opts?: {
  initialValues?: Partial<LeadFormValues>;
  state?: LeadFormStatus;
  disabled?: boolean;
  onSubmit?: (v: LeadFormValues) => void;
}) {
  const state = ref<LeadFormStatus>(opts?.state ?? "idle");
  const disabled = ref<boolean>(opts?.disabled ?? false);
  const initialValues = ref<Partial<LeadFormValues> | undefined>(opts?.initialValues ?? {});
  const onSubmit = opts?.onSubmit ?? vi.fn();

  const scope = effectScope();
  const vm = scope.run(() =>
    useLeadFormValidation({
      state,
      disabled,
      initialValues,
      onSubmit,
    })
  )!;

  return { vm, scope, state, disabled, initialValues, onSubmit };
}

/**
 * Makes the form valid by default:
 * - fullName valid
 * - email valid (satisfies contact requirement)
 * - phone/message optional
 */
function makeBaselineValid(vm: ReturnType<typeof createVm>["vm"]) {
  vm.values.fullName = "Bran Stark";
  vm.values.email = "a@b.com";
  vm.values.phoneNumber = "";
  vm.values.message = "";
}

describe("useLeadFormValidation", () => {
  describe("fullName", () => {
    it.each([
      ["La La", true],
      ["Bran Stark", true],
    ] satisfies Array<[string, boolean]>)("valid cases: %s => %s", (value, expected) => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.fullName = value;
      expect(vm.isValid.value).toBe(expected);

      scope.stop();
    });

    it.each([
      ["", false], // required
      [" ", false], // required after trim
      ["L", false], // too short
      ["L La", false], // first name length < 2
      ["La L", false], // last name length < 2
      ["L   La", false], // multiple spaces, still invalid tokens
    ] satisfies Array<[string, boolean]>)("invalid cases: %s => %s", (value, expected) => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.fullName = value;
      expect(vm.isValid.value).toBe(expected);

      scope.stop();
    });
  });

  describe("email", () => {
    it.each([
      ["john.doe@example.com", true],
      ["a@b.co", true],
    ] satisfies Array<[string, boolean]>)("valid format: %s => %s", (value, expected) => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.email = value;
      expect(vm.isValid.value).toBe(expected);

      scope.stop();
    });

    it.each([
      ["not-an-email", false],
      ["a@", false],
      ["bad", false],
      ["bad@.", false],
      ["bad.com", false],
    ] satisfies Array<[string, boolean]>)("invalid format: %s => %s", (value, expected) => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.email = value;
      expect(vm.isValid.value).toBe(expected);

      scope.stop();
    });

    it("empty email is allowed if phone is present (contact rule)", () => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.email = "";
      vm.values.phoneNumber = "+12 34-56";
      expect(vm.isValid.value).toBe(true);

      scope.stop();
    });

    it("invalid email stays invalid even if phone is present", () => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.email = "bad";
      vm.values.phoneNumber = "+12 34-56";
      expect(vm.isValid.value).toBe(false);

      scope.stop();
    });
  });

  describe("phoneNumber", () => {
    it.each([
      ["", true], // optional
      ["+12 34-56", true],
      ["12 34 56", true], // 2 spaces
      ["12-34-56", true], // 2 dashes
      ["12 3-456", true], // 1 space + 1 dash
    ] satisfies Array<[string, boolean]>)("valid cases: %s => %s", (value, expected) => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.phoneNumber = value;
      expect(vm.isValid.value).toBe(expected);

      scope.stop();
    });

    it.each([
      ["12+34", false], // '+' not at start
      ["abc", false], // non-numbers
      ["12", false], // minimum 7
      ["1".repeat(21), false], // maximum 20
      ["+12(34)56", false], // invalid chars (parentheses)
      ["+12_34", false], // invalid chars
      ["12?34", false], // another invalid chars
      ["12 3-4 5-6", false], // separators (space + '-') = 4 (I set on backend not more than 3 of allowed symbols)
    ] satisfies Array<[string, boolean]>)("invalid cases: %s => %s", (value, expected) => {
      const { vm, scope } = createVm();
      makeBaselineValid(vm);

      vm.values.phoneNumber = value;
      expect(vm.isValid.value).toBe(expected);

      scope.stop();
    });
  });

  // UX: either email or phone
  it("gates contact-required: not shown until both email and phone were touched", () => {
    const { vm, scope } = createVm();

    vm.values.fullName = "La La";
    vm.values.email = "";
    vm.values.phoneNumber = "";

    vm.validateAll();
    expect(vm.isValid.value).toBe(false);
    expect(vm.showFormError.value).toBe(false);
    expect(vm.errors.email).toBeNull();
    expect(vm.errors.phoneNumber).toBeNull();

    vm.onBlur("email");
    expect(vm.showFormError.value).toBe(false);

    vm.onBlur("phoneNumber");
    expect(vm.showFormError.value).toBe(true);
    expect(vm.errors.email?.key).toContain("eitherEmailOrPhone");
    expect(vm.errors.phoneNumber?.key).toContain("eitherEmailOrPhone");

    scope.stop();
  });

  it("submit: calls onSubmit only when valid", () => {
    const onSubmit = vi.fn();
    const { vm, scope } = createVm({ onSubmit });

    vm.values.fullName = "L";
    vm.values.email = "a@b.com";
    vm.submit();
    expect(onSubmit).toHaveBeenCalledTimes(0);

    vm.values.fullName = "La La";
    vm.values.email = "a@b.com";
    vm.values.phoneNumber = "";
    vm.values.message = "";

    vm.submit();
    expect(onSubmit).toHaveBeenCalledTimes(1);

    scope.stop();
  });

  it("submit: does not force-touch email/phone (keeps contact-required gated)", () => {
    const { vm, scope } = createVm();

    vm.values.fullName = "La La";
    vm.values.email = "";
    vm.values.phoneNumber = "";

    vm.submit();

    expect(vm.touched.email).toBe(false);
    expect(vm.touched.phoneNumber).toBe(false);
    expect(vm.showFormError.value).toBe(false);
    expect(vm.errors.email).toBeNull();
    expect(vm.errors.phoneNumber).toBeNull();

    scope.stop();
  });

  it("initialValues: merges without overwriting existing values with undefined", async () => {
    const { vm, scope, initialValues } = createVm({ initialValues: { fullName: "La La" } });

    expect(vm.values.fullName).toBe("La La");

    vm.values.email = "x@y.com";

    initialValues.value = { phoneNumber: "+12 34-56" };
    await nextTick();

    expect(vm.values.fullName).toBe("La La");
    expect(vm.values.email).toBe("x@y.com");
    expect(vm.values.phoneNumber).toBe("+12 34-56");

    scope.stop();
  });

  it("state flags: disabled/loading/success disables submit", () => {
    const { vm, scope, state, disabled } = createVm();
    makeBaselineValid(vm);

    expect(vm.isSubmitDisabled.value).toBe(false);

    disabled.value = true;
    expect(vm.isSubmitDisabled.value).toBe(true);

    disabled.value = false;
    state.value = "loading";
    expect(vm.isSubmitDisabled.value).toBe(true);

    state.value = "success";
    expect(vm.isSubmitDisabled.value).toBe(true);

    scope.stop();
  });
});

describe("useLeadFormValidation: server errors mapping", () => {
  it("maps ProblemDetails.errors to field errors + touches fields", () => {
    const { vm, scope } = createVm();

    const pd: ProblemDetails = {
      title: "Validation failed",
      status: 400,
      errors: {
        FullName: ["FullName must be between 2 and 50 characters."],
        Email: ["Email is not valid."],
      },
    };

    vm.applyServerErrors(pd);

    expect(vm.touched.fullName).toBe(true);
    expect(vm.errors.fullName?.key).toBe("errors:validation.lead.server");

    expect(vm.touched.email).toBe(true);
    expect(vm.errors.email?.key).toBe("errors:validation.lead.server");

    scope.stop();
  });

  it("ignores unknown fields", () => {
    const { vm, scope } = createVm();

    const pd: ProblemDetails = {
      title: "Validation failed",
      status: 400,
      errors: {
        SomeUnknownField: ["nope"],
      },
    };

    vm.applyServerErrors(pd);

    expect(vm.errors.fullName).toBeNull();
    expect(vm.errors.email).toBeNull();
    expect(vm.errors.phoneNumber).toBeNull();
    expect(vm.errors.message).toBeNull();

    scope.stop();
  });
});
