<script setup lang="ts">
import {
  deleteTableData,
  getTableData,
  getTableSchema,
  insertTableData,
  updateTableData,
} from "@/service/dataService";
import { TableSchema } from "@/types/responses";
import { useColorMode } from "@vueuse/core";
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
  limit: {
    type: Number,
    required: true,
  },
  offset: {
    type: Number,
    required: true,
  },
  filters: {
    type: Array<string>,
    required: true,
  },
  table: {
    type: String,
    required: true,
  },
});

const loading = ref(false);

const tableSchema = ref<TableSchema | null>(null);
const tableData = ref<Record<string, any>[]>([]);

/**
 * Watch over the table prop and repopulate the grid when it changes
 */
watch(
  [
    () => props.table,
    () => props.filters,
    () => props.limit,
    () => props.offset,
  ],
  async ([newTable, newFilters, newLimit, newOffset]: [
    string,
    string[],
    number,
    number
  ]) => {
    await getGridData(newTable, newFilters, newLimit, newOffset);
  },
  { deep: true }
);

// AG-GRID COLUMNS Definitions
const columnDefs = ref<ColDef[]>([]);

// AG-GRID DATA
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

const getGridData = async (
  table: string,
  filters: string[],
  limit: number,
  offset: number
) => {
  loading.value = true;

  // i kinnda like this
  await new Promise((resolve) => setTimeout(resolve, 1000));

  let newColDefs: ColDef[] = [];

  tableSchema.value = await getTableSchema(table);

  newColDefs = tableSchema.value.columns.map((column) => {
    return {
      field: column.name.toString(),
      filter: false,
      suppressColumnsToolPanel: true,
      defaultAggFunc: "saveChanges",
      editable: column.isAutoIncrement ? false : true,
      headerClass: "font-bold text-slate-500 dark:text-slate-300",
      cellStyle: (params) => {
        const fieldName = params.colDef.field;
        const initialValue = params.data["__initial_" + fieldName];

        if (params.value !== initialValue || params.data.isnewRow) {
          return { backgroundColor: "#FDE68A", color: "#92400E" };
        }
        return { backgroundColor: "transparent", color: "inherit" };
      },
      cellClass: column.isAutoIncrement ? "focus:cursor-not-allowed" : "",
      suppressMovable: true,
    };
  });

  // selection column
  newColDefs.unshift({
    sortable: false,
    filter: false,
    width: 30,
    cellClass: "!flex !px-1 !pt-0",
    resizable: false,
    suppressAutoSize: true,
    suppressNavigable: true,
    checkboxSelection: true,
    headerCheckboxSelection: true,
    headerCheckboxSelectionFilteredOnly: true,
  });

  columnDefs.value = newColDefs;

  if (!offset) offset = 0;
  if (!limit) limit = 50;

  tableData.value = await getTableData(table, filters, limit, offset);

  rowData.value = tableData.value.map((row: any) => {
    const newRow: Record<string, any> = {};
    Object.keys(row).forEach((key) => {
      newRow[key] = row[key];
      newRow["__initial_" + key] = row[key];
      newRow["isnewRow"] = false;
    });
    return newRow;
  });

  loading.value = false;
};

