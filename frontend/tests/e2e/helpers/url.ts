export const getQueryParam = (url: string, key: string): string | null => {
  const u = new URL(url, "http://localhost");
  return u.searchParams.get(key);
};
