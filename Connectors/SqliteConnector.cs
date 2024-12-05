using System.Data.SQLite;
using Dapper;
using squi.Models;
using squi.Utils;

namespace squi.Connectors;

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
            Logger.Error(e.Message);
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
        var columns = Connection.GetSchemaAsync("Columns", new[] { null, null, tableName, null });
        var count = Connection.QuerySingleAsync<int>($"SELECT COUNT(*) FROM {tableName}");
        return Task.FromResult(new TableSchema(columns.Result, count.Result));
    }

    public Task<Result> SelectData(
        string tableName,
        IEnumerable<DataFilters> filters,
        int limit = 50,
        int offset = 0
    )
    {
        var sql = SqlHelpers.GenerateSelect(tableName, filters, limit, offset);
        try
        {
            var data = Connection.QueryAsync(sql);
            var result = data.Result.Select(row => new TableData(row));
            return Task.FromResult(new Result { Ok = true, Data = result });
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
            return Task.FromResult(new Result { Ok = false, Err = e.Message });
        }
    }

    public Task<Result> InsertData(string tableName, TableData data)
    {
        var sql = SqlHelpers.GenerateInsert(tableName, data);
        try
        {
            Connection.Execute(sql);
            return CheckChanges() switch
            {
                true => Task.FromResult(new Result { Ok = true }),
                false => Task.FromResult(new Result { Ok = false, Err = "No rows were inserted" }),
            };
        }
        catch (Exception e)
        {
            return Task.FromResult(new Result { Ok = false, Err = e.Message });
        }
    }

    public Task<Result> UpdateData(string tableName, TableData oldValues, TableData newValues)
    {
        var sql = SqlHelpers.GenerateUpdate(tableName, oldValues, newValues);
        try
        {
            Connection.Execute(sql);
            return CheckChanges() switch
            {
                true => Task.FromResult(new Result { Ok = true }),
                false => Task.FromResult(new Result { Ok = false, Err = "No rows were updated" }),
            };
        }
        catch (Exception e)
        {
            return Task.FromResult(new Result { Ok = false, Err = e.Message });
        }
    }

    public Task<Result> DeleteData(string tableName, TableData data)
    {
        var sql = SqlHelpers.GenerateDelete(tableName, data);
        try
        {
            Connection.Execute(sql);
            return CheckChanges() switch
            {
                true => Task.FromResult(new Result { Ok = true }),
                false => Task.FromResult(new Result { Ok = false, Err = "No rows were deleted" }),
            };
        }
        catch (Exception e)
        {
            return Task.FromResult(new Result { Ok = false, Err = e.Message });
        }
    }

    public bool CheckChanges()
    {
        return Connection.QuerySingle<int>($"SELECT changes()") > 0;
    }
}
