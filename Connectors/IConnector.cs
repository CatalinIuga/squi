using Squi.Models;

namespace squi.API.Connectors;

public interface IConnector
{
    string ConnectionString { get; set; }

    void Connect();
    void Disconnect();

    Task<List<string>> GetTableNames();
    Task<TableSchema> GetTableSchema(string tableName);

    Task<dynamic> GetTableData(string tableName, int limit, int offset, string[] filters);
    Task<dynamic> InsertData(string tableName, dynamic data);
    Task<dynamic> UpdateData(string tableName, dynamic data);
    Task<dynamic> DeleteData(string tableName, dynamic data);
}
