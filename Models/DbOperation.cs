/// <summary>
/// Represents the result of a database operation. 
/// This includes whether the operation was successful and any messages that should be displayed to the user.
/// </summary>
public class DBOperation
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}
