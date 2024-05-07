namespace squi.Models;

/// <summary>
/// Represents the result of a database operation.
/// This includes whether the operation was successful and any messages that should be displayed to the user.
/// </summary>
public class Result
{
    public bool Ok { get; set; }
    public string Err { get; set; } = "";
}
