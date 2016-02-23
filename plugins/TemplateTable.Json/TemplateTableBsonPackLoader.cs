using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace TemplateTable
{
    public class TemplateTableBsonPackLoader<TKey, TValue> : ITemplateTableLoader<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly Stream _stream;
        private readonly JsonSerializer _serializer;
        private readonly bool _delayedLoad;

        public TemplateTableBsonPackLoader(Stream stream, bool delayLoad)
        {
            _stream = stream;
            _serializer = new JsonSerializer();
            _delayedLoad = delayLoad;
        }

        public TemplateTableBsonPackLoader(Stream stream, JsonSerializer serializer, bool delayLoad)
        {
            _stream = stream;
            _serializer = serializer;
            _delayedLoad = delayLoad;
        }

        public IEnumerable<KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>> Load()
        {
            return _delayedLoad ? LoadDelayed() : LoadNow();
        }

        private IEnumerable<KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>> LoadNow()
        {
            TKey[] keys;
            int[] valueLengths;
            byte[] valueBuf;
            TemplateTableBsonPackIO<TKey>.LoadHeader(_stream, out keys, out valueLengths, out valueBuf);

            var valueBufOffset = 0;
            for (var i = 0; i < keys.Length; i++)
            {
                var ms = new MemoryStream(valueBuf, valueBufOffset, valueLengths[i]);
                valueBufOffset += valueLengths[i];

                TValue value;
                using (var reader = new BsonReader(ms))
                {
                    value = _serializer.Deserialize<TValue>(reader);
                }

                yield return new KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>(
                    keys[i],
                    new TemplateTableLoadData<TKey, TValue>(value));
            }
        }

        private IEnumerable<KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>> LoadDelayed()
        {
            TKey[] keys;
            int[] valueLengths;
            byte[] valueBuf;
            TemplateTableBsonPackIO<TKey>.LoadHeader(_stream, out keys, out valueLengths, out valueBuf);

            var valueBufOffset = 0;
            for (var i = 0; i < keys.Length; i++)
            {
                var ms = new MemoryStream(valueBuf, valueBufOffset, valueLengths[i]);
                valueBufOffset += valueLengths[i];

                yield return new KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>(
                    keys[i],
                    new TemplateTableLoadData<TKey, TValue>(
                        _ =>
                        {
                            using (var reader = new BsonReader(ms))
                            {
                                return _serializer.Deserialize<TValue>(reader);
                            }
                        }));
            }
        }
    }
}
