import { http } from '@/shared/api/http';
import type { PagedResultDto } from '@/shared/api/dtos/common/paged-result.dto';
import type { PropertyDetailsDto } from "@/shared/api/dtos/properties/property-details.dto";
import type { PropertyListItemDto } from "@/shared/api/dtos/properties/property-list-item.dto";
import type { PropertiesListQuery } from "@/shared/types/queries"
import { serializeQuery } from '@/shared/api/query';
import routes from "@/shared/routes.ts"

// Note to myself: Pages / stores MUST use this module instead of http directly
export const propertiesApi = {
  // GET /api/properties
  async list(
    query: PropertiesListQuery = {},
  ): Promise<PagedResultDto<PropertyListItemDto>> {
    const queryString = serializeQuery(query);

    return http.get<PagedResultDto<PropertyListItemDto>>(
      routes.api.properties.list(queryString),
    );
  },

  // GET /api/properties/{id}
  async getById(id: string): Promise<PropertyDetailsDto> {
    if (!id) {
      throw new Error('propertiesApi.getById: id is required');
    }

    return http.get<PropertyDetailsDto>(routes.api.properties.getById(id));
  },
};
