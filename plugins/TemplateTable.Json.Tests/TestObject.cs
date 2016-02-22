using System;
using System.Collections.Generic;

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
        public static readonly string LoadJson = @"
            [ 
                { 'Id': 1, 'Name': 'One', 'Power': 10, 'Speed': 1 },
                { 'Id': 2, 'Name': 'Two', 'Power': 20, 'Speed': 2 },
            ]
        ";

        public static readonly string UpdateJson = @"
            [ 
                { 'Id': 2, 'Name': 'TwoTwo', 'Power': 200, 'Speed': 20 },
                { 'Id': 3, 'Name': 'Three', 'Power': 30, 'Speed': 3 },
            ]
        ";

        public static readonly string PatchJson = @"
            [ 
                { 'Id': 1, 'Power': 20 },
                { 'Id': 3, 'Name': 'Three', 'Power': 30, 'Speed': 3 },
            ]
        ";
    }
}