onMounted(async () => {
  await getGridData(props.table, props.filters, props.limit, props.offset);
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

const refreshGrid = async () => {
  newRows.value = [];
  valueChanges.value = [];

  await getGridData(props.table, props.filters, props.limit, props.offset);
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

async function saveChanges() {
  gridApi.value?.stopEditing();

  valueChanges.value.forEach(async (change) => {
    const rowNode = gridApi.value?.getRowNode(change.nodeId);

    if (rowNode === undefined) return console.log("row not found");

    try {
      const updatedRow = await updateTableData(props.table, rowNode.data);
      if (!updatedRow.data) return;

      rowNode.setData({
        ...Object.keys(rowNode.data).reduce((acc, key) => {
          if (key !== "isnewRow" && !key.includes("__initial")) {
            Object.assign(acc, {
              [key]: updatedRow.data ? updatedRow.data[key] : null,
              ["__initial_" + key]: updatedRow.data
                ? updatedRow.data[key]
                : null,
            });
          }
          return acc;
        }, {}),
        isnewRow: false,
      });
    } catch (e) {
      console.error(e);
    }
  });

  valueChanges.value = [];

  newRows.value.forEach(async (row) => {
    try {
      const newRow = await insertTableData(props.table, row.data);

      if (!newRow.data) return console.log("row not found");

      row.setData({
        ...Object.keys(row.data).reduce((acc, key) => {
          if (key !== "isnewRow" && !key.includes("__initial")) {
            Object.assign(acc, {
              [key]: newRow.data ? newRow.data[key] : null,
              ["__initial_" + key]: newRow.data ? newRow.data[key] : null,
            });
          }
          return acc;
        }, {}),
        isnewRow: false,
      });
      tableSchema.value!.rowCount++;
    } catch (e) {
      console.error(e);
    }
    newRows.value = newRows.value.filter((r) => r.data.isnewRow);
  });
}

function discardChanges() {
  valueChanges.value.forEach((change) => {
    const rowNode = gridApi.value?.getRowNode(change.nodeId);

    if (!rowNode) return console.log("row not found");

    rowNode.setData({
      ...rowNode.data,
      [change.col]: change.oldValue,
      ["__initial_" + change.col]: change.oldValue,
    });
  });

  newRows.value.forEach((row) => {
    const toDelete = gridApi.value?.getDisplayedRowAtIndex(row.rowIndex!);

    if (!toDelete) return console.log("row not found");

    gridApi.value?.applyTransaction({ remove: [toDelete.data] });
  });

  valueChanges.value = [];
  newRows.value = [];
}

async function deleteSelectedRows() {
  const selectedRows = gridApi.value?.getSelectedRows();

  if (!selectedRows) return;

  selectedRows.forEach(async (row) => {
    let deleted = true;
    if (!row.isnewRow) {
      deleted = await deleteTableData(props.table, row);
    }

    if (deleted) {
      gridApi.value?.applyTransaction({ remove: [row] });
      tableSchema.value!.rowCount--;
    }
  });

  selectedRowsCount.value = gridApi.value?.getSelectedNodes().length || 0;
}

const totalChanges = computed(() => {
  return valueChanges.value.length + newRows.value.length;
});

const allColumns = computed(() => {
  return columnDefs.value
    .filter((colDef) => colDef.field !== undefined)
    .map((colDef) => colDef.field as string);
});

const displayedColumns = ref<string[]>([]);

watch(
  () => gridApi.value,
  (newGridApi) => {
    if (!newGridApi || newGridApi.isDestroyed()) return;

    displayedColumns.value = newGridApi
      .getColumns()
      ?.filter((col) => col.isVisible() && col.getColDef().field !== undefined)
      .map((col) => col.getColDef().field as string) as string[];
  },
  { immediate: true, deep: true }
);

const toggleColumn = (col: string) => {
  if (col === "__all") {
    gridApi.value?.setColumnsVisible(gridApi.value.getColumns()!, true);
    displayedColumns.value = allColumns.value;
    return;
  }

  if (col === "__none") {
    gridApi.value?.setColumnsVisible(
      gridApi.value
        .getColumns()!
        .filter((c) => c.getColDef().field != undefined),
      false
    );
    displayedColumns.value = [];
    return;
  }
  const column = gridApi.value?.getColumn(col);

  if (!column) return;

  gridApi.value?.setColumnsVisible([column], !column?.isVisible());
  displayedColumns.value = gridApi.value
    ?.getColumns()
    ?.filter((col) => col.isVisible() && col.getColDef().field !== undefined)
    .map((col) => col.getColDef().field as string) as string[];
};

const rowCounter = computed(() => {
  return tableSchema.value?.rowCount;
});

const download = () => {
  let data: Array<any> = [];
  gridApi.value?.forEachNode((node) => {
    data.push(
      // dont include the __initial_ fields, isnewRow and the isNewROw
      Object.keys(node.data).reduce((acc, key) => {
        if (!key.includes("__initial") && key !== "isnewRow") {
          Object.assign(acc, { [key]: node.data[key] });
        }
        return acc;
      }, {})
    );
  });
  const blob = new Blob([JSON.stringify(data, null, 4)], {
    type: "application/json",
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `${props.table}.json`;
  a.click();
  URL.revokeObjectURL(url);
  a.remove();
};

defineExpose({
  refreshGrid,
  addRow,
  saveChanges,
  discardChanges,
  deleteSelectedRows,

  download,

  displayedColumns,
  allColumns,
  toggleColumn,

  tableSchema,

  changes: totalChanges,
  rowCounter,
  selectedRowsCount,
});

const mode = useColorMode();
</script>

<template>
  <div
    v-if="loading"
    class="flex items-center justify-center w-full h-full"
    role="status"
  >
    <svg
      aria-hidden="true"
      class="size-10 text-secondary animate-spin fill-red-500"
      viewBox="0 0 100 101"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
        fill="currentColor"
      />
      <path
        d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
        fill="currentFill"
      />
    </svg>
    <span class="sr-only">Loading...</span>
  </div>
  <Transition>
    <AgGridVue
      v-if="!loading"
      class="!rounded-none"
      :class="{
        'ag-theme-quartz': mode === 'light',
        'ag-theme-quartz-dark': mode === 'dark',
      }"
      @grid-ready="onGridReady"
      :rowData="rowData"
      :gridOptions="gridOptions"
      :columnDefs="columnDefs"
    />
  </Transition>
</template>

<style scoped>
.v-enter-active,
.v-leave-active {
  transition: opacity 0.5s;
}

.v-enter-from,
.v-leave-to {
  opacity: 0;
}
</style>
