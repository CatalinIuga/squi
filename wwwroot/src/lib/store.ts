import { defineStore } from "pinia";
import { ref, watch } from "vue";
import { fetchTableData, fetchTableSchema } from "./services";
import { TableSchema } from "./types";

export const useSquiStore = defineStore("squi", () => {
  // REFERENCE VARIABLES
  const table = ref<string | null>(localStorage.getItem("table"));
  const tableSchema = ref<TableSchema | null>(null);
  const tableData = ref<Record<string, any>[]>([]);

  const offset = ref(0);
  const limit = ref(50);

  // STATE VARIABLES
  const loading = ref(false);
  const error = ref<string | null>(null);
  const openMenu = ref(true);

  // SETTERS
  function setTable(value: string | null) {
    table.value = value;
  }

  function setOpenMenu(value: boolean) {
    openMenu.value = value;
  }

  // FETCHERS - this might actually not be needed here...
  function getTableSchema() {
    if (!table.value) return;
    fetchTableSchema(table.value).then(
      (r) => (tableSchema.value = r.ok ? r.data : null)
    );
  }

  function getTableData() {
    if (!table.value) return;

    loading.value = true;
    fetchTableData(table.value, [], limit.value, offset.value)
      .then((r) => {
        tableData.value = r.ok ? r.data : [];
        loading.value = false;
      })
      .catch((e) => {
        error.value = e.message;
        loading.value = false;
      });
  }

  function refreshTableData() {
    getTableData();
  }

  // WATCHERS
  watch(table, (value) => {
    if (value) {
      localStorage.setItem("table", value);
      getTableSchema();
      getTableData();
    } else {
      localStorage.removeItem("table");
      tableSchema.value = null;
      tableData.value = [];
    }
  });

  // main watcher for table data
  watch([offset, limit], () => {
    if (!table.value) return;
    getTableData();
  });

  return {
    table,
    setTable,
    tableSchema,
    getTableSchema,
    tableData,
    getTableData,

    offset,
    limit,
    refreshTableData,

    loading,
    error,
    openMenu,
    setOpenMenu,
  };
});
