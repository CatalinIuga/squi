using System.Data;
using System.Text.Json;
using Squi.Connectors;

var connectionString = args[0];

var sqliteProvider = new SQLiteProvider(connectionString);
var tables = sqliteProvider.GetTables();
foreach (var table in tables)
{
    var schema = sqliteProvider.GetSchema(table);
    foreach (var column in schema.Columns)
    {
        Console.WriteLine(
            $"{column.Name}: {column.Type} {column.IsNullable} {column.IsPrimaryKey} {column.IsAutoIncrement} {column.IsUnique} {column.HasDefault} {column.Default}"
        );
    }
    Console.WriteLine("------");
}

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet(
    "/tables",
    () =>
    {
        return tables;
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
        var aux = data.Rows
            .OfType<DataRow>()
            .Select(
                x =>
                    data.Columns
                        .OfType<DataColumn>()
                        .ToDictionary(column => column.ColumnName, column => x[column.ColumnName])
            );

        return JsonSerializer.Serialize(aux);
    }
);

app.Run();
