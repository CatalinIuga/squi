<script setup lang="ts">
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { CheckIcon, SlidersHorizontal } from "lucide-vue-next";
import { PropType } from "vue";

const { columns, filteredColumns, setFilteredColumns } = defineProps({
  columns: {
    type: Array as PropType<Array<string>>,
    required: true,
  },
  filteredColumns: {
    type: Array as PropType<Array<string>>,
    required: true,
  },
  setFilteredColumns: {
    type: Function as PropType<(columns: Array<string>) => void>,
    required: true,
  },
});

const filterFunction = (list: any[], term: string) =>
  list.filter((i) => i.toLowerCase()?.includes(term.toLowerCase()));

const changeColumns = (col: string) => {
  if (filteredColumns.includes(col)) {
    setFilteredColumns(filteredColumns.filter((c) => c !== col));
  } else {
    setFilteredColumns([...filteredColumns, col]);
  }
};
</script>

<template>
  <Popover>
    <PopoverTrigger as-child>
      <Button
        class="flex items-center text-sm gap-2"
        variant="outline"
        size="sm"
      >
        <SlidersHorizontal :size="16" />
        Columns
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-[200] p-0">
      <Command :multiple="true" :filter-function="filterFunction">
        <CommandInput class="w-full" placeholder="Search columns" />
        <CommandList :align="'start'">
          <CommandEmpty>No columns found</CommandEmpty>
          <CommandGroup>
            <template v-for="col in columns" :key="col">
              <CommandItem :value="col" @click="changeColumns(col)">
                <CheckIcon
                  :class="
                    filteredColumns.includes(col) ? 'visible' : 'invisible'
                  "
                />
                {{ col }}
              </CommandItem>
            </template>
          </CommandGroup>
          <CommandSeparator />
          <Button class="w-full" variant="outline" size="sm" @click="">
            Clear all
          </Button>
        </CommandList>
      </Command>
    </PopoverContent>
  </Popover>
</template>
