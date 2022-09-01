using System.Text.Json;

namespace CommonJson
{
    public static class JsonFormatter
    {
        public static string ToString(object @object)
        {
            return JsonSerializer.Serialize(@object);
        }

        public static T ParseString<T>(string @string)
        {
            return JsonSerializer.Deserialize<T>(@string);
        }

        public static T ParseStream<T>(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream);
        }
    }
}
