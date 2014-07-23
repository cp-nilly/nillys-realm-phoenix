#region

using System;
using System.Collections.Generic;
using System.Linq;
using db.data;

#endregion

namespace wServer.realm.entities
{
    internal class MerchantLists
    {
        public static Dictionary<int, Tuple<int, CurrencyType>> prices = new Dictionary<int, Tuple<int, CurrencyType>>
        {
            //////////////PRICES ONLY\\\\\\\\\\\\\
                                  



//Rings t4
            {0xab8, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Defense
            {0xab7, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Attack
            {0xab9, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Speed
            {0xaba, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Vitality
            {0xabb, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Wisdom
            {0xabc, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Dexterity
            {0xabd, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Health
            {0xabe, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Magic


//Armor t8
            {0xadc, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Robe
            {0xa12, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Heavy
            {0xa0e, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Leather


                                  
//weapons t10
            {0xa19, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Dagger
            {0xa82, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Sword
            {0xa1e, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Bow
            {0xa9f, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Staff
            {0xa07, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Wand
            {0xc4c, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Katana


                                  
//Abilities t4


            {2774, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, // Spell
            {0xa33, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Tome
            {0xa64, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Quiver
            {0xa59, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Cloak
            {0xa6a, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Helm
            {0xa0b, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Shield
            {0xa54, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Seal
            {0xaa7, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Poison
            {0xaae, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Skull
            {0xab5, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Trap
            {0xa45, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Orb
            {0xb1f, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Prism
            {0xb31, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Scepter
            {0xfbd, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Pearl



// Effusions
            {0xaef, new Tuple<int, CurrencyType>(500, CurrencyType.Fame)}, // Defense
            {0xaf0, new Tuple<int, CurrencyType>(400, CurrencyType.Fame)}, // Defense
            {0xaf1, new Tuple<int, CurrencyType>(400, CurrencyType.Fame)}, // Defense
            {0xaf2, new Tuple<int, CurrencyType>(400, CurrencyType.Fame)}, // Defense
            {0xaf3, new Tuple<int, CurrencyType>(400, CurrencyType.Fame)}, // Defense


//randum stuff
            {2878, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Amulet of Resurrection
            {0xaeb, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Greater Health Potion
            {0xaec, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Greater Magic Potion
            {0x161f, new Tuple<int, CurrencyType>(2000, CurrencyType.Gold)}, //Valentine


// dyes (For now)
// Clothing
            {0x1007, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Black
            {0x1009, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Blue
            {0x100b, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Brown
            {0x1010, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Coral
            {0x1012, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Cornsilk
            {0x1015, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Dark Blue
            {0x101f, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Dark Red
            {0x1002, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Aqua
            {0x1004, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Azure
            {0x1033, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Green
            {0x102f, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, // Ghost White
            {0x1079, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Sienna
            {0x1030, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Gold

//Accessory
            {0x1107, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Black
            {0x1109, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Blue
            {0x110b, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Brown
            {0x1110, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Coral
            {0x1112, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Cornsilk
            {0x1115, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Dark Blue
            {0x111f, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Dark Red
            {0x1102, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Aqua
            {0x1104, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Azure
            {0x1133, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Green
            {0x112f, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Ghost White
            {0x1179, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Sienna
            {0x1130, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Gold

            {0xc0a, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //cronus
        };

        /* public static int[] store1List = { 0x701, 0x705, 0x706, 0x70a, 0x70b, 0x710, 0x71f, 0x722, 0xc11, 0xc19, 0xc23, 0xc2e, 0xc3d }; //keys */
        public static int[] store1List = {}; // Keys (?) added to the map

        public static int[] store2List =
        {
            2774, 0xa33, 0xa64, 0xa59, 0xa6a, 0xa0b, 0xa54, 0xaa7, 0xaae, 0xab5, 0xa45,
            0xb1f, 0xb31, 0xfbd
        }; // ????

        public static int[] store3List = {0xa19, 0xa82, 0xa1e, 0xa9f, 0xa07, 0xc4c}; // weapons
        public static int[] store4List = {0xadc, 0xa12, 0xa0e}; //armor
        public static int[] store5List = {0xaef, 0xaf0, 0xaf1, 0xaf2, 0xaf3}; //Consumables
        public static int[] store6List = {0xab8, 0xab9, 0xaba, 0xabb, 0xabc, 0xabd, 0xabe, 0xab7}; // Rings
        public static int[] store7List = {0x161f}; // Empty

        public static int[] store8List =
        {
            0x1007, 0x1009, 0x100b, 0x1010, 0x1012, 0x1015, 0x101f, 0x1002, 0x1004, 0x1033,
            0x102f, 0x1079, 0x1030
        }; // Large Dyes

        public static int[] store9List =
        {
            0x1107, 0x1109, 0x110b, 0x1110, 0x1112, 0x1115, 0x111f, 0x1102, 0x1104, 0x1133,
            0x112f, 0x1179, 0x1130
        }; // Small Dyes

        public static int[] store10List = {0xc0a};


        public static Dictionary<string, int[]> shopLists = new Dictionary<string, int[]>();

        public static void AddPetShop()
        {
            var PetShop = new List<int>();
            for (var i = 0x1500; i < 0x1562; i++)
            {
                PetShop.Add(i);
            }
            shopLists.Add("pets", PetShop.ToArray());
            PetShop.Shuffle();
            shopLists.Add("pets2", PetShop.ToArray());
            PetShop.Shuffle();
            shopLists.Add("pets3", PetShop.ToArray());
        }

        public static void GetKeys()
        {
            var nkeys = new List<int>();
            foreach (var i in XmlDatas.Keys)
            {
                prices[i] = new Tuple<int, CurrencyType>(XmlDatas.KeyPrices[i], CurrencyType.Fame);
                nkeys.Add(i);
            }
            shopLists["keys"] = nkeys.ToArray();
        }


        public static void AddCustomShops()
        {
            foreach (var i in XmlDatas.ItemPrices)
            {
                prices.Add(i.Key, new Tuple<int, CurrencyType>(i.Value, CurrencyType.Fame));
            }
            foreach (var i in XmlDatas.ItemShops)
            {
                if (shopLists.ContainsKey(i.Value))
                {
                    var ls = shopLists[i.Value].ToList();
                    ls.Add(i.Key);
                    shopLists[i.Value] = ls.ToArray();
                }
                else
                {
                    shopLists.Add(i.Value, new[] {i.Key});
                }
            }
        }
    }
}