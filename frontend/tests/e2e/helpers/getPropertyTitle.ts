import type { APIRequestContext } from "@playwright/test";
import { apiRoutes } from "../__fixtures__/apiRoutes";

type PropertyDetailsDto = {
  id: number;
  title: string;
};

export async function getPropertyTitle(
  request: APIRequestContext,
  propertyId: number,
): Promise<string> {
  const resp = await request.get(apiRoutes.properties.byId(propertyId));
  if (!resp.ok()) {
    throw new Error(`getPropertyTitle: GET /api/properties/${propertyId} failed: ${resp.status()}`);
  }

  const dto = (await resp.json()) as PropertyDetailsDto;
  return dto.title ?? "";
}
