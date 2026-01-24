export const testData = {
  blog: {
    article: {
      knownSlug: "how-to-buy-an-apartment-in-norway-a-practical-checklist",
    },

    categories: {
      buyingGuide: {
        slug: "buying-guide",
        title: "Buying Guide",
      },
    },

    labels: {
      allOption: "All"
    },
  },

  leads: {
    fullName: {
      valid: "La La",
      invalid: {
        tooShort: "L",
        tokenTooShort: "La L",
      },
    },

    email: {
      valid: "la@example.com",
      invalid: {
        format: "not-an-email",
        tooShort: "a",
        tooLong: "a".repeat(101) + "@example.com",
      },
    },

    phoneNumber: {
      valid: "+47 123-45 678",
      invalid: {
        plusNotAtStart: "12+34567",
        separatorsOverMax: "12 3-4 5-6", // separators: space, -, space, - => 4
        tooShort: "123456",                 // 6 (< 7)
        tooLong: "1".repeat(21),
        forbiddenChars: "+47(123)456", // only '+', space and "-" are allowed
      },
    },

    message: {
      valid: "Hi! I want to schedule a viewing.",
    },
  },
} as const;
