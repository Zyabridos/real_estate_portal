<script setup lang="ts">
import SortableHeader from "@/shared/ui/table/SortableHeader.vue";
import type { LeadListItemDto } from "@/shared/api/dtos/leads/lead-list-item.dto";
import type { SortDirection } from "@/shared/types/queries";

type Props = {
  items: LeadListItemDto[];
  sortBy: string;
  sortDirection: SortDirection;
  onSort: (sortKey: string) => void;
  propertyTitleById: Record<string, string>;
};

const emit = defineEmits<{
  (e: "open-message", payload: { id: string; fullName: string | null }): void;
}>();

function formatDate(iso: string | null | undefined): string {
  if (!iso) return "—";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "—";
  return new Intl.DateTimeFormat(undefined, {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  }).format(d);
}

function valueOrDash(v: string | null | undefined): string {
  const s = (v ?? "").trim();
  return s.length ? s : "—";
}

const props = defineProps<Props>();

function propertyLabel(id: string): string {
  return props.propertyTitleById[id] ?? "Property";
}
</script>

<template>
  <div class="w-full overflow-x-auto" data-testid="leads-table-list-wrap">
    <table class="min-w-[980px] w-full border-separate border-spacing-0" data-testid="leads-table-list">
      <caption class="sr-only">{{ $t("pages:leads.table.captionList") }}</caption>

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
        <SortableHeader
          :label="$t('pages:leads.table.columns.property')"
          sortKey="PropertyId"
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

      <tbody data-testid="leads-tbody-list">
      <tr
        v-for="lead in items"
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

        <td class="px-3 py-3" :data-testid="`td-property-${lead.id}`">
          <div class="mt-1 font-mono text-[11px] text-slate-500" :data-testid="`lead-property-id-${lead.id}`">
            {{ lead.propertyId }}
          </div>
        </td>

        <td class="px-3 py-3 text-right" :data-testid="`td-actions-${lead.id}`">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-slate-300"
            :data-testid="`lead-action-comment-${lead.id}`"
            :aria-label="$t('pages:leads.table.openMessageAria', { name: valueOrDash(lead.fullName) })"
            @click="emit('open-message', { id: lead.id, fullName: lead.fullName ?? null })"
          >
            {{ $t("pages:leads.actions.comment") }}
          </button>
        </td>
      </tr>
      </tbody>
    </table>
  </div>
</template>
