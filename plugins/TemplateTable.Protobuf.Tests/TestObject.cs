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
        public IEnumerable<KeyValuePair<int, TemplateTableLoadData<int, TestObject>>> Load()
        {
            yield return new KeyValuePair<int, TemplateTableLoadData<int, TestObject>>(
                1,
                new TemplateTableLoadData<int, TestObject>(
                    new TestObject { Id = 1, Name = "One", Power = 10, Speed = 1, Description = "FirstOne" }));

            yield return new KeyValuePair<int, TemplateTableLoadData<int, TestObject>>(
                2,
                new TemplateTableLoadData<int, TestObject>(
                    new TestObject { Id = 2, Name = "Two", Power = 20, Speed = 2 }));
        }
    }
}
