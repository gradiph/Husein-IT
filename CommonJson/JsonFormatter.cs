using System.Text.Json;

namespace CommonJson
{
    public class JsonFormatter
    {
        public static string toString(object @object)
        {
            return JsonSerializer.Serialize(@object);
        }

        public static T toObject<T>(string @string)
        {
            return JsonSerializer.Deserialize<T>(@string);
        }
    }
}
