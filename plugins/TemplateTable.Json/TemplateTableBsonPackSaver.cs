using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace TemplateTable
{
    public class TemplateTableBsonPackSaver<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly JsonSerializer _serializer;

        public TemplateTableBsonPackSaver()
            : this(new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            })
        {
        }

        public TemplateTableBsonPackSaver(JsonSerializerSettings serializerSettings)
            : this(JsonSerializer.Create(serializerSettings))
        {
        }

        public TemplateTableBsonPackSaver(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public void SaveTo(TemplateTable<TKey, TValue> table, Stream stream)
        {
            var itemList = table.Select(i =>
            {
                JToken token;
                using (var writer = new JTokenWriter())
                {
                    _serializer.Serialize(writer, i.Value);
                    token = writer.Token;
                }
                return new KeyValuePair<TKey, JToken>(i.Key, token);
            });
            TemplateTableBsonPackIO<TKey>.Save(itemList, stream);
        }
    }
}
