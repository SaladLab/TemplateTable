using System;
using System.Collections;
using System.Collections.Generic;

namespace TemplateTable
{
    public interface ITemplateTableLoader<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        // Delayed loader you may return should be thread-safe.
        IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> Load();
    }
}
