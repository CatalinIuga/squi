<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { ChevronLeftIcon, ChevronRightIcon } from "lucide-vue-next";

import { useSquiStore } from "@/lib/store";
import { storeToRefs } from "pinia";
import { computed, watch } from "vue";

const store = useSquiStore();

const rowCount = computed(() => store.tableSchema?.rowCount ?? 0);

const { limit, offset } = storeToRefs(store);

watch([limit, offset], () => {
  if (limit.value < 1) store.limit = 1;
  if (offset.value < 0) store.offset = 0;
});

function setOffset(value: number) {
  if (value < 0) return;
  store.offset = value;
}
</script>

<template>
  <div class="ml-auto flex items-center gap-0">
    <!-- No. of rows -->
    <TooltipProvider :delay-duration="100">
      <Tooltip>
        <TooltipTrigger class="cursor-default">
          <div class="text-xs font-mono font-medium mr-2">
            {{ rowCount }} rows
          </div>
        </TooltipTrigger>
        <TooltipContent class="bg-background border text-foreground">
          TOTAL ROWS
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
    <!-- Prev -->
    <Button
      class="flex items-center rounded-r-none size-8 gap-1"
      variant="outline"
      size="icon"
      @click="setOffset(offset - limit)"
    >
      <ChevronLeftIcon :size="24" />
    </Button>

    <!-- Limit -->
    <TooltipProvider :delay-duration="100">
      <Tooltip>
        <TooltipTrigger>
          <!-- TODO: test bigger value for max -->
          <Input
            type="number"
            min="1"
            max="100"
            placeholder="50"
            class="w-14 rounded-none focus-visible:ring-0 border-l-0 no-number-input text-center h-8"
            v-model="limit"
          />
        </TooltipTrigger>
        <TooltipContent class="bg-background border text-foreground">
          LIMIT
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>

    <!-- Offset -->
    <TooltipProvider :delay-duration="100">
      <Tooltip>
        <TooltipTrigger>
          <Input
            :min="0"
            placeholder="0"
            type="number"
            class="w-14 rounded-none border-x-0 focus-visible:ring-0 text-center no-number-input h-8"
            v-model="offset"
          />
        </TooltipTrigger>
        <TooltipContent class="bg-background border text-foreground">
          OFFSET
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>

    <!-- Next -->
    <Button
      class="flex items-center rounded-l-none gap-1 size-8"
      variant="outline"
      size="icon"
      @click="setOffset(offset + limit)"
    >
      <ChevronRightIcon :size="24" />
    </Button>
  </div>
</template>
