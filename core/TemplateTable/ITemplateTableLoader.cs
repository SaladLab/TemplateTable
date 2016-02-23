using System;
using System.Collections.Generic;

namespace TemplateTable
{
    public class TemplateTableLoadData<TKey, TValue>
    {
        public TValue Value;
        public Func<TKey, TValue> LazyLoader;

        public TemplateTableLoadData()
        {
        }

        public TemplateTableLoadData(TValue value)
        {
            Value = value;
        }

        public TemplateTableLoadData(Func<TKey, TValue> lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
    }

    public interface ITemplateTableLoader<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        // Delayed loader you may return should be thread-safe.
        IEnumerable<KeyValuePair<TKey, TemplateTableLoadData<TKey, TValue>>> Load();
    }
}
