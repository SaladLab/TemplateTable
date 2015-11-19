using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TemplateTable.Tests
{
    public class TemplateTableTest
    {
        public TemplateTable<int, TestObject> PrepareTable()
        {
            var table = new TemplateTable<int, TestObject>();
            table.Load(new TestObjectLoader());
            return table;
        }

        [Fact]
        public void Test_GetNonExistentKey_ReturnNull()
        {
            var table = new TemplateTable<int, TestObject>();
            var value = table.TryGet(1);
            Assert.Null(value);
        }

        [Fact]
        public void Test_GetNonExistentKey_ThrowException()
        {
            var table = new TemplateTable<int, TestObject>();
            Assert.Throws<KeyNotFoundException>(() => { var value = table[1]; });
        }

        [Fact]
        public void Test_GetNonExistentKey_CreateGhostValue()
        {
            var table = new TemplateTable<int, TestObject>();
            table.GhostValueFactory = key => new TestObject { Id = key, Name = "Ghost" };
            var value = table[1];
            Assert.Equal("Ghost", value.Name);
        }

        [Fact]
        public void Test_Keys()
        {
            var table = PrepareTable();
            Assert.Equal(new[] { 1, 2 }, table.Keys.ToArray());
        }

        [Fact]
        public void Test_Values()
        {
            var table = PrepareTable();
            Assert.Equal(2, table.Values.Count());
        }
    }
}
