<script setup lang="ts">
import ColumnsSelector from "./ColumnsSelector.vue";
import EditingButtons from "./EditingButtons.vue";
import FilteringSection from "./FilteringSection.vue";
import FiltersTrigger from "./FiltersTrigger.vue";
import LimitOffset from "./LimitOffset.vue";
import TableGrid from "./grid/TableGrid.vue";

import { useSquiStore } from "@/lib/store";
import { computed } from "vue";

const store = useSquiStore();

const table = computed(() => store.table);
</script>

<template>
  <article v-if="table" class="h-full flex flex-col w-full">
    <section class="flex h-14 items-center gap-2 px-4 py-3 border-b-[1px]">
      <FiltersTrigger />
      <ColumnsSelector />
      <EditingButtons />
      <LimitOffset />
    </section>
    <FilteringSection />

    <output class="h-full overflow-auto pb-12">
      <TableGrid />
    </output>
  </article>
</template>
