<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";

import { leadsApi } from "@/features/leads/api/leadsApi";
import { propertiesApi } from "@/features/properties/api/propertiesApi";

import type { ApiError } from "@/shared/types/errors";
import type { LeadListItemDto } from "@/features/leads/api/dtos/lead-list-item.dto";
import type { SortDirection } from "@/shared/types/queries";
import type { UIState } from "@/shared/types/ui";

import LoadingState from "@/shared/ui/states/LoadingState.vue";
import ErrorState from "@/shared/ui/states/ErrorState.vue";
import EmptyState from "@/shared/ui/states/EmptyState.vue";

import LeadMessageModal from "@/features/leads/list/components/LeadMessageModal.vue";

import LeadsTableGrouped from "@/features/leads/list/components/LeadsTableGrouped.vue";
import LeadsTableList from "@/features/leads/list/components/LeadsTableList.vue";

type ViewMode = "grouped" | "list";

const route = useRoute();
const router = useRouter();

const state = ref<UIState>("loading");
const error = ref<ApiError | null>(null);

const items = ref<LeadListItemDto[]>([]);
const totalItems = ref(0);

const propertyTitleById = ref<Record<string, string>>({});

const view = computed<ViewMode>(() => {
  const v = String(route.query.view ?? "").toLowerCase();
  return v === "list" ? "list" : "grouped";
});

const sortBy = computed(() => String(route.query.sortBy ?? "PropertyId"));
const sortDirection = computed<SortDirection>(() => (route.query.sortDirection === "desc" ? "desc" : "asc"));

const isLoading = computed(() => state.value === "loading");
const isError = computed(() => state.value === "error");
const isEmpty = computed(() => state.value === "empty");

async function loadPropertyMap(): Promise<void> {
  const propsResult = await propertiesApi.list({ page: 1, pageSize: 100 });
  const map: Record<string, string> = {};

  for (const p of propsResult.items ?? []) {
    map[p.id] = p.title;
  }

  propertyTitleById.value = map;
}

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;

  try {
    const [leadsResult] = await Promise.all([
      leadsApi.list({
        page: 1,
        pageSize: 20,
        sortBy: sortBy.value,
        sortDirection: sortDirection.value,
      }),
      loadPropertyMap(),
    ]);

    items.value = leadsResult.items ?? [];
    totalItems.value = leadsResult.totalItems ?? items.value.length;
    state.value = items.value.length === 0 ? "empty" : "ready";
  } catch (e) {
    error.value = e as ApiError;
    state.value = "error";
  }
}

function retry(): void {
  void load();
}

function setView(next: ViewMode): void {
  router.replace({ query: { ...route.query, view: next } });
}

function onSort(nextSortBy: string): void {
  const same = sortBy.value === nextSortBy;
  const nextDir: SortDirection = same ? (sortDirection.value === "asc" ? "desc" : "asc") : "asc";

  router.replace({ query: { ...route.query, sortBy: nextSortBy, sortDirection: nextDir } });
  void load();
}

onMounted(() => {
  const q = { ...route.query };

  if (!q.view) q.view = "grouped";
  if (!q.sortBy) q.sortBy = "PropertyId";
  if (!q.sortDirection) q.sortDirection = "asc";

  router.replace({ query: q });
  void load();
});

const isMessageOpen = ref(false);
const messageLeadId = ref<string | null>(null);
const messageFullName = ref<string | null>(null);

const messageText = ref<string | null>(null);
const messageLoading = ref(false);
const messageError = ref<string | null>(null);

function closeMessageModal(): void {
  isMessageOpen.value = false;
  messageLeadId.value = null;
  messageFullName.value = null;
  messageText.value = null;
  messageError.value = null;
  messageLoading.value = false;
}

async function openMessageModal(payload: { id: string; fullName: string | null }): Promise<void> {
  isMessageOpen.value = true;
  messageLeadId.value = payload.id;
  messageFullName.value = payload.fullName;
  messageText.value = null;
  messageError.value = null;

  messageLoading.value = true;
  try {
    const dto = await leadsApi.getById(payload.id);
    messageText.value = dto.message ?? null;
  } catch (e) {
    const err = e as ApiError;
    messageError.value = err.message ?? "Failed to load message.";
  } finally {
    messageLoading.value = false;
  }
}
</script>

<template>
  <section class="w-full px-6 py-4" data-testid="leads-list-page" aria-labelledby="leads-list-title">
    <header class="mb-6">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 id="leads-list-title" class="text-2xl font-semibold tracking-tight text-slate-900" data-testid="leads-list-title">
            Leads list
          </h1>
          <p class="mt-1 text-sm text-slate-600" data-testid="leads-list-subtitle">
            Grouped by PropertyId (default) or List view
          </p>
        </div>

        <!-- Toggle -->
        <div
          class="inline-flex rounded-xl border border-slate-200 bg-white p-1"
          role="group"
          aria-label="Leads view mode"
          data-testid="leads-view-toggle"
        >
          <button
            type="button"
            class="rounded-lg px-3 py-2 text-sm font-medium focus:outline-none focus:ring-2 focus:ring-slate-300"
            :class="view === 'grouped' ? 'bg-slate-900 text-white' : 'text-slate-700 hover:bg-slate-50'"
            :aria-pressed="view === 'grouped'"
            data-testid="view-grouped"
            @click="setView('grouped')"
          >
            Grouped
          </button>

          <button
            type="button"
            class="rounded-lg px-3 py-2 text-sm font-medium focus:outline-none focus:ring-2 focus:ring-slate-300"
            :class="view === 'list' ? 'bg-slate-900 text-white' : 'text-slate-700 hover:bg-slate-50'"
            :aria-pressed="view === 'list'"
            data-testid="view-list"
            @click="setView('list')"
          >
            List
          </button>
        </div>
      </div>

      <p class="mt-3 text-xs text-slate-500" data-testid="leads-total">
        Total: {{ totalItems }}
      </p>
    </header>

    <!-- States -->
    <LoadingState v-if="isLoading" data-testid="leads-list-loading" />

    <ErrorState
      v-else-if="isError"
      data-testid="leads-list-error"
      :message="error?.message ?? $t('errors:messages.unexpected')"
      :onRetry="retry"
    />

    <EmptyState v-else-if="isEmpty" data-testid="leads-list-empty" />

    <!-- Tables -->
    <div v-else class="max-w-6xl" data-testid="leads-list-content">
      <LeadsTableGrouped
        v-if="view === 'grouped'"
        :items="items"
        :sortBy="sortBy"
        :sortDirection="sortDirection"
        :onSort="onSort"
        :propertyTitleById="propertyTitleById"
        @open-message="openMessageModal"
      />

      <LeadsTableList
        v-else
        :items="items"
        :sortBy="sortBy"
        :sortDirection="sortDirection"
        :onSort="onSort"
        :propertyTitleById="propertyTitleById"
        @open-message="openMessageModal"
      />
    </div>

    <LeadMessageModal
      :isOpen="isMessageOpen"
      :leadId="messageLeadId"
      :fullName="messageFullName"
      :message="messageText"
      :isLoading="messageLoading"
      :errorMessage="messageError"
      :onClose="closeMessageModal"
    />

  </section>
</template>
