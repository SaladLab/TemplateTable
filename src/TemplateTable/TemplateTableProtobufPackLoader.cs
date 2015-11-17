using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;

namespace TemplateTable
{
    public class TemplateTableProtobufPackLoader<TKey, TValue> : ITemplateTableLoader<TKey, TValue>
        where TValue : class, new()
    {
        public IEnumerable<KeyValuePair<TKey, Tuple<TValue, Func<TKey, TValue>>>> Load()
        {
            return null;
        }
    }
}
