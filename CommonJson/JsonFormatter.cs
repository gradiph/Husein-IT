using System.Text.Json;

namespace CommonJson
{
    public class JsonFormatter
    {
        public static string ToString(object @object)
        {
            return JsonSerializer.Serialize(@object);
        }

        public static T ToObject<T>(string @string)
        {
            return JsonSerializer.Deserialize<T>(@string);
        }
    }
}
