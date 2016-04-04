using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TemplateTable;

namespace Basic
{
    internal class Benchmark
    {
        private string _json;
        private byte[] _bsonPack;
        private byte[] _protobufPack;

        public void Run()
        {
            PrepareData();
            LoadData();
        }

        private void PrepareData()
        {
            var json = File.ReadAllText(@"..\..\CardTable.json");
            _json = json;

            var cardTable = new TemplateTable<string, CardDescription>();
            cardTable.Load(new TemplateTableJsonLoader<string, CardDescription>(json, true));

            // Save BsonPack

            using (var file = File.Create("CardTableBsonPack.bin"))
            {
                new TemplateTableBsonPackSaver<string, CardDescription>().SaveTo(cardTable, file);
            }
            _bsonPack = File.ReadAllBytes("CardTableBsonPack.bin");

            // Save ProtobufPack

            using (var file = File.Create("CardTableProtobufPack.bin"))
            {
                new TemplateTableProtobufPackSaver<string, CardDescription>().SaveTo(cardTable, file);
            }
            _protobufPack = File.ReadAllBytes("CardTableProtobufPack.bin");
        }

        private void LoadData()
        {
            var testCount = 100;
            var cardTable = new TemplateTable<string, CardDescription>();

            for (int i = 0; i < 2; i++)
            {
                var delayedLoad = i == 1;
                var tag = delayedLoad ? "(Delayed)" : "";
                RunTest($"Load{tag} Json", testCount,
                        () =>
                        {
                            cardTable.Load(new TemplateTableJsonLoader<string, CardDescription>(
                                               _json, delayedLoad));
                        });

                RunTest($"Load{tag} BsonPack", testCount,
                        () =>
                        {
                            cardTable.Load(new TemplateTableBsonPackLoader<string, CardDescription>(
                                               new MemoryStream(_bsonPack), delayedLoad));
                        });

                RunTest($"Load{tag} ProtobufPack", testCount,
                        () =>
                        {
                            cardTable.Load(new TemplateTableProtobufPackLoader<string, CardDescription>(
                                               new MemoryStream(_protobufPack), delayedLoad));
                        });
            }
        }

        private static void RunTest(string testName, int testCount, Action test)
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < testCount; i++)
            {
                test();
            }
            sw.Stop();

            var elapsed = sw.ElapsedMilliseconds;
            var unit = (double)elapsed / testCount;
            Console.WriteLine($"{testName, -30} {elapsed, 6} ms {unit, 2} ms");
        }
    }
}
