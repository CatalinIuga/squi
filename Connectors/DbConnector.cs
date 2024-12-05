namespace squi.Connectors;

/// <summary>
/// This class is used to create the correct database connector based on the connection string.
/// </summary>
public class DbConnector
{
    /// <summary>
    /// Returns the database connector based on the connection string.
    /// </summary>
    public static IConnector CreateConnector(string connectionString) =>
        connectionString switch
        {
            // TODO: Add more connectors
            string s when s.StartsWith("postgres:")
                => throw new NotImplementedException(
                    "PostgreSQL connector is not implemented yet."
                ),
            string s when s.StartsWith("mysql:")
                => throw new NotImplementedException("MySQL connector is not implemented yet."),
            _ => new SQLiteConnector(connectionString)
        };
}
