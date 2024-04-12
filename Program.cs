using Squi.Connectors;

var connectionString = args[0];

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Please provide a connection string to a SQLite database.");
    Environment.Exit(1);
}

var sqliteProvider = new SQLiteProvider(connectionString);

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders(); 

builder
    .WebHost
    .ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5076);
    });

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

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "squi.API v1"));
}

// Log the connection string
Console.ForegroundColor = ConsoleColor.DarkBlue;
Console.Write("Running on: ");
Console.ForegroundColor = ConsoleColor.DarkGreen;
Console.WriteLine("http://localhost:5076");
Console.ResetColor();

app.Lifetime.ApplicationStopping.Register(sqliteProvider.Close);

app.Run();
