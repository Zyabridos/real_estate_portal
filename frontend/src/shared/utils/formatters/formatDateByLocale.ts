export type DateFormatPreset = "short" | "long";

function resolveDateLocale(language?: string | null): string {
  if (language === "ru") return "ru-RU";
  if (language === "no") return "nb-NO";

  return "en-GB";
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

export function formatDateByLocale(
  value?: string | Date | null,
  language?: string | null,
  preset: DateFormatPreset = "long",
): string | null {
  if (!value) return null;

  const date = value instanceof Date ? value : new Date(value);

  if (Number.isNaN(date.getTime())) {
    return null;
  }

  return new Intl.DateTimeFormat(
    resolveDateLocale(language),
    resolveDateFormatOptions(preset),
  ).format(date);
}
