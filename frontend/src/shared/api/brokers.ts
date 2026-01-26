import { http } from "@/shared/api/http";
import buildQuery from "@/shared/api/query/buildQuery";
import normalizePagedQuery from "@/shared/api/query/normalizePagingSort";
import routes from "@/shared/routes";
import { REQUEST_QUERY_DEFAULTS } from "@/shared/config/defaults";

import type { PagedResultDto } from '@/shared/api/dtos/common/paged-result.dto';
import type { BrokerDetailsDto } from "@/shared/api/dtos/brokers/broker-details.dto";
import type { BrokerListItemDto } from "@/shared/api/dtos/brokers/broker-list-item.dto";
import type { BrokersListQuery } from "@/shared/types/queries"

export const brokersApi = {
  // GET /api/brokers
  async list(
    query: BrokersListQuery = {},
  ): Promise<PagedResultDto<BrokerListItemDto>> {
    const normalized = normalizePagedQuery(query as Record<string, unknown>, REQUEST_QUERY_DEFAULTS);
    const params = buildQuery(normalized);
    return http.get<PagedResultDto<BrokerListItemDto>>(routes.api.brokers.list(), { params });
  },

  // GET /api/brokers/{id}
  async getById(id: string): Promise<BrokerDetailsDto> {
    if (!id) {
      throw new Error('brokersApi.getById: id is required');
    }

    return http.get<BrokerDetailsDto>(routes.api.brokers.getById(id));
  },
};
