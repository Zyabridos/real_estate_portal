import routes from "@/shared/routes";
import { http } from "@/shared/api/http";
import type { CreateLeadRequestDto } from "@/shared/api/dtos/leads/create-lead.dto";
import type { LeadDetailsDto } from "@/shared/api/dtos/leads/lead-details.dto";

export const leadsApi = {
  async createLead(payload: CreateLeadRequestDto): Promise<LeadDetailsDto> {
    return await http.post<LeadDetailsDto>(routes.api.leads.create(), payload);
  },
};
