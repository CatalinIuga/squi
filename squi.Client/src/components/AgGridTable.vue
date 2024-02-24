<script setup lang="ts">
import { getTableData, getTableSchema } from "@/service/dataService";
import { TableSchema } from "@/types/responses";
import {
  ColDef,
  GridApi,
  GridOptions,
  GridReadyEvent,
  IRowNode,
} from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { AgGridVue } from "ag-grid-vue3";
import { computed, onMounted, ref, watch } from "vue";

const props = defineProps({
  table: {
    type: String,
    required: true,
  },
});

const tableSchema = ref<TableSchema | null>(null);
const tableData = ref<Record<string, any>[]>([]);

/**
 * Watch over the table prop and repopulate the grid when it changes
 */
watch(
  () => props.table,
  async (newTable) => {
    await getGridData(newTable);
  }
);

/**
 * Column definitions and row data for the ag-grid
 */
const columnDefs = ref<ColDef[]>([]);

/**
 * Row data for the ag-grid
 */
const rowData = ref<Record<string, any>>([]);

const selectedRowsCount = ref(0);

const newRows = ref<IRowNode[]>([]);

// AG-GRID CONFIG/API
const gridApi = ref<GridApi | null>();
const gridOptions: GridOptions = {
  rowSelection: "multiple",
  headerHeight: 32,
  alwaysShowHorizontalScroll: true,
  alwaysShowVerticalScroll: true,
  onRowDataUpdated: (params) => {
    const newRowCount = params.api.getDisplayedRowCount();

    if (newRowCount === tableData.value.length) return;

    newRows.value = params.api
      .getRenderedNodes()
      .filter((row) => row.data.isnewRow);

    console.log(newRows.value);
  },
  onSelectionChanged: () => {
    selectedRowsCount.value = gridApi.value?.getSelectedNodes().length || 0;
  },
  onCellValueChanged: (params) => {
    // if the row is new, we don't need to add it to the changes array
    if (!params.node.id || params.data.isnewRow) return;

    // if the value changed, add it to the changes array
    if (
      params.oldValue !== params.newValue &&
      valueChanges.value.findIndex(
        (change) =>
          change.nodeId === params.node.id &&
          change.col === params.column.getId()
      ) === -1
    ) {
      valueChanges.value.push({
        nodeId: params.node.id,
        col: params.column.getId(),
        oldValue: params.oldValue,
        newValue: params.newValue,
      });
    } else {
      valueChanges.value = valueChanges.value.filter(
        (change) =>
          change.nodeId !== params.node.id ||
          change.col !== params.column.getId()
      );
    }
  },
  pinnedTopRowData: newRows.value,
  animateRows: false,
  rowHeight: 32,
  unSortIcon: true,
  suppressRowClickSelection: true,
};

const getGridData = async (table: string) => {
  let newColDefs: ColDef[] = [];

  tableSchema.value = await getTableSchema(table);

  newColDefs = tableSchema.value.columns.map((column) => {
    return {
      field: column.name.toString(),
      defaultAggFunc: "saveChanges",
      editable: column.isAutoIncrement ? false : true,
      headerClass: "font-bold text-[#64748b]",
      cellStyle: (params) => {
        const fieldName = params.colDef.field;
        const initialValue = params.data["__initial_" + fieldName];

        if (params.value !== initialValue || params.data.isnewRow) {
          return { backgroundColor: "#FDE68A", color: "#92400E" };
        }
        return { backgroundColor: "transparent", color: "inherit" };
      },
      cellClass: "py-[1px]",
      suppressMovable: true,
    };
  });

  // selection column
  newColDefs.unshift({
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

  columnDefs.value = newColDefs;

  tableData.value = await getTableData(table);

  rowData.value = tableData.value.map((row: any) => {
    const newRow: Record<string, any> = {};
    Object.keys(row).forEach((key) => {
      newRow[key] = row[key];
      newRow["__initial_" + key] = row[key];
      newRow["isnewRow"] = false;
    });
    return newRow;
  });
};

onMounted(async () => {
  await getGridData(props.table);
});

const valueChanges = ref<
  {
    nodeId: string;
    col: string;
    oldValue: any;
    newValue: any;
  }[]
>([]);

const onGridReady = (params: GridReadyEvent) => {
  gridApi.value = params.api;
};

const addRow = () => {
  const newRow: Record<string, any> = {};
  tableSchema.value?.columns.forEach((column) => {
    newRow[column.name] = null;
    newRow["__initial_" + column.name] = null;
    newRow["isnewRow"] = true;
  });

  gridApi.value?.applyTransaction({ add: [newRow], addIndex: 0 });
};

function saveChanges() {
  valueChanges.value.forEach((change) => {
    const rowNode = gridApi.value?.getRowNode(change.nodeId);

    if (rowNode) {
      rowNode.setData({
        ...rowNode.data,
        [change.col]: change.newValue,
        ["__initial_" + change.col]: change.newValue,
        ["isnewRow"]: false,
      });
      // should probably use a toast here as well
    }

    // TODO send the changes to the server

    // TODO use a toast!
    else console.log("row not found");
  });

  valueChanges.value = [];

  // new rows
  newRows.value.forEach((row) => {
    row.setData({
      ...Object.keys(row.data).reduce((acc, key) => {
        if (key !== "isnewRow" && !key.includes("__initial")) {
          Object.assign(acc, {
            [key]: row.data[key],
            ["__initial_" + key]: row.data[key],
          });
        }
        return acc;
      }, {}),
      isnewRow: false,
    });
  });

  newRows.value = newRows.value.filter((row) => row.data.isnewRow);
}

function discardChanges() {
  valueChanges.value.forEach((change) => {
    const rowNode = gridApi.value?.getRowNode(change.nodeId);
    if (rowNode) {
      console.log(rowNode.data, change.col, change.oldValue);

      rowNode.setData({
        ...rowNode.data,
        [change.col]: change.oldValue,
        ["__initial_" + change.col]: change.oldValue,
      });
    } else console.log("row not found");
  });

  newRows.value.forEach((row) => {
    const toDelete = gridApi.value?.getDisplayedRowAtIndex(row.rowIndex!);

    if (toDelete) {
      gridApi.value?.applyTransaction({ remove: [toDelete.data] });
    }
  });

  valueChanges.value = [];
  newRows.value = [];

  console.log(newRows.value.length, valueChanges.value.length);
}

function deleteSelectedRows() {
  const selectedRows = gridApi.value?.getSelectedRows();
  if (selectedRows) {
    gridApi.value?.applyTransaction({ remove: selectedRows });
  }
  // TODO use a unique id for each row and check the changes array for any changes to the deleted rows

  selectedRowsCount.value = 0;
}

const totalChanges = computed(() => {
  return valueChanges.value.length + newRows.value.length;
});

defineExpose({
  addRow,
  saveChanges,
  discardChanges,
  deleteSelectedRows,
  changes: totalChanges,
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
