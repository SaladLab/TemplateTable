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
        public void Test_JsonPatchLoad()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, false);
            table.Load(jsonLoader);
            var jsonPatcher = new TemplateTableJsonPatchLoader<int, TestObject>(table, TestObjectJson.PatchJson);
            table.Update(jsonPatcher);
            Assert.Equal("One", table[1].Name);
            Assert.Equal(20, table[1].Power);
            Assert.Equal("Three", table[3].Name);
        }

        [Fact]
        public void Test_JsonUpdateNow()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, false);
            table.Load(jsonLoader);
            var jsonUpdater = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.UpdateJson, false);
            table.Update(jsonUpdater);
            var value = table[2];
            Assert.Equal("TwoTwo", value.Name);
        }

        [Fact]
        public void Test_JsonUpdateLazy()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.LoadJson, true);
            table.Load(jsonLoader);
            var jsonUpdater = new TemplateTableJsonLoader<int, TestObject>(TestObjectJson.UpdateJson, true);
            table.Update(jsonUpdater);
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
