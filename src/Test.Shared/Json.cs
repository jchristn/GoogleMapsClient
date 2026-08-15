using System.Text.Json;

namespace Test.Shared
{
    /// <summary>
    /// Thin wrapper over System.Text.Json for the test suites. Deserialization relies on the
    /// <c>[JsonPropertyName]</c> attributes declared on the library's public models, so these
    /// helpers double as verification that those attributes are correct.
    /// </summary>
    internal static class Json
    {
        private static readonly JsonSerializerOptions _Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        internal static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _Options);
        }

        internal static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, _Options);
        }
    }
}
