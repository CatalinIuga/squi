<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { useSquiStore } from "@/lib/store";
import { ListFilter } from "lucide-vue-next";
import { computed } from "vue";

const store = useSquiStore();

const showFilters = computed(() => store.showFilters);
const filtersCount = computed(() => store.filters.length);
</script>

<template>
  <Button
    class="relative flex items-center text-sm gap-2"
    :class="[showFilters ? 'bg-accent/80' : '']"
    variant="outline"
    size="sm"
    @click="store.setShowFilters(!showFilters)"
  >
    <ListFilter :size="16" />
    Filters
    <div
      v-if="filtersCount"
      class="absolute text-xs top-0 right-0 -mt-1 -mr-1 flex items-center justify-center w-4 h-4 bg-primary text-primary-foreground rounded-full"
    >
      {{ filtersCount }}
    </div>
  </Button>
</template>
