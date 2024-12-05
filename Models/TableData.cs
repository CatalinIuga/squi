using System.Runtime.CompilerServices;
using System.Text.Json;
using squi.Utils;

namespace squi.Models;

/// <summary>
/// This class is used to represent a row in a table. It's just a dictionary with a fancy name.
/// The values can be of any base type, including null.
/// </summary>
public class TableData : Dictionary<string, object?>
{
    public TableData() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableData"/> class that contains elements copied from the specified <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public TableData(IDictionary<string, object?> dictionary)
    {
        foreach (var key in dictionary.Keys)
        {
            if (dictionary[key] is JsonElement jsonElement)
                dictionary[key] = JsonElementConvert.ConvertJsonElement(jsonElement);
            Add(key, dictionary[key]);
        }
    }
}
