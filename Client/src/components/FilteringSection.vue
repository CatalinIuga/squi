<script setup lang="ts">
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { InfoIcon } from "lucide-vue-next";
import { ref } from "vue";

interface Props {
  columns: string[];
}

defineProps<Props>();

const operations = {
  eq: {
    label: "equals",
    value: "eq",
    hasValue: true,
  },
  neq: {
    label: "not equals",
    value: "neq",
    hasValue: true,
  },
  gt: {
    label: "greater than",
    value: "gt",
    hasValue: true,
  },
  gte: {
    label: "greater than or equals",
    value: "gte",
    hasValue: true,
  },
  lt: {
    label: "less than",
    value: "lt",
    hasValue: true,
  },
  lte: {
    label: "less than or equals",
    value: "lte",
    hasValue: true,
  },
  isNull: {
    label: "is null",
    value: "isNull",
    hasValue: false,
  },
  isNotNull: {
    label: "is not null",
    value: "isNotNull",
    hasValue: false,
  },

  like: {
    label: "like",
    value: "like",
    hasValue: true,
  },
  notLike: {
    label: "not like",
    value: "notLike",
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
</script>

<template>
  <section class="flex justify-between gap-2 px-4 py-2 border-b-[1px]">
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
        <div class="rounded-md bg-secondary px-2 py-1">where</div>
        <Select class="w-[200px]" v-model:model-value="filter.column">
          <SelectTrigger>
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup label="Columns">
              <SelectItem
                v-for="column in columns"
                :key="column"
                :value="column"
              >
                {{ column }}
              </SelectItem>
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select class="w-[200px]" v-model:model-value="filter.operator">
          <SelectTrigger>
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup label="Operations">
              <SelectItem
                v-for="(operation, key) in operations"
                :key="key"
                :value="key"
              >
                {{ operation.label }}
              </SelectItem>
            </SelectGroup>
          </SelectContent>
        </Select>

        <input
          v-if="operations[filter.operator].hasValue"
          v-model="filter.value"
          class="w-[200px] rounded-md border-[1px] border-gray-300 px-2 py-1"
          type="text"
        />
      </div>
    </div>

    <div class="flex justify-between items-center">
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
