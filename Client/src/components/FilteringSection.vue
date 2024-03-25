<script setup lang="ts">
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { ChevronDownIcon, InfoIcon, XIcon } from "lucide-vue-next";
import { ref, watch } from "vue";

interface Props {
  show: boolean;
  columns: string[];
}

defineProps<Props>();

const operations = {
  eq: {
    label: "equals",
    value: "=",
    hasValue: true,
  },
  neq: {
    label: "not equals",
    value: "<>",
    hasValue: true,
  },
  gt: {
    label: "greater",
    value: ">",
    hasValue: true,
  },
  gte: {
    label: "greater or equals",
    value: ">=",
    hasValue: true,
  },
  lt: {
    label: "less",
    value: "<",
    hasValue: true,
  },
  lte: {
    label: "less or equals",
    value: "<=",
    hasValue: true,
  },
  isNull: {
    label: "is null",
    value: "IS NULL",
    hasValue: false,
  },
  isNotNull: {
    label: "is not null",
    value: "IS NOT NULL",
    hasValue: false,
  },

  like: {
    label: "like",
    value: "LIKE",
    hasValue: true,
  },
  notLike: {
    label: "not like",
    value: "NOT LIKE",
    hasValue: true,
  },
};

type Filter = {
  column: string;
  operator: keyof typeof operations;
  value?: string | number;
};

// this might need to go to a global store
const filters = ref<Filter[]>([]);

const filtersDict = ref<string[]>([]);

watch(
  filters.value,
  (value) => {
    filtersDict.value = value.map(
      (filter) =>
        `${filter.column} ${operations[filter.operator].value} ${
          operations[filter.operator].hasValue ? `'${filter.value}'` : ""
        }`
    );

    console.log(filtersDict.value);
  },
  { deep: true }
);

defineExpose({ filters, filtersDict });
</script>

<template>
  <section
    v-if="show"
    class="flex justify-between gap-2 px-4 py-2 border-b-[1px]"
  >
    <div v-if="filters.length === 0" class="flex items-center gap-1">
      <InfoIcon :size="16" />
      <p class="text-sm">Use the filters to narrow down the results</p>
    </div>
    <div class="flex flex-col text-sm gap-2">
      <div
        v-for="(filter, index) in filters"
        :key="index"
        class="flex items-center gap-2"
      >
        <div class="rounded-md px-4 py-2 border">where</div>
        <DropdownMenu>
          <DropdownMenuTrigger as-child>
            <Button
              class="flex items-center w-40 justify-between px-2"
              variant="outline"
            >
              {{ filter.column }}
              <ChevronDownIcon class="size-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <div class="w-52 max-h-64 normal-scrollbar overflow-y-auto">
              <DropdownMenuItem
                v-for="column in columns"
                :key="column"
                @click="filter.column = column"
              >
                {{ column }}
              </DropdownMenuItem>
            </div>
          </DropdownMenuContent>
        </DropdownMenu>

        <DropdownMenu>
          <DropdownMenuTrigger as-child>
            <Button
              class="flex items-center w-40 justify-between px-2"
              variant="outline"
            >
              {{ operations[filter.operator].label }}
              <ChevronDownIcon class="size-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent class="w-56">
            <DropdownMenuItem
              class="flex justify-between items-center"
              v-for="(operation, key) in operations"
              :key="key"
              @click="filter.operator = key"
            >
              {{ operation.label }}
              <span class="bg-secondary shadow-sm text-xs rounded-md py-1 px-2">
                {{ operation.value }}
              </span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>

        <Input
          v-if="operations[filter.operator].hasValue"
          placeholder="Value"
          v-model="filter.value"
          class="w-[200px] rounded-md px-2 py-1"
          type="text"
        />

        <Button
          class="px-2"
          variant="ghost"
          size="sm"
          @click="filters.splice(index, 1)"
        >
          <XIcon class="size-4 text-red-500" />
        </Button>
      </div>
    </div>

    <div class="flex justify-between items-start">
      <div class="flex items-center gap-1">
        <Button
          class="flex items-center text-sm gap-2"
          variant="default"
          size="sm"
          @click="
            filters.push({ column: columns[0], operator: 'eq', value: '' })
          "
        >
          Add filter
        </Button>
        <Button
          v-if="filters.length > 0"
          class="flex items-center text-sm gap-2"
          variant="link"
          size="sm"
          @click="filters = []"
        >
          Clear filters
        </Button>
      </div>
    </div>
  </section>
</template>
