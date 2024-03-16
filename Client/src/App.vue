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
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import {
  ChevronLeftIcon,
  ChevronRightIcon,
  DownloadIcon,
  ListFilter,
  PlusIcon,
  RefreshCw,
  XIcon,
} from "lucide-vue-next";

import ColumnsSelector from "./components/ColumnsSelector.vue";
import ThemeSwitcher from "./components/ThemeSwitcher.vue";

import { onMounted, ref, watch } from "vue";
import AgGridTable from "./components/AgGridTable.vue";
import { getTables } from "./service/dataService";

const tables = ref<Array<string>>([]);
const limit = ref<number>(50);
const offset = ref<number>(0);
const currentTable = ref<string | null>(localStorage.getItem("currentTable"));
const openTables = ref<Array<string>>(
  JSON.parse(localStorage.getItem("openTables") || "[]")
);

// check if offset or limit go below 0
watch(
  [() => limit.value, () => offset.value],
  ([newLimit, newOffset]) => {
    if (newLimit < 1) {
      limit.value = 1;
    }
    if (newOffset < 0) {
      offset.value = 0;
    }
  },
  { deep: true }
);

const removeTable = (table: string) => {
  openTables.value = openTables.value.filter((t) => t !== table);
  if (currentTable.value === table) {
    currentTable.value = null;
  }
};
const addTable = (table: string) => {
  if (!openTables.value.includes(table)) {
    openTables.value.push(table);
    currentTable.value = table;
  }
};

watch(
  () => openTables.value,
  (newOpenTables) => {
    localStorage.setItem("openTables", JSON.stringify(newOpenTables));
  },
  { deep: true, immediate: true }
);

watch(
  () => currentTable.value,
  (newTable) => {
    if (newTable) {
      addTable(newTable);
    }
    localStorage.setItem("currentTable", newTable || "");
  }
);

const filterTables = ref<string>("");
const filteredTables = ref<Array<string>>(tables.value);

// Filter tables
watch(
  () => filterTables.value,
  (newFilter) => {
    if (newFilter) {
      filteredTables.value = tables.value.filter((table) =>
        table.toLowerCase().includes(newFilter.toLowerCase())
      );
    } else {
      filteredTables.value = tables.value;
    }
  }
);

// Table reference for calling exposed methods
const tableRef = ref<InstanceType<typeof AgGridTable> | null>(null);

onMounted(async () => {
  tables.value = await getTables();
  filteredTables.value = tables.value;
});
</script>

