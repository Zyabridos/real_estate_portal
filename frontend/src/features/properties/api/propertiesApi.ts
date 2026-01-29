import { http } from "@/shared/api/http";
import buildQuery from "@/shared/api/query/buildQuery";
import normalizePagedQuery from "@/shared/api/query/normalizePagingSort";
import routes from "@/shared/routes";
import { REQUEST_QUERY_DEFAULTS } from "@/shared/config/defaults";

import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { PropertyDetailsDto } from "@/features/properties/api/dtos/property-details.dto";
import type { PropertyListItemDto } from "@/features/properties/api/dtos/property-list-item.dto";
import type { PropertiesListQuery } from "@/shared/types/queries";

export const propertiesApi = {
  // GET /api/properties
  async list(query: PropertiesListQuery = {}): Promise<PagedResultDto<PropertyListItemDto>> {
    const normalized = normalizePagedQuery(query as Record<string, unknown>, REQUEST_QUERY_DEFAULTS);
    const params = buildQuery(normalized);
    return http.get<PagedResultDto<PropertyListItemDto>>(routes.api.properties.list(), { params });
  },

  // GET /api/properties/{id}
  async getById(id: string): Promise<PropertyDetailsDto> {
    if (!id) {
      throw new Error("propertiesApi.getById: id is required");
    }

    return http.get<PropertyDetailsDto>(routes.api.properties.getById(id));
  },
};
