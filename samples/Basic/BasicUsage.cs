using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TemplateTable;

namespace Basic
{
    class BasicUsage
    {
        private TemplateTable<string, CardDescription> _cardTable;

        public void Run()
        {
            Load();
            Use();
            UseGhost();
            Save();
        }

        private void Load()
        {
            var json = File.ReadAllText(@"..\..\CardTable.json");
            _cardTable = new TemplateTable<string, CardDescription>();
            _cardTable.Load(new TemplateTableJsonLoader<string, CardDescription>(json, true));
        }

        private void Use()
        {
            var card = _cardTable["GVG_112"];
            Console.WriteLine("Name: " + card.Name);
            Console.WriteLine("Text: " + card.Text);
        }

        private void UseGhost()
        {
            _cardTable.GhostValueFactory = id => new CardDescription { Id = id, Name = "Ghost " };

            var card = _cardTable["GVG_200"];
            Console.WriteLine("Name: " + card.Name);
            Console.WriteLine("Text: " + card.Text);
        }

        private void Save()
        {
            var saver = new TemplateTableJsonSaver<string, CardDescription>(
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Converters = new List<JsonConverter> { new StringEnumConverter() },
                });
            var json2 = saver.SaveToJson(_cardTable);
            File.WriteAllText("CardTableSaved.json", json2);
        }
    }
}
