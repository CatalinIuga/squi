namespace squi.Models;

/// <summary>
/// Represents a database column.
/// </summary>
public class ColumnSchema
{
    /// <summary>
    /// The name of the column.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of the column.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Whether the column is nullable.
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Whether the column is a primary key.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Whether the column is an auto-incrementing primary key.
    /// </summary>
    public bool IsAutoIncrement { get; set; }

    /// <summary>
    /// Whether the column is unique.
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// Whether the column has a default value.
    /// </summary>
    public bool HasDefault { get; set; }

    /// <summary>
    /// The default value of the column.
    /// </summary>
    public object Default { get; set; } = string.Empty;
}
