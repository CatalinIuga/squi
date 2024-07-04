<script setup lang="ts">
import ColumnsSelector from "./ColumnsSelector.vue";
import EditingButtons from "./EditingButtons.vue";
import FilteringSection from "./FilteringSection.vue";
import FiltersTrigger from "./FiltersTrigger.vue";
import LimitOffset from "./LimitOffset.vue";
import RefreshTrigger from "./RefreshTrigger.vue";
import TableGrid from "./grid/TableGrid.vue";

import { fetchTables } from "@/lib/services";
import { useSquiStore } from "@/lib/store";
import { unrwapResult } from "@/lib/utils";

import { computed, onMounted } from "vue";
import SidebarTrigger from "./SidebarTrigger.vue";

const store = useSquiStore();

const table = computed(() => store.table);

onMounted(async () => {
  const tables = unrwapResult(await fetchTables());
  store.setTable(tables[0]);
});
</script>

<template>
  <article v-if="table" class="h-full min-w-[0] flex-1 flex flex-col w-full">
    <section class="flex h-14 items-center gap-2 px-2 py-3 border-b-[1px]">
      <SidebarTrigger />
      <FiltersTrigger />
      <ColumnsSelector />
      <EditingButtons />
      <LimitOffset />
      <RefreshTrigger />
    </section>
    <FilteringSection />
    <output class="relative h-full overflow-auto">
      <TableGrid />
    </output>
  </article>
</template>
