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
import { onMounted, ref, watch } from "vue";

const props = defineProps({
  table: {
    type: String,
    required: true,
  },
});

// TABLE DATA
const columnDefs = ref<ColDef[]>([]);
const rowData = ref<Record<string, any>>([]);

// AG-GRID CONFIG/API
const gridApi = ref<GridApi | null>();
const gridOptions: GridOptions = {
  rowSelection: "multiple",
  headerHeight: 32,
  alwaysShowHorizontalScroll: true,
  alwaysShowVerticalScroll: true,
  onSelectionChanged: () => {
    selectedRowsCount.value = gridApi.value?.getSelectedNodes().length || 0;
  },
  onCellValueChanged: (params) => {
    // check if the value has changed from the initial value or by the discardChanges method
    if (!params.node.id) return;
    if (
      params.oldValue !== params.newValue &&
      changes.value.findIndex(
        (change) =>
          change.nodeId === params.node.id &&
          change.col === params.column.getId()
      ) === -1
    ) {
      changes.value.push({
        nodeId: params.node.id,
        col: params.column.getId(),
        oldValue: params.oldValue,
        newValue: params.newValue,
      });
    } else {
      changes.value = changes.value.filter(
        (change) =>
          change.nodeId !== params.node.id ||
          change.col !== params.column.getId()
      );
    }
  },
  animateRows: false,
  rowHeight: 32,
  unSortIcon: true,
  suppressRowClickSelection: true,
};

const populateGrid = async (table: string) => {
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
};

onMounted(async () => {
  await populateGrid(props.table);
});

watch(
  () => props.table,
  async (newTable) => {
    await populateGrid(newTable);
  }
);

const onGridReady = (params: GridReadyEvent) => {
  gridApi.value = params.api;
};

const addRow = () => {
  // missing intials tho
  gridApi.value?.applyTransaction({
    add: [
      Object.fromEntries(columnDefs.value.map((col) => [col.field, "default"])),
    ],
    addIndex: 0,
  });
};

const changes = ref<
  {
    nodeId: string;
    col: string;
    oldValue: any;
    newValue: any;
  }[]
>([]);

const saveChanges = () => {
  changes.value.forEach((change) => {
    const rowNode = gridApi.value?.getRowNode(change.nodeId);
    console.log(rowNode?.data);

    if (rowNode) {
      rowNode.setData({
        ...rowNode.data,
        [change.col]: change.newValue,
        ["__initial_" + change.col]: change.newValue,
      });
      // should probably use a toast here as well
    }

    // TODO send the changes to the server

    // TODO use a toast!
    else console.log("row not found");
  });

  changes.value = [];
};

const discardChanges = () => {
  changes.value.forEach((change) => {
    const rowNode = gridApi.value?.getRowNode(change.nodeId);
    if (rowNode) rowNode.setDataValue(change.col, change.oldValue);
    // TODO use a toast!
    else console.log("row not found");
  });
};

const selectedRowsCount = ref(0);

const deleteSelectedRows = () => {
  const selectedRows = gridApi.value?.getSelectedRows();
  if (selectedRows) {
    gridApi.value?.applyTransaction({ remove: selectedRows });
  }
  // TODO use a unique id for each row and check the changes array for any changes to the deleted rows

  selectedRowsCount.value = 0;
};

defineExpose({
  addRow,
  saveChanges,
  discardChanges,
  deleteSelectedRows,
  changes,
  selectedRowsCount,
});
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
