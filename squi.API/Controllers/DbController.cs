using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Squi.Connectors;

namespace squi.API.Controllers;

[ApiController]
[Route("tables")]
public class DbController : ControllerBase
{
    private readonly SQLiteProvider _sqliteProvider;

    public DbController(SQLiteProvider sqliteProvider)
    {
        _sqliteProvider = sqliteProvider;
    }

    [HttpGet]
    public ActionResult<IEnumerable<string>> GetTables()
    {
        return _sqliteProvider.GetTables();
    }

    [HttpGet("{tableName}")]
    public ActionResult GetSchema(string tableName)
    {
        var schema = _sqliteProvider.GetSchema(tableName);
        return Ok(schema);
    }

    [HttpGet("{tableName}/data")]
    public ActionResult<IEnumerable<IDictionary<string, object>>> GetData(
        string tableName,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0
    )
    {
        var data = _sqliteProvider.GetData(tableName, limit, offset);
        var aux = data.AsEnumerable()
            .Select(
                row =>
                    row.Table
                        .Columns
                        .Cast<DataColumn>()
                        .ToDictionary(
                            col => col.ColumnName,
                            col => row[col] is DBNull ? null : row[col]
                        )
            )
            .ToArray();

        return Ok(aux);
    }

    [HttpPost("{tableName}/data")]
    public IActionResult InsertData(string tableName, [FromBody] IDictionary<string, object?> data)
    {
        try
        {
            var inserted = _sqliteProvider.InsertData(tableName, data);
            return Ok(new { message = "Data inserted", data = inserted });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPut("{tableName}/data")]
    public IActionResult UpdateData(string tableName, [FromBody] IDictionary<string, object?> data)
    {
        try
        {
            var updated = _sqliteProvider.UpdateData(tableName, data);
            return Ok(new { message = "Data updated", data = updated });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpDelete("{tableName}/data")]
    public IActionResult DeleteData(string tableName, [FromBody] IDictionary<string, object?> data)
    {
        try
        {
            _sqliteProvider.DeleteData(tableName, data);
            return Ok(new { message = "Data deleted" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}
