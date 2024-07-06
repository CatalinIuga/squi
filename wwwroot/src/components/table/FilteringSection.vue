<script setup lang="ts">
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { useSquiStore } from "@/lib/store";
import { Operator } from "@/lib/types";
import { ChevronDownIcon, InfoIcon, XIcon } from "lucide-vue-next";
import { computed } from "vue";

const store = useSquiStore();

const columnsNames = computed(
  () =>
    store.tableSchema?.columns
      .map((col) => col.name)
      .filter((col) => col !== undefined) as string[]
);

const filters = computed(() => store.filters);
const showFilters = computed(() => store.showFilters);
</script>

<template>
  <section
    v-if="showFilters"
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
        <div class="rounded-md px-4 py-2 border cursor-not-allowed">where</div>
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
                v-for="column in columnsNames"
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
              {{ filter.operator }}
              <ChevronDownIcon class="size-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent class="w-56">
            <DropdownMenuItem
              class="flex justify-between items-center hover:bg-muted/60"
              v-for="(op, str) in Operator"
              :key="str"
              @click="filter.operator = op"
            >
              {{
                str
                  .replace(/([A-Z])/g, " $1")
                  .toLowerCase()
                  .trim()
              }}
              <span
                class="bg-secondary border dark:border-background shadow-sm text-xs rounded-md py-1 px-2"
              >
                {{ op }}
              </span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>

        <Input
          v-if="
            filter.operator !== Operator.IsNull &&
            filter.operator !== Operator.IsNotNull
          "
          placeholder="Value"
          v-model="filter.value"
          class="w-[200px] rounded-md px-2 py-1"
          type="text"
        />

        <Button
          class="px-2"
          variant="ghost"
          size="sm"
          @click="store.removeFilter(index)"
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
          @click="store.addFilter()"
        >
          Add filter
        </Button>
        <Button
          v-if="filters.length > 0"
          class="flex items-center text-sm gap-2"
          variant="link"
          size="sm"
          @click="store.setFilters([])"
        >
          Clear filters
        </Button>
      </div>
    </div>
  </section>
</template>
