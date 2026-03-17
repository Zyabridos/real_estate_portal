import type { APIRequestContext } from "@playwright/test";
import { apiRoutes } from "../__fixtures__/apiRoutes";

export async function updateLeadMessage(
  request: APIRequestContext,
  leadId: number,
  newMessage: string,
): Promise<void> {
  const resp = await request.put(apiRoutes.leads.byId(leadId), {
    data: { message: newMessage },
  });

  if (!resp.ok()) {
    throw new Error(`updateLeadMessage: PUT /api/leads/${leadId} failed: ${resp.status()}`);
  }
}
