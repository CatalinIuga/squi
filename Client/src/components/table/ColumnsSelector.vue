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
        <div
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
          <Button variant="link" size="sm" class="h-5" @click="">
            {{ "Show all" }}
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
          <CommandGroup class="max-h-60 overflow-y-auto">
            <template v-for="col in []" :key="col">
              <CommandItem :value="col" @click="">
                <div
                  class="mr-2 flex h-4 w-4 items-center justify-center rounded-sm"
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
