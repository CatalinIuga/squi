using System.Data;

namespace squi.Models;

/// <summary>
/// Represents a database table, with its columns.
/// </summary>
public class TableSchema
{
    public int RowCount { get; set; }

    /// <summary>
    /// The columns of the table.
    /// </summary>
    public IEnumerable<ColumnSchema> Columns { get; set; } = new List<ColumnSchema>();

    // TODO: Add more properties like indexes, constraints, etc.
    // TODO: This only works for SQLite, add support for other databases.

    public TableSchema() { }

    public TableSchema(IEnumerable<ColumnSchema> columns, int rowCount)
    {
        Columns = columns;
        RowCount = rowCount;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableSchema"/> class.
    /// </summary>
    public TableSchema(DataTable schema, int rowCount)
    {
        foreach (DataRow row in schema.Rows)
        {
            var column = new ColumnSchema
            {
                Name = row["COLUMN_NAME"].ToString()!,
                Type = row["DATA_TYPE"].ToString()!,
                IsNullable = row["IS_NULLABLE"].ToString()! == "YES",
                IsPrimaryKey = row["PRIMARY_KEY"].ToString()! == "True",
                IsAutoIncrement = row["AUTOINCREMENT"].ToString()! == "True",
                IsUnique = row["UNIQUE"].ToString()! == "True",
                HasDefault = row["COLUMN_HASDEFAULT"].ToString()! == "True",
                Default = row["COLUMN_DEFAULT"].ToString()!,
            };

            _ = Columns.Append(column);
        }
        RowCount = rowCount;
    }
}
