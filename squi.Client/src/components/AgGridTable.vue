<script setup lang="ts">
import { getTableData, getTableSchema } from "@/service/dataService";
import { TableSchema } from "@/types/responses";
import {
  ColDef,
  GridApi,
  GridOptions,
  GridReadyEvent,
} from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { AgGridVue } from "ag-grid-vue3";
import { onMounted, ref } from "vue";

// TABLE DATA
const table = "Customers";
const columnDefs = ref<ColDef[]>([]);
const rowData = ref<Record<string, any>>([]);

// AG-GRID CONFIG/API
const gridApi = ref<GridApi | null>();
const gridOptions: GridOptions = {
  rowSelection: "multiple",
  headerHeight: 32,
  alwaysShowHorizontalScroll: true,
  alwaysShowVerticalScroll: true,
  onCellValueChanged: (params) => {
    console.log(params.oldValue, params.newValue);
    discardedChanges.value.push({
      row: params.rowIndex!,
      col: params.column.getId(),
      oldValue: params.oldValue,
      newValue: params.newValue,
    });
  },
  animateRows: false,
  rowHeight: 32,
  unSortIcon: true,
  suppressRowClickSelection: true,
};

onMounted(async () => {
  let aux: ColDef[] = [];

  await getTableSchema(table).then((data: TableSchema) => {
    aux = data.columns.map((column) => {
      return {
        field: column.name.toString(),
        editable: true,
        headerClass: "font-bold text-[#64748b] ",
        cellStyle: (params) => {
          const fieldName = params.colDef.field;
          const initialValue = params.data["__initial_" + fieldName];

          if (params.value !== initialValue) {
            return { backgroundColor: "#FDE68A", color: "#92400E" };
          }
          return { backgroundColor: "transparent", color: "inherit" };
        },

        cellClass: "py-[1px]",
        suppressMovable: true,
      };
    });
  });

  aux.unshift({
    sortable: false,
    filter: false,
    width: 30,
    cellClass: "flex",
    resizable: false,
    suppressAutoSize: true,
    suppressNavigable: true,
    checkboxSelection: true,
    headerCheckboxSelection: true,
    headerCheckboxSelectionFilteredOnly: true,
  });

  columnDefs.value = aux;

  await getTableData(table).then((data) => {
    rowData.value = data.map((row: any) => {
      const newRow = { ...row };
      Object.keys(newRow).forEach((key) => {
        newRow["__initial_" + key] = row[key];
      });
      return newRow;
    });
  });
});

const onGridReady = (params: GridReadyEvent) => {
  gridApi.value = params.api;
};

const discardedChanges = ref<
  {
    row: number;
    col: string;
    oldValue: any;
    newValue: any;
  }[]
>([]);

const discardChanges = () => {
  console.log("called");
  console.log(discardedChanges.value);
  discardedChanges.value.forEach((change) => {
    gridApi.value?.applyTransaction({
      update: [
        {
          rowIndex: change.row,
          data: { [change.col]: change.oldValue },
        },
      ],
    });
  });
};

defineExpose({ discardChanges });
</script>

<template>
  <AgGridVue
    class="ag-theme-quartz"
    @grid-ready="onGridReady"
    :rowData="rowData"
    :gridOptions="gridOptions"
    :columnDefs="columnDefs"
  />
</template>
