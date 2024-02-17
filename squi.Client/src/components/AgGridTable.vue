<script setup lang="ts">
import { getTableData, getTableSchema } from "@/service/dataService";
import { TableSchema } from "@/types/responses";
import { GridOptions } from "ag-grid-community";
import "ag-grid-community/styles//ag-grid.css";
import "ag-grid-community/styles//ag-theme-quartz.css";
import { AgGridVue } from "ag-grid-vue3";
import { onMounted, ref } from "vue";

const table = "Customers";

const gridOptions: GridOptions = {
  rowSelection: "multiple",
  unSortIcon: true,
  suppressRowClickSelection: true,
};

const columnDefs = ref<
  {
    field: string;
    checkboxSelection?: boolean;
    headerCheckboxSelection?: boolean;
    headerCheckboxSelectionFilteredOnly?: boolean;
  }[]
>([]);
const rowData = ref<Record<string, any>>([]);

onMounted(async () => {
  let aux = [] as any;
  await getTableSchema(table).then((data: TableSchema) => {
    aux = data.columns.map((column) => {
      return {
        field: column.name.toString(),
      };
    });
  });

  aux[0] = {
    field: aux[0].field,
    checkboxSelection: true,
    headerCheckboxSelection: true,
    headerCheckboxSelectionFilteredOnly: true,
  };

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
/*
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
    */
.ag-theme-quartz {
  --ag-active-color: hsl(240, 5.9%, 10%);
  --ag-data-color: hsl(240, 5.9%, 10%);
  --ag-foreground-color: hsl(240, 5.9%, 10%);
  @apply h-full w-full font-menlo;
}

.ag-theme-quartz .ag-header-cell-label {
  @apply font-extrabold;
}
</style>
