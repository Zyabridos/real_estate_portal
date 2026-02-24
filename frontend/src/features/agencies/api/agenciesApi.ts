import { http } from "@/shared/api/http";
import buildQuery from "@/shared/api/query/buildQuery";
import normalizePagedQuery from "@/shared/api/query/normalizePagingSort";
import routes from "@/shared/routes";
import { REQUEST_QUERY_DEFAULTS } from "@/shared/config/defaults";

import type { PagedResultDto } from '@/shared/api/dtos/common/paged-result.dto';
import type { AgencyDetailsDto } from "@/features/agencies/api/dtos/agency-details.dto";
import type { AgencyListItemDto } from "@/features/agencies/api/dtos/agency-list-item.dto";
import type { AgenciesListQuery } from "@/shared/types/queries"
import type {RequestConfig} from "@/shared/api/client/types.ts";

export const agenciesApi = {
  // GET /api/agencies
  async list(
    query: AgenciesListQuery = {},
    config?: RequestConfig
  ): Promise<PagedResultDto<AgencyListItemDto>> {
    const normalized = normalizePagedQuery(
      query as Record<string, unknown>,
      REQUEST_QUERY_DEFAULTS
    );

    const params = buildQuery(normalized);

    return http.get<PagedResultDto<AgencyListItemDto>>(
      routes.api.brokers.list(),
      { ...(config ?? {}), params }
    );
  },

  // GET /api/agencies/{id}
  async getById(id: string, config?: RequestConfig): Promise<AgencyDetailsDto> {
    if (!id) throw new Error("agenciesApi.getById: id is required");
    return http.get<AgencyDetailsDto>(routes.api.agencies.getById(id), config);
  },
};
