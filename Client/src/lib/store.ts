import { defineStore } from "pinia";
import { ref, watch } from "vue";
import { fetchTableData, fetchTableSchema } from "./services";
import { TableSchema } from "./types";

export const useSquiStore = defineStore("squi", () => {
  // REFERENCE VARIABLES
  const table = ref<string | null>(null);
  const tableSchema = ref<TableSchema | null>(null);
  const tableData = ref<Record<string, any>[]>([]);

  const offset = ref(0);
  const limit = ref(50);

  // STATE VARIABLES
  const loading = ref(false);
  const error = ref<string | null>(null);
  const openMenu = ref(false);

  // SETTERS
  function setTable(value: string | null) {
    table.value = value;
  }

  function setOpenMenu(value: boolean) {
    openMenu.value = value;
  }

  function setOffset(value: number) {
    offset.value = value;
  }

  function setLimit(value: number) {
    limit.value = value;
  }

  // FETCHERS - this might actually not be needed here...
  function getTableSchema() {
    if (!table.value) return;
    loading.value = true;
    fetchTableSchema(table.value).then(
      (r) => (tableSchema.value = r.ok ? r.data : null)
    );
    loading.value = false;
  }

  function getTableData() {
    if (!table.value) return;
    loading.value = true;
    fetchTableData(table.value).then(
      (r) => (tableData.value = r.ok ? r.data : [])
    );
    loading.value = false;
  }

  // WATCHERS
  watch(table, (value) => {
    if (value) {
      getTableSchema();
      getTableData();
    } else {
      tableSchema.value = null;
      tableData.value = [];
    }
  });

  return {
    table,
    setTable,
    tableSchema,
    getTableSchema,
    tableData,
    getTableData,

    offset,
    setOffset,
    limit,
    setLimit,

    loading,
    error,
    openMenu,
    setOpenMenu,
  };
});
