using System.Data;

namespace squi.Models;

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
}
