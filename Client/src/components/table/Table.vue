<script setup lang="ts">
import CloseSideBar from "./CloseSidebar.vue";
import ColumnsSelector from "./ColumnsSelector.vue";
import EditingButtons from "./EditingButtons.vue";
import FilteringSection from "./FilteringSection.vue";
import FiltersTrigger from "./FiltersTrigger.vue";
import LimitOffset from "./LimitOffset.vue";
import TableGrid from "./grid/TableGrid.vue";

import { fetchTables } from "@/lib/services";
import { useSquiStore } from "@/lib/store";
import { unrwapResult } from "@/lib/utils";

import { computed, onMounted } from "vue";

const store = useSquiStore();

const table = computed(() => store.table);

onMounted(async () => {
  const tables = unrwapResult(await fetchTables());
  store.setTable(tables[0]);
});
</script>

<template>
  <article v-if="table" class="h-full min-w-[0] flex-1 flex flex-col w-full">
    <section class="flex h-14 items-center gap-2 px-4 py-3 border-b-[1px]">
      <CloseSideBar />
      <FiltersTrigger />
      <ColumnsSelector />
      <EditingButtons />
      <LimitOffset />
    </section>
    <FilteringSection />
    <output class="h-full overflow-auto">
      <TableGrid />
    </output>
  </article>
</template>
