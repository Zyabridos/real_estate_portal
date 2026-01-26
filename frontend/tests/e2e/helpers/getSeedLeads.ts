import type { APIRequestContext } from "@playwright/test";
import { apiRoutes } from "../__fixtures__/apiRoutes";
import { leadsSeed } from "../__fixtures__/leadsSeed";

type PagedResultDto<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalItems?: number;
  totalPages?: number;
};

export type LeadListItemDto = {
  id: string;
  propertyId: string;
  fullName: string;
  email: string | null;
  phoneNumber: string | null;
  status: string;
  createdAt?: string;
  updatedAt?: string;
};

export async function getSeedLeads(request: APIRequestContext): Promise<{
  propertyId: string;
  emailOnly: LeadListItemDto;
  phoneOnly: LeadListItemDto;
  both: LeadListItemDto;
  all: LeadListItemDto[];
}> {
  const resp = await request.get(`${apiRoutes.leads.path()}?page=1&pageSize=50`);
  if (!resp.ok()) {
    throw new Error(`getSeedLeads: GET /api/leads failed: ${resp.status()}`);
  }

  const data = (await resp.json()) as PagedResultDto<LeadListItemDto>;
  const items = data.items ?? [];

  const findByFullName = (fullName: string) => {
    const found = items.find((x) => (x.fullName ?? "").trim() === fullName);
    if (!found) throw new Error(`Seed lead not found in DB: fullName="${fullName}"`);
    return found;
  };

  const emailOnly = findByFullName(leadsSeed.emailOnly.fullName);
  const phoneOnly = findByFullName(leadsSeed.phoneOnly.fullName);
  const both = findByFullName(leadsSeed.both.fullName);

  const propertyId = (emailOnly.propertyId ?? "").trim();
  if (!propertyId) throw new Error("Seed leads: propertyId is empty");

  return { propertyId, emailOnly, phoneOnly, both, all: items };
}
