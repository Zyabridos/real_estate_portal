import { defineStore } from 'pinia'
import type { PropertyListItem } from "@/shared/types/properties";
import type { ApiError } from '@/shared/types/errors'
import type { UIStatus } from '@/shared/types/ui'
interface PropertiesState {
  items: PropertyListItem[];
  fetchStatus: UIStatus;
  error: ApiError | null;
}

export const usePropertiesStore = defineStore('properties', {
  state: (): PropertiesState => ({
    items: [],
    fetchStatus: 'idle',
    error: null,
  }),
})
