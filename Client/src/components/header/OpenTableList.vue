<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { useSquiStore } from "@/lib/store";
import { XIcon } from "lucide-vue-next";
import { computed, onMounted, ref, watch } from "vue";

const store = useSquiStore();
const currentTable = computed(() => store.table);
const openTables = ref<Set<string>>(new Set());

onMounted(() => {
  if (currentTable.value) {
    openTables.value.add(currentTable.value);
  }
});

const closeTable = (table: string) => {
  openTables.value.delete(table);
  if (currentTable.value === table) {
    store.table = openTables.value.values().next().value;
  }
};

watch(currentTable, (table) => {
  if (table) {
    openTables.value.add(table);
  } else {
    openTables.value.clear();
  }
});
</script>

<template>
  <div
    class="flex flex-1 items-centerp pt-2 pb-1 gap-2 overflow-x-scroll smol-bar"
  >
    <Button
      v-for="table in openTables"
      class="flex items-center gap-2"
      :class="
        table === currentTable ? 'bg-secondary hover:bg-secondary/80' : ''
      "
      variant="outline"
      size="sm"
      @click="store.setTable(table)"
    >
      <div>{{ table }}</div>
      <XIcon
        @click="
          (e) => {
            e.stopImmediatePropagation();
            closeTable(table);
          }
        "
        class="hover:cursor- pr-0"
        :size="12"
      />
    </Button>
  </div>
</template>
