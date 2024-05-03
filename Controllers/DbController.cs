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

    // [HttpPost("{tableName}/data")]
    // public IActionResult InsertData(string tableName, [FromBody] IDictionary<string, object?> data)
    // {
    //     try
    //     {
    //         var inserted = _dbConnector.InsertData(tableName, data);
    //         return Ok(new { message = "Data inserted", data = inserted });
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(new { message = e.Message });
    //     }
    // }

    // [HttpPut("{tableName}/data")]
    // public IActionResult UpdateData(string tableName, [FromBody] IDictionary<string, object?> data)
    // {
    //     try
    //     {
    //         var updated = _dbConnector.UpdateData(tableName, data);
    //         return Ok(new { message = "Data updated", data = updated });
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(new { message = e.Message });
    //     }
    // }

    // [HttpDelete("{tableName}/data")]
    // public IActionResult DeleteData(string tableName, [FromBody] IDictionary<string, object?> data)
    // {
    //     try
    //     {
    //         _dbConnector.DeleteData(tableName, data);
    //         return Ok(new { message = "Data deleted" });
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(new { message = e.Message });
    //     }
    // }
}
