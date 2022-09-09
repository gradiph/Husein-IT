using System.Text.Json;
using System.Text.Json.Serialization;

namespace CommonJson
{
    public class JsonResponseBuilder
    {
        private object _object;

        public JsonResponseBuilder(object @object)
        {
            _object = @object;
        }

        public T Build<T>()
        {
            string json = JsonFormatter.ToString(_object);
            return JsonFormatter.ParseString<T>(json);
        }
    }
}