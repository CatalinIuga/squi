using squi.Connectors;

if (args.Length != 1)
{
    Console.WriteLine("Usage: squi <connectionString>");
    Console.WriteLine("Example: squi chinook.db");
    Console.WriteLine(
        "Example: squi postgresql://[user[:password]@][netloc][:port][/dbname][?param1=value1&...]"
    );
    Environment.Exit(1);
}

var connector = DbConnector.CreateConnector(args[0]);
connector.Connect();

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
builder.Services.AddSingleton(connector);
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

app.Lifetime.ApplicationStopping.Register(connector.Disconnect);

app.Run();
