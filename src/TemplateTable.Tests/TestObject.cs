using Xunit;

namespace TemplateTable.Tests
{
    public class TestObject
    {
        public int Id;
        public string Name;
        public int Power;
        public int Speed;
        public string Description;
    }

    public static class TestObjectJson
    {
        public readonly static string LoadJson = @"
            [ 
                { 'Id': 1, 'Name': 'One', 'Power': 10, 'Speed': 1 },
                { 'Id': 2, 'Name': 'Two', 'Power': 20, 'Speed': 2 },
            ]
        ";

        public readonly static string PatchJson = @"
            [ 
                { 'Id': 2, 'Name': 'TwoTwo', 'Power': 200, 'Speed': 20 },
                { 'Id': 3, 'Name': 'Three', 'Power': 30, 'Speed': 3 },
            ]
        ";
    }
}
