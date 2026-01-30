import { http } from "@/shared/api/http";
import buildQuery from "@/shared/api/query/buildQuery";
import normalizePagedQuery from "@/shared/api/query/normalizePagingSort";
import routes from "@/shared/routes";
import { REQUEST_QUERY_DEFAULTS } from "@/shared/config/defaults";

import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { CreateLeadRequestDto } from "@/features/leads/api/dtos/create-lead.dto";
import type { LeadDetailsDto } from "@/features/leads/api/dtos/lead-details.dto";
import type { LeadListItemDto } from "@/features/leads/api/dtos/lead-list-item.dto";
import type { LeadsListQuery } from "@/shared/types/queries";

export const leadsApi = {
  // GET /api/leads
  async list(query: LeadsListQuery = {}): Promise<PagedResultDto<LeadListItemDto>> {
    // TODO: move next two string in one func?
    const normalized = normalizePagedQuery(query as Record<string, unknown>, REQUEST_QUERY_DEFAULTS);
    const params = buildQuery(normalized);
    return http.get<PagedResultDto<LeadListItemDto>>(routes.api.leads.create(), { params });
  },

  // GET /api/leads/{id}
  async getById(id: string): Promise<LeadDetailsDto> {
    if (!id) {
      throw new Error("leadsApi.getById: id is required");
    }
    return http.get<LeadDetailsDto>(routes.api.leads.getById(id));
  },

  // POST /api/leads
  async createLead(payload: CreateLeadRequestDto): Promise<LeadDetailsDto> {
    return http.post<LeadDetailsDto>(routes.api.leads.create(), payload);
  },
};
