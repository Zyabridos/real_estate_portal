export type Lang = "en" | "ru" | "no";

export const leadValidationMessages: Record<
  Lang,
  {
    fixErrors: string;
    eitherEmailOrPhone: string;

    fullNameRequired: string;

    emailInvalid: string;
    emailMin: string;
    emailMax: string;

    phoneInvalid: string;
    phoneSeparatorsMax: string;
    phoneMin: string;
    phoneMax: string;
  }
> = {
  en: {
    fixErrors: "Please fix the errors above",
    eitherEmailOrPhone: "Please enter email and/or phone number.",

    fullNameRequired: "Name is required",

    emailInvalid: "Please enter a valid email address",
    emailMin: "Email must be at least 5 characters",
    emailMax: "Email must be at most 100 characters",

    phoneInvalid: "Phone can contain only digits, spaces, '-' and '+' (plus only at the start)",
    phoneSeparatorsMax: "Phone number can contain at most 3 spaces and '-' in total",
    phoneMin: "Phone number must be at least 7 characters",
    phoneMax: "Phone number must be at most 20 characters",
  },

  no: {
    fixErrors: "Vennligst rett feilene over",
    eitherEmailOrPhone: "Vennligst oppgi e-post og/eller telefonnummer.",

    fullNameRequired: "Navn er påkrevd",

    emailInvalid: "Vennligst skriv inn en gyldig e-postadresse",
    emailMin: "E-post må være minst 5 tegn",
    emailMax: "E-post kan være maks 100 tegn",

    phoneInvalid: "Telefon kan kun inneholde sifre, mellomrom, '-' og '+' (pluss kun i starten)",
    phoneSeparatorsMax: "Telefonnummer kan ha maks 3 mellomrom og '-' totalt",
    phoneMin: "Telefonnummer må være minst 7 tegn",
    phoneMax: "Telefonnummer kan være maks 20 tegn",
  },

  ru: {
    fixErrors: "Пожалуйста, исправьте ошибки выше",
    eitherEmailOrPhone: "Пожалуйста, укажите email и/или номер телефона.",

    fullNameRequired: "Имя обязательно",

    emailInvalid: "Введите корректный email",
    emailMin: "Email должен быть не короче 5 символов",
    emailMax: "Email должен быть не длиннее 100 символов",

    phoneInvalid: "Телефон может содержать только цифры, пробелы, '-' и '+' (плюс только в начале)",
    phoneSeparatorsMax: "В номере телефона суммарно может быть не больше 3 пробелов и '-'",
    phoneMin: "Телефон должен быть не короче 7 символов",
    phoneMax: "Телефон должен быть не длиннее 20 символов",
  },
};
