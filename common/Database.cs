using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using common.data;
using Ionic.Zlib;
using MySql.Data.MySqlClient;

namespace common
{
    public partial class Database : IDisposable
    {
        private const bool Testing = false;

        private static readonly string[] Names =
        {
            "Darq", "Deyst", "Drac", "Drol",
            "Eango", "Eashy", "Eati", "Eendi", "Ehoni",
            "Gharr", "Iatho", "Iawa", "Idrae", "Iri", "Issz", "Itani",
            "Laen", "Lauk", "Lorz",
            "Oalei", "Odaru", "Oeti", "Orothi", "Oshyu",
            "Queq", "Radph", "Rayr", "Ril", "Rilr", "Risrr",
            "Saylt", "Scheev", "Sek", "Serl", "Seus",
            "Tal", "Tiar", "Uoro", "Urake", "Utanu",
            "Vorck", "Vorv", "Yangu", "Yimi", "Zhiar"
        };

        private MySqlConnection _con;

        public Database()
        {
            _con = Testing
                ? new MySqlConnection( /* Testing = true; */
                    "Server=104.131.131.72;Database=rotmg;uid=beachin;password=xf7pCgk4uJk0;Pooling=true;Connection Timeout=15;max pool size=500;")
                : new MySqlConnection( /* Testing = false; */
                    "Server=104.131.131.72;Database=rotmg;uid=beachin;password=xf7pCgk4uJk0;Pooling=true;Connection Timeout=15;max pool size=500;");
            _con.Open();
        }

        public void Dispose()
        {
            if (_con != null)
            {
                _con.Close();
                _con = null;
            }
        }

        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public MySqlCommand CreateQuery()
        {
            return _con.CreateCommand();
        }

