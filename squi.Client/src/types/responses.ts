export type ColumnSchema = {
  name: string;
  type: string;
  isNullable: boolean;
  isPrimaryKey: boolean;
  isAutoIncrement: boolean;
  isUnique: boolean;
  hasDefault: boolean;
  default: any;
};

export type TableSchema = {
  columns: ColumnSchema[];
};
