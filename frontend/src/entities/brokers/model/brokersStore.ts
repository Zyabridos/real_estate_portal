import { defineStore } from "pinia";

import { brokersApi } from "@/features/brokers/api/brokersApi";

import type { BrokerListItemDto } from "@/features/brokers/api/dtos/broker-list-item.dto";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { BrokersListQuery } from "@/shared/types/queries";
import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";

let activeController: AbortController | null = null;

export const useBrokersStore = defineStore("brokers", {
  state: () => ({
    brokers: [] as BrokerListItemDto[],
    lastPagedResult: null as PagedResultDto<BrokerListItemDto> | null,
    lastQuery: {} as BrokersListQuery,

    listStatus: "idle" as UIState,
    listError: null as ApiError | null,
  }),

  getters: {
    paging: (state) => ({
      page: state.lastPagedResult?.page ?? 1,
      pageSize: state.lastPagedResult?.pageSize ?? 20,
      totalBrokers: state.lastPagedResult?.totalItems ?? 0,
      totalPages: state.lastPagedResult?.totalPages ?? 0,
    }),
    isLoading: (state) => state.listStatus === "loading",
  },

  actions: {
    async fetchList(query: BrokersListQuery = {}) {
      // cancel previous request
      activeController?.abort();
      const controller = new AbortController();
      activeController = controller;

      this.listStatus = "loading";
      this.listError = null;
      this.lastQuery = { ...query };

      try {
        const res = await brokersApi.list(query, { signal: controller.signal });

        // if another request has started, abrupt this one
        if (activeController !== controller) return;

        this.brokers = res.items ?? [];
        this.lastPagedResult = res;
        this.listStatus = this.brokers.length === 0 ? "empty" : "success";
      } catch (err) {
        // if the request been cancelled by new request, exit
        if (activeController !== controller) return;

        this.brokers = [];
        this.listStatus = "error";
        this.listError = err as ApiError;
      }
    },

    async refresh() {
      return this.fetchList(this.lastQuery);
    },

    cancelListRequest() {
      activeController?.abort();
      activeController = null;
    },

    reset() {
      this.cancelListRequest();

      this.brokers = [];
      this.lastPagedResult = null;
      this.lastQuery = {} as BrokersListQuery;

      this.listStatus = "idle";
      this.listError = null;
    },
  },
});
