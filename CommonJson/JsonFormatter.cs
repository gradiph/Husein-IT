using System.Text.Json;
using System.Text.Json.Serialization;

namespace CommonJson
{
    public static class JsonFormatter
    {
        public static string ToString(object @object)
        {
            return JsonSerializer.Serialize(@object, GetDefaultOption());
        }
        
        public static T ParseString<T>(string @string)
        {
            return JsonSerializer.Deserialize<T>(@string);
        }

        public static T ParseStream<T>(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream);
        }

        private static JsonSerializerOptions GetDefaultOption()
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            return option;
        }
    }
}
