using AspNetCore.Scalar;
using squi.Connectors;
using squi.Utils;

if (args.Length != 1)
{
    Logger.Error("No connection string provided");
    Environment.Exit(1);
}

var connector = DbConnector.CreateConnector(args[0]);
connector.Connect();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.WebHost.ConfigureKestrel(o => o.ListenLocalhost(5076));

builder.Services.AddSingleton(connector);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

app.UseStaticFiles();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapControllers();

app.UseSwagger();
app.UseScalar(o => o.RoutePrefix = "docs");

Logger.Info("Running on http://localhost:5076");

app.Lifetime.ApplicationStopping.Register(connector.Disconnect);

app.Run();