        public static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (int) (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public List<NewsItem> GetNews(Account acc)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT icon, title, text, link, date FROM news ORDER BY date LIMIT 10;";
            var ret = new List<NewsItem>();
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    ret.Add(new NewsItem
                    {
                        Icon = rdr.GetString("icon"),
                        Title = rdr.GetString("title"),
                        TagLine = rdr.GetString("text"),
                        Link = rdr.GetString("link"),
                        Date = DateTimeToUnixTimestamp(rdr.GetDateTime("date")),
                    });
            }
            if (acc != null)
            {
                cmd.CommandText = @"SELECT charId, characters.charType, level, death.totalFame, time
FROM characters, death
WHERE dead = TRUE AND
characters.accId=@accId AND death.accId=@accId
AND characters.charId=death.chrId;";
                cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        ret.Add(new NewsItem
                        {
                            Icon = "fame",
                            Title = string.Format("Your {0} died at level {1}",
                                XmlDatas.TypeToId[(short) rdr.GetInt32("charType")],
                                rdr.GetInt32("level")),
                            TagLine = string.Format("You earned {0} glorious Fame",
                                rdr.GetInt32("totalFame")),
                            Link = "fame:" + rdr.GetInt32("charId"),
                            Date = DateTimeToUnixTimestamp(rdr.GetDateTime("time")),
                        });
                }
            }
            ret.Sort((a, b) => -Comparer<int>.Default.Compare(a.Date, b.Date));
            return ret.Take(10).ToList();
        }

        public Account CreateGuestAccount(string uuid)
        {
            return Register(uuid, "", true);
            //return Verify(uuid, password);
            /*return new Account
            {
                Name = Names[(uint) uuid.GetHashCode()%Names.Length],
                AccountId = 0,
                Admin = false,
                Banned = false,
                Tag = "Guest",
                Rank = 0,
                BeginnerPackageTimeLeft = 0,
                Converted = false,
                Credits = 0,
                zTokens = 0,
                Guild = new Guild
                {
                    Name = "",
                    Id = 0,
                    Rank = 0
                },
                NameChosen = false,
                NextCharSlotPrice = 1000,
                VerifiedEmail = false,
                Stats = new Stats
                {
                    BestCharFame = 0,
                    ClassStates = new List<ClassStats>(),
                    Fame = 1000,
                    TotalFame = 1000
                },
                Vault = new VaultData
                {
                    Chests = new List<VaultChest>()
                },
                Bonuses = new List<short>()
            };*/
        }

        public Account Verify(string uuid, string password)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT id, name, rank, namechosen, verified, guild, guildRank, guildFame, banned, locked, ignored, tag, bonuses FROM accounts WHERE uuid=@uuid AND password=SHA1(@password);";
            cmd.Parameters.AddWithValue("@uuid", uuid);
            cmd.Parameters.AddWithValue("@password", password);
            Account ret;
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return null;
                rdr.Read();
                ret = new Account
                {
                    Name = rdr.GetString(UppercaseFirst("name")),
                    AccountId = rdr.GetInt32("id"),
                    Tag = rdr.GetString(UppercaseFirst("tag")),
                    Admin = rdr.GetInt32("rank") >= 2,
                    Rank = rdr.GetInt32("rank"),
                    Banned = rdr.GetBoolean("banned"),
                    BeginnerPackageTimeLeft = 0,
                    Converted = false,
                    Guild = new Guild
                    {
                        Id = rdr.GetInt32("guild"),
                        Rank = rdr.GetInt32("guildRank"),
                        Fame = rdr.GetInt32("guildFame")
                    },
                    NameChosen = rdr.GetBoolean("namechosen"),
                    NextCharSlotPrice = 1000,
                    VerifiedEmail = true, //rdr.GetBoolean("verified")
                    Locked = Utils.StringListToIntList(rdr.GetString("locked").Split(',').ToList()),
                    Ignored = Utils.StringListToIntList(rdr.GetString("ignored").Split(',').ToList()),
                    Bonuses = Utils.FromCommaSepString16(rdr.GetString("bonuses")).ToList()
                };
            }
            ReadStats(ret);
            ret.Guild.Name = GetGuildName(ret.Guild.Id);
            return ret;
        }

        public Account Register(string uuid, string password, bool isGuest)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT COUNT(id) FROM accounts WHERE uuid=@uuid;";
            cmd.Parameters.AddWithValue("@uuid", uuid);
            if ((int) (long) cmd.ExecuteScalar() > 0) return null;

            cmd = CreateQuery();
            cmd.CommandText =
                "INSERT INTO accounts(uuid, password, name, rank, namechosen, verified, guild, guildRank, guildFame, vaultCount, maxCharSlot, regTime, guest, banned, locked, ignored, bonuses) VALUES(@uuid, SHA1(@password), @name, 0, @nameChosen, 0, 0, 0, 0, 1, 2, @regTime, @guest, 0, @empty, @empty, @empty);";
            cmd.Parameters.AddWithValue("@uuid", uuid);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@name", (isGuest) ? Names[(uint) uuid.GetHashCode()%Names.Length] : uuid);
                //names[(uint)uuid.GetHashCode() % names.Length]);
            cmd.Parameters.AddWithValue("@nameChosen", !isGuest);
            cmd.Parameters.AddWithValue("@guest", isGuest);
            cmd.Parameters.AddWithValue("@regTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@empty", "");
            int v = cmd.ExecuteNonQuery();
            bool ret = v > 0;

            if (ret)
            {
                cmd = CreateQuery();
                cmd.CommandText = "SELECT last_insert_id();";
                int accId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = CreateQuery();
                cmd.CommandText =
                    "INSERT INTO stats(accId, fame, totalFame, credits, totalCredits, deaths, totalDeaths) VALUES(@accId, 1000, 1000, 0, 0, 0, 0);";
                cmd.Parameters.AddWithValue("@accId", accId);
                cmd.ExecuteNonQuery();

                cmd = CreateQuery();
                cmd.CommandText = "INSERT INTO vaults(accId, items) VALUES(@accId, '-1, -1, -1, -1, -1, -1, -1, -1');";
                cmd.Parameters.AddWithValue("@accId", accId);
                cmd.ExecuteNonQuery();
            }
            return Verify(uuid, password);
        }

        public bool HasUuid(string uuid)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT COUNT(id) FROM accounts WHERE uuid=@uuid;";
            cmd.Parameters.AddWithValue("@uuid", uuid);
            return (int) (long) cmd.ExecuteScalar() > 0;
        }

        public Account GetAccount(int id)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT id, name, rank, namechosen, verified, guild, guildRank, guildFame, banned, locked, ignored,  tag, bonuses FROM accounts WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", id);
            Account ret;
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return null;
                rdr.Read();
                ret = new Account
                {
                    Name = rdr.GetString(UppercaseFirst("name")),
                    AccountId = rdr.GetInt32("id"),
                    Admin = rdr.GetInt32("rank") >= 2,
                    Rank = rdr.GetInt32("rank"),
                    Tag = rdr.GetString(UppercaseFirst("tag")),
                    Banned = rdr.GetBoolean("banned"),
                    BeginnerPackageTimeLeft = 0,
                    Converted = false,
                    Guild = new Guild
                    {
                        Id = rdr.GetInt32("guild"),
                        Rank = rdr.GetInt32("guildRank"),
                        Fame = rdr.GetInt32("guildFame")
                    },
                    NameChosen = rdr.GetBoolean("namechosen"),
                    NextCharSlotPrice = 1000,
                    VerifiedEmail = rdr.GetBoolean("verified"),
                    Locked = Utils.StringListToIntList(rdr.GetString("locked").Split(',').ToList()),
                    Ignored = Utils.StringListToIntList(rdr.GetString("ignored").Split(',').ToList()),
                    Bonuses = Utils.FromCommaSepString16(rdr.GetString("bonuses")).ToList()
                };
            }
            ReadStats(ret);
            ret.Guild.Name = GetGuildName(ret.Guild.Id);
            return ret;
        }

        public Account GetAccount(string name)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT id, name, rank, namechosen, verified, guild, guildRank, guildFame, banned, locked, ignored, tag, bonuses FROM accounts WHERE name=@name;";
            cmd.Parameters.AddWithValue("@name", name);
            Account ret;
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return null;
                rdr.Read();
                ret = new Account
                {
                    Name = rdr.GetString(UppercaseFirst("name")),
                    AccountId = rdr.GetInt32("id"),
                    Admin = rdr.GetInt32("rank") >= 2,
                    Rank = rdr.GetInt32("rank"),
                    Tag = rdr.GetString(UppercaseFirst("tag")),
                    BeginnerPackageTimeLeft = 0,
                    Converted = false,
                    Guild = new Guild
                    {
                        Id = rdr.GetInt32("guild"),
                        Rank = rdr.GetInt32("guildRank"),
                        Fame = rdr.GetInt32("guildFame")
                    },
                    NameChosen = rdr.GetBoolean("namechosen"),
                    NextCharSlotPrice = 1000,
                    VerifiedEmail = rdr.GetBoolean("verified"),
                    Locked = Utils.StringListToIntList(rdr.GetString("locked").Split(',').ToList()),
                    Ignored = Utils.StringListToIntList(rdr.GetString("ignored").Split(',').ToList()),
                    Bonuses = Utils.FromCommaSepString16(rdr.GetString("bonuses")).ToList()
                };
            }
            ReadStats(ret);
            ret.Guild.Name = GetGuildName(ret.Guild.Id);
            return ret;
        }

        public int UpdateCredit(Account acc, int amount)
        {
            MySqlCommand cmd = CreateQuery();
            if (amount > 0)
            {
                cmd.CommandText = "UPDATE stats SET totalCredits = totalCredits + @amount WHERE accId=@accId;";
                cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.ExecuteNonQuery();
                cmd = CreateQuery();
            }
            cmd.CommandText = @"UPDATE stats SET credits = credits + (@amount) WHERE accId=@accId;
SELECT credits FROM stats WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@amount", amount);
            return (int) cmd.ExecuteScalar();
        }

        public int UpdateZToken(Account acc, int amount)
        {
            MySqlCommand cmd = CreateQuery();
            if (amount > 0)
            {
                cmd.CommandText = "UPDATE stats SET totalDeaths = totalDeaths + @amount WHERE accId=@accId;";
                cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.ExecuteNonQuery();
                cmd = CreateQuery();
            }
            cmd.CommandText = @"UPDATE stats SET deaths = deaths + (@amount) WHERE accId=@accId;
SELECT deaths FROM stats WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@amount", amount);
            return (int) cmd.ExecuteScalar();
        }

        public int UpdateFame(Account acc, int amount)
        {
            MySqlCommand cmd = CreateQuery();
            if (amount > 0)
            {
                cmd.CommandText = "UPDATE stats SET totalFame = totalFame + @amount WHERE accId=@accId;";
                cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.ExecuteNonQuery();
                cmd = CreateQuery();
            }
            cmd.CommandText = @"UPDATE stats SET fame = fame + (@amount) WHERE accId=@accId;
SELECT fame FROM stats WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@amount", amount);
            return (int) cmd.ExecuteScalar();
        }

        public void ReadStats(Account acc)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT fame, totalFame, credits, deaths FROM stats WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    rdr.Read();
                    acc.Credits = rdr.GetInt32("credits");
                    acc.zTokens = rdr.GetInt32("deaths");
                    acc.Stats = new Stats
                    {
                        Fame = rdr.GetInt32("fame"),
                        TotalFame = rdr.GetInt32("totalFame")
                    };
                }
                else
                {
                    acc.Credits = 0;
                    acc.zTokens = 0;
                    acc.Stats = new Stats
                    {
                        Fame = 0,
                        TotalFame = 0,
                        BestCharFame = 0,
                        ClassStates = new List<ClassStats>()
                    };
                }
            }

            acc.Stats.ClassStates = ReadClassStates(acc);
            if (acc.Stats.ClassStates.Count > 0)
                acc.Stats.BestCharFame = acc.Stats.ClassStates.Max(_ => _.BestFame);
            acc.Vault = ReadVault(acc);
        }

        public List<ClassStats> ReadClassStates(Account acc)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT objType, bestLv, bestFame FROM classstats WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            var ret = new List<ClassStats>();
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    ret.Add(new ClassStats
                    {
                        ObjectType = rdr.GetInt32("objType"),
                        BestFame = rdr.GetInt32("bestFame"),
                        BestLevel = rdr.GetInt32("bestLv")
                    });
            }
            return ret;
        }

        public VaultData ReadVault(Account acc)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT chestId, items FROM vaults WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    var ret = new VaultData {Chests = new List<VaultChest>()};
                    while (rdr.Read())
                    {
                        ret.Chests.Add(new VaultChest
                        {
                            ChestId = rdr.GetInt32("chestId"),
                            _Items = rdr.GetString("items")
                        });
                    }
                    return ret;
                }
                return new VaultData
                {
                    Chests = new List<VaultChest>()
                };
            }
        }

        public void SaveChest(Account acc, VaultChest chest)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE vaults SET items=@items WHERE accId=@accId AND chestId=@chestId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@chestId", chest.ChestId);
            cmd.Parameters.AddWithValue("@items", chest._Items);
            cmd.ExecuteNonQuery();
        }

        public VaultChest CreateChest(Account acc)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = @"INSERT INTO vaults(accId, items) VALUES(@accId, '-1, -1, -1, -1, -1, -1, -1, -1');
