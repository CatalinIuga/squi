<script setup lang="ts">
import { useSquiStore } from "@/lib/store";
import { ClientSideRowModelModule } from "@ag-grid-community/client-side-row-model";
import { ColDef, GridOptions, ModuleRegistry } from "@ag-grid-community/core";
import { AgGridVue } from "@ag-grid-community/vue3";
import { computed, onMounted, ref, watch } from "vue";

import "@ag-grid-community/styles/ag-grid.css";
import "@ag-grid-community/styles/ag-theme-quartz.css";
import { useColorMode } from "@vueuse/core";

ModuleRegistry.registerModules([ClientSideRowModelModule]);

const mode = useColorMode();
const store = useSquiStore();

const data = computed(() => store.tableData);
const columns = computed(() => store.tableSchema?.columns);

const colDefs = ref<ColDef[]>([]);
const rowData = ref<any[]>([]);

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

  const baseColDefs: ColDef[] = columns.value.map((col) => {
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

function setRowData(): Record<string, any>[] {
  if (!data.value) return [];

  return data.value;
}

onMounted(() => {
  store.getTableData();
  store.getTableSchema();

  colDefs.value = setColDefs();
  rowData.value = setRowData();
});

watch([data, columns], () => {
  colDefs.value = setColDefs();
  rowData.value = setRowData();
});
</script>

<template>
  <output class="h-full overflow-auto pb-12">
    <AgGridVue
      class="!rounded-none"
      :class="{
        'ag-theme-quartz': mode === 'light',
        'ag-theme-quartz-dark': mode === 'dark',
      }"
      :gridOptions="gridOptions"
      :columnDefs="colDefs"
      :rowData="rowData"
    />
  </output>
</template>
