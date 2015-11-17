using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace TemplateTable.Tests
{
    public class TemplateTableTest
    {
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
            Assert.Throws<KeyNotFoundException>(() => {
                var value = table[1];
            });
        }

        [Fact]
        public void Test_GetNonExistentKey_CreateGhostValue()
        {
            var table = new TemplateTable<int, TestObject>();
            table.GhostValueFactory = key => new TestObject { Id = key, Name = "Ghost" };
            var value = table[1];
            Assert.Equal("Ghost", value.Name);
        }

        private readonly static string TestLoadJson = @"
            [ 
                { 'Id': 1, 'Name': 'One', 'Power': 10, 'Speed': 1 },
                { 'Id': 2, 'Name': 'Two', 'Power': 20, 'Speed': 2 },
            ]
        ";

        [Fact]
        public void Test_JsonLoadNow()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestLoadJson, false);
            table.Load(jsonLoader);
            var value = table[1];
            Assert.Equal("One", value.Name);
        }

        [Fact]
        public void Test_JsonLoadLazy()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestLoadJson, true);
            table.Load(jsonLoader);
            var value = table[1];
            Assert.Equal("One", value.Name);
        }

        private readonly static string TestPatchJson = @"
            [ 
                { 'Id': 2, 'Name': 'TwoTwo', 'Power': 200, 'Speed': 20 },
                { 'Id': 3, 'Name': 'Three', 'Power': 30, 'Speed': 3 },
            ]
        ";

        [Fact]
        public void Test_JsonPatchNow()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestLoadJson, false);
            table.Load(jsonLoader);
            var jsonPatcher = new TemplateTableJsonLoader<int, TestObject>(TestPatchJson, false);
            table.Patch(jsonPatcher);
            var value = table[2];
            Assert.Equal("TwoTwo", value.Name);
        }

        [Fact]
        public void Test_JsonPatchLazy()
        {
            var table = new TemplateTable<int, TestObject>();
            var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(TestLoadJson, true);
            table.Load(jsonLoader);
            var jsonPatcher = new TemplateTableJsonLoader<int, TestObject>(TestPatchJson, true);
            table.Patch(jsonPatcher);
            var value = table[2];
            Assert.Equal("TwoTwo", value.Name);
        }
    }
}
