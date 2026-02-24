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
    // list
    agencies: [] as AgencyListItemDto[],
    lastPagedResult: null as PagedResultDto<AgencyListItemDto> | null,
    lastQuery: {} as AgenciesListQuery,

    listStatus: "idle" as UIState,
    listError: null as ApiError | null,

    // details
    detailsById: {} as Record<string, AgencyDetailsDto>,
    detailsStatusById: {} as Record<string, UIState>,
    detailsErrorById: {} as Record<string, ApiError | null>,
  }),

  getters: {
    paging: (s) => ({
      page: s.lastPagedResult?.page ?? 1,
      pageSize: s.lastPagedResult?.pageSize ?? 20,
      totalItems: s.lastPagedResult?.totalItems ?? 0,
      totalPages: s.lastPagedResult?.totalPages ?? 0,
    }),

    isLoading: (s) => s.listStatus === "loading",

    getById: (s) => (id: string) => (id ? s.detailsById[id] ?? null : null),
    getDetailsStatus: (s) => (id: string) => (id ? s.detailsStatusById[id] ?? "idle" : "idle"),
    getDetailsError: (s) => (id: string) => (id ? s.detailsErrorById[id] ?? null : null),
  },

  actions: {
    // list
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

    // details
    async fetchById(id: string, opts?: { force?: boolean }) {
      const cleanId = String(id ?? "").trim();
      if (!cleanId) return;

      if (!opts?.force && this.detailsById[cleanId]) {
        this.detailsStatusById[cleanId] = "success";
        this.detailsErrorById[cleanId] = null;
        return;
      }

      activeDetailsController?.abort();
      const controller = new AbortController();
      activeDetailsController = controller;

      this.detailsStatusById[cleanId] = "loading";
      this.detailsErrorById[cleanId] = null;

      try {
        const res = await agenciesApi.getById(cleanId, { signal: controller.signal });

        if (activeDetailsController !== controller) return;

        this.detailsById[cleanId] = res;
        this.detailsStatusById[cleanId] = "success";
        this.detailsErrorById[cleanId] = null;
      } catch (err) {
        if (activeDetailsController !== controller) return;

        this.detailsStatusById[cleanId] = "error";
        this.detailsErrorById[cleanId] = err as ApiError;
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

      this.detailsById = {};
      this.detailsStatusById = {};
      this.detailsErrorById = {};
    },
  },
});

export default agenciesStore;
