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

    [HttpPost("{tableName}/data/select")]
    public async Task<IEnumerable<dynamic>> GetData(
        string tableName,
        [FromBody] Dictionary<string, object?> filters,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0
    )
    {
        try
        {
            // must convert JsonElement to Dictionary<string, object?>
            foreach (var key in filters.Keys)
            {
                filters[key] = filters[key] is null
                    ? null
                    : JsonElementConvert.ConvertJsonElement((JsonElement)filters[key]!);
            }
            var data = await _dbConnector.GetTableData(tableName, filters, limit, offset);
            return data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Array.Empty<dynamic>();
        }
    }

    [HttpPost("{tableName}/data")]
    public async Task<IActionResult> InsertData(
        string tableName,
        [FromBody] IDictionary<string, object?> data
    )
    {
        foreach (var key in data.Keys)
        {
            Console.WriteLine(data[key]);
        }

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
        foreach (var key in data.Keys)
        {
            data[key] = data[key] is null
                ? null
                : JsonElementConvert.ConvertJsonElement((JsonElement)data[key]!);
        }
        var deleted = await _dbConnector.DeleteData(tableName, new TableData(data));

        return deleted.Ok switch
        {
            true => Ok(new { message = "Data deleted" }),
            _ => BadRequest(new { message = deleted.Err }),
        };
    }

    // TODO: Implement the following methods
    public class DataFilters
    {
        public string Column { get; set; } = null!;
        public string Value { get; set; } = null!;

        public Operator Operator { get; set; }
    }

    public enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Like,
        NotLike,
        In,
        NotIn,
        IsNull,
        IsNotNull,
    }
}
