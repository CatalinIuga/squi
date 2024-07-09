<script setup lang="ts">
import { useSquiStore } from "@/lib/store";
import { ClientSideRowModelModule } from "@ag-grid-community/client-side-row-model";
import {
  ColDef,
  GridApi,
  GridOptions,
  GridReadyEvent,
  ModuleRegistry,
} from "@ag-grid-community/core";
import { AgGridVue } from "@ag-grid-community/vue3";
import { Loader2Icon } from "lucide-vue-next";
import { computed, onMounted, ref, shallowRef, watch } from "vue";

import { useToast } from "@/components/ui/toast";
import "@ag-grid-community/styles/ag-grid.css";
import "@ag-grid-community/styles/ag-theme-quartz.css";
import { useColorMode } from "@vueuse/core";

ModuleRegistry.registerModules([ClientSideRowModelModule]);

const { toast } = useToast();
const mode = useColorMode();
const store = useSquiStore();

const loading = computed(() => store.loading);
const data = computed(() => store.tableData);
const columns = computed(() => store.tableColumns);

const gridApi = shallowRef<GridApi>();
const columnDefs = ref<ColDef[]>([]);
const rowData = ref<Record<string, any>[]>([]);

const gridOptions: GridOptions = {
  rowSelection: "multiple",
  headerHeight: 32,
  alwaysShowHorizontalScroll: true,
  alwaysShowVerticalScroll: true,
  animateRows: false,
  rowHeight: 32,
  unSortIcon: true,
  suppressRowClickSelection: true,
};

function setColDefs(): ColDef[] {
  if (!columns.value) return [];

  const baseColDefs: ColDef[] = columns.value
    .filter((col) => col.selected)
    .map((col) => {
      return {
        headerName: col.name,
        field: col.name,
        sortable: true,
        filter: false,
        cellClass: "!flex !px-1 !py-1 ",
        editable: true,
        resizable: true,
        suppressSizeToFit: true,
        width: 200,
      };
    });

  if (baseColDefs.length === 0) return [];

  baseColDefs.unshift({
    sortable: false,
    filter: false,
    width: 30,
    cellClass: "!flex !items-center !p-[1px]",
    resizable: false,
    suppressAutoSize: true,
    suppressNavigable: true,
    checkboxSelection: true,
    headerCheckboxSelection: true,
    headerCheckboxSelectionFilteredOnly: true,
  });

  return baseColDefs;
}

function onGridReady(e: GridReadyEvent) {
  gridApi.value = e.api;
}

function setRowData(): Record<string, any>[] {
  if (!data.value) return [];

  return data.value;
}

function deleteSelectedRows() {
  if (!gridApi.value) return;

  const selectedNodes = gridApi.value.getSelectedRows();
  if (selectedNodes.length === 0) {
    toast({
      title: "No rows selected",
      description: "Please select rows to delete",
      variant: "destructive",
    });
    return;
  }
  console.log(selectedNodes);
}

defineExpose({ deleteSelectedRows });

onMounted(() => {
  store.getTableData();
  store.getTableSchema();

  columnDefs.value = setColDefs();
  rowData.value = setRowData();
});

watch(
  [data, columns, columns],
  () => {
    columnDefs.value = setColDefs();
    rowData.value = setRowData();
  },
  { deep: true }
);
</script>

<template>
  <div
    v-if="loading"
    class="absolute flex items-center justify-center w-full h-full"
    role="status"
  >
    <Loader2Icon class="size-20 text-primary animate-spin" />
    <span class="sr-only">Loading...</span>
  </div>

  <output v-else class="h-full overflow-auto pb-12">
    <AgGridVue
      class="!rounded-none"
      :class="{
        'ag-theme-quartz': mode === 'light',
        'ag-theme-quartz-dark': mode === 'dark',
      }"
      @grid-ready="onGridReady"
      :gridOptions
      :rowData
      :columnDefs
    />
  </output>
</template>
