using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TemplateTable
{
    public class TemplateTableJsonPatchLoader<TKey, TValue> : ITemplateTableLoader<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly TemplateTable<TKey, TValue> _referenceTable;
        private readonly JArray _patchJson;
        private readonly JsonSerializer _serializer;

        public TemplateTableJsonPatchLoader(TemplateTable<TKey, TValue> referenceTable, string patchJson)
        {
            _referenceTable = referenceTable;
            _patchJson = JArray.Parse(patchJson);
            _serializer = new JsonSerializer();
        }

        public TemplateTableJsonPatchLoader(TemplateTable<TKey, TValue> referenceTable, JArray patchJson, JsonSerializer serializer)
        {
            _referenceTable = referenceTable;
            _patchJson = patchJson;
            _serializer = serializer;
        }

        public IEnumerable<KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>> Load()
        {
            foreach (var json in _patchJson)
            {
                if (json.Type == JTokenType.Comment)
                    continue;

                var idToken = ((JObject)json).Property("Id");
                if (idToken == null)
                    throw new JsonReaderException("Id not found (Line:" + ((IJsonLineInfo)json).LineNumber + ")");

                var key = idToken.Value.ToObject<TKey>();
                var value = _referenceTable.TryGet(key);
                using (var reader = new JTokenReader(json))
                {
                    if (value != null)
                        _serializer.Populate(reader, value);
                    else
                        value = _serializer.Deserialize<TValue>(reader);
                }

                yield return new KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>(
                    key,
                    new TemplateTableLoadData<TKey, TValue>(value));
            }
        }
    }
}
