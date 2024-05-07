using System.Data.SQLite;
using Dapper;
using squi.Models;

namespace squi.Connectors;

// TODO: Swap to Dapper or something cause i cant get this reference to work

/// <summary>
/// This class is used to connect to a SQLite database.
/// </summary>
public class SQLiteConnector : IConnector
{
    public string ConnectionString { get; init; }

    public List<string> PrivateTables { get; } =
        new List<string> { "sqlite_sequence", "sqlite_stat1", };

    public SQLiteConnection Connection { get; set; } = null!;

    public SQLiteConnector(string connectionString)
    {
        ConnectionString = $"Data Source={connectionString}";
    }

    public void Connect()
    {
        try
        {
            Connection = new SQLiteConnection(ConnectionString);
            Connection.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
    }

    public void Disconnect()
    {
        Connection.Close();
    }

    public Task<IEnumerable<string>> GetTableNames()
    {
        var sql = PrivateTables.Aggregate(
            "SELECT name FROM sqlite_master WHERE type='table' AND name NOT IN (",
            (current, table) => current + $"'{table}', "
        );
        sql = sql.Remove(sql.Length - 2) + ")";
        var tables = Connection.QueryAsync<string>(sql);
        return tables;
    }

    public Task<TableSchema> GetTableSchema(string tableName)
    {
        var columns = Connection.QueryAsync<ColumnSchema>($"PRAGMA table_info({tableName})");
        var count = Connection.QuerySingleAsync<int>($"SELECT COUNT(*) FROM {tableName}");
        return Task.FromResult(
            new TableSchema { Columns = columns.Result, RowCount = count.Result, }
        );
    }

    public Task<IEnumerable<TableData>> GetTableData(
        string tableName,
        string[] filters,
        int limit = 50,
        int offset = 0
    )
    {
        var sql = $"SELECT * FROM {tableName}";
        if (filters.Length > 0)
        {
            sql += " WHERE ";
            sql += filters.Aggregate((current, filter) => current + $" AND {filter}");
        }
        sql += $" LIMIT {limit} OFFSET {offset}";
        var data = Connection.QueryAsync(sql);
        return Task.FromResult(data.Result.Select(row => new TableData(row)));
    }

    // TODO: Frontend must be updated for the bellow methods to work
    public Task<Result> InsertData(string tableName, TableData data)
    {
        var columns = data.Keys
            .Where(column => !column.StartsWith("__initial_") && column != "isnewRow")
            .Aggregate("", (current, column) => current + $"{column}, ");
        columns = columns.Remove(columns.Length - 2);
        var values = data.Values.Aggregate(
            "",
            (current, value) => current + $"'{(value is null ? DBNull.Value : value)}', "
        );
        values = values.Remove(values.Length - 2);
        var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        Console.WriteLine(sql);
        try
        {
            Connection.Execute(sql);
            return Task.FromResult(new Result { Ok = true });
        }
        catch (Exception e)
        {
            return Task.FromResult(new Result { Ok = false, Err = e.Message });
        }
    }

    public Task<Result> UpdateData(string tableName, TableData data)
    {
        var set = data.Where(column => !column.Key.StartsWith("__initial_"))
            .Aggregate("", (current, column) => current + $"{column.Key} = '{column.Value}', ");

        var where = data.Where(column => column.Key.StartsWith("__initial_"))
            .Aggregate(
                "",
                (current, column) =>
                    current + $"{column.Key.Remove(column.Key.Length - 5)} = '{column.Value}' AND "
            );

        set = set.Remove(set.Length - 2);
        where = where.Remove(where.Length - 5);
        var sql = $"UPDATE {tableName} SET {set} WHERE {where}";
        Console.WriteLine(sql);
        try
        {
            Connection.Execute(sql);
            return Task.FromResult(new Result { Ok = true });
        }
        catch (Exception e)
        {
            return Task.FromResult(new Result { Ok = false, Err = e.Message });
        }
    }

    public Task<Result> DeleteData(string tableName, TableData data)
    {
        var where = data.Aggregate(
            "",
            (current, column) => current + $"{column.Key} = '{column.Value}' AND "
        );
        where = where.Remove(where.Length - 5);
        var sql = $"DELETE FROM {tableName} WHERE {where}";
        try
        {
            Connection.Execute(sql);
            // test if the delete was successful
            if (Connection.QuerySingle<int>($"SELECT changes()") == 0)
            {
                return Task.FromResult(new Result { Ok = false, Err = "No rows were deleted" });
            }
            return Task.FromResult(new Result { Ok = true });
        }
        catch (Exception e)
        {
            return Task.FromResult(new Result { Ok = false, Err = e.Message });
        }
    }
}
