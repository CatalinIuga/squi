using System.Data;
using System.Text.Json;
using Squi.Connectors;

var connectionString = args[0];

var sqliteProvider = new SQLiteProvider(connectionString);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet(
    "/tables",
    () =>
    {
        return sqliteProvider.GetTables();
    }
);

app.MapGet(
    "/tables/{tableName}",
    (string tableName) =>
    {
        var schema = sqliteProvider.GetSchema(tableName);
        return schema;
    }
);

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

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
    options.AllowAnyMethod();
});

app.Run();
