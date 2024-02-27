using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Squi.Connectors;

var connectionString = args[0];

var sqliteProvider = new SQLiteProvider(connectionString);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sqliteProvider);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();
builder.Services.AddDirectoryBrowser();

var app = builder.Build();
app.UseStaticFiles();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
    options.AllowAnyMethod();
});

app.MapGet("/", () => Results.Redirect("/index.html"));
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "squi.API v1"));
}

// close the db connection when the app stops
app.Lifetime.ApplicationStopping.Register(() => sqliteProvider.Close());

// open the default browser
Process.Start(
    new ProcessStartInfo
    {
        FileName = app.Environment.IsDevelopment()
            ? "http://localhost:5173"
            : "http://localhost:5076",
        UseShellExecute = true,
        Verb = "open"
    }
);

app.Run();
