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

const { allColumns, displayedColumns, toggleColumn } = defineProps({
  allColumns: {
    type: Array as PropType<string[]>,
    required: true,
  },
  displayedColumns: {
    type: Array as PropType<string[]>,
    required: true,
  },
  toggleColumn: {
    type: Function as PropType<(col: string) => void>,
    required: true,
  },
});

const filterFunction = (list: any[], term: string) =>
  list.filter((i) => i.toLowerCase()?.includes(term.toLowerCase()));
</script>

<template>
  <Popover>
    <PopoverTrigger as-child>
      <Button
        class="relative flex items-center text-sm gap-2"
        variant="outline"
        size="sm"
      >
        <SlidersHorizontal :size="16" />
        Columns
        <!-- icon when not all columns are selected! -->
        <div
          v-if="displayedColumns.length < allColumns.length"
          class="absolute top-0 right-0 -mt-1 -mr-1 flex items-center justify-center w-4 h-4 bg-primary text-primary-foreground rounded-full"
        >
          !
        </div>
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-[200px] p-0">
      <Command :multiple="true" :filter-function="filterFunction">
        <div class="px-2 py-2 flex items-center justify-between">
          <h3 class="text-sm font-semibold">Toggle columns</h3>
          <Button
            variant="link"
            size="sm"
            class="h-5"
            @click="
              toggleColumn(
                displayedColumns.length === allColumns.length
                  ? '__none'
                  : '__all'
              )
            "
          >
            {{
              displayedColumns.length === allColumns.length
                ? "Hide all"
                : "Show all"
            }}
          </Button>
        </div>
        <CommandSeparator />
        <div class="m-1 border rounded-md">
          <CommandInput
            class="w-full h-8 p-0 text-xs"
            placeholder="Search columns"
          />
        </div>
        <CommandList :align="'start'">
          <CommandEmpty>No columns found</CommandEmpty>
          <CommandGroup>
            <template v-for="col in allColumns" :key="col">
              <CommandItem :value="col" @click="toggleColumn(col)">
                <div
                  class="mr-2 flex h-4 w-4 items-center justify-center rounded-sm"
                  :class="
                    displayedColumns.includes(col)
                      ? 'bg-primary text-primary-foreground'
                      : 'bg-gray-200 [&_svg]:invisible'
                  "
                >
                  <CheckIcon class="size-4" />
                </div>
                <span class="text-pretty">{{ col }}</span>
              </CommandItem>
            </template>
          </CommandGroup>
          <CommandSeparator />
          <div class="flex items-center justify-evenly gap-1 m-1">
            <Button class="h-6" variant="default" size="sm">New</Button>
            <Button class="h-6" variant="outline" size="sm"
              >Edit columns</Button
            >
          </div>
        </CommandList>
      </Command>
    </PopoverContent>
  </Popover>
</template>
