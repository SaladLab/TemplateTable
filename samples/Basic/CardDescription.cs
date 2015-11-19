using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Basic
{
    public enum CardType
    {
        None,
        Enchantment,
        Hero,
        HeroPower,
        Minion,
        Spell,
        Weapon,
    }

    public enum Rarity
    {
        None,
        Free,
        Common,
        Rare,
        Legendary,
        Epic
    }

    public enum Faction
    {
        None,
        Neutral,
        Alliance,
        Horde
    }

    public enum Mechanic
    {
        None,
        AdjacentBuff,
        AffectedBySpellPower,
        Aura,
        Battlecry,
        Charge,
        Combo,
        Deathrattle,
        DivineShield,
        Enrage,
        Freeze,
        HealTarget,
        ImmuneToSpellpower,
        Inspire,
        Morph,
        OneTurnEffect,
        Overload,
        Poisonous,
        Secret,
        Silence,
        Spellpower,
        Stealth,
        Summoned,
        Taunt,
        Windfury,
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
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
}
