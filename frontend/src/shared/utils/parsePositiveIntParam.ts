export function parsePositiveIntParam(value: unknown): number {
  const raw = String(value ?? "").trim();
  const parsed = Number(raw);

  return Number.isInteger(parsed) && parsed > 0 ? parsed : 0;
}
