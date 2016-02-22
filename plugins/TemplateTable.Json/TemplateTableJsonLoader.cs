using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TemplateTable
{
    public class TemplateTableJsonLoader<TKey, TValue> : ITemplateTableLoader<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly JsonReader _jsonReader;
        private readonly JsonSerializer _serializer;
        private readonly bool _delayedLoad;

        public TemplateTableJsonLoader(string json, bool delayedLoad)
        {
            _jsonReader = new JsonTextReader(new StringReader(json));
            _serializer = new JsonSerializer();
            _delayedLoad = delayedLoad;
        }

        public TemplateTableJsonLoader(JsonReader textReader, JsonSerializer serializer, bool delayedLoad)
        {
            _jsonReader = textReader;
            _serializer = serializer;
            _delayedLoad = delayedLoad;
        }

        public IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> Load()
        {
            return _delayedLoad ? LoadDelayed() : LoadNow();
        }

        private IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> LoadNow()
        {
            var idGetter = GetIdGetter();
            if (idGetter == null)
                throw new InvalidOperationException("Cannot get getter function.");

            var values = _serializer.Deserialize<TValue[]>(_jsonReader);
            foreach (var value in values)
            {
                yield return new KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>(
                    idGetter(value),
                    Tuple.Create(value, (Func<TKey, TValue>)null));
            }
        }

        private IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> LoadDelayed()
        {
            var root = JToken.ReadFrom(_jsonReader);
            foreach (var json in root)
            {
                if (json.Type == JTokenType.Comment)
                    continue;

                var idToken = ((JObject)json).Property("Id");
                if (idToken == null)
                    throw new JsonReaderException("Id not found (Line:" + ((IJsonLineInfo)json).LineNumber + ")");

                var key = idToken.Value.ToObject<TKey>();
                yield return new KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>(
                    key,
                    Tuple.Create<TValue, Func<TKey, TValue>>(
                        null,
                        _ => _serializer.Deserialize<TValue>(new JTokenReader(json))));
            }
        }

        private static Func<TValue, TKey> GetIdGetter()
        {
            var idField = typeof(TValue).GetField("Id");
            if (idField != null)
            {
                return value => (TKey)idField.GetValue(value);
            }

            var idProperty = typeof(TValue).GetProperty("Id");
            if (idProperty != null)
            {
                return value => (TKey)idProperty.GetValue(value, null);
            }

            return null;
        }
    }
}
