<script setup lang="ts">
import { getTableData, getTableSchema } from "@/service/dataService";
import { TableSchema } from "@/types/responses";
import { ColDef, GridOptions } from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { AgGridVue } from "ag-grid-vue3";
import { onMounted, ref } from "vue";

const table = "Customers";

const gridOptions: GridOptions = {
  rowSelection: "multiple",
  headerHeight: 32,
  // rowClass: "divide-x-2 divide-black ",
  rowHeight: 32,
  unSortIcon: true,
  suppressRowClickSelection: true,
};

const columnDefs = ref<ColDef[]>([]);
const rowData = ref<Record<string, any>>([]);

onMounted(async () => {
  let aux = [] as ColDef[];
  await getTableSchema(table).then((data: TableSchema) => {
    aux = data.columns.map((column) => {
      return {
        field: column.name.toString(),
        editable: true,
        headerClass: "font-bold text-[#64748b]",
        cellClass: "",
        suppressMovable: true,
      };
    });
  });

  aux.unshift({
    sortable: false,
    filter: false,
    width: 20,
    resizable: false,
    suppressAutoSize: true,
    checkboxSelection: true,
    headerCheckboxSelection: true,
    headerCheckboxSelectionFilteredOnly: true,
  });

  columnDefs.value = aux;

  await getTableData(table).then((data) => {
    rowData.value = data;
  });
});
</script>

<template>
  <AgGridVue
    class="ag-theme-quartz"
    :rowData="rowData"
    :gridOptions="gridOptions"
    :columnDefs="columnDefs"
  />
</template>

<style scoped>
.ag-theme-quartz {
  --background: 0 0% 100%;
  --foreground: 240 10% 3.9%;

  --card: 0 0% 100%;
  --card-foreground: 240 10% 3.9%;

  --popover: 0 0% 100%;
  --popover-foreground: 240 10% 3.9%;

  --primary: 240 5.9% 10%;
  --primary-foreground: 0 0% 98%;

  --secondary: 240 4.8% 95.9%;
  --secondary-foreground: 240 5.9% 10%;

  --muted: 210 40% 96%;
  --muted-foreground: 240 3.8% 46.1%;

  --accent: 240 4.8% 95.9%;
  --accent-foreground: 240 5.9% 10%;

  --destructive: 0 84.2% 60.2%;
  --destructive-foreground: 0 0% 98%;

  --border: #e5e5e5;
  --input: 240 5.9% 90%;
  --ring: 240 5.9% 10%;
  --radius: 0.5rem;

  --ag-active-color: hsl(var(--accent-foreground));
  --ag-data-color: hsl(var(--foreground));
  --ag-foreground-color: hsl(var(--foreground));
  --ag-background-color: hsl(var(--background));
  --ag-secondary-foreground-color: hsl(var(--secondary-foreground));
  --ag-header-foreground-color: hsl(var(--foreground));
  --ag-tooltip-background-color: hsl(var(--popover));
  --ag-disabled-foreground-color: hsl(var(--muted-foreground));
  --ag-subheader-background-color: hsl(var(--secondary));
  --ag-subheader-toolbar-background-color: hsl(var(--secondary));
  --ag-control-panel-background-color: hsl(var(--secondary));
  --ag-side-button-selected-background-color: hsl(var(--secondary));
  --ag-header-column-resize-handle-display: none;
  --ag-selected-row-background-color: hsl(var(--muted));
  --ag-modal-overlay-background-color: hsl(var(--popover));
  --ag-row-hover-color: hsl(var(--muted));
  --ag-grid-size: 4px;
  --ag-row-height: 32px;
  --ag-list-item-height: 20px;
  --ag-font-size: 12px;

  --ag-borders: none;
  --ag-border-color: var(--border);
  --ag-borders-critical: solid 1px;
  --ag-borders-secondary: solid 1px;
  --ag-row-border-style: solid;
  --ag-row-border-color: var(--border);
  --ag-row-border-width: 1px;
  --ag-borders-input: none;
  --ag-cell-horizontal-border: solid var(--border);
  --ag-header-column-separator-display: block;

  --ag-borders-input: dotted 2px;
  --ag-input-border-color: orange;

  @apply h-full w-full font-menlo;
}

/* fix input outline */
</style>
