export const leadsSeed = {
  emailOnly: {
    fullName: "Seed User EmailOnly",
    email: "seed.emailonly@example.com",
    phoneNumber: "", // may render as dash though.
    message: "Seed lead (email only).",
  },
  phoneOnly: {
    fullName: "Seed User PhoneOnly",
    email: "",
    phoneNumber: "+4799988777",
    message: "Seed lead (phone only).",
  },
  both: {
    fullName: "Seed User Both",
    email: "seed.both@example.com",
    phoneNumber: "+4711122333",
    message: "Seed lead (both email and phone).",
  },
} as const;