SELECT MAX(chestId) FROM vaults WHERE accId = @accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            return new VaultChest
            {
                ChestId = (int) cmd.ExecuteScalar(),
                _Items = "-1, -1, -1, -1, -1, -1, -1, -1"
            };
        }

        public void GetCharData(Account acc, Chars chrs)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT IFNULL(MAX(id), 0) + 1 FROM characters WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            chrs.NextCharId = (int) (long) cmd.ExecuteScalar();

            cmd = CreateQuery();
            cmd.CommandText = "SELECT maxCharSlot FROM accounts WHERE id=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            chrs.MaxNumChars = (int) cmd.ExecuteScalar();
        }

        public int GetNextCharId(Account acc)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT IFNULL(MAX(id), 0) + 1 FROM characters WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            var ret = (int) (long) cmd.ExecuteScalar();
            return ret;
        }

        public void LoadCharacters(Account acc, Chars chrs)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT * FROM characters WHERE accId=@accId AND dead = FALSE;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int[] stats = Utils.FromCommaSepString32(rdr.GetString("stats"));
                    chrs.Characters.Add(new Char
                    {
                        ObjectType = (short) rdr.GetInt32("charType"),
                        CharacterId = rdr.GetInt32("charId"),
                        Level = rdr.GetInt32("level"),
                        Exp = rdr.GetInt32("exp"),
                        CurrentFame = rdr.GetInt32("fame"),
                        _Equipment = rdr.GetString("items"),
                        MaxHitPoints = stats[0],
                        HitPoints = rdr.GetInt32("hp"),
                        MaxMagicPoints = stats[1],
                        MagicPoints = rdr.GetInt32("mp"),
                        Attack = stats[2],
                        Defense = stats[3],
                        Speed = stats[4],
                        Dexterity = stats[5],
                        HpRegen = stats[6],
                        MpRegen = stats[7],
                        Tex1 = rdr.GetInt32("tex1"),
                        Tex2 = rdr.GetInt32("tex2"),
                        Dead = false,
                        PCStats = rdr.GetString("fameStats"),
                        Pet = rdr.GetInt32("pet"),
                    });
                }
                cmd.Dispose();
            }
            foreach (Char i in chrs.Characters)
            {
                i.Backpacks = GetBackpacks(i, acc);
                i.Backpack = 1;
                i._Equipment += ", " + Utils.GetCommaSepString(i.Backpacks[i.Backpack]);
            }
        }

        public static Char CreateCharacter(short type, int chrId)
        {
            XElement cls = XmlDatas.TypeToElement[type];
            if (cls == null) return null;
            var ret = new Char
            {
                ObjectType = type,
                CharacterId = chrId,
                Level = 20,
                Exp = 0,
                CurrentFame = 0,
                Backpack = 1,
                _Equipment = cls.Element("Equipment").Value,
                MaxHitPoints = int.Parse((string) cls.Element("MaxHitPoints").Attribute("max")),
                HitPoints = int.Parse((string) cls.Element("MaxHitPoints").Attribute("max")),
                MaxMagicPoints = int.Parse((string) cls.Element("MaxMagicPoints").Attribute("max")),
                MagicPoints = int.Parse((string) cls.Element("MaxMagicPoints").Attribute("max")),
                Attack = int.Parse((string) cls.Element("Attack").Attribute("max")),
                Defense = int.Parse((string) cls.Element("Defense").Attribute("max")),
                Speed = int.Parse((string) cls.Element("Speed").Attribute("max")),
                Dexterity = int.Parse((string) cls.Element("Dexterity").Attribute("max")),
                HpRegen = int.Parse((string) cls.Element("HpRegen").Attribute("max")),
                MpRegen = int.Parse((string) cls.Element("MpRegen").Attribute("max")),
                Tex1 = 0,
                Tex2 = 0,
                Dead = false,
                PCStats = "",
                FameStats = new FameStats(),
                Pet = -1
            };
            ret.Backpacks = new Dictionary<int, short[]> {{1, ret.PackFromEquips()}};
            return ret;
        }

        public Char LoadCharacter(Account acc, int charId)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT * FROM characters WHERE accId=@accId AND charId=@charId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@charId", charId);
            Char ret;
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return null;
                rdr.Read();
                int[] stats = Utils.FromCommaSepString32(rdr.GetString("stats"));
                ret = new Char
                {
                    ObjectType = (short) rdr.GetInt32("charType"),
                    CharacterId = rdr.GetInt32("charId"),
                    Level = rdr.GetInt32("level"),
                    Exp = rdr.GetInt32("exp"),
                    CurrentFame = rdr.GetInt32("fame"),
                    _Equipment = rdr.GetString("items"),
                    Backpack = 1,
                    MaxHitPoints = stats[0],
                    HitPoints = rdr.GetInt32("hp"),
                    MaxMagicPoints = stats[1],
                    MagicPoints = rdr.GetInt32("mp"),
                    Attack = stats[2],
                    Defense = stats[3],
                    Speed = stats[4],
                    HpRegen = stats[5],
                    MpRegen = stats[6],
                    Dexterity = stats[7],
                    Tex1 = rdr.GetInt32("tex1"),
                    Tex2 = rdr.GetInt32("tex2"),
                    Dead = rdr.GetBoolean("dead"),
                    Pet = rdr.GetInt32("pet"),
                    PCStats = rdr.GetString("fameStats"),
                    FameStats = new FameStats()
                };
                if (!string.IsNullOrEmpty(ret.PCStats) && ret.PCStats != "FameStats")
                    try
                    {
                        ret.FameStats.Read(
                            ZlibStream.UncompressBuffer(
                                Convert.FromBase64String(ret.PCStats.Replace('-', '+').Replace('_', '/'))));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
            }
            ret.Backpacks = GetBackpacks(ret, acc);
            ret._Equipment += ", " + Utils.GetCommaSepString(ret.Backpacks[1]);
            cmd.Dispose();
            return ret;
        }

        public void SaveCharacter(Account acc, Char chr)
        {
            Console.WriteLine("Saving " + acc.Name + "...");
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = @"UPDATE characters SET 
level=@level, 
exp=@exp, 
fame=@fame, 
items=@items, 
stats=@stats, 
hp=@hp, 
mp=@mp, 
tex1=@tex1, 
tex2=@tex2, 
pet=@pet, 
fameStats=@fameStats 
WHERE accId=@accId AND charId=@charId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@charId", chr.CharacterId);

            cmd.Parameters.AddWithValue("@level", chr.Level);
            cmd.Parameters.AddWithValue("@exp", chr.Exp);
            cmd.Parameters.AddWithValue("@fame", chr.CurrentFame);
            cmd.Parameters.AddWithValue("@items", Utils.GetCommaSepString(chr.EquipSlots()));
            cmd.Parameters.AddWithValue("@stats", Utils.GetCommaSepString(new[]
            {
                chr.MaxHitPoints,
                chr.MaxMagicPoints,
                chr.Attack,
                chr.Defense,
                chr.Speed,
                chr.HpRegen,
                chr.MpRegen,
                chr.Dexterity
            }));
            cmd.Parameters.AddWithValue("@hp", chr.HitPoints);
            cmd.Parameters.AddWithValue("@mp", chr.MagicPoints);
            cmd.Parameters.AddWithValue("@tex1", chr.Tex1);
            cmd.Parameters.AddWithValue("@tex2", chr.Tex2);
            cmd.Parameters.AddWithValue("@pet", chr.Pet);
            chr.PCStats =
                Convert.ToBase64String(ZlibStream.CompressBuffer(chr.FameStats.Write()))
                    .Replace('+', '-')
                    .Replace('/', '_');
            cmd.Parameters.AddWithValue("@fameStats", chr.PCStats);
            cmd.ExecuteNonQuery();

            cmd = CreateQuery();
            cmd.CommandText = @"INSERT INTO classstats(accId, objType, bestLv, bestFame) 
VALUES(@accId, @objType, @bestLv, @bestFame) 
ON DUPLICATE KEY UPDATE 
bestLv = GREATEST(bestLv, @bestLv), 
bestFame = GREATEST(bestFame, @bestFame);";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@objType", chr.ObjectType);
            cmd.Parameters.AddWithValue("@bestLv", chr.Level);
            cmd.Parameters.AddWithValue("@bestFame", chr.CurrentFame);
            cmd.ExecuteNonQuery();
            cmd.Dispose();


            SaveBackpacks(chr, acc);
        }

        public Dictionary<int, short[]> GetBackpacks(Char chr, Account acc)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT * FROM backpacks WHERE charId=@charId AND accId=@accId";
            cmd.Parameters.AddWithValue("@charId", chr.CharacterId);
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            var ret = new Dictionary<int, short[]>();
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows)
                    return new Dictionary<int, short[]> {{1, new short[] {-1, -1, -1, -1, -1, -1, -1, -1}}};
                while (rdr.Read())
                    ret.Add(rdr.GetInt32("num"), Utils.FromCommaSepString16(rdr.GetString("items")));
                cmd.Dispose();
                return ret;
            }
        }

        public void SaveBackpacks(Char chr, Account acc)
        {
            if (chr.Backpacks.Count > 0)
            {
                foreach (var i in chr.Backpacks)
                {
                    MySqlCommand cmd = CreateQuery();
                    cmd.CommandText = @"INSERT INTO backpacks(accId, charId, num, items)
VALUES(@accId, @charId, @num, @items)
ON DUPLICATE KEY UPDATE
items = @items;";
                    cmd.Parameters.AddWithValue("@charId", chr.CharacterId);
                    cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                    cmd.Parameters.AddWithValue("@num", i.Key);
                    cmd.Parameters.AddWithValue("@items", Utils.GetCommaSepString(i.Value));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }

        public void Death(Account acc, Char chr, string killer) //Save first
        {
            Console.WriteLine(acc.Name + " died to " + killer + ".");
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = @"UPDATE characters SET 
dead=TRUE, 
deathTime=NOW() 
WHERE accId=@accId AND charId=@charId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@charId", chr.CharacterId);
            cmd.ExecuteNonQuery();

            bool firstBorn;
            int finalFame = chr.FameStats.CalculateTotal(acc, chr, chr.CurrentFame, out firstBorn);

            cmd = CreateQuery();
            cmd.CommandText = @"UPDATE stats SET 
fame=fame+@amount, 
totalFame=totalFame+@amount 
WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@amount", finalFame);
            cmd.ExecuteNonQuery();

            if (acc.Guild.Id != 0)
            {
                cmd = CreateQuery();
                cmd.CommandText = @"UPDATE guilds SET
guildFame=guildFame+@amount,
totalGuildFame=totalGuildFame+@amount
WHERE name=@name;";
                cmd.Parameters.AddWithValue("@amount", finalFame);
                cmd.Parameters.AddWithValue("@name", acc.Guild.Name);
                cmd.ExecuteNonQuery();

                cmd = CreateQuery();
                cmd.CommandText = @"UPDATE accounts SET
guildFame=guildFame+@amount
WHERE id=@id;";
                cmd.Parameters.AddWithValue("@amount", finalFame);
                cmd.Parameters.AddWithValue("@id", acc.AccountId);
                cmd.ExecuteNonQuery();
            }

            cmd = CreateQuery();
            cmd.CommandText =
                @"INSERT INTO death(accId, chrId, name, charType, tex1, tex2, items, fame, fameStats, totalFame, firstBorn, killer) 
VALUES(@accId, @chrId, @name, @objType, @tex1, @tex2, @items, @fame, @fameStats, @totalFame, @firstBorn, @killer);";
            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
            cmd.Parameters.AddWithValue("@chrId", chr.CharacterId);
            cmd.Parameters.AddWithValue("@name", acc.Name);
            cmd.Parameters.AddWithValue("@objType", chr.ObjectType);
            cmd.Parameters.AddWithValue("@tex1", chr.Tex1);
            cmd.Parameters.AddWithValue("@tex2", chr.Tex2);
            cmd.Parameters.AddWithValue("@items", chr._Equipment);
            cmd.Parameters.AddWithValue("@fame", chr.CurrentFame);
            cmd.Parameters.AddWithValue("@fameStats", chr.PCStats);
            cmd.Parameters.AddWithValue("@totalFame", finalFame);
            cmd.Parameters.AddWithValue("@firstBorn", firstBorn);
            cmd.Parameters.AddWithValue("@killer", killer);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public string GetCurrentVer()
        {
            MySqlCommand cmd = CreateQuery();

            cmd.CommandText = "SELECT cliVersion FROM clinews ORDER BY date DESC LIMIT 1";

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) throw new Exception("No rows found!");
                rdr.Read();
                cmd.Dispose();
                return rdr.GetString("cliVersion");
            }
        }

        public void DbAccountList(int listId, List<int> accIds)
        {
            switch (listId)
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }

        public void AddToArenaLb(int wave, List<string> participants)
        {
            string players = string.Join(", ", participants.ToArray());
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "INSERT INTO arenalb(wave, players) VALUES(@wave, @players)";
            cmd.Parameters.AddWithValue("@wave", wave);
            cmd.Parameters.AddWithValue("@players", players);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            cmd.Dispose();
        }

        public string[] GetArenaLeaderboards()
        {
            var lbrankings = new List<string>();

            MySqlCommand cmd = CreateQuery();

            cmd.CommandText = "SELECT * FROM arenalb ORDER BY wave DESC LIMIT 10";

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) lbrankings.Add("None");
                else
                {
                    while (rdr.Read())
                    {
                        lbrankings.Add("Wave " + rdr.GetInt32("wave") + " - " + rdr.GetString("players"));
                        lbrankings.Add("");
                    }
                }
            }
            cmd.Dispose();
            return lbrankings.ToArray();
        }

        public string[] GetGuildLeaderboards()
        {
            var guildrankings = new List<string>();

            MySqlCommand cmd = CreateQuery();

            cmd.CommandText = "SELECT * FROM guilds ORDER BY guildFame DESC LIMIT 10";

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) guildrankings.Add("None");
                else
                {
                    while (rdr.Read())
                    {
                        guildrankings.Add(rdr.GetString("name") + " - " + rdr.GetInt32("guildFame") + " Fame");
                    }
                }
            }
            cmd.Dispose();
            return guildrankings.ToArray();
        }


        public List<int> GetLockeds(int accId)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT locked FROM accounts WHERE id=@accId";
            cmd.Parameters.AddWithValue("@accid", accId);
            try
            {
                return cmd.ExecuteScalar().ToString().Split(',').Select(int.Parse).ToList();
            }
            catch
            {
                return new List<int>();
            }
        }

        public List<int> GetIgnoreds(int accId)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT ignored FROM accounts WHERE id=@accId";
            cmd.Parameters.AddWithValue("@accid", accId);
            try
            {
                return cmd.ExecuteScalar().ToString().Split(',').Select(int.Parse).ToList();
            }
            catch
            {
                return new List<int>();
            }
        }

        public bool AddLock(int accId, int lockId)
        {
            List<int> x = GetLockeds(accId);
            x.Add(lockId);
            string s = Utils.GetCommaSepString(x.ToArray());
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET locked=@newlocked WHERE id=@accId";
            cmd.Parameters.AddWithValue("@newlocked", s);
            cmd.Parameters.AddWithValue("@accId", accId);
            if (cmd.ExecuteNonQuery() == 0)
                return false;
            cmd.Dispose();
            return true;
        }

        public bool RemoveLock(int accId, int lockId)
        {
            List<int> x = GetLockeds(accId);
            x.Remove(lockId);
            string s = Utils.GetCommaSepString(x.ToArray());
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET locked=@newlocked WHERE id=@accId";
            cmd.Parameters.AddWithValue("@newlocked", s);
            cmd.Parameters.AddWithValue("@accId", accId);
            if (cmd.ExecuteNonQuery() == 0)
                return false;
            return true;
        }


        public bool AddIgnore(int accId, int ignoreId)
        {
            List<int> x = GetIgnoreds(accId);
            x.Add(ignoreId);
            string s = Utils.GetCommaSepString(x.ToArray());
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET ignored=@newignored WHERE id=@accId";
            cmd.Parameters.AddWithValue("@newignored", s);
            cmd.Parameters.AddWithValue("@accId", accId);
            if (cmd.ExecuteNonQuery() == 0)
                return false;
            return true;
        }

        public bool RemoveIgnore(int accId, int ignoreId)
        {
            List<int> x = GetIgnoreds(accId);
            x.Remove(ignoreId);
            string s = Utils.GetCommaSepString(x.ToArray());
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET ignored=@newignored WHERE id=@accId";
            cmd.Parameters.AddWithValue("@newignored", s);
            cmd.Parameters.AddWithValue("@accId", accId);
            if (cmd.ExecuteNonQuery() == 0)
                return false;
            return true;
        }

        public bool changetag(int accId, string tag)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET tag=@tag WHERE id=@accId";
            cmd.Parameters.AddWithValue("@tag", tag);
            cmd.Parameters.AddWithValue("@accId", accId);
            if (cmd.ExecuteNonQuery() == 0)
                return false;
            return true;
        }

        public bool SetBonuses(int accId, List<short> bonuses)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET bonuses=@newbonuses WHERE id=@accId";
            cmd.Parameters.AddWithValue("@newbonuses", Utils.GetCommaSepString(bonuses.ToArray()));
            cmd.Parameters.AddWithValue("@accId", accId);
            if (cmd.ExecuteNonQuery() == 0)
                return false;
            return true;
        }

        public bool SetBonuses(string ip, List<short> gifts)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE ips SET gifts=@newgifts WHERE ip=@ip";
            cmd.Parameters.AddWithValue("@newgifts", Utils.GetCommaSepString(gifts.ToArray()));
            cmd.Parameters.AddWithValue("@ip", ip);
            if (cmd.ExecuteNonQuery() == 0)
                return false;
            return true;
        }

        public IP CheckIp(string ip)
        {
            IP ret = null;
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT * FROM ips WHERE ip=@ip;";
            cmd.Parameters.AddWithValue("@ip", ip);
            bool foundIp = false;
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    foundIp = true;
                    rdr.Read();
                    ret = new IP
                    {
                        Address = rdr.GetString("ip"),
                        Banned = rdr.GetBoolean("banned"),
                        Gifts = Utils.FromCommaSepString16(rdr.GetString("gifts")).ToList()
                    };
                }
            }
            if (!foundIp)
            {
                cmd = CreateQuery();
                cmd.CommandText = @"INSERT INTO ips(ip, banned, gifts) VALUES (@ip, @banned, @gifts)";
                cmd.Parameters.AddWithValue("@ip", ip);
                cmd.Parameters.AddWithValue("@banned", 0);
                cmd.Parameters.AddWithValue("@gifts", "");
                cmd.ExecuteNonQuery();
                ret = new IP
                {
                    Address = ip,
                    Banned = false,
                    Gifts = new List<short>()
                };
            }
            return ret;
        }
    }
}