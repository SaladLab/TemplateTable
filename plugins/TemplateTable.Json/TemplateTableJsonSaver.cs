using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TemplateTable
{
    public class TemplateTableJsonSaver<TKey, TValue>
        where TValue : class, new()
    {
        private readonly JsonSerializer _serializer;

        public TemplateTableJsonSaver()
            : this(new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Ignore
            })
        {
        }

        public TemplateTableJsonSaver(JsonSerializerSettings serializerSettings)
            : this(JsonSerializer.Create(serializerSettings))
        {
        }

        public TemplateTableJsonSaver(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public void SaveTo(TemplateTable<TKey, TValue> table, JsonWriter writer)
        {
            var items = table.ToList();
            var values = items.Select(i => i.Value);
            _serializer.Serialize(writer, values);
        }

        public void SaveTo(TemplateTable<TKey, TValue> table, TextWriter writer)
        {
            var items = table.ToList();
            var values = items.Select(i => i.Value);
            _serializer.Serialize(writer, values);
        }

        public string SaveToJson(TemplateTable<TKey, TValue> table)
        {
            var writer = new StringWriter();
            SaveTo(table, writer);
            return writer.ToString();
        }
    }
}
