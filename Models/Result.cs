namespace squi.Models;

/// <summary>
/// Represents the result of a database operation.
/// This includes whether the operation was successful and any messages that should be displayed to the user.
/// </summary>
public class Result
{
    /// <summary>
    /// Whether the operation was successful.
    /// </summary>
    public bool Ok { get; set; }

    /// <summary>
    /// Any data that should be returned to the user.
    /// </summary>
    public dynamic Data { get; set; } = null!;

    /// <summary>
    /// Any error messages that should be displayed to the user.
    /// </summary>
    public string Err { get; set; } = "";
}
