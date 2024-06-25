using System.Text.Json;

namespace squi.Utils;

public class JsonElementConvert
{
    public static object? ConvertJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new Dictionary<string, object?>();
                foreach (var property in element.EnumerateObject())
                {
                    obj[property.Name] = ConvertJsonElement(property.Value);
                }
                return obj;
            case JsonValueKind.Array:
                var arr = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    arr.Add(ConvertJsonElement(item));
                }
                return arr;
            case JsonValueKind.String:
                return element.GetString() ?? "";
            case JsonValueKind.Number:
                return element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            default:
                throw new Exception("Unknown JsonElement type");
        }
    }
}
