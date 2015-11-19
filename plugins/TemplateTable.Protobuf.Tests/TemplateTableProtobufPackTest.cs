using System.IO;
using Xunit;

namespace TemplateTable.Tests
{
    public class TemplateTableProtobufPackTest
    {
        public TemplateTable<int, TestObject> PrepareTable()
        {
            var table = new TemplateTable<int, TestObject>();
            table.Load(new TestObjectLoader());
            return table;
        }

        [Fact]
        public void Test_SaveTo()
        {
            var table = PrepareTable();
            var saver = new TemplateTableProtobufPackSaver<int, TestObject>();
            var stream = new MemoryStream();
            saver.SaveTo(table, stream);
            Assert.NotNull(stream.ToArray());
        }

        [Fact]
        public void Test_Load()
        {
            var table = PrepareTable();

            var saver = new TemplateTableProtobufPackSaver<int, TestObject>();
            var stream = new MemoryStream();
            saver.SaveTo(table, stream);
            stream.Seek(0, SeekOrigin.Begin);

            var loader = new TemplateTableProtobufPackLoader<int, TestObject>(stream, false);
            var table2 = new TemplateTable<int, TestObject>();
            table2.Load(loader);

            Assert.Equal(table.Count, table2.Count);
            Assert.Equal(table[1].Name, table2[1].Name);
        }
    }
}
