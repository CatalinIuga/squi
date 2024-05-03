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

    public Task<IEnumerable<dynamic>> GetTableData(
        string tableName,
        int limit = 20,
        int offset = 0,
        string[] filters = null!
    )
    {
        var sql = $"SELECT * FROM {tableName}";
        if (filters.Length > 0)
        {
            sql += " WHERE ";
            sql += filters.Aggregate((current, filter) => current + $" AND {filter}");
        }
        sql += $" LIMIT {limit} OFFSET {offset}";
        Console.WriteLine(sql);
        var data = Connection.QueryAsync<dynamic>(sql);
        return data;
    }

    public Task<dynamic> InsertData(string tableName, dynamic data)
    {
        throw new NotImplementedException();
    }

    public Task<dynamic> UpdateData(string tableName, dynamic data)
    {
        throw new NotImplementedException();
    }

    public Task<dynamic> DeleteData(string tableName, dynamic data)
    {
        throw new NotImplementedException();
    }
}
