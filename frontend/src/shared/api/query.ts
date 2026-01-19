// src/shared/api/query.ts

/* Serializes query parameters into URL query string.
  - removes undefined / null / empty string values
  - converts numbers and booleans to strings
*/
export function serializeQuery<T extends object>(
  params: T,
): string {
  const searchParams = new URLSearchParams();

  Object.entries(params as Record<string, unknown>).forEach(([key, value]) => {
    if (value === undefined || value === null || value === '') {
      return;
    }

    // only primitives should go to query string
    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
      searchParams.append(key, String(value));
    }
  });

  const queryString = searchParams.toString();
  return queryString ? `?${queryString}` : '';
}
