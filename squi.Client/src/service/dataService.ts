import { TableSchema } from "../types/responses";

const baseURI = import.meta.env.VITE_BASE_URI;

export const getTables = async () => {
  const response = await fetch(`${baseURI}/tables`);
  const tables = (await response.json()) as string[];
  return tables;
};

export const getTableSchema = async (tableName: string) => {
  const response = await fetch(`${baseURI}/tables/${tableName}`);
  const table = (await response.json()) as TableSchema;
  return table;
};

export const getTableData = async (tableName: string) => {
  const response = await fetch(`${baseURI}/tables/${tableName}/data`);
  const tableData = await response.json();
  return tableData.slice(0, 100) as Record<string, any>[];
};
