const getEnvString = (key: string, fallback: string): string => {
  const raw = process.env[key];
  if (typeof raw !== "string") return fallback;

  const trimmed = raw.trim();
  return trimmed.length ? trimmed : fallback;
};

export default getEnvString;
