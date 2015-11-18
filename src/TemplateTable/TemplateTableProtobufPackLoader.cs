using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Meta;

namespace TemplateTable
{
    public class TemplateTableProtobufPackLoader<TKey, TValue> : ITemplateTableLoader<TKey, TValue>
        where TValue : class, new()
    {
        private readonly Stream _stream;
        private readonly TypeModel _typeModel;
        private readonly bool _delayedLoad;

        public TemplateTableProtobufPackLoader(Stream stream, bool delayLoad)
        {
            _stream = stream;
            _typeModel = RuntimeTypeModel.Default;
            _delayedLoad = delayLoad;
        }

        public TemplateTableProtobufPackLoader(Stream stream, TypeModel typeModel, bool delayLoad)
        {
            _stream = stream;
            _typeModel = typeModel;
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
            LoadHeader(_stream, out keys, out valueLengths, out valueBuf);

            var valueBufOffset = 0;
            for (var i = 0; i < keys.Length; i++)
            {
                var ms = new MemoryStream(valueBuf, valueBufOffset, valueLengths[i]);
                valueBufOffset += valueLengths[i];

                var value = new TValue();
                _typeModel.Deserialize(ms, value, typeof(TValue));

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
            LoadHeader(_stream, out keys, out valueLengths, out valueBuf);

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
                            var value = new TValue();
                            _typeModel.Deserialize(ms, value, typeof(TValue));
                            return value;
                        }));
            }
        }

        private void LoadHeader(Stream stream, out TKey[] keys, out int[] valueLengths, out byte[] valueBuf)
        {
            var sigBuf = new byte[4];
            stream.Read(sigBuf, 0, 4);
            if (sigBuf[0] != 0x54 || sigBuf[1] != 0x50 || sigBuf[2] != 0x50 || sigBuf[3] != 0x31)
                throw new FormatException("Signature mismatch");

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
        }
    }
}