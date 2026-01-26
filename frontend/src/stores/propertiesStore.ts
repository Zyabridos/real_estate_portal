import { defineStore } from 'pinia'
import type { PropertyListItem } from "@/shared/types/properties";
import type { ApiError } from '@/shared/types/errors'
import type { UIState } from '@/shared/types/ui'
interface PropertiesState {
  items: PropertyListItem[];
  fetchStatus: UIState;
  error: ApiError | null;
}

export const usePropertiesStore = defineStore('properties', {
  state: (): PropertiesState => ({
    items: [],
    fetchStatus: 'idle',
    error: null,
  }),
})
