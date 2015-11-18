using System;
using System.IO;
using System.Linq;
using ProtoBuf.Meta;

namespace TemplateTable
{
    public class TemplateTableProtobufPackSaver<TKey, TValue>
        where TValue : class, new()
    {
        private readonly TypeModel _typeModel;

        public TemplateTableProtobufPackSaver()
        {
            _typeModel = RuntimeTypeModel.Default;
        }

        public TemplateTableProtobufPackSaver(TypeModel typeModel)
        {
            _typeModel = typeModel;
        }

        public void SaveTo(TemplateTable<TKey, TValue> table, Stream stream)
        {
            var items = table.ToList();
            var values = items.Select(i => i.Value).ToList();

            // signature 'TPP1' (4 bytes)

            stream.Write(new byte[] { 0x54, 0x50, 0x50, 0x31 }, 0, 4);

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

                var curPos = valuesStream.Position;
                _typeModel.Serialize(valuesStream, i.Value);
                var valueLength = (int)(valuesStream.Position - curPos);

                // serialize key and the length of value stream

                StreamGenericHelper.Write(keysStreamWriter, i.Key);
                keysStreamWriter.Write(valueLength);
            }

            var keysBuf = keysStream.ToArray();
            stream.Write(BitConverter.GetBytes(keysBuf.Length), 0, 4);
            stream.Write(keysBuf, 0, keysBuf.Length);

            var valuesBuf = valuesStream.ToArray();
            stream.Write(valuesBuf, 0, valuesBuf.Length);
        }
    }
}
