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
    }

    /// <summary>
    /// Gets the schema of a table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    public TableSchema GetSchema(string tableName)
    {
        connection.Open();
        var schema = connection.GetSchema("Columns", new[] { null, null, tableName });
        connection.Close();

        return new TableSchema(schema);
    }

    /// <summary>
    /// Gets the data from a table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
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

    /// <summary>
    /// Gets the names of the tables in the database.
    /// </summary>
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
            .Select(x => x["TABLE_NAME"].ToString() ?? string.Empty)
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
