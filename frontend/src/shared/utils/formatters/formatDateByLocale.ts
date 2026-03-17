export type DateFormatPreset = "short" | "long";
export type DateTimeFormatPreset = "short" | "long";

function resolveDateLocale(language?: string | null): string {
  if (language === "ru") return "ru-RU";
  if (language === "no") return "nb-NO";

  return "en-GB";
}

function toDate(value?: string | Date | null): Date | null {
  if (!value) return null;

  const date = value instanceof Date ? value : new Date(value);

  if (Number.isNaN(date.getTime())) {
    return null;
  }

  return date;
}

function resolveDateFormatOptions(preset: DateFormatPreset): Intl.DateTimeFormatOptions {
  if (preset === "short") {
    return {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
    };
  }

  return {
    year: "numeric",
    month: "long",
    day: "numeric",
  };
}

function resolveDateTimeFormatOptions(
  preset: DateTimeFormatPreset,
): Intl.DateTimeFormatOptions {
  if (preset === "short") {
    return {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    };
  }

  return {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  };
}

export function formatDateByLocale(
  value?: string | Date | null,
  language?: string | null,
  preset: DateFormatPreset = "long",
): string | null {
  const date = toDate(value);
  if (!date) return null;

  return new Intl.DateTimeFormat(
    resolveDateLocale(language),
    resolveDateFormatOptions(preset),
  ).format(date);
}

export function formatDateTimeByLocale(
  value?: string | Date | null,
  language?: string | null,
  preset: DateTimeFormatPreset = "long",
): string | null {
  const date = toDate(value);
  if (!date) return null;

  return new Intl.DateTimeFormat(
    resolveDateLocale(language),
    resolveDateTimeFormatOptions(preset),
  ).format(date);
}
