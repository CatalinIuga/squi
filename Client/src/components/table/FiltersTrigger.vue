<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { ListFilter } from "lucide-vue-next";
</script>

<template>
  <Button
    class="relative flex items-center text-sm gap-2"
    variant="outline"
    size="sm"
  >
    <ListFilter :size="16" />
    Filters
    <div
      class="absolute top-0 right-0 -mt-1 -mr-1 flex items-center justify-center w-4 h-4 bg-primary text-primary-foreground rounded-full"
    >
      !
    </div>
  </Button>
</template>
