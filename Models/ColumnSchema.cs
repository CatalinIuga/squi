namespace squi.Models;

/// Available row information:
// TABLE_CATALOG: main
// TABLE_SCHEMA: sqlite_default_schema
// TABLE_NAME: artists
// COLUMN_NAME: Name
// COLUMN_GUID:
// COLUMN_PROPID:
// ORDINAL_POSITION: 1
// COLUMN_HASDEFAULT: False
// COLUMN_DEFAULT:
// COLUMN_FLAGS:
// IS_NULLABLE: True
// DATA_TYPE: nvarchar
// TYPE_GUID:
// CHARACTER_MAXIMUM_LENGTH: 120
// CHARACTER_OCTET_LENGTH:
// NUMERIC_PRECISION:
// NUMERIC_SCALE:
// DATETIME_PRECISION:
// CHARACTER_SET_CATALOG:
// CHARACTER_SET_SCHEMA:
// CHARACTER_SET_NAME:
// COLLATION_CATALOG:
// COLLATION_SCHEMA:
// COLLATION_NAME: BINARY
// DOMAIN_CATALOG:
// DOMAIN_NAME:
// DESCRIPTION:
// PRIMARY_KEY: False
// EDM_TYPE: nvarchar
// AUTOINCREMENT: False
// UNIQUE: False


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
    public string Default { get; set; } = string.Empty;
}
