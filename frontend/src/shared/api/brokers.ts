import { http } from '@/shared/api/http';
import type { PagedResultDto } from '@/shared/api/dtos/common/paged-result.dto';
import type { BrokerDetailsDto } from "@/shared/api/dtos/brokers/broker-details.dto";
import type { BrokerListItemDto } from "@/shared/api/dtos/brokers/broker-list-item.dto";
import type { BrokersListQuery } from "@/shared/types/queries"
import { serializeQuery } from '@/shared/api/query';
import routes from "@/shared/routes.ts"
import buildQuery from "@/shared/api/query/buildQuery.ts";
import type {PropertyListItemDto} from "@/shared/api/dtos/properties/property-list-item.dto.ts";

// Note to myself: Pages / stores MUST use this module instead of http directly
export const brokersApi = {
  // GET /api/brokers
  async list(
    query: BrokersListQuery = {},
  ): Promise<PagedResultDto<BrokerListItemDto>> {
    const params = buildQuery(query as Record<string, unknown>);
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
