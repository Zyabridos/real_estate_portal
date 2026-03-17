import { defineStore } from "pinia";

import { agenciesApi } from "@/features/agencies/api/agenciesApi";

import type { AgencyListItemDto } from "@/features/agencies/api/dtos/agency-list-item.dto";
import type { AgencyDetailsDto } from "@/features/agencies/api/dtos/agency-details.dto";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { AgenciesListQuery } from "@/shared/types/queries";
import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";

let activeListController: AbortController | null = null;
let activeDetailsController: AbortController | null = null;

const agenciesStore = defineStore("agencies", {
  state: () => ({
    agencies: [] as AgencyListItemDto[],
    lastPagedResult: null as PagedResultDto<AgencyListItemDto> | null,
    lastQuery: {} as AgenciesListQuery,

    listStatus: "idle" as UIState,
    listError: null as ApiError | null,

    detailsById: {} as Record<number, AgencyDetailsDto>,
    detailsStatusById: {} as Record<number, UIState>,
    detailsErrorById: {} as Record<number, ApiError | null>,
  }),

  getters: {
    paging: (s) => ({
      page: s.lastPagedResult?.page ?? 1,
      pageSize: s.lastPagedResult?.pageSize ?? 20,
      totalItems: s.lastPagedResult?.totalItems ?? 0,
      totalPages: s.lastPagedResult?.totalPages ?? 0,
    }),

    isLoading: (s) => s.listStatus === "loading",

    getById: (s) => (id: number) => (id > 0 ? s.detailsById[id] ?? null : null),

    getDetailsStatus: (s) => (id: number) =>
      id > 0 ? s.detailsStatusById[id] ?? "idle" : "idle",

    getDetailsError: (s) => (id: number) =>
      id > 0 ? s.detailsErrorById[id] ?? null : null,
  },

  actions: {
    async fetchAgenciesList(query: AgenciesListQuery = {}) {
      activeListController?.abort();
      const controller = new AbortController();
      activeListController = controller;

      this.listStatus = "loading";
      this.listError = null;
      this.lastQuery = { ...query };

      try {
        const res = await agenciesApi.list(query, { signal: controller.signal });

        if (activeListController !== controller) return;

        this.agencies = res.items ?? [];
        this.lastPagedResult = res;
        this.listStatus = this.agencies.length === 0 ? "empty" : "success";
      } catch (err) {
        if (activeListController !== controller) return;

        this.agencies = [];
        this.listStatus = "error";
        this.listError = err as ApiError;
      }
    },

    async refreshList() {
      return this.fetchAgenciesList(this.lastQuery);
    },

    cancelListRequest() {
      activeListController?.abort();
      activeListController = null;
    },

    async fetchById(id: number, opts?: { force?: boolean }) {
      if (!Number.isInteger(id) || id <= 0) {
        return;
      }

      if (!opts?.force && this.detailsById[id]) {
        this.detailsStatusById[id] = "success";
        this.detailsErrorById[id] = null;
        return;
      }

      activeDetailsController?.abort();
      const controller = new AbortController();
      activeDetailsController = controller;

      this.detailsStatusById[id] = "loading";
      this.detailsErrorById[id] = null;

      try {
        const res = await agenciesApi.getById(id, { signal: controller.signal });

        if (activeDetailsController !== controller) return;

        this.detailsById[id] = res;
        this.detailsStatusById[id] = "success";
        this.detailsErrorById[id] = null;
      } catch (err) {
        if (activeDetailsController !== controller) return;

        this.detailsStatusById[id] = "error";
        this.detailsErrorById[id] = err as ApiError;
      }
    },

    cancelDetailsRequest() {
      activeDetailsController?.abort();
      activeDetailsController = null;
    },

    reset() {
      this.cancelListRequest();
      this.cancelDetailsRequest();

      this.agencies = [];
      this.lastPagedResult = null;
      this.lastQuery = {} as AgenciesListQuery;
      this.listStatus = "idle";
      this.listError = null;

      this.detailsById = {} as Record<number, AgencyDetailsDto>;
      this.detailsStatusById = {} as Record<number, UIState>;
      this.detailsErrorById = {} as Record<number, ApiError | null>;
    },
  },
});

export default agenciesStore;
