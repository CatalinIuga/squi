using squi.Models;

namespace squi.Connectors;

/// <summary>
/// This interface is used to define the methods that a connector must implement.
/// </summary>
public interface IConnector
{
    /// <summary>
    /// The mandatory connection string, in the specific format for the database of choice.
    /// </summary>
    /// <example>
    /// For SQLite, this would be the path to the database file.
    /// For PostgreSQL, this would be the connection string like: "postgres://user:password@localhost:5432/database"
    /// </example>
    string ConnectionString { get; init; }

    /// <summary>
    /// The list of tables that are specific to the database, and should not be shown to the user.
    /// </summary>
    public List<string> PrivateTables { get; }

    /// <summary>
    /// Connects to the database.
    /// </summary>
    void Connect();

    /// <summary>
    /// Disconnects from the database.
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Gets the names of the tables in the database.
    /// </summary>
    Task<IEnumerable<string>> GetTableNames();

    /// <summary>
    /// Gets the schema of a table, including the columns and the number of rows.
    /// </summary>
    Task<TableSchema> GetTableSchema(string tableName);

    /// <summary>
    /// Gets the data from a table. This method should support filtering, limiting and offsetting.
    /// </summary>
    Task<IEnumerable<TableData>> GetTableData(
        string tableName,
        IDictionary<string, object?> filters,
        int limit = 50,
        int offset = 0
    );

    /// <summary>
    /// Inserts data into a table. The data should be a dictionary with the column names as keys.
    /// </summary>
    Task<Result> InsertData(string tableName, TableData data);

    /// <summary>
    /// Updates data in a table. The data should be a dictionary with the column names as keys.
    /// </summary>
    Task<Result> UpdateData(string tableName, TableData data);

    /// <summary>
    /// Deletes data from a table. The data should be a dictionary with the column names as keys.
    /// </summary>
    Task<Result> DeleteData(string tableName, TableData data);
}
