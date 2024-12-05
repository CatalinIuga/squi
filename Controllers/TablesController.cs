using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using squi.Connectors;
using squi.Models;
using squi.Utils;

namespace squi.Controllers;

[ApiController]
[Route("tables")]
public class TablesController : ControllerBase
{
    private readonly IConnector _dbConnector;

    public TablesController(IConnector connector)
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

    [HttpPost("{tableName}/select")]
    public async Task<IEnumerable<dynamic>> SelectData(
        string tableName,
        [FromBody] DataFilters[]? filters,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0
    )
    {
        filters ??= Array.Empty<DataFilters>();

        var selected = await _dbConnector.SelectData(tableName, filters, limit, offset);

        return selected.Ok switch
        {
            true => selected.Data,
            _ => BadRequest(new { message = selected.Err })
        };
    }

    [HttpPost("{tableName}/insert")]
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

    [HttpPost("{tableName}/update")]
    public async Task<IActionResult> UpdateData(string tableName, [FromBody] UpdateDataRequest data)
    {
        var oldValues = new TableData(data.OldData);
        var newValues = new TableData(data.NewData);

        var updated = await _dbConnector.UpdateData(tableName, oldValues, newValues);

        return updated.Ok switch
        {
            true => Ok(new { message = "Data updated" }),
            _ => BadRequest(new { message = updated.Err }),
        };
    }

    [HttpPost("{tableName}/delete")]
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

    public class UpdateDataRequest
    {
        public required IDictionary<string, object?> OldData { get; set; }
        public required IDictionary<string, object?> NewData { get; set; }
    }
}
