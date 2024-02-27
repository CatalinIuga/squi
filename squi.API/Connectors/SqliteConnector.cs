using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using Squi.Models;

namespace Squi.Connectors;

/// <summary>
/// This class is used to connect to a SQLite database.
/// </summary>
public class SQLiteProvider
{
    /// <summary>
    /// The connection to the database.
    /// </summary>
    private readonly DbConnection connection = SQLiteFactory.Instance.CreateConnection();

    private readonly DbDataAdapter adapter = SQLiteFactory.Instance.CreateDataAdapter();

    private readonly DbCommandBuilder builder = SQLiteFactory.Instance.CreateCommandBuilder();

    /// <summary>
    /// The tables that should not be displayed.
    /// </summary>
    public List<string> PrivateTables { get; } =
        new List<string> { "sqlite_sequence", "sqlite_stat1", };

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLiteProvider"/> class.
    /// </summary>
    public SQLiteProvider(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Database file does not exist.");
            return;
        }
        connection.ConnectionString = $"Data Source={path}";
        connection.Open();
    }

    /// <summary>
    /// Gets the schema of a table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    public TableSchema GetSchema(string tableName)
    {
        var schema = connection.GetSchema("Columns", new[] { null, null, tableName });
        var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM {tableName}";
        var rowCount = cmd.ExecuteScalar();
        return new TableSchema(schema, rowCount is null ? 0 : Convert.ToInt32(rowCount));
    }

    /// <summary>
    /// Gets the data from a table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    public DataTable GetData(string tableName, int limit, int offset)
    {
        var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName} LIMIT {limit} OFFSET {offset}";
        var reader = command.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);

        return table;
    }

    /// <summary>
    /// Gets the names of the tables in the database.
    /// </summary>
    public string[] GetTables()
    {
        if (connection is null)
        {
            throw new Exception("Connection is null");
        }

        var schema = connection.GetSchema("Tables");

        return schema
            .Rows
            .Cast<DataRow>()
            .Select(x => x["TABLE_NAME"].ToString() ?? string.Empty)
            .Where(x => !PrivateTables.Contains(x))
            .ToArray();
    }

    public IDictionary<string, object?> InsertData(
        string tableName,
        IDictionary<string, object> data
    )
    {
        var dt = connection.GetSchema("Columns", new[] { null, null, tableName });
        var columns = dt.Rows
            .Cast<DataRow>()
            .Where(x => x["AUTOINCREMENT"].ToString()! == "False")
            .Select(x => x["COLUMN_NAME"].ToString())
            .ToArray();

        var command = connection.CreateCommand();
        command.CommandText =
            $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(x => $"@{x}"))}) RETURNING *";
        foreach (var column in columns)
        {
            if (column is not null)
                command.Parameters.Add(new SQLiteParameter($"@{column}", data[column]));
        }

        var reader = command.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);

        var row =
            table
                .Rows
                .Cast<DataRow>()
                .First()
                .Table
                .Columns
                .Cast<DataColumn>()
                .ToDictionary(
                    col => col.ColumnName,
                    col => table.Rows[0][col] is DBNull ? null : table.Rows[0][col]
                ) ?? throw new Exception("Could not insert data.");
        return row;
    }

    public IDictionary<string, object?> UpdateData(
        string tableName,
        IDictionary<string, object> data
    )
    {
        var dt = connection.GetSchema("Columns", new[] { null, null, tableName });
        var columns = dt.Rows.Cast<DataRow>().Select(x => x["COLUMN_NAME"].ToString()).ToArray();

        var command = connection.CreateCommand();

        command.CommandText =
            $"UPDATE {tableName} SET {string.Join(", ", columns.Select(x => $"{x} = @{x}"))} WHERE {string.Join(" AND ", columns.Select(x => $"{x} = @__initial_{x}"))} RETURNING *";

        foreach (var column in columns)
        {
            if (column is not null)
            {
                command.Parameters.Add(new SQLiteParameter($"@{column}", data[column]));
                command
                    .Parameters
                    .Add(new SQLiteParameter($"@__initial_{column}", data[$"__initial_{column}"]));
            }
        }

        var reader = command.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);

        var row =
            table
                .Rows
                .Cast<DataRow>()
                .First()
                .Table
                .Columns
                .Cast<DataColumn>()
                .ToDictionary(
                    col => col.ColumnName,
                    col => table.Rows[0][col] is DBNull ? null : table.Rows[0][col]
                ) ?? throw new Exception("Could not update data.");

        return row;
    }

    public void DeleteData(string tableName, IDictionary<string, object?> data)
    {
        var dt = connection.GetSchema("Columns", new[] { null, null, tableName });
        var columns = dt.Rows.Cast<DataRow>().Select(x => x["COLUMN_NAME"].ToString()).ToArray();

        var command = connection.CreateCommand();
        command.CommandText =
            $"DELETE FROM {tableName} WHERE {string.Join(" AND ", columns.Select(x => $"{x} {(data[x!] is null ? "IS NULL" : $"= @{x}")}"))}";

        foreach (var column in columns)
        {
            if (column is not null)
                command.Parameters.Add(new SQLiteParameter($"@{column}", data[column]));
        }
        if (command.ExecuteNonQuery() == 0)
            throw new Exception("Could not delete data.");
    }

    /// <summary>
    /// Close the connection to the database.
    /// </summary>
    public void Close()
    {
        connection.Close();
    }
}
