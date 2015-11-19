using System;
using System.Collections.Generic;
using ProtoBuf;

namespace TemplateTable.Tests
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class TestObject
    {
        public int Id;
        public string Name;
        public int Power;
        public int Speed;
        public string Description;
    }

    public class TestObjectLoader : ITemplateTableLoader<int, TestObject>
    {
        public IEnumerable<KeyValuePair<int, Tuple<TestObject, Func<int, TestObject>>>> Load()
        {
            yield return new KeyValuePair<int, Tuple<TestObject, Func<int, TestObject>>>(
                1,
                Tuple.Create(
                    new TestObject { Id = 1, Name = "One", Power = 10, Speed = 1, Description = "FirstOne" },
                    (Func<int, TestObject>)null));

            yield return new KeyValuePair<int, Tuple<TestObject, Func<int, TestObject>>>(
                2,
                Tuple.Create(
                    new TestObject { Id = 2, Name = "Two", Power = 20, Speed = 2 },
                    (Func<int, TestObject>)null));
        }
    }
}
