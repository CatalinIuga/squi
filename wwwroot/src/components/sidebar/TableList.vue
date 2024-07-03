<script setup lang="ts">
import ThemeSwitcher from "@/components/sidebar/ThemeSwitcher.vue";
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
  <div class="flex flex-col justify-between h-full overflow-hidden">
    <div class="p-2 h-14 border-b">
      <Input
        v-model="filterTables"
        placeholder="Search tables"
        class="w-full mr-2"
      />
    </div>
    <div class="flex mt-1 px-2 flex-col flex-1 gap-[2px] overflow-y-auto">
      <Button
        v-for="table in filteredTables"
        :key="table"
        size="sm"
        :variant="table === currentTable ? 'secondary' : 'ghost'"
        @click="store.setTable(table)"
        class="w-full flex-shrink-0 capitalize justify-start"
        :class="currentTable === table ? 'opacity-80' : ''"
      >
        <Table2Icon class="mr-2 size-4" />
        {{ table }}
      </Button>
    </div>
    <div class="p-2 border-t">
      <Button size="sm" variant="outline" class="w-full">Add Table</Button>
    </div>
    <div class="flex items-center border-t p-1">
      <ThemeSwitcher />
      <!-- Maybe add connection info here? -->
    </div>
  </div>
</template>
