using Squi.Connectors;

// TODO: Add proper CLI parsing
var connectionString = args[0];

// TODO: Add DI container based on the args
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Please provide a connection string to a SQLite database.");
    Environment.Exit(1);
}

// TODO: Swap for a proper DI container
var sqliteProvider = new SQLiteProvider(connectionString);

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

// TODO: Add a proper configuration - this warns for some reason
builder
    .WebHost
    .ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5076);
    });

// TODO: Add a proper DI container
builder.Services.AddSingleton(sqliteProvider);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// TODO: is this needed?
builder.Services.AddDirectoryBrowser();

var app = builder.Build();
app.UseStaticFiles();

// TODO: Add a middleware to handle exceptions & actual cors policy
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
// TODO: Add an actual logger
Console.ForegroundColor = ConsoleColor.DarkBlue;
Console.Write("Running on: ");
Console.ForegroundColor = ConsoleColor.DarkGreen;
Console.WriteLine("http://localhost:5076");
Console.ResetColor();

app.Lifetime.ApplicationStopping.Register(sqliteProvider.Close);

app.Run();
