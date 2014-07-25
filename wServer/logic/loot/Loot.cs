#region

using System;
using System.Collections.Generic;
using System.Linq;
using db.data;

#endregion

namespace wServer.logic.loot
{
    public interface ILoot
    {
        Item GetLoot(Random rand);
    }

    public enum ItemType
    {
        Weapon,
        Ability,
        Armor,
        Ring,
        Misc
    }

    internal class TierLoot : ILoot
    {
        public static readonly int[] WeaponsT = {1, 2, 3, 8, 17, 24, 29};

        public static readonly int[] AbilityT =
        {
            4, 5, 11, 12, 13, 15, 16, 18, 19, 20, 21, 22, 23, 25, 26, 27, 28, 30,
            32, 31
        };

        public static readonly int[] ArmorsT = {6, 7, 14};
        public static readonly int[] RingT = {9};
        public static readonly int[] MiscT = {10};

        public static readonly Dictionary<int, Item[]> WeaponItems;
        public static readonly Dictionary<int, Item[]> AbilityItems;
        public static readonly Dictionary<int, Item[]> ArmorItems;
        public static readonly Dictionary<int, Item[]> RingItems;
        public static readonly Dictionary<int, Item[]> MiscItems;

        static TierLoot()
        {
            WeaponItems = new Dictionary<int, Item[]>();
            for (int tier = 1; tier < 20; tier++)
            {
                var items = new List<Item>();
                foreach (int i in WeaponsT)
                    items.AddRange(XmlDatas.ItemDescs.Select(_ => _.Value).Where(_ => _.Tier == tier && _.SlotType == i));
                if (items.Count == 0)
                    break;
                WeaponItems[tier] = items.ToArray();
            }
            AbilityItems = new Dictionary<int, Item[]>();
            for (int tier = 1; tier < 20; tier++)
            {
                var items = new List<Item>();
                foreach (int i in AbilityT)
                    items.AddRange(XmlDatas.ItemDescs.Select(_ => _.Value).Where(_ => _.Tier == tier && _.SlotType == i));
                if (items.Count == 0)
                    break;
                AbilityItems[tier] = items.ToArray();
            }
            ArmorItems = new Dictionary<int, Item[]>();
            for (int tier = 1; tier < 20; tier++)
            {
                var items = new List<Item>();
                foreach (int i in ArmorsT)
                    items.AddRange(XmlDatas.ItemDescs.Select(_ => _.Value).Where(_ => _.Tier == tier && _.SlotType == i));
                if (items.Count == 0)
                    break;
                ArmorItems[tier] = items.ToArray();
            }
            RingItems = new Dictionary<int, Item[]>();
            for (int tier = 1; tier < 20; tier++)
            {
                var items = new List<Item>();
                foreach (int i in RingT)
                    items.AddRange(XmlDatas.ItemDescs.Select(_ => _.Value).Where(_ => _.Tier == tier && _.SlotType == i));
                if (items.Count == 0)
                    break;
                RingItems[tier] = items.ToArray();
            }
            MiscItems = new Dictionary<int, Item[]>();
            for (int tier = 1; tier < 20; tier++)
            {
                var items = new List<Item>();
                foreach (int i in MiscT)
                    items.AddRange(XmlDatas.ItemDescs.Select(_ => _.Value).Where(_ => _.Tier == tier && _.SlotType == i));
                if (items.Count == 0)
                    break;
                MiscItems[tier] = items.ToArray();
            }
        }

        public TierLoot(byte tier, ItemType type)
        {
            Tier = tier;
            Type = type;
        }

        public byte Tier { get; private set; }
        public ItemType Type { get; private set; }

        public Item GetLoot(Random rand)
        {
            Item[] candidates;
            switch (Type)
            {
                case ItemType.Weapon:
                    candidates = WeaponItems[Tier];
                    break;
                case ItemType.Ability:
                    candidates = AbilityItems[Tier];
                    break;
                case ItemType.Armor:
                    candidates = ArmorItems[Tier];
                    break;
                case ItemType.Ring:
                    candidates = RingItems[Tier];
                    break;
                case ItemType.Misc:
                default:
                    candidates = MiscItems[Tier];
                    break;
            }
            int idx = rand.Next(0, candidates.Length);
            return candidates[idx];
        }
    }

    public class ItemLoot : ILoot
    {
        public ItemLoot(string loot) : this(XmlDatas.IdToType[loot])
        {
        }

        protected ItemLoot(short objType)
        {
            Item = XmlDatas.ItemDescs[objType];
        }

        public Item Item { get; private set; }

        public Item GetLoot(Random rand)
        {
            return Item;
        }
    }

    internal class HpPotionLoot : ItemLoot
    {
        public static readonly HpPotionLoot Instance = new HpPotionLoot();

        private HpPotionLoot() : base(0x0a22)
        {
        }
    }

    internal class MpPotionLoot : ItemLoot
    {
        public static readonly MpPotionLoot Instance = new MpPotionLoot();

        private MpPotionLoot() : base(0x0a23)
        {
        }
    }

    internal class PotionLoot : ILoot
    {
        public static readonly PotionLoot Instance = new PotionLoot();

        private PotionLoot()
        {
        }

        public Item GetLoot(Random rand)
        {
            return rand.Next()%2 == 0 ? XmlDatas.ItemDescs[0x0a22] : XmlDatas.ItemDescs[0x0a23];
        }
    }

    internal class IncLoot : ItemLoot
    {
        public static readonly IncLoot Instance = new IncLoot();

        private IncLoot() : base(0x722)
        {
        }
    }

    internal enum StatPotion : short
    {
        Att = 0xa1f,
        Def = 0xa20,
        Spd = 0xa21,
        Vit = 0xa34,
        Wis = 0xa35,
        Dex = 0xa4c,
        Life = 0xae9,
        Mana = 0xaea,
    }

    internal class StatPotionLoot : ItemLoot
    {
        public StatPotionLoot(StatPotion pot) : base((short) pot)
        {
        }
    }

    internal class StatPotionsLoot : ILoot
    {
        private static readonly StatPotion[][] Tiers =
        {
            new[] {StatPotion.Att, StatPotion.Def, StatPotion.Spd},
            new[] {StatPotion.Vit, StatPotion.Wis, StatPotion.Dex},
            new[] {StatPotion.Life, StatPotion.Mana}
        };

        private readonly StatPotion[] pots;

        public StatPotionsLoot(params int[] tiers)
        {
            var p = new List<StatPotion>();
            foreach (int i in tiers)
                p.AddRange(Tiers[i - 1]);
            pots = p.Distinct().ToArray();
        }

        public Item GetLoot(Random rand)
        {
            return XmlDatas.ItemDescs[(short) pots[rand.Next(0, pots.Length)]];
        }
    }
}