<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { XIcon } from "lucide-vue-next";
</script>

<template>
  <div
    class="flex flex-1 items-centerp pt-2 pb-1 gap-2 overflow-x-scroll smol-bar"
  >
    <Button
      class="flex items-center gap-2"
      :class="'bg-secondary hover:bg-secondary/80'"
      variant="outline"
      size="sm"
    >
      <div class="">table</div>
      <XIcon
        @click="
          (e) => {
            e.stopImmediatePropagation();
          }
        "
        class="hover:cursor- pr-0"
        :size="12"
      />
    </Button>
  </div>
</template>
