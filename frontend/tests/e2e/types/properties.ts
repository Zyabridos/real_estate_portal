export const PropertyType = {
  Apartment: "Apartment",
  House: "House",
  Commercial: "Commercial",
} as const;

export type PropertyTypeValue = (typeof PropertyType)[keyof typeof PropertyType];
