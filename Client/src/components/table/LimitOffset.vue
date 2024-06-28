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
import { watch } from "vue";

const store = useSquiStore();

const { limit, offset } = storeToRefs(store);

watch([limit, offset], () => {
  if (limit.value < 1) store.limit = 1;
  if (offset.value < 0) store.offset = 0;
});
</script>

<template>
  <div class="ml-auto flex items-center gap-2">
    <!-- No. of rows -->
    <div class="text-sm">{{ 0 }} rows</div>
    <!-- Prev -->
    <Button
      class="flex items-center size-8 gap-1"
      variant="ghost"
      size="icon"
      @click="offset -= limit"
      :disabled="offset < 1"
    >
      <ChevronLeftIcon :size="24" />
    </Button>

    <!-- Limit -->
    <TooltipProvider :delay-duration="300">
      <Tooltip>
        <TooltipTrigger>
          <Input
            type="number"
            min="1"
            placeholder="50"
            class="w-14 no-number-input text-center h-8"
            v-model="limit"
          />
        </TooltipTrigger>
        <TooltipContent class="bg-background border text-foreground">
          LIMIT
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>

    <!-- Offset -->
    <TooltipProvider :delay-duration="300">
      <Tooltip>
        <TooltipTrigger>
          <Input
            :min="0"
            placeholder="0"
            type="number"
            class="w-14 text-center no-number-input h-8"
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
      class="flex items-center gap-1 size-8"
      variant="ghost"
      size="icon"
      @click="offset += limit"
    >
      <ChevronRightIcon :size="24" />
    </Button>
  </div>
</template>
