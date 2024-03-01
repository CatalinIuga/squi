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
app.Lifetime.ApplicationStopping.Register(sqliteProvider.Close);

app.Run();
