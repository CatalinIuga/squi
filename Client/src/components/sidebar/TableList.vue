<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Table2Icon } from "lucide-vue-next";

import { fetchTables } from "@/lib/services";
import { useSquiStore } from "@/lib/store";
import { unrwapResult } from "@/lib/utils";
import { computed, onMounted, ref, watch } from "vue";

const store = useSquiStore();

const currentTable = computed(() => store.table);
const tables = ref<string[]>([]);
const filterTables = ref<string>("");
const filteredTables = ref<string[]>([]);

onMounted(async () => {
  tables.value = unrwapResult(await fetchTables());
  updateFilteredTables();
});

const updateFilteredTables = () => {
  filteredTables.value = tables.value.filter((table) =>
    table.toLowerCase().includes(filterTables.value.toLowerCase())
  );
};

watch(filterTables, updateFilteredTables);
</script>

<template>
  <div class="p-3 space-y-2">
    <Input v-model="filterTables" placeholder="Search tables" class="w-full" />
    <div class="flex flex-col gap-[2px]">
      <Button
        v-for="table in filteredTables"
        :key="table"
        size="sm"
        :variant="table === currentTable ? 'outline' : 'ghost'"
        @click="store.setTable(table)"
        class="w-full capitalize justify-start"
      >
        <Table2Icon class="mr-2 size-4" />
        {{ table }}
      </Button>
    </div>
  </div>
</template>
