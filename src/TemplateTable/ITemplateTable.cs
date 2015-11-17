using System;
using System.Collections;
using System.Collections.Generic;

namespace TemplateTable
{
    public interface ITemplateTable
    {
        Type KeyType { get; }
        Type ValueType { get; }
        int Count { get; }
    }

    public interface ITemplateTable<TKey> : ITemplateTable, IEnumerable
    {
        object GetValue(TKey id);
        bool ContainsKey(TKey id);
        IEnumerable<TKey> GetKeyEnumerable();
    }
}
