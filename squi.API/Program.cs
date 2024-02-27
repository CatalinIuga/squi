using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Squi.Connectors;

var connectionString = args[0];

var sqliteProvider = new SQLiteProvider(connectionString);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddDirectoryBrowser();

var app = builder.Build();
app.UseStaticFiles();

/// <summary>
/// Serves the wwwwroot folder. Here we will server the vite production build.
/// </summary>
app.MapGet(
    "/",
    () =>
    {
        return Results.Redirect("/index.html");
    }
);

/// <summary>
/// Gets the tables in the database.
/// </summary>
app.MapGet(
    "/tables",
    () =>
    {
        return sqliteProvider.GetTables();
    }
);

/// <summary>
/// Gets the schema of a table.
/// </summary>

app.MapGet(
    "/tables/{tableName}",
    (string tableName) =>
    {
        var schema = sqliteProvider.GetSchema(tableName);
        return schema;
    }
);

/// <summary>
/// Gets the data from a table.
/// </summary>
app.MapGet(
    "/tables/{tableName}/data",
    (string tableName) =>
    {
        var data = sqliteProvider.GetData(tableName);
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

        return JsonSerializer.Serialize(aux);
    }
);

app.MapPost(
    "/tables/{tableName}/data",
    (string tableName, [FromBody] IDictionary<string, object> data) =>
    {
        sqliteProvider.InsertData(tableName, data);
        return Results.Ok();
    }
);

app.MapPut(
    "/tables/{tableName}/data",
    (string tableName, [FromBody] IDictionary<string, object> data) =>
    {
        sqliteProvider.UpdateData(tableName, data);
        return Results.Ok();
    }
);

app.MapDelete(
    "/tables/{tableName}/data",
    (string tableName, [FromBody] IDictionary<string, object> data) =>
    {
        sqliteProvider.DeleteData(tableName, data);
        return Results.Ok();
    }
);

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
    options.AllowAnyMethod();
});

// close the db connection when the app stops
app.Lifetime.ApplicationStopping.Register(() => sqliteProvider.Close());

app.Run();
