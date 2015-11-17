using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;

namespace TemplateTable
{
    public class TemplateTableBsonPackLoader<TKey, TValue> : ITemplateTableLoader<TKey, TValue>
        where TValue : class, new()
    {
        private Stream _stream;
        private JsonSerializer _serializer;
        private bool _delayedLoad;

        public TemplateTableBsonPackLoader(Stream stream, JsonSerializer serializer, bool delayLoad)
        {
            _stream = stream;
            _serializer = serializer;
            _delayedLoad = delayLoad;
        }

        public IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> Load()
        {
            return _delayedLoad ? LoadDelayed() : LoadNow();
        }

        private IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> LoadNow()
        {
            TKey[] keys;
            int[] valueLengths;
            byte[] valueBuf;
            if (LoadPackHeader(_stream, out keys, out valueLengths, out valueBuf) == false)
                throw new FormatException("LoadPackHeader failed.");

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

                yield return new KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>(
                    keys[i],
                    new Tuple<TValue, Func<TKey, TValue>>(value, null));
            }
        }

        private IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> LoadDelayed()
        {
            TKey[] keys;
            int[] valueLengths;
            byte[] valueBuf;
            if (LoadPackHeader(_stream, out keys, out valueLengths, out valueBuf) == false)
                throw new FormatException("LoadPackHeader failed.");

            var valueBufOffset = 0;
            for (var i = 0; i < keys.Length; i++)
            {
                var ms = new MemoryStream(valueBuf, valueBufOffset, valueLengths[i]);
                valueBufOffset += valueLengths[i];

                yield return new KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>(
                    keys[i],
                    new Tuple<TValue, Func<TKey, TValue>>(
                        null,
                        _ =>
                        {
                            using (var reader = new BsonReader(ms))
                            {
                                return _serializer.Deserialize<TValue>(reader);
                            };
                        }));
            }
        }

        private bool LoadPackHeader(Stream stream, out TKey[] keys, out int[] valueLengths, out byte[] valueBuf)
        {
            var dtblBuf = new byte[4];
            stream.Read(dtblBuf, 0, 4);

            var countBuf = new byte[4];
            stream.Read(countBuf, 0, 4);
            var count = BitConverter.ToInt32(countBuf, 0);

            var keysSizeBuf = new byte[4];
            stream.Read(keysSizeBuf, 0, 4);
            var keysSize = BitConverter.ToInt32(keysSizeBuf, 0);

            // Read keys & values

            var keysBuf = new byte[keysSize];
            stream.Read(keysBuf, 0, keysBuf.Length);

            // Reconstruct keys & values

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
            return true;
        }
    }

    public static class StreamGenericHelper
    {
        public static T Read<T>(BinaryReader reader)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)reader.ReadInt32();
            else if (typeof(T) == typeof(string))
                return (T)(object)reader.ReadString();
            else
                throw new NotSupportedException();
        }

        public static void Write<T>(BinaryWriter writer, T value)
        {
            if (value is int)
                writer.Write((int)(object)value);
            else if (value is string)
                writer.Write((string)(object)value);
            else
                throw new NotSupportedException();
        }
    }
}
