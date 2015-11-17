using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TemplateTable
{
    public enum KeyNotFoundMode
    {
        ThrowException,
        ReturnGhostValue,
    }

    public class TemplateTable<TKey, TValue> : ITemplateTable<TKey>, IEnumerable<TValue>
        where TValue : class, new()
    {
        private struct ValueData
        {
            public TValue Value;
            public bool IsLoaded;
        }

        private readonly ConcurrentDictionary<TKey, ValueData> _table = new ConcurrentDictionary<TKey, ValueData>();
        private readonly KeyNotFoundMode _keyNotFoundMode = KeyNotFoundMode.ThrowException;

        // TODO: inexistent key
        // TODO: delayed loading
        // TODO: patching
        // TODO: Thread-safe

        public Type KeyType => typeof(TKey);

        public Type ValueType => typeof(TValue);

        public int Count => _table.Count;

        public TValue Get(TKey id)
        {
            return null;
        }

        public object GetValue(TKey id)
        {
            return null;
        }

        public bool ContainsKey(TKey id)
        {
            return _table.ContainsKey(id);
        }

        public TValue this[TKey id]
        {
            get
            {
                var value = Get(id);
                if (value != null)
                    return value;

                if (_keyNotFoundMode == KeyNotFoundMode.ThrowException)
                    throw new KeyNotFoundException("Unknown key: " + id);

                return default(TValue); // TODO: ghost value
            }
            set { }
        }

        public IEnumerable<TKey> GetKeyEnumerable()
        {
            foreach (var pair in _table)
            {
                yield return pair.Key;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var pair in _table)
            {
                // TODO: handle delayed load
                yield return pair.Value.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var pair in _table)
            {
                // TODO: handle delayed load
                yield return pair.Value.Value;
            }
        }
    }
}
