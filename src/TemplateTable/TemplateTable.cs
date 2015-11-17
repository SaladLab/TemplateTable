using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TemplateTable
{
    public class TemplateTable<TKey, TValue> : ITemplateTable<TKey>, IEnumerable<TValue>
        where TValue : class, new()
    {
        public class ValueData
        {
            public TValue Value;
            public Func<TKey, TValue> LazyLoader;
        }

        private ConcurrentDictionary<TKey, ValueData> _table = new ConcurrentDictionary<TKey, ValueData>();
        private ConcurrentDictionary<TKey, TValue> _ghostTable = new ConcurrentDictionary<TKey, TValue>();

        public Func<TKey, TValue> GhostValueFactory = null;

        // TODO: inexistent key
        // TODO: delayed loading
        // TODO: patching
        // TODO: Thread-safe

        public Type KeyType => typeof(TKey);

        public Type ValueType => typeof(TValue);

        public int Count => _table.Count;

        public TValue TryGet(TKey id)
        {
            ValueData data;
            if (_table.TryGetValue(id, out data) == false)
                return null;

            return data.Value ?? LoadLazyValue(id, data);
        }

        private TValue LoadLazyValue(TKey id, ValueData data)
        {
            var lazyLoader = data.LazyLoader;
            if (lazyLoader != null)
            {
                data.Value = data.LazyLoader(id);
                data.LazyLoader = null;
            }
            return data.Value;
        }

        public object TryGetValue(TKey id)
        {
            return TryGet(id);
        }

        public bool ContainsKey(TKey id)
        {
            return _table.ContainsKey(id);
        }

        public TValue this[TKey id]
        {
            get
            {
                var value = TryGet(id);
                if (value != null)
                    return value;

                if (GhostValueFactory == null)
                    throw new KeyNotFoundException("KeyNotFound: " + id);

                return _ghostTable.GetOrAdd(id, GhostValueFactory);
            }
        }

        public IEnumerable<TKey> GetKeyEnumerable()
        {
            foreach (var i in _table)
            {
                yield return i.Key;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var i in _table)
            {
                yield return i.Value.Value ?? LoadLazyValue(i.Key, i.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var i in _table)
            {
                yield return i.Value.Value ?? LoadLazyValue(i.Key, i.Value);
            }
        }

        public void Load(ITemplateTableLoader<TKey, TValue> loader)
        {
            var table = new ConcurrentDictionary<TKey, ValueData>();

            foreach (var i in loader.Load())
            {
                bool added;

                if (i.Value.Item1 != null)
                    added = table.TryAdd(i.Key, new ValueData { Value = i.Value.Item1 });
                else
                    added = table.TryAdd(i.Key, new ValueData { LazyLoader = i.Value.Item2 });

                if (added == false)
                    throw new InvalidOperationException("Duplicate Key: " + i.Key);
            }

            _table = table;
            _ghostTable = new ConcurrentDictionary<TKey, TValue>();
        }

        public void Patch(ITemplateTableLoader<TKey, TValue> loader)
        {
            foreach (var i in loader.Load())
            {
                if (i.Value.Item1 != null)
                    _table[i.Key] = new ValueData { Value = i.Value.Item1 };
                else
                    _table[i.Key] = new ValueData { LazyLoader = i.Value.Item2 };
            }
        }
    }
}
