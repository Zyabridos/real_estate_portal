import { defineStore } from 'pinia'
import type { BrokerListItem } from "@/entities/brokers/model/types";
import type { ApiError } from '@/shared/types/errors'
import type { UIState } from '@/shared/types/ui'
interface BrokersState {
  items: BrokerListItem[];
  fetchStatus: UIState;
  error: ApiError | null;
}

export const useBrokersStore = defineStore('brokers', {
  state: (): BrokersState => ({
    items: [],
    fetchStatus: 'idle',
    error: null,
  }),
})
