using Microsoft.AspNetCore.Mvc;
using squi.Connectors;
using squi.Models;

namespace squi.Controllers;

[ApiController]
[Route("tables")]
public class DbController : ControllerBase
{
    private readonly IConnector _dbConnector;

    public DbController(IConnector connector)
    {
        _dbConnector = connector;
    }

    [HttpGet]
    public async Task<IEnumerable<string>> GetTables()
    {
        var tables = await _dbConnector.GetTableNames();
        return tables;
    }

    [HttpGet("{tableName}")]
    public async Task<TableSchema> GetSchema(string tableName)
    {
        var schema = await _dbConnector.GetTableSchema(tableName);
        return schema;
    }

    [HttpGet("{tableName}/data")]
    public async Task<IEnumerable<dynamic>> GetData(
        string tableName,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string[]? filter = null
    )
    {
        try
        {
            filter ??= Array.Empty<string>();
            var data = await _dbConnector.GetTableData(tableName, filter, limit, offset);
            return data;
        }
        catch
        {
            return Array.Empty<dynamic>();
        }
    }

    [HttpPost("{tableName}/data")]
    public async Task<IActionResult> InsertData(
        string tableName,
        [FromBody] IDictionary<string, object?> data
    )
    {
        var inserted = await _dbConnector.InsertData(tableName, new TableData(data));

        return inserted.Ok switch
        {
            true => Ok(new { message = "Data inserted" }),
            _ => BadRequest(new { message = inserted.Err }),
        };
    }

    [HttpPut("{tableName}/data")]
    public async Task<IActionResult> UpdateData(
        string tableName,
        [FromBody] IDictionary<string, object?> data
    )
    {
        var updated = await _dbConnector.UpdateData(tableName, new TableData(data));

        return updated.Ok switch
        {
            true => Ok(new { message = "Data updated" }),
            _ => BadRequest(new { message = updated.Err }),
        };
    }

    [HttpDelete("{tableName}/data")]
    public async Task<IActionResult> DeleteData(
        string tableName,
        [FromBody] IDictionary<string, object?> data
    )
    {
        var deleted = await _dbConnector.DeleteData(tableName, new TableData(data));

        return deleted.Ok switch
        {
            true => Ok(new { message = "Data deleted" }),
            _ => BadRequest(new { message = deleted.Err }),
        };
    }
}
