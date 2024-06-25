using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace squi.Utils;

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

    public static string GetComparatorForValue(object? value)
    {
        if (value is null)
        {
            return "IS";
        }

        return "=";
    }

    public static string GenerateSelect(string tableName, IDictionary<string, object?> conditions)
    {
        var wherePart =
            "WHERE "
            + string.Join(
                " AND ",
                conditions
                    .AsEnumerable()
                    .Select(
                        pair =>
                            $"{EscapeIdentity(pair.Key)} {GetComparatorForValue(pair.Value)} {EscapeSqlValue(pair.Value)}"
                    )
                    .ToArray()
            );

        return $"SELECT * FROM {tableName} {
            (conditions.Count > 0 ? wherePart : "")
        }";
    }

    public static string GenerateInsert(string tableName, IDictionary<string, object?> values)
    {
        var columns = string.Join(", ", values.Keys.Select(EscapeIdentity).ToArray());

        var valuesPart = string.Join(", ", values.Values.Select(EscapeSqlValue).ToArray());

        return $"INSERT INTO {tableName} ({columns}) VALUES ({valuesPart})";
    }

    public static string GenerateUpdate(
        string tableName,
        IDictionary<string, object?> where,
        IDictionary<string, object?> values
    )
    {
        var wherePart = string.Join(
            " AND ",
            where
                .AsEnumerable()
                .Select(
                    pair =>
                        $"{EscapeIdentity(pair.Key)} {GetComparatorForValue(pair.Value)} {EscapeSqlValue(pair.Value)}"
                )
                .ToArray()
        );

        var setPart = string.Join(
            ", ",
            values
                .AsEnumerable()
                .Select(pair => $"{EscapeIdentity(pair.Key)} = {EscapeSqlValue(pair.Value)}")
                .ToArray()
        );

        return $"UPDATE {tableName} SET {setPart} WHERE {wherePart}";
    }

    public static string GenerateDelete(string tableName, IDictionary<string, object?> where)
    {
        var wherePart = string.Join(
            " AND ",
            where
                .AsEnumerable()
                .Select(
                    pair =>
                        $"{EscapeIdentity(pair.Key)} {GetComparatorForValue(pair.Value)} {EscapeSqlValue(pair.Value)}"
                )
                .ToArray()
        );

        return $"DELETE FROM {tableName} WHERE {wherePart}";
    }

    // public static bool Tests()
    // {
    //     // I wont write tests separately for this file

    //     // test escape sql
    //     if (EscapeSqlString("test") != "'test'")
    //         return false;
    //     if (EscapeSqlString("There are 'two' single quote") != "'There are ''two'' single quote'")
    //         return false;
    //     if (EscapeSqlBinay(new byte[] { 0x01, 0x02 }) != "X'0102'")
    //         return false;
    //     if (EscapeSqlValue("test") != "'test'")
    //         return false;
    //     if (EscapeSqlValue(new byte[] { 0x01, 0x02 }) != "X'0102'")
    //         return false;
    //     if (EscapeSqlValue(1) != "1")
    //         return false;

    //     // test escape identity
    //     if (EscapeIdentity("col\"name") != "\"col\"\"name\"")
    //         return false;

    //     // test unescape sql
    //     if (UnescapeIdentity("\"test\"") != "test")
    //         return false;
    //     if (UnescapeIdentity("\"\"\"\"") != "\"")
    //         return false;

    //     // test escape binary
    //     if (EscapeSqlBinay(new byte[] { 0x01, 0x02 }) != "X'0102'")
    //         return false;

    //     // test unescape identity
    //     if (UnescapeIdentity("\"test\"") != "test")
    //         return false;
    //     if (UnescapeIdentity("us\"\"ers") != "us\"ers")
    //         return false;

    //     // test GenerateSelectWithCondition
    //     if (
    //         GenerateSelectWithCondition("table", new Dictionary<string, object?> { { "id", 1 } })
    //         != "SELECT * FROM table WHERE \"id\" = 1"
    //     )
    //         return false;

    //     // test GenerateInsert
    //     if (
    //         GenerateInsert("table", new Dictionary<string, object?> { { "id", 1 } })
    //         != "INSERT INTO table (\"id\") VALUES (1)"
    //     )
    //         return false;

    //     // test GenerateUpdate
    //     if (
    //         GenerateUpdate(
    //             "table",
    //             new Dictionary<string, object?> { { "id", 1 } },
    //             new Dictionary<string, object?> { { "name", "test" } }
    //         ) != "UPDATE table SET \"name\" = 'test' WHERE \"id\" = 1"
    //     )
    //         return false;

    //     // test GenerateDelete
    //     if (
    //         GenerateDelete("table", new Dictionary<string, object?> { { "id", 1 } })
    //         != "DELETE FROM table WHERE \"id\" = 1"
    //     )
    //         return false;

    //     return true;
    // }
}
