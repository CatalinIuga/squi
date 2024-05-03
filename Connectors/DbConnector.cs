using System.Text.RegularExpressions;

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
            string s when s.StartsWith("postgres:") => new SQLiteConnector(connectionString),
            string s when s.StartsWith("mysql:") => new SQLiteConnector(connectionString),
            _ => new SQLiteConnector(connectionString)
        };
}
