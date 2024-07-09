<script setup lang="ts">
import { Button } from "../ui/button";
import ColumnsSelector from "./ColumnsSelector.vue";
import FilteringSection from "./FilteringSection.vue";
import FiltersTrigger from "./FiltersTrigger.vue";
import LimitOffset from "./LimitOffset.vue";
import RefreshTrigger from "./RefreshTrigger.vue";
import TableGrid from "./TableGrid.vue";

import { useSquiStore } from "@/lib/store";

import { computed, ref } from "vue";
import SidebarTrigger from "./SidebarTrigger.vue";

const store = useSquiStore();
const table = computed(() => store.table);

const gridRef = ref();

function deleteRows() {
  gridRef.value.deleteSelectedRows();
}
</script>

<template>
  <article v-if="table" class="h-full min-w-[0] flex-1 flex flex-col w-full">
    <section class="flex h-14 items-center gap-2 px-2 py-3 border-b-[1px]">
      <SidebarTrigger />
      <FiltersTrigger />
      <ColumnsSelector />
      <Button variant="default" size="sm">Insert</Button>
      <Button class="bg-green-600 hover:bg-green-600/80" size="sm">Save</Button>
      <Button variant="outline" size="sm">Discard</Button>
      <Button @click="deleteRows" variant="destructive" size="sm"
        >Delete</Button
      >
      <LimitOffset />
      <RefreshTrigger />
    </section>
    <FilteringSection />
    <output class="relative h-full overflow-auto">
      <TableGrid ref="gridRef" />
    </output>
  </article>
</template>
