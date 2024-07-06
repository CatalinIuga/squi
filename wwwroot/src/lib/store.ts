import { defineStore } from "pinia";
import { ref, watch } from "vue";
import { fetchTableData, fetchTableSchema } from "./services";
import { Operator, type DataFilter, type TableSchema } from "./types";

export const useSquiStore = defineStore("squi", () => {
  // REFERENCE VARIABLES
  const table = ref<string | null>(localStorage.getItem("table"));
  const tableSchema = ref<TableSchema | null>(null);
  const tableData = ref<Record<string, any>[]>([]);

  const offset = ref(0);
  const limit = ref(50);

  const showFilters = ref(false);
  const filters = ref<DataFilter[]>([]);

  // STATE VARIABLES
  const loading = ref(false);
  const error = ref<string | null>(null);
  const showSidebar = ref(true);

  // SETTERS
  function setTable(value: string | null) {
    table.value = value;
  }

  function setShowSidebar(value: boolean) {
    showSidebar.value = value;
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
    fetchTableData(table.value, filters.value, limit.value, offset.value)
      .then((r) => {
        tableData.value = r.ok ? r.data : [];
        loading.value = false;
      })
      .catch((e) => {
        error.value = e.message;
        loading.value = false;
      });
  }

  function setShowFilters(value: boolean) {
    showFilters.value = value;
  }

  function addFilter() {
    if (!tableSchema.value || tableSchema.value.columns.length === 0) return;
    const newFilter: DataFilter = {
      column: tableSchema.value.columns[0].name,
      operator: Operator.Equal,
      value: "null",
    };
    filters.value.push(newFilter);
  }

  function removeFilter(index: number) {
    filters.value.splice(index, 1);
  }

  function setFilters(newFilters: DataFilter[]) {
    filters.value = newFilters;
  }

  function refreshTableData() {
    getTableData();
  }

  // WATCHERS
  watch(table, (value) => {
    if (value) {
      localStorage.setItem("table", value);
      offset.value = 0;
      limit.value = 50;
      setFilters([]);
      getTableSchema();
      getTableData();
    } else {
      localStorage.removeItem("table");
      tableSchema.value = null;
      tableData.value = [];
    }
  });

  // main watcher for table data
  watch(
    [offset, limit, filters],
    () => {
      if (!table.value) return;
      getTableData();
    },
    { deep: true }
  );

  return {
    table,
    setTable,

    tableSchema,
    getTableSchema,

    tableData,
    getTableData,

    offset,
    limit,

    showFilters,
    setShowFilters,
    filters,
    addFilter,
    removeFilter,
    setFilters,

    loading,
    error,

    showSidebar,
    setShowSidebar,
    refreshTableData,
  };
});
