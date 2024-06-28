import { ApiResponse, DataFilter, Result, TableSchema } from "./types";
import { errorToResult } from "./utils";

// TODO: Clean this stuff up, could also try to use a vite proxy

const baseURI = import.meta.env.VITE_BASE_URI;

export async function fetchTables(): Promise<Result<string[]>> {
  try {
    const response = await fetch(`${baseURI}/tables`);
    const tables = (await response.json()) as string[];
    return { ok: true, data: tables };
  } catch (error) {
    return errorToResult(error);
  }
}

export async function fetchTableSchema(
  table: string
): Promise<Result<TableSchema>> {
  try {
    const response = await fetch(`${baseURI}/tables/${table}`);
    const schema = (await response.json()) as TableSchema;
    return { ok: true, data: schema };
  } catch (error) {
    return errorToResult(error);
  }
}

export async function fetchTableData(
  tableName: string,
  filters: DataFilter[] = [],
  limit: number = 50,
  offset: number = 0
): Promise<Result<Record<string, any>[]>> {
  try {
    const url = new URL(`${baseURI}/tables/${tableName}/select`);
    url.searchParams.append("limit", limit.toString());
    url.searchParams.append("offset", offset.toString());

    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(filters),
    });

    const tableData = (await response.json()) as Record<string, any>[];
    return { ok: true, data: tableData };
  } catch (error) {
    return errorToResult(error);
  }
}

export const getTableData = async (
  tableName: string,
  filters: string[] = [],
  limit: number = 50,
  offset: number = 0
) => {
  const url = new URL(`${baseURI}/tables/${tableName}/data`);
  filters.forEach((filter) => {
    url.searchParams.append("filter", filter);
  });
  url.searchParams.append("limit", limit.toString());
  url.searchParams.append("offset", offset.toString());
  const response = await fetch(url);
  const tableData = await response.json();
  return tableData.slice(0, 100) as Record<string, any>[];
};

export const insertTableData = async (
  tableName: string,
  row: Record<string, any>
) => {
  const response = await fetch(`${baseURI}/tables/${tableName}/data`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(row),
  });
  if (!response.ok) {
    throw new Error("Failed to insert data");
  }
  return response.json() as Promise<ApiResponse>;
};

export const updateTableData = async (
  tableName: string,
  row: Record<string, any>
) => {
  const response = await fetch(`${baseURI}/tables/${tableName}/data`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(row),
  });
  if (!response.ok) {
    throw new Error("Failed to update data");
  }
  return response.json() as Promise<ApiResponse>;
};

export const deleteTableData = async (
  tableName: string,
  row: Record<string, any>
) => {
  const response = await fetch(`${baseURI}/tables/${tableName}/data`, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(row),
  });
  if (!response.ok) {
    return false;
  }
  return true;
};
