using System.Data;

namespace squi.Models;

public class RelationshipSchema
{
    public string Table { get; set; } = null!;
    public string Column { get; set; } = null!;
    public string ReferenceTable { get; set; } = null!;
    public string ReferenceColumn { get; set; } = null!;

    public RelationshipSchema(
        string table,
        string column,
        string referenceTable,
        string referenceColumn
    )
    {
        Table = table;
        Column = column;
        ReferenceTable = referenceTable;
        ReferenceColumn = referenceColumn;
    }
}

/// <summary>
/// Represents a database table, with its columns.
/// </summary>
public class TableSchema
{
    // TODO: Add more properties like indexes, constraints, etc.

    /// <summary>
    /// The number of rows in the table.
    /// </summary>
    public int RowCount { get; set; }

    /// <summary>
    /// The columns of the table.
    /// </summary>
    public IEnumerable<ColumnSchema> Columns { get; set; } = new List<ColumnSchema>();

    public IEnumerable<RelationshipSchema> Relationships { get; set; } =
        new List<RelationshipSchema>();

    public TableSchema(DataTable data, int rowCount)
    {
        RowCount = rowCount;
        Columns = data.AsEnumerable()
            .Select(row =>
            {
                var name = row.Field<string>("COLUMN_NAME")!;
                var type = row.Field<string>("DATA_TYPE")!;
                var isNullable = row.Field<bool>("IS_NULLABLE")!;
                var isPrimaryKey = row.Field<bool>("PRIMARY_KEY")!;
                var isAutoIncrement = row.Field<bool>("AUTOINCREMENT")!;
                var isUnique = row.Field<bool>("UNIQUE")!;
                var hasDefault = row.Field<bool>("COLUMN_HASDEFAULT")!;
                var defaultValue = row.Field<object>("COLUMN_DEFAULT")!;

                return new ColumnSchema
                {
                    Name = name,
                    Type = type,
                    IsNullable = isNullable,
                    IsPrimaryKey = isPrimaryKey,
                    IsAutoIncrement = isAutoIncrement,
                    IsUnique = isUnique,
                    HasDefault = hasDefault,
                    Default = defaultValue,
                };
            });
    }
}
