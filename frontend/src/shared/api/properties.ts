import { http } from "@/shared/api/http";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { PropertyDetailsDto } from "@/shared/api/dtos/properties/property-details.dto";
import type { PropertyListItemDto } from "@/shared/api/dtos/properties/property-list-item.dto";
import type { PropertiesListQuery } from "@/shared/types/queries";
import routes from "@/shared/routes";
import buildQuery from "@/shared/api/query/buildQuery";

// Note to myself: Pages / stores MUST use this module instead of http directly
export const propertiesApi = {
  // GET /api/properties
  async list(query: PropertiesListQuery = {}): Promise<PagedResultDto<PropertyListItemDto>> {
    const params = buildQuery(query as Record<string, unknown>);
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
