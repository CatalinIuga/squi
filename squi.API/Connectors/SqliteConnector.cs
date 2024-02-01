using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using Squi.Models;

namespace Squi.Connectors;

public class SQLiteProvider
{
    private readonly DbConnection connection = SQLiteFactory.Instance.CreateConnection();
    public List<string> PrivateTables { get; } =
        new List<string> { "sqlite_sequence", "sqlite_stat1", };

    public SQLiteProvider(string path = "sample.db")
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Database file does not exist.");
            return;
        }
        connection.ConnectionString = $"Data Source={path}";
    }

    public TableSchema GetSchema(string tableName)
    {
        connection.Open();
        var schema = connection.GetSchema("Columns", new[] { null, null, tableName });
        connection.Close();

        return new TableSchema(schema);
    }

    public DataTable GetData(string tableName)
    {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        var reader = command.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        connection.Close();

        return table;
    }

    public string[] GetTables()
    {
        if (connection is null)
        {
            throw new Exception("Connection is null");
        }

        connection.Open();
        var schema = connection.GetSchema("Tables");
        connection.Close();

        return schema
            .Rows
            .Cast<DataRow>()
            .Select(x => x["TABLE_NAME"].ToString()!)
            .Where(x => !PrivateTables.Contains(x))
            .ToArray();
    }

    public static void DisplayData(DataTable table)
    {
        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
            {
                Console.WriteLine($"{column.ColumnName}: {row[column]}");
            }
            Console.WriteLine("----------------------------------");
        }
    }
}
