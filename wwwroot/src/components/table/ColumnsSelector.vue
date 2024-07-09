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
import { useSquiStore } from "@/lib/store";
import { CheckIcon, SlidersHorizontal } from "lucide-vue-next";
import { computed } from "vue";

const store = useSquiStore();

const columns = computed(() => store.tableColumns);

const selectedCount = computed(
  () => columns.value.filter((col) => col.selected).length
);

const filterFunction = (list: any[], term: string) =>
  list.filter((i) => i.toLowerCase()?.includes(term.toLowerCase()));
</script>

<template>
  <Popover>
    <PopoverTrigger as-child>
      <Button
        class="relative flex data-[state=open]:bg-accent/80 items-center text-sm gap-2 active:bg-accent/80"
        variant="outline"
        size="sm"
      >
        <SlidersHorizontal :size="16" />
        Columns
        <div
          v-if="selectedCount !== columns.length"
          class="absolute text-xs top-0 right-0 -mt-1 -mr-1 flex items-center justify-center w-4 h-4 bg-primary text-primary-foreground rounded-full"
        >
          {{ selectedCount }}
        </div>
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-[200px] p-0">
      <Command :multiple="true" :filter-function="filterFunction">
        <div class="m-1 mt-2 border rounded-md">
          <CommandInput
            class="w-full h-8 p-0 text-xs"
            placeholder="Search columns"
          />
        </div>
        <CommandList :align="'start'">
          <CommandEmpty class="">No columns found</CommandEmpty>
          <CommandGroup class="max-h-60 overflow-y-auto">
            <template v-for="col in columns" :key="col">
              <CommandItem :value="col" @click="store.toggleColumn(col)">
                <div
                  class="mr-2 flex h-4 w-4 items-center justify-center rounded-sm"
                >
                  <CheckIcon :size="16" v-if="col.selected" class="size-4" />
                </div>
                <span class="text-pretty">{{ col.name }}</span>
              </CommandItem>
            </template>
          </CommandGroup>
          <CommandSeparator />
          <div class="flex items-center justify-evenly gap-1 m-1 px-2 py-1">
            <Button
              @click="store.toggleAllColumns"
              size="sm"
              variant="secondary"
              class="w-full font-semibold"
            >
              {{
                selectedCount < columns.length ? "Select all" : "Deselect all"
              }}
            </Button>
          </div>
        </CommandList>
      </Command>
    </PopoverContent>
  </Popover>
</template>
