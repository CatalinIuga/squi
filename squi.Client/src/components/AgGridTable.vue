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
        suppressMovable: true,
      };
    });
  });

  aux.unshift({
    sortable: false,
    filter: false,
    width: 50,
    resizable: false,
    checkboxSelection: true,
    editable: true,
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

  --muted: 240 4.8% 95.9%;
  --muted-foreground: 240 3.8% 46.1%;

  --accent: 240 4.8% 95.9%;
  --accent-foreground: 240 5.9% 10%;

  --destructive: 0 84.2% 60.2%;
  --destructive-foreground: 0 0% 98%;

  --border: 240 5.9% 90%;
  --input: 240 5.9% 90%;
  --ring: 240 5.9% 10%;
  --radius: 0.5rem;

  --ag-active-color: hsl(var(--accent-foreground));
  --ag-data-color: hsl(var(--foreground));
  --ag-foreground-color: hsl(var(--foreground));
  --ag-background-color: hsl(var(--background));
  --ag-secondary-foreground-color: hsl(var(--secondary-foreground));
  --ag-header-foreground-color: #64748b;
  --ag-tooltip-background-color: hsl(var(--popover));
  --ag-disabled-foreground-color: hsl(var(--muted-foreground));
  --ag-subheader-background-color: hsl(var(--secondary));
  --ag-subheader-toolbar-background-color: hsl(var(--secondary));
  --ag-control-panel-background-color: hsl(var(--secondary));
  --ag-side-button-selected-background-color: hsl(var(--secondary));
  --ag-header-column-resize-handle-display: none;
  --ag-selected-row-background-color: hsl(var(--secondary));
  --ag-modal-overlay-background-color: hsl(var(--popover));
  --ag-row-hover-color: hsl(var(--secondary));
  --ag-column-hover-color: hsl(var(--secondary));
  --ag-border-size: 1px;
  /* --ag-grid-size: 4px; */
  --ag-row-height: 32px;
  /* --ag-list-item-height: 20px; */
  --ag-font-size: 14px;

  @apply h-full w-full font-menlo;
}

input[type="checkbox"] {
  @apply !rounded-none;
}

.ag-theme-quartz .ag-header-cell-label {
  @apply font-extrabold;
}
</style>
