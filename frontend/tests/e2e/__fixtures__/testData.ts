export const testData = {
  properties: {
    id: "72e1d7e1-d11a-4d2b-b1b8-a475c4fe04f6",
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
        // forbiddenChars: "+47(123)456", // скобки запрещены по твоему правилу
      },
    },

    message: {
      valid: "Hi! I want to schedule a viewing.",
    },
  },
} as const;