<template>
  <main class="mx-auto h-full flex-col overflow-hidden">
    <!-- Table selector -->
    <header
      class="mx-auto flex h-[50px] items-center gap-2 border-b-[1px] px-4"
    >
      <div class="flex items-center">
        <img class="size-20 mr-2" src="/squi.svg" alt="logo" />
      </div>
      <!-- Refresh button -->
      <Button
        class="flex items-center gap-1 size-8"
        variant="outline"
        @click="tableRef?.refreshGrid()"
        size="icon"
      >
        <RefreshCw :size="16" />
      </Button>
      <div
        class="flex flex-1 items-centerp pt-2 pb-1 gap-2 overflow-x-scroll smol-bar"
      >
        <!-- Buttons for all open tables -->
        <Button
          v-for="(table, id) in openTables"
          :key="id"
          class="flex items-center gap-2"
          :class="
            currentTable === table ? 'bg-slate-100 hover:bg-slate-100/80' : ''
          "
          variant="outline"
          size="sm"
          :active="table === currentTable"
          @click="currentTable = table"
        >
          <div class="">{{ table }}</div>
          <XIcon
            @click="
              (e) => {
                e.stopImmediatePropagation();
                removeTable(table);
              }
            "
            class="hover:cursor- pr-0"
            :size="12"
          />
        </Button>
      </div>
      <!-- Open table button -->
      <Button
        v-if="openTables.length !== tables.length"
        class="size-8"
        :class="
          openTables.length !== 0 ? 'bg-slate-100 hover:bg-slate-100/80' : ''
        "
        @click="currentTable = null"
        variant="outline"
        size="icon"
      >
        <PlusIcon :size="16" />
      </Button>

      <!-- Settings -->
      <div class="ml-auto gap-2 flex items-center">
        <!-- Download Button -->
        <Button
          class="flex items-center gap-1 size-8"
          variant="outline"
          size="icon"
        >
          <DownloadIcon :size="16" />
        </Button>
        <!-- Theme swticher -->
        <ThemeSwitcher />
      </div>
    </header>

    <article v-if="currentTable" class="h-full flex flex-col w-full">
      <!-- Table options -->
      <section
        v-if="tableRef"
        class="flex h-14 items-center gap-2 px-4 py-3 border-b-[1px]"
      >
        <!-- Filters -->
        <Button
          class="flex items-center text-sm gap-2"
          variant="outline"
          size="sm"
        >
          <ListFilter :size="16" />
          Filters
        </Button>
        <!-- Columns -->
        <ColumnsSelector
          v-if="tableRef"
          :allColumns="tableRef.allColumns"
          :displayedColumns="tableRef.displayedColumns"
          :toggleColumn="tableRef.toggleColumn"
        />
        <!-- Add new -->
        <Button
          @click="tableRef.addRow"
          class="flex text-sm"
          variant="default"
          size="sm"
        >
          Add record
        </Button>
        <!-- Save Changes -->
        <Button
          class="flex text-sm bg-green-600 hover:bg-green-600/80"
          v-if="tableRef.changes > 0"
          @click="tableRef.saveChanges"
          variant="default"
          size="sm"
        >
          Save
          {{
            tableRef.changes +
            " " +
            (tableRef.changes > 1 ? "changes" : "change")
          }}
        </Button>
        <!-- Discard Changes -->
        <Button
          v-if="tableRef.changes > 0"
          @click="tableRef.discardChanges"
          class="flex text-sm underline"
          variant="ghost"
          size="sm"
        >
          Discard changes
        </Button>
        <!-- Delete records -->
        <Button
          v-if="tableRef.selectedRowsCount > 0"
          @click="tableRef.deleteSelectedRows"
          class="flex text-sm"
          variant="destructive"
          size="sm"
        >
          Delete {{ tableRef.selectedRowsCount }} records
        </Button>

        <div class="ml-auto flex items-center gap-2">
          <!-- No. of rows -->
          <div class="">{{ tableRef.rowCounter }} rows</div>
          <!-- Prev -->
          <Button
            class="flex items-center size-8 gap-1"
            variant="ghost"
            size="icon"
            @click="offset -= limit"
          >
            <ChevronLeftIcon :size="24" />
          </Button>

          <!-- Limit -->
          <TooltipProvider :delay-duration="300">
            <Tooltip>
              <TooltipTrigger>
                <Input type="number" min="1" class="w-20 h-8" v-model="limit" />
              </TooltipTrigger>
              <TooltipContent class="bg-background border text-foreground">
                LIMIT
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>

          <!-- Offset -->
          <TooltipProvider :delay-duration="300">
            <Tooltip>
              <TooltipTrigger>
                <Input
                  min="0"
                  type="number"
                  class="w-20 h-8"
                  v-model="offset"
                />
              </TooltipTrigger>
              <TooltipContent class="bg-background border text-foreground">
                OFFSET
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>

          <!-- Next -->
          <Button
            class="flex items-center gap-1 size-8"
            variant="ghost"
            size="icon"
            @click="offset += limit"
          >
            <ChevronRightIcon :size="24" />
          </Button>
        </div>
      </section>
      <!-- Table -->
      <output class="h-full overflow-auto pb-12">
        <AgGridTable
          :limit="limit"
          :offset="offset"
          :table="currentTable"
          ref="tableRef"
        />
      </output>
    </article>

    <article
      v-else
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
              @click="currentTable = table"
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
  </main>
</template>
