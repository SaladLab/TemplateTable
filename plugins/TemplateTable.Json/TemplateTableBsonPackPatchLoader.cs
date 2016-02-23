using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace TemplateTable
{
    public class TemplateTableBsonPackPatchLoader<TKey, TValue> : ITemplateTableLoader<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly TemplateTable<TKey, TValue> _referenceTable;
        private readonly Stream _stream;
        private readonly JsonSerializer _serializer;

        public TemplateTableBsonPackPatchLoader(TemplateTable<TKey, TValue> referenceTable, Stream stream, JsonSerializer serializer)
        {
            _referenceTable = referenceTable;
            _stream = stream;
            _serializer = new JsonSerializer();
        }

        public IEnumerable<KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>> Load()
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

                TValue value = _referenceTable.TryGet(keys[i]);
                using (var reader = new BsonReader(ms))
                {
                    if (value != null)
                        _serializer.Populate(reader, value);
                    else
                        value = _serializer.Deserialize<TValue>(reader);
                }

                yield return new KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>(
                    keys[i],
                    new TemplateTableLoadData<TKey, TValue>(value));
            }
        }
    }
}
