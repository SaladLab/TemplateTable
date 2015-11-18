using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;
using System.Linq;

namespace TemplateTable
{
    public class TemplateTableBsonPackSaver<TKey, TValue>
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
            var items = table.ToList();
            var values = items.Select(i => i.Value).ToList();

            // signature 'TBP1' (4 bytes)

            stream.Write(new byte[] { 0x54, 0x42, 0x50, 0x31 }, 0, 4);

            // count (4 bytes)

            var countBuf = BitConverter.GetBytes(values.Count);
            stream.Write(countBuf, 0, countBuf.Length);

            // keys & values

            var keysStream = new MemoryStream();
            var keysStreamWriter = new BinaryWriter(keysStream);
            var valuesStream = new MemoryStream();

            foreach (var i in items)
            {
                // serialize value by BsonWriter

                var ms = new MemoryStream(1024);
                using (var writer = new BsonWriter(ms))
                {
                    _serializer.Serialize(writer, i.Value);
                }
                var valueBuf = ms.ToArray();

                // serialize key and the length of value stream

                StreamGenericHelper.Write(keysStreamWriter, i.Key);
                keysStreamWriter.Write(valueBuf.Length);

                // copy value buffer to valuesStream

                valuesStream.Write(valueBuf, 0, valueBuf.Length);
            }

            var keysBuf = keysStream.ToArray();
            stream.Write(BitConverter.GetBytes(keysBuf.Length), 0, 4);
            stream.Write(keysBuf, 0, keysBuf.Length);

            var valuesBuf = valuesStream.ToArray();
            stream.Write(valuesBuf, 0, valuesBuf.Length);
        }
    }
}
