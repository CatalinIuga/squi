<script setup lang="ts">
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { fetchTables } from "@/lib/services";
import { useSquiStore } from "@/lib/store";
import { unrwapResult } from "@/lib/utils";
import { computed, onMounted, ref, watch } from "vue";

const store = useSquiStore();

const currentTable = computed(() => store.table);
const tables = ref<string[]>([]);
const openMenu = computed(() => store.openMenu);

const filterTables = ref<string>("");
const filteredTables = ref<string[]>([]);

onMounted(async () => {
  tables.value = unrwapResult(await fetchTables());
  updateFilteredTables();
  if (currentTable.value) {
    store.setOpenMenu(false);
  } else {
    store.setOpenMenu(true);
  }
});

watch(currentTable, (newTable) => {
  if (newTable) {
    store.setOpenMenu(false);
  } else {
    store.setOpenMenu(true);
  }
});

const updateFilteredTables = () => {
  filteredTables.value = tables.value.filter((table) =>
    table.toLowerCase().includes(filterTables.value.toLowerCase())
  );
};

watch(filterTables, updateFilteredTables);
</script>

<template>
  <article
    v-if="openMenu"
    class="h-full flex justify-center items-start overflow-y-auto py-10"
  >
    <Card class="w-[360px] h-fit mb-20">
      <CardHeader class="border-b">
        <CardTitle>Open a table</CardTitle>
      </CardHeader>
      <CardContent class="py-6 flex flex-col gap-3">
        <CardDescription>
          <Input
            v-model="filterTables"
            class="w-full"
            placeholder="Search for a table"
          />
        </CardDescription>
        <CardDescription class="flex flex-col gap-1">
          <Label class="font-bold text-xl">All tables</Label>
          <Button
            variant="ghost"
            :disabled="currentTable === table"
            @click="store.setTable(table)"
            v-for="(table, id) in filteredTables"
            :key="id"
            class="w-full justify-start"
          >
            {{ table }}
          </Button>
        </CardDescription>
      </CardContent>
    </Card>
  </article>
</template>
