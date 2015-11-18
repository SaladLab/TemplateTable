using Xunit;

namespace TemplateTable.Tests
{
    public class TemplateTableJsonTest
    {
        public TemplateTable<int, TestObject> PrepareTable()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, false);
            table.Load(jsonLoader);
            return table;
        }

        [Fact]
        public void Test_JsonLoadNow()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, false);
            table.Load(jsonLoader);
            var value = table[1];
            Assert.Equal("One", value.Name);
        }

        [Fact]
        public void Test_JsonLoadLazy()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, true);
            table.Load(jsonLoader);
            var value = table[1];
            Assert.Equal("One", value.Name);
        }

        [Fact]
        public void Test_JsonPatchNow()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, false);
            table.Load(jsonLoader);
            var jsonPatcher = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.PatchJson, false);
            table.Update(jsonPatcher);
            var value = table[2];
            Assert.Equal("TwoTwo", value.Name);
        }

        [Fact]
        public void Test_JsonPatchLazy()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, true);
            table.Load(jsonLoader);
            var jsonPatcher = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.PatchJson, true);
            table.Update(jsonPatcher);
            var value = table[2];
            Assert.Equal("TwoTwo", value.Name);
        }

        [Fact]
        public void Test_SaveToJson()
        {
            var table = PrepareTable();
            var saver = new TemplateTableJsonSaver<int, TestObject>();
            var json = saver.SaveToJson(table);
            Assert.NotNull(json);
        }
    }
}
