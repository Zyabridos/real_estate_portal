<script setup lang="ts">
import { computed } from "vue";

import type { LeadListItemDto } from "@/shared/api/dtos/leads/lead-list-item.dto";
import type { SortDirection } from "@/shared/types/queries";

import SortableHeader from "@/shared/ui/table/SortableHeader.vue";
import routes from "@/shared/routes"
import { formatDate } from "@/shared/utils/formatters/dates"

type Group = {
  propertyId: string;
  items: LeadListItemDto[];
};

type Props = {
  items: LeadListItemDto[];
  sortBy: string;
  sortDirection: SortDirection;
  onSort: (sortKey: string) => void;
  propertyTitleById: Record<string, string>;
};

const props = defineProps<Props>();

const groups = computed<Group[]>(() => {
  const map = new Map<string, LeadListItemDto[]>();

  for (const lead of props.items) {
    const key = (lead.propertyId ?? "").trim() || "—";
    const arr = map.get(key) ?? [];
    arr.push(lead);
    map.set(key, arr);
  }

  return Array.from(map.entries()).map(([propertyId, items]) => ({ propertyId, items }));
});

function valueOrDash(v: string | null | undefined): string {
  const s = (v ?? "").trim();
  return s.length ? s : "—";
}

function propertyLabel(propertyId: string): string {
  return props.propertyTitleById[propertyId] ?? "Property";
}

const emit = defineEmits<{
  (e: "open-message", payload: { id: string; fullName: string | null }): void;
}>();

function propertyDetailsTo(propertyId: string) {
  return routes.app.properties.details(propertyId);
}

</script>

<template>
  <div class="w-full overflow-x-auto" data-testid="leads-table-grouped-wrap">
    <table class="min-w-[980px] w-full border-separate border-spacing-0" data-testid="leads-table-grouped">
      <caption class="sr-only">{{ $t("pages:leads.table.captionGrouped") }}</caption>

      <thead class="sticky top-0 bg-white">
      <tr class="border-b border-slate-200">
        <SortableHeader
          :label="$t('pages:leads.table.columns.fullName')"
          sortKey="FullName"
          :activeSortBy="sortBy"
          :activeSortDirection="sortDirection"
          :onSort="onSort"
        />
        <SortableHeader
          :label="$t('pages:leads.table.columns.email')"
          sortKey="Email"
          :activeSortBy="sortBy"
          :activeSortDirection="sortDirection"
          :onSort="onSort"
        />
        <SortableHeader
          :label="$t('pages:leads.table.columns.phone')"
          sortKey="PhoneNumber"
          :activeSortBy="sortBy"
          :activeSortDirection="sortDirection"
          :onSort="onSort"
        />
        <SortableHeader
          :label="$t('pages:leads.table.columns.status')"
          sortKey="Status"
          :activeSortBy="sortBy"
          :activeSortDirection="sortDirection"
          :onSort="onSort"
        />
        <SortableHeader
          :label="$t('pages:leads.table.columns.created')"
          sortKey="CreatedAt"
          :activeSortBy="sortBy"
          :activeSortDirection="sortDirection"
          :onSort="onSort"
        />
        <SortableHeader
          :label="$t('pages:leads.table.columns.updated')"
          sortKey="UpdatedAt"
          :activeSortBy="sortBy"
          :activeSortDirection="sortDirection"
          :onSort="onSort"
        />
        <th
          scope="col"
          class="whitespace-nowrap px-3 py-3 text-right text-xs font-semibold uppercase tracking-wide text-slate-600"
          data-testid="th-actions"
        >
          {{ $t("pages:leads.table.columns.actions") }}
        </th>
      </tr>
      </thead>

      <template v-for="g in groups" :key="g.propertyId">
        <tbody :data-testid="`lead-group-${g.propertyId}`">
        <!-- Group header -->
        <tr class="bg-slate-50">
          <th
            scope="rowgroup"
            :colspan="7"
            class="px-3 py-3 text-sm font-semibold text-slate-900"
            :data-testid="`lead-group-header-${g.propertyId}`"
          >
            <div class="flex flex-col gap-1">
              <div class="flex items-center gap-2">
                <RouterLink
                  v-if="g.propertyId !== '—'"
                  :to="propertyDetailsTo(g.propertyId)"
                  class="inline-flex items-center rounded-md px-1 py-0.5 text-slate-900 hover:underline focus:outline-none focus:ring-2 focus:ring-slate-300"
                  :data-testid="`lead-group-link-${g.propertyId}`"
                  :aria-label="`Open property ${propertyLabel(g.propertyId)}`"
                >
                  {{ propertyLabel(g.propertyId) }}
                </RouterLink>
                <span v-else>
                {{ propertyLabel(g.propertyId) }}
                </span>

                <span class="text-xs font-normal text-slate-600">({{ g.items.length }})</span>
              </div>
            </div>
          </th>
        </tr>

        <!-- Rows -->
        <tr
          v-for="lead in g.items"
          :key="lead.id"
          class="border-b border-slate-100 hover:bg-slate-50"
          :data-testid="`lead-row-${lead.id}`"
        >
          <td class="px-3 py-3 text-sm text-slate-900" :data-testid="`td-fullName-${lead.id}`">
            {{ valueOrDash(lead.fullName) }}
          </td>

          <td class="px-3 py-3 text-sm text-slate-700" :data-testid="`td-email-${lead.id}`">
            {{ valueOrDash(lead.email) }}
          </td>

          <td class="px-3 py-3 text-sm text-slate-700" :data-testid="`td-phone-${lead.id}`">
            {{ valueOrDash(lead.phoneNumber) }}
          </td>

          <td class="px-3 py-3 text-sm text-slate-700" :data-testid="`td-status-${lead.id}`">
            {{ valueOrDash(lead.status) }}
          </td>

          <td class="px-3 py-3 text-sm text-slate-700" :data-testid="`td-createdAt-${lead.id}`">
            {{ formatDate(lead.createdAt) }}
          </td>

          <td class="px-3 py-3 text-sm text-slate-700" :data-testid="`td-updatedAt-${lead.id}`">
            {{ formatDate(lead.updatedAt) }}
          </td>

          <td class="px-3 py-3 text-right" :data-testid="`td-actions-${lead.id}`">
            <button
              type="button"
              class="rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-slate-300"
              :data-testid="`lead-action-comment-${lead.id}`"
              :aria-label="`Open message for ${valueOrDash(lead.fullName)}`"
              @click="emit('open-message', { id: lead.id, fullName: lead.fullName ?? null })"
            >
              {{ $t("pages:leads.actions.comment") }}
            </button>
          </td>
        </tr>
        </tbody>
      </template>
    </table>
  </div>
</template>
