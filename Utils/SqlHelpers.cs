using System.Text.RegularExpressions;
using squi.Models;

namespace squi.Utils;

public class Operator
{
    public new const string Equals = "=";
    public const string NotEquals = "!=";
    public const string GreaterThan = ">";
    public const string LessThan = "<";
    public const string GreaterThanOrEquals = ">=";
    public const string LessThanOrEquals = "<=";
    public const string Like = "LIKE";
    public const string NotLike = "NOT LIKE";
    public const string IsNull = "IS NULL";
    public const string IsNotNull = "IS NOT NULL";

    public static string[] All = new[]
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEquals,
        LessThanOrEquals,
        Like,
        NotLike,
        IsNull,
        IsNotNull,
    };
}

public class DataFilters
{
    public string Column { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public string? Value { get; set; }
}

enum TableColumnType
{
    Integer,
    Text,
    Real,
    Blob,
}

partial class SqlHelpers
{
    [GeneratedRegex("\"")]
    private static partial Regex IdenityEscapeRegex();

    [GeneratedRegex("^[\"`\\[]")]
    private static partial Regex UnescapeRegex();

    [GeneratedRegex("[\"`\\]]$")]
    private static partial Regex UnescapeRegex1();

    [GeneratedRegex("\"\"")]
    private static partial Regex UnescapeRegex2();

    public static string EscapeIdentity(string identity)
    {
        return $"\"{IdenityEscapeRegex().Replace(identity, "\"\"")}\"";
    }

    public static string UnescapeIdentity(string identity)
    {
        string r = UnescapeRegex().Replace(identity, "");
        r = UnescapeRegex1().Replace(r, "");
        return UnescapeRegex2().Replace(r, "\"");
    }

    public static string EscapeSqlString(string str)
    {
        return "'" + str.Replace("'", "''") + "'";
    }

    public static string EscapeSqlBinay(byte[] value)
    {
        return "X'" + BitConverter.ToString(value).Replace("-", "") + "'";
    }

    public static string EscapeSqlValue(object? value)
    {
        if (value is null)
        {
            return "NULL";
        }

        if (value is string v)
        {
            return EscapeSqlString(v);
        }

        if (value is byte[] b)
        {
            return EscapeSqlBinay(b);
        }

        if (
            value is int
            || value is long
            || value is float
            || value is double
            || value is decimal
            || value is bool
            || value is DateTime
        )
        {
            return value.ToString()!;
        }

        throw new ArgumentException("Unsupported type, " + value.GetType());
    }

    public static TableColumnType ConvertSqlType(string type)
    {
        return type.ToUpper() switch
        {
            "CHAR" => TableColumnType.Text,
            "VARCHAR" => TableColumnType.Text,
            "TEXT" => TableColumnType.Text,
            "STRING" => TableColumnType.Text,

            "INTEGER" => TableColumnType.Integer,
            "INT" => TableColumnType.Integer,

            "BLOB" => TableColumnType.Blob,

            "REAL" => TableColumnType.Real,
            "FLOAT" => TableColumnType.Real,
            "DOUBLE" => TableColumnType.Real,
            "DECIMAL" => TableColumnType.Real,
            _ => TableColumnType.Text,
        };
    }

    public static string CreateComparator(object? value)
    {
        if (value is null)
        {
            return $"{Operator.IsNull}";
        }

        return $"{Operator.Equals} {EscapeSqlValue(value)}";
    }

    public static string FilterToSql(DataFilters filter)
    {
        if (!Operator.All.Contains(filter.Operator.ToUpper()))
        {
            throw new ArgumentException("Unsupported operator, " + filter.Operator);
        }

        if (filter.Operator.ToUpper() is Operator.IsNull or Operator.IsNotNull)
        {
            return $"{EscapeIdentity(filter.Column)} {filter.Operator}";
        }
        return $"{EscapeIdentity(filter.Column)} {filter.Operator} {EscapeSqlValue(filter.Value)}";
    }

    public static string GenerateSelect(
        string tableName,
        IEnumerable<DataFilters> filters,
        int limit,
        int offset
    )
    {
        var filtersArray = filters.Select(FilterToSql).ToArray();
        var filtersPart = string.Join(" AND ", filtersArray);
        var wherePart = filtersPart.Length > 0 ? $"WHERE {filtersPart}" : "";
        return $"SELECT * FROM {tableName} {wherePart} LIMIT {limit} OFFSET {offset}";
    }

    public static string GenerateInsert(string tableName, TableData values)
    {
        var columns = string.Join(", ", values.Keys.Select(EscapeIdentity).ToArray());
        var valuesPart = string.Join(", ", values.Values.Select(EscapeSqlValue).ToArray());
        return $"INSERT INTO {tableName} ({columns}) VALUES ({valuesPart})";
    }

    public static string GenerateUpdate(string tableName, TableData oldValues, TableData newValues)
    {
        var oldValuesArray = oldValues
            .Select(static pair => $"{EscapeIdentity(pair.Key)} {CreateComparator(pair.Value)}")
            .ToArray();
        var oldValuesPart = string.Join(" AND ", oldValuesArray);

        var newValuesArray = newValues.ToArray();
        var newValuesPart = string.Join(
            ", ",
            newValuesArray.Select(
                static pair => $"{EscapeIdentity(pair.Key)} = {EscapeSqlValue(pair.Value)}"
            )
        );

        return $"UPDATE {tableName} SET {newValuesPart} WHERE {oldValuesPart}";
    }

    public static string GenerateDelete(string tableName, TableData where)
    {
        var valuesArray = where
            .Select(static pair => $"{EscapeIdentity(pair.Key)} {CreateComparator(pair.Value)}")
            .ToArray();
        var wherePart = string.Join(" AND ", valuesArray);

        return $"DELETE FROM {tableName} WHERE {wherePart}";
    }
}
