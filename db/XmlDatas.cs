using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace db.data
{
    public class XmlDatas
    {
        private const int XmlCount = 36;
        public static Dictionary<short, int> ItemPrices;
        public static Dictionary<int, string> ItemShops;

        public static List<short> Keys;
        public static List<string> ServerXmls;

        static XmlDatas()
        {
            ReadXmls();
        }

        public static Dictionary<short, string> TypeToIdGround { get; private set; }
        public static Dictionary<short, string> TypeToId { get; private set; }
        public static Dictionary<string, short> IdToType { get; private set; }
        public static Dictionary<short, string> IdToDungeon { get; private set; }
        public static Dictionary<short, int> KeyPrices { get; private set; }
        public static Dictionary<short, XElement> TypeToElement { get; private set; }
        public static Dictionary<short, TileDesc> TileDescs { get; private set; }
        public static Dictionary<short, Item> ItemDescs { get; private set; }
        public static Dictionary<short, ObjectDesc> ObjectDescs { get; private set; }
        public static Dictionary<short, PortalDesc> PortalDescs { get; private set; }
        public static Dictionary<string, DungeonDesc> DungeonDescs { get; private set; }

        public static Dictionary<string, byte[]> RemoteTextures { get; private set; }

        public static void ReadXmls()
        {
            TypeToIdGround = new Dictionary<short, string>();
            TypeToId = new Dictionary<short, string>();
            IdToType = new Dictionary<string, short>();
            IdToDungeon = new Dictionary<short, string>();
            KeyPrices = new Dictionary<short, int>();
            TypeToElement = new Dictionary<short, XElement>();
            TileDescs = new Dictionary<short, TileDesc>();
            ItemDescs = new Dictionary<short, Item>();
            ObjectDescs = new Dictionary<short, ObjectDesc>();
            PortalDescs = new Dictionary<short, PortalDesc>();
            DungeonDescs = new Dictionary<string, DungeonDesc>();

            RemoteTextures = new Dictionary<string, byte[]>();

            ItemPrices = new Dictionary<short, int>();
            ItemShops = new Dictionary<int, string>();

            ServerXmls = new List<string>();

            Keys = new List<short>();

            Stream stream;
            for (int i = 0; i < XmlCount; i++)
            {
                stream = typeof (XmlDatas).Assembly.GetManifestResourceStream("db.data.dat" + i + ".xml");
                ProcessXmlStream(stream);
            }

            stream = typeof (XmlDatas).Assembly.GetManifestResourceStream("db.data.item.xml");
            ProcessXmlStream(stream);

            stream = typeof (XmlDatas).Assembly.GetManifestResourceStream("db.data.addition2.xml");
            ProcessXmlStream(stream);

            stream = typeof (XmlDatas).Assembly.GetManifestResourceStream("db.data.addition.xml");
            ProcessXmlStream(stream);

            stream = typeof (XmlDatas).Assembly.GetManifestResourceStream("db.data.donatorpets.xml");
            ProcessXmlStream(stream);
        }

        private static void ProcessXmlStream(Stream stream)
        {
            ProcessXml(stream);
            stream.Position = 0;
            using (var rdr = new StreamReader(stream))
                ServerXmls.Add(rdr.ReadToEnd());
        }

        private static void ProcessXml(Stream stream)
        {
            XElement root = XElement.Load(stream);
            foreach (XElement elem in root.Elements("Ground"))
            {
                var type = (short) Utils.FromString(elem.Attribute("type").Value);
                string id = elem.Attribute("id").Value;

                TypeToId[type] = id;
                TypeToIdGround[type] = id;
                IdToType[id] = type;
                TypeToElement[type] = elem;

                TileDescs[type] = new TileDesc(elem);
            }
            foreach (XElement elem in root.Elements("Object"))
            {
                if (elem.Element("Class") == null) continue;
                string cls = elem.Element("Class").Value;
                var type = (short) Utils.FromString(elem.Attribute("type").Value);
                string id = elem.Attribute("id").Value;

                TypeToId[type] = id;
                IdToType[id] = type;
                TypeToElement[type] = elem;

                if (cls == "Equipment" || cls == "Dye" || cls == "Pet")
                {
                    ItemDescs[type] = new Item(elem);
                    if (elem.Element("Shop") != null)
                    {
                        XElement shop = elem.Element("Shop");
                        ItemShops[type] = shop.Element("Name").Value;
                        ItemPrices[type] = Utils.FromString(shop.Element("Price").Value);
                    }
                }
                if (cls == "Character" || cls == "GameObject" || cls == "Wall" ||
                    cls == "ConnectedWall" || cls == "CaveWall" || cls == "Portal")
                    ObjectDescs[type] = new ObjectDesc(elem);
                if (cls == "Portal")
                {
                    try
                    {
                        PortalDescs[type] = new PortalDesc(elem);
                    }
                    catch
                    {
                        Console.WriteLine(@"Error for portal: " + type + @" id: " + id);
                        /*3392,1792,1795,1796,1805,1806,1810,1825 -- no location, assume nexus?* 
*  Tomb Portal of Cowardice,  Dungeon Portal,  Portal of Cowardice,  Realm Portal,  Glowing Portal of Cowardice,  Glowing Realm Portal,  Nexus Portal,  Locked Wine Cellar Portal*/
                    }
                }

                if (elem.Element("RemoteTexture") != null)
                {
                    XElement rtElem = elem.Element("RemoteTexture");
                    if (rtElem.Element("Id") != null)
                    {
                        string rtId = rtElem.Element("Id").Value;
                        if (!RemoteTextures.ContainsKey(rtId))
                            if (rtId.StartsWith("draw:"))
                                RemoteTextures.Add(rtId,
                                    new WebClient().DownloadData("http://realmofthemadgod.appspot.com/picture/get?id=" +
                                                                 new String(rtId.Skip(5).ToArray())));
                            else if (rtId.StartsWith("tdraw:"))
                                RemoteTextures.Add(rtId,
                                    new WebClient().DownloadData("http://rotmgtesting.appspot.com/picture/get?id=" +
                                                                 new String(rtId.Skip(6).ToArray())));
                            else if (rtId.StartsWith("file:"))
                                RemoteTextures.Add(rtId,
                                    File.ReadAllBytes("texture/" + new String(rtId.Skip(5).ToArray()) + ".png"));
                    }
                }


                XElement key = elem.Element("Key");
                if (key != null)
                {
                    Keys.Add(type);
                    KeyPrices[type] = Utils.FromString(key.Value);
                }
            }
            foreach (XElement elem in root.Elements("Dungeon"))
            {
                string name = elem.Attribute("name").Value;
                var portalid = (short) Utils.FromString(elem.Attribute("type").Value);

                IdToDungeon[portalid] = name;
                DungeonDescs[name] = new DungeonDesc(elem);
            }
        }
    }
}