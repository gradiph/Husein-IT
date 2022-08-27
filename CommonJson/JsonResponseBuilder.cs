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
            var option = GetDefaultOption();
            string json = JsonSerializer.Serialize(_object, option);
            return JsonSerializer.Deserialize<T>(json); ;
        }

        private JsonSerializerOptions GetDefaultOption()
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return option;
        }
    }
}