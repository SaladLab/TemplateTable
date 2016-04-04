using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace TemplateTable
{
    public static class TemplateTableBsonPackIO<TKey>
        where TKey : IComparable
    {
        static public void LoadHeader(Stream stream, out TKey[] keys, out int[] valueLengths, out byte[] valueBuf)
        {
            // signature 'TBP1' (4 bytes)

            var sigBuf = new byte[4];
            stream.Read(sigBuf, 0, 4);
            if (sigBuf[0] != 0x54 || sigBuf[1] != 0x42 || sigBuf[2] != 0x50 || sigBuf[3] != 0x31)
                throw new FormatException("Signature mismatch");

            // count (4 bytes)

            var countBuf = new byte[4];
            stream.Read(countBuf, 0, 4);
            var count = BitConverter.ToInt32(countBuf, 0);

            // read keys & values

            var keysSizeBuf = new byte[4];
            stream.Read(keysSizeBuf, 0, 4);
            var keysSize = BitConverter.ToInt32(keysSizeBuf, 0);

            var keysBuf = new byte[keysSize];
            stream.Read(keysBuf, 0, keysBuf.Length);

            keys = new TKey[count];
            valueLengths = new int[count];

            var valueBufSize = 0;
            var keysStream = new MemoryStream(keysBuf);
            var keysStreamReader = new BinaryReader(keysStream);
            for (var i = 0; i < count; i++)
            {
                var key = StreamGenericHelper.Read<TKey>(keysStreamReader);
                var valueLength = keysStreamReader.ReadInt32();

                keys[i] = key;
                valueLengths[i] = valueLength;

                valueBufSize += valueLength;
            }

            valueBuf = new byte[valueBufSize];
            stream.Read(valueBuf, 0, valueBufSize);
        }

        static public void Load(Stream stream, out List<KeyValuePair<TKey, JToken>> itemList)
        {
            // load header

            TKey[] keys;
            int[] valueLengths;
            byte[] valueBuf;
            LoadHeader(stream, out keys, out valueLengths, out valueBuf);

            // load value data and construct itemList

            itemList = new List<KeyValuePair<TKey, JToken>>();
            var valueBufOffset = 0;
            for (var i = 0; i < keys.Length; i++)
            {
                var ms = new MemoryStream(valueBuf, valueBufOffset, valueLengths[i]);
                valueBufOffset += valueLengths[i];

                JToken token;
                using (var reader = new BsonReader(ms))
                {
                    token = JToken.ReadFrom(reader);
                }
                itemList.Add(new KeyValuePair<TKey, JToken>(keys[i], token));
            }
        }

        static public void Save(IEnumerable<KeyValuePair<TKey, JToken>> itemList, Stream stream)
        {
            var items = itemList.OrderBy(i => i.Key).ToList();

            // signature 'TBP1' (4 bytes)

            stream.Write(new byte[] { 0x54, 0x42, 0x50, 0x31 }, 0, 4);

            // count (4 bytes)

            var countBuf = BitConverter.GetBytes(items.Count);
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
                    i.Value.WriteTo(writer);
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
