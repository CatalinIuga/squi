using Microsoft.AspNetCore.Mvc;

namespace squi.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ClientController : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream("squi.wwwroot.index.html");

        if (resourceStream is null)
            return NotFound();

        using var reader = new StreamReader(resourceStream);
        var content = reader.ReadToEnd();
        return Content(content, "text/html");
    }

    [HttpGet("/assets/{*path}")]
    public IActionResult Assets(string path)
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream($"squi.wwwroot.assets.{path}");

        if (resourceStream is null)
            return NotFound();

        using var reader = new StreamReader(resourceStream);
        var content = reader.ReadToEnd();
        return Path.GetExtension(path) switch
        {
            ".js" => Content(content, "application/javascript"),
            ".css" => Content(content, "text/css"),
            ".png" => Content(content, "image/png"),
            ".svg" => Content(content, "image/svg+xml"),
            _ => Content(content, "text/plain"),
        };
    }
}
