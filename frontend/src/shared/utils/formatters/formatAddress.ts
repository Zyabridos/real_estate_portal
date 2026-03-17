export type AddressLike = {
  street?: string | null;
  zipCode?: string | null;
  city?: string | null;
};

function normalizeText(value?: string | null): string | null {
  const normalized = value?.trim();
  return normalized ? normalized : null;
}

export function formatAddress(value: AddressLike): string | null {
  const street = normalizeText(value.street);
  const zipCode = normalizeText(value.zipCode);
  const city = normalizeText(value.city);

  const line2 = [zipCode, city].filter(Boolean).join(" ");
  const parts = [street, line2].filter(Boolean);

  return parts.length > 0 ? parts.join(", ") : null;
}
