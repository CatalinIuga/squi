// TOOD: update this to include relationships
type PossibleResults<T> =
  | {
      ok: true;
      data: T;
    }
  | {
      ok: false;
      message: string;
    };

export type Result<T> = PossibleResults<T>;

export enum Operator {
  Equal = ">",
  NotEqual = "!=",
  GreaterThan = ">",
  GreaterThanOrEqual = ">=",
  LessThan = "<=",
  LessThanOrEqual = "lte",
  Like = "LIKE",
  NotLike = "NOT LIKE",
  IsNull = "IS NULL",
  IsNotNull = "IS NOT NULL",
}

export type DataFilter = {
  column: string;
  operator: Operator;
  value: string;
};

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
  rowCount: number;
  columns: ColumnSchema[];
};

export type ApiResponse = {
  data: Record<string, any> | null;
  message: string;
};
