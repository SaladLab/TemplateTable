https://www.nuget.org/packages/TemplateTable/

[![NuGet Status](http://img.shields.io/nuget/v/TemplateTable.svg?style=flat)](https://www.nuget.org/packages/TemplateTable/)
[![Build status](https://ci.appveyor.com/api/projects/status/xbn2h0704qd7orad?svg=true)](https://ci.appveyor.com/project/veblush/templatetable)
[![Coverage Status](https://coveralls.io/repos/github/SaladLab/TemplateTable/badge.svg?branch=master)](https://coveralls.io/github/SaladLab/TemplateTable?branch=master)
[![Coverity Status](https://scan.coverity.com/projects/8439/badge.svg?flat=1)](https://scan.coverity.com/projects/saladlab-templatetable)

# TemplateTable

TemplateTable provides simple dictionary with fast-loading and dynamic-updating.

#### Where can I get it?

```
PM> Install-Package TemplateTable
PM> Install-Package TemplateTable.Json
PM> Install-Package TemplateTable.Protobuf
```

## Example

Let's see an example. In this example, we will define card,
load card data from file and consume it.

To show more realistic case, we'll use card data of
famous game [Hearthstone](http://us.battle.net/hearthstone). But it doesn't mean
this library was used by the game.
Data was collected from https://hearthstonejson.com.

### Define template class

With POCO, we define class `CardDescription`.

```csharp
public class CardDescription
{
    public string Id;
    public string Name;
    public CardType Type;
    public Rarity Rarity;
    public Faction Fraction;
    public string Text;
    public Mechanic[] Mechanics;
    public string Flavor;
    public string Artist;
    public int Attack;
    public int Health;
    public bool Collectible;
    public bool Elite;
}
```

And card data is written as json array like this.
```json
[
  {
    "Id": "AT_001",
    "Name": "Flame Lance",
    "...": "...",
  },
  {
    "Id": "AT_002",
    "Name": "Effigy",
    "...": "...",
  },
]
```

### Construct a table from json

From json data, we can construct a template table holding all card data as following.

```csharp
var json = File.ReadAllText(@"CardTable.json");
_cardTable = new TemplateTable<string, CardDescription>();
_cardTable.Load(new TemplateTableJsonLoader<string, CardDescription>(json, false));
```

You can choose bson-pack and protobuf-pack for data format instead of json.

### Lookup

```csharp
var card = _cardTable["GVG_112"];
Console.WriteLine("Name: " + card.Name);
Console.WriteLine("Text: " + card.Text);

// Ouput:
// Name: Mogor the Ogre
// Text: All minions have a 50% chance to attack the wrong enemy.
```

### Ghost Value

TemplateTable implements indexer [key] like `Dictionary`. Like `Dictionary`,
it returns value matched for key but when nothing matched it throws `KeyNotFoundException`
exception.

```csharp
var card = _cardTable["GVG_200"]; // _cardTable doesn't contain "GVG_200"

// Output:
// System.Collections.Generic.KeyNotFoundException: KeyNotFound: GVG_200
```

But it's convenient to get ghost value when nothing matched instead of catching
a brutal exception. You can set GhostValueFactory which will be used for creating
ghost value like this.

```csharp
_cardTable.GhostValueFactory = id => new CardDescription { Id = id, Name = "Ghost" };
```

With this factory, we always get value when using indexer of TemplateTable.

```csharp
var card = _cardTable["GVG_200"];
Console.WriteLine("Name: " + card.Name);
Console.WriteLine("Text: " + card.Text);

// Output:
// Name: Ghost
// Text:
```

Ghost value created will be reused in case that look-up same key.
But this doesn't mean ghost value is inserted in the table on the fly.

### Update table

Basically TemplateTable is used as readonly table. It can be used in editing
table or in runtime-patching. For these cases, you can use `Update` method of
TemplateTable

```csharp
var jsonLoader = new TemplateTableJsonLoader<int, TestObject>(json, false);
_cardTable.Update(jsonLoader);
```

`Update` method create, replace and remove as loader provides.

## Thread Safety

TemplateTable uses `ConcurrentDictionary` for implementing table. You can expect
same thread-safety for TemplateTable like ConcurrentDictionary.

## Delayed Loading

To make initial loading fast, TemplateTable provides delayed loading.
It is usually used in bson-pack and protobuf-pack because packing can
separate key table and value data.

If you have tons of data, it will spend lots of time for loading huge data before
running app. It's not good for fast development with many iterations.
Delayed loading can be used to handle this problem. But keep in mind that if you
load all data at the beginning, delayed loading might give more burden to GC.

## Format

Current TemplateTable can load data from json, bson-pack and protobuf-pack.
Each format has pros / cons.

 - Json is very human readable but not that fast in loading and
   doesn't speed efficient delayed loading.
 - Bson-pack provides speed efficient delayed loading. But not human readable and
   not faster than simple json in regular loading.
 - Protobuf-pack provides most fastest loading speed. But not human reable and
   it need more strict and limited POCO.

### Json

With `TemplateTableJsonLoader`, TemplateTable can load data from json file.

```csharp
cardTable.Load(new TemplateTableJsonLoader<string, CardDescription>(
                   File.ReadAllText("Table.json"), false));
```

To save, we can use class `TemplateTableJsonSaver`.

```csharp
using (var file = File.Create("Table.json"))
{
    new TemplateTableJsonSaver<string, CardDescription>().SaveTo(cardTable, file);
}
```

### Bson-pack

With `TemplateTableBsonPackLoader`, TemplateTable can load data from json bson-pack.

```csharp
cardTable.Load(new TemplateTableBsonPackLoader<string, CardDescription>(
                   File.Open("Table.bin", FileMode.Open, FileAccess.Read), false));
```

To save, we can use class `TemplateTableBsonPackSaver`.

```csharp
using (var file = File.Create("Table.bin"))
{
    new TemplateTableBsonPackSaver<string, CardDescription>().SaveTo(cardTable, file);
}
```

### Protobuf-pack

For using protobuf-pack, related data classes should have a proper ProtoContractAttribute.

```csharp
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class CardDescription
{
    public string Id;
    // ...
}
```

With `TemplateTableProtobufPackLoader`, TemplateTable can load data from protobuf-pack.

```csharp
cardTable.Load(new TemplateTableProtobufPackLoader<string, CardDescription>(
                   File.Open("Table.bin", FileMode.Open, FileAccess.Read), false));
```

To save, we can use class `TemplateTableProtobufPackSaver`.

```csharp
using (var file = File.Create("Table.bin"))
{
    new TemplateTableProtobufPackSaver<string, CardDescription>().SaveTo(cardTable, file);
}
```
