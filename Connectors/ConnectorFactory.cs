namespace squi.API.Connectors
{
    public class ConnectorFactory
    {
        public static IConnector CreateConnector(string connectionString)
        {
            var type = MatchDatabaseType(connectionString);
            return type switch
            {
                "postgres" => new PostgresConnector(connectionString),
                "mysql" => new MySqlConnector(connectionString),
                _ => new SqliteConnector(connectionString)
            };
        }

        /// <summary>
        /// Returns the database type based on the connection string.
        /// </summary>

        public static string MatchDatabaseType(string connectionString) =>
            connectionString switch
            {
                string s when s.StartsWith("postgres:") => "postgres",
                string s when s.StartsWith("mysql:") => "mysql",
                _ => "sqlite"
            };
    }
}
