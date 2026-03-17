<script setup lang="ts">
import type { LeadListItemDto } from "@/features/leads/api/dtos/lead-list-item.dto";
import type { SortDirection } from "@/shared/types/queries";

type Props = {
  items: LeadListItemDto[];
  sortBy: string;
  sortDirection: SortDirection;
  onSort: (value: string) => void;
  propertyTitleById: Record<string, string>;
};

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "open-message", payload: { id: string; fullName: string | null }): void;
}>();

function toRecord(item: LeadListItemDto): Record<string, unknown> {
  return item as unknown as Record<string, unknown>;
}

function readValue(item: LeadListItemDto, keys: string[]): unknown {
  const record = toRecord(item);

  for (const key of keys) {
    if (key in record) {
      return record[key];
    }
  }

  return null;
}

function readString(item: LeadListItemDto, keys: string[]): string | null {
  const value = readValue(item, keys);

  if (typeof value === "string" && value.trim().length > 0) {
    return value;
  }

  if (typeof value === "number") {
    return String(value);
  }

  return null;
}

function leadIdOf(item: LeadListItemDto): string {
  return readString(item, ["id", "Id", "leadId", "LeadId"]) ?? "";
}

function propertyIdOf(item: LeadListItemDto): string {
  return readString(item, ["propertyId", "PropertyId"]) ?? "";
}

function fullNameOf(item: LeadListItemDto): string | null {
  return readString(item, ["fullName", "FullName", "name", "Name"]);
}

function emailOf(item: LeadListItemDto): string | null {
  return readString(item, ["email", "Email"]);
}

function phoneOf(item: LeadListItemDto): string | null {
  return readString(item, ["phone", "Phone", "phoneNumber", "PhoneNumber"]);
}

function createdAtOf(item: LeadListItemDto): string | null {
  return readString(item, ["createdAt", "CreatedAt"]);
}

function propertyTitleOf(item: LeadListItemDto): string {
  const propertyId = propertyIdOf(item);

  if (!propertyId) {
    return "Unknown property";
  }

  return props.propertyTitleById[propertyId] ?? `Property #${propertyId}`;
}

function formatDate(value: string | null): string {
  if (!value) {
    return "—";
  }

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date);
}

function sortArrow(column: string): string {
  if (props.sortBy !== column) {
    return "";
  }

  return props.sortDirection === "asc" ? "↑" : "↓";
}

function openMessage(item: LeadListItemDto): void {
  emit("open-message", {
    id: leadIdOf(item),
    fullName: fullNameOf(item),
  });
}
</script>

<template>
  <div
    class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm"
    data-testid="leads-list-table"
  >
    <div class="overflow-x-auto">
      <table class="min-w-full divide-y divide-slate-200">
        <thead class="bg-slate-50">
        <tr>
          <th class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
            <button type="button" class="hover:text-slate-900" @click="onSort('PropertyId')">
              Property {{ sortArrow("PropertyId") }}
            </button>
          </th>

          <th class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
            <button type="button" class="hover:text-slate-900" @click="onSort('FullName')">
              Name {{ sortArrow("FullName") }}
            </button>
          </th>

          <th class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
            Email
          </th>

          <th class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
            Phone
          </th>

          <th class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
            <button type="button" class="hover:text-slate-900" @click="onSort('CreatedAt')">
              Created {{ sortArrow("CreatedAt") }}
            </button>
          </th>

          <th class="px-5 py-3 text-right text-xs font-semibold uppercase tracking-wide text-slate-500">
            Actions
          </th>
        </tr>
        </thead>

        <tbody class="divide-y divide-slate-100 bg-white">
        <tr
          v-for="item in items"
          :key="leadIdOf(item)"
          class="hover:bg-slate-50"
          :data-testid="`lead-row-${leadIdOf(item)}`"
        >
          <td class="px-5 py-4 text-sm text-slate-900">
            {{ propertyTitleOf(item) }}
          </td>

          <td class="px-5 py-4 text-sm text-slate-900">
            {{ fullNameOf(item) ?? "—" }}
          </td>

          <td class="px-5 py-4 text-sm text-slate-700">
            {{ emailOf(item) ?? "—" }}
          </td>

          <td class="px-5 py-4 text-sm text-slate-700">
            {{ phoneOf(item) ?? "—" }}
          </td>

          <td class="px-5 py-4 text-sm text-slate-700">
            {{ formatDate(createdAtOf(item)) }}
          </td>

          <td class="px-5 py-4 text-right">
            <button
              type="button"
              class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
              :data-testid="`open-message-${leadIdOf(item)}`"
              @click="openMessage(item)"
            >
              Open message
            </button>
          </td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
