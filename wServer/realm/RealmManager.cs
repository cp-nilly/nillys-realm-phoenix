#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using db.data;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.worlds;

#endregion

namespace wServer.realm
{
    public struct RealmTime
    {
        public int thisTickCounts;
        public int thisTickTimes;
        public long tickCount;
        public long tickTimes;
    }

    public class TimeEventArgs : EventArgs
    {
        public TimeEventArgs(RealmTime time)
        {
            Time = time;
        }

        public RealmTime Time { get; private set; }
    }

    public enum PendingPriority
    {
        Emergent,
        Destruction,
        Networking,
        Normal,
        Creation,
    }

    public class RealmManager
    {
        public const int MAX_CLIENT = 200;
        public const int MAX_INREALM = 85;

        public static List<string> realmNames = new List<string>
        {
            "Medusa",
            "Beholder",
            "Flayer",
            "Ogre",
            "Cyclops",
            "Sprite",
            "Djinn",
            "Slime",
            "Blob",
            "Demon",
            "Spider",
            "Scorpion",
            "Ghost"
        };

        public static List<string> allRealmNames = new List<string>
        {
            "Medusa",
            "Beholder",
            "Flayer",
            "Ogre",
            "Cyclops",
            "Sprite",
            "Djinn",
            "Slime",
            "Blob",
            "Demon",
            "Spider",
            "Scorpion",
            "Ghost"
        };

        public static List<string> battleArenaName = new List<string>
        {
            "Battle Arena Portal"
        };

        public static int nextWorldId = 0;
        public static int nextTestId = 0;
        public static readonly ConcurrentDictionary<int, World> Worlds = new ConcurrentDictionary<int, World>();
        public static readonly ConcurrentDictionary<int, Vault> Vaults = new ConcurrentDictionary<int, Vault>();
        public static readonly Dictionary<string, GuildHall> GuildHalls = new Dictionary<string, GuildHall>();

        public static readonly ConcurrentDictionary<int, ClientProcessor> Clients =
            new ConcurrentDictionary<int, ClientProcessor>();

        public static ConcurrentDictionary<int, World> PlayerWorldMapping = new ConcurrentDictionary<int, World>();

        public static ConcurrentDictionary<string, World> ShopWorlds = new ConcurrentDictionary<string, World>();
        private static Thread network;
        private static Thread logic;

        static RealmManager()
        {
            Worlds[World.TUT_ID] = new Tutorial(true);
            Worlds[World.NEXUS_ID] = Worlds[0] = new Nexus();
            Worlds[World.NEXUS_LIMBO] = new NexusLimbo();
            Worlds[World.VAULT_ID] = new Vault(true);
            Worlds[World.TEST_ID] = new Test();
            Worlds[World.RAND_REALM] = new RandomRealm();
            
            

            Monitor = new RealmPortalMonitor(Worlds[World.NEXUS_ID] as Nexus);

            AddWorld(GameWorld.AutoName(1, true));

            MerchantLists.GetKeys();
            MerchantLists.AddPetShop();
            MerchantLists.AddCustomShops();
            foreach (var i in MerchantLists.shopLists)
            {
                ShopWorlds.TryAdd(i.Key, AddWorld(new ShopMap(i.Key)));
            }
        }

        public static RealmPortalMonitor Monitor { get; private set; }
        public static NetworkTicker Network { get; private set; }
        public static LogicTicker Logic { get; private set; }

        public XmlDatas GameData { get; private set; }
        public ChatManager Chat { get; private set; }

        public static bool TryConnect(ClientProcessor psr)
        {
            var acc = psr.Account;
            if (psr.IP.Banned)
                return false;
            if (acc.Banned)
                return false;
            if (Clients.Count >= MAX_CLIENT)
                return false;
            return Clients.TryAdd(psr.Account.AccountId, psr);
        }

        public static void Disconnect(ClientProcessor psr)
        {
            Clients.TryRemove(psr.Account.AccountId, out psr);
        }

        public static Vault PlayerVault(ClientProcessor processor)
        {
            Vault v;
            var id = processor.Account.AccountId;
            if (Vaults.ContainsKey(id))
            {
                v = Vaults[id];
            }
            else
            {
                v = Vaults[id] = (Vault) AddWorld(new Vault(false, processor));
            }
            return v;
        }

        public static World GuildHallWorld(string g)
        {
            if (!GuildHalls.ContainsKey(g))
            {
                var gh = (GuildHall) AddWorld(new GuildHall(g));
                GuildHalls.Add(g, gh);
                return GuildHalls[g];
            }
            if (GuildHalls[g].Players.Count == 0)
            {
                GuildHalls.Remove(g);
                var gh = (GuildHall) AddWorld(new GuildHall(g));
                GuildHalls.Add(g, gh);
            }
            return GuildHalls[g];
        }

        public static void CloseWorld(World world)
        {
            Monitor.WorldRemoved(world);
        }

        public static World AddWorld(World world)
        {
            world.Id = Interlocked.Increment(ref nextWorldId);
            Worlds[world.Id] = world;
            if (world is GameWorld)
                Monitor.WorldAdded(world);
            return world;
        }

        public static World GetWorld(int id)
        {
            World ret;
            if (!Worlds.TryGetValue(id, out ret)) return null;
            if (ret.Id == 0) return null;
            return ret;
        }

        public static List<Player> GuildMembersOf(string guild)
        {
            return (from i in Worlds where i.Key != 0 from e in i.Value.Players where String.Equals(e.Value.Guild, guild, StringComparison.CurrentCultureIgnoreCase) select e.Value).ToList();
        }

        public static Player FindPlayer(string name)
        {
            if (name.Split(' ').Length > 1)
                name = name.Split(' ')[1];
            return (from i in Worlds where i.Key != 0 from e in i.Value.Players where String.Equals(e.Value.Client.Account.Name, name, StringComparison.CurrentCultureIgnoreCase) select e.Value).FirstOrDefault();
        }

        public static Player FindPlayerRough(string name)
        {
            Player dummy;
            foreach (var i in Worlds)
                if (i.Key != 0)
                    if ((dummy = i.Value.GetUniqueNamedPlayerRough(name)) != null)
                        return dummy;
            return null;
        }

        //public CommandManager Commands { get; private set; }

        public static void CoreTickLoop()
        {
            Network = new NetworkTicker();
            Logic = new LogicTicker();
            network = new Thread(Network.TickLoop)
            {
                Name = "Network Process Thread",
                CurrentCulture = CultureInfo.InvariantCulture
            };
            logic = new Thread(Logic.TickLoop)
            {
                Name = "Logic Ticking Thread",
                CurrentCulture = CultureInfo.InvariantCulture
            };
            //Start logic loop first
            logic.Start();
            network.Start();
            Thread.CurrentThread.Join();
        }
    }
}