#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using db;
using db.data;
using wServer.cliPackets;
using wServer.logic;
using wServer.realm.worlds;
using wServer.svrPackets;
using Mono.Game;

#endregion

namespace wServer.realm.entities.player
{
    internal interface IPlayer
    {
        void Damage(int dmg, Character chr);
        bool IsVisibleToEnemy();
    }

    public partial class Player : Character, IContainer, IPlayer
    {
        private readonly ClientProcessor psr;
        private bool dying;
        private readonly StatsManager statsMgr;
        private FameCounter fames;
        private float hpRegenCounter;
        private RealmManager manager;
        public int maxChests = 47;
        private float mpRegenCounter;
        private bool resurrecting;
        private byte[,] tiles;
        public bool vanished = false;
        private int pingSerial;

        public Player(ClientProcessor psr)
            : base((short) psr.Character.ObjectType, psr.Random)
        {
            this.psr = psr;
            statsMgr = new StatsManager(this);
            nName = psr.Account.Name;
            AccountId = psr.Account.AccountId;
            if (psr.Account.Tag != "")
            {
                Name = "[" + psr.Account.Tag + "] " + psr.Account.Name;
            }
            else
            {
                Name = psr.Account.Name;
            }
            Level = psr.Character.Level;
            Experience = psr.Character.Exp;
            ExperienceGoal = GetExpGoal(psr.Character.Level);
            if (psr.Account.Rank > 2)
                Stars = 100;
            else if (psr.Account.Rank > 1)
                Stars = 95;
            else
                Stars = GetStars(); //Temporary (until pub server)
            Texture1 = psr.Character.Tex1;
            Texture2 = psr.Character.Tex2;
            Credits = psr.Account.Credits;
            zTokens = psr.Account.zTokens;
            NameChosen = psr.Account.NameChosen;
            CurrentFame = psr.Account.Stats.Fame;
            Fame = psr.Character.CurrentFame;
            ClassStats state = psr.Account.Stats.ClassStates.SingleOrDefault(_ => _.ObjectType == ObjectType);
            FameGoal = GetFameGoal(state != null ? state.BestFame : 0);
            Glowing = IsUserInLegends();
            Guild = psr.Account.Guild.Name;
            GuildRank = psr.Account.Guild.Rank;
            if (psr.Character.HitPoints <= 0)
            {
                HP = psr.Character.MaxHitPoints;
                psr.Character.HitPoints = psr.Character.MaxHitPoints;
            }
            else
                HP = psr.Character.HitPoints;
            MP = psr.Character.MagicPoints;
            ConditionEffects = 0;
            OxygenBar = 100;

            Decision = 0;
            price = new Prices();

            //Locked = psr.Account.Locked ?? new List<int>();
            //Ignored = psr.Account.Ignored ?? new List<int>();
            try
            {
                if (psr.Database != null)
                {
                    Locked = psr.Database.GetLockeds(AccountId);
                    Ignored = psr.Database.GetIgnoreds(AccountId);
                }
                else
                {
                    using (var dbx = new Database())
                    { // auto dispose so don't need to dispose here
                        Locked = dbx.GetLockeds(AccountId);
                        Ignored = dbx.GetIgnoreds(AccountId);
                    }
                }
            }
            catch
            {
            }

            Inventory =
                psr.Character.Equipment.Select(
                    _ => _ == -1 ? null : (XmlDatas.ItemDescs.ContainsKey(_) ? XmlDatas.ItemDescs[_] : null)).ToArray();
            SlotTypes = Utils.FromCommaSepString32(XmlDatas.TypeToElement[ObjectType].Element("SlotTypes").Value);
            Stats = new[]
            {
                psr.Character.MaxHitPoints,
                psr.Character.MaxMagicPoints,
                psr.Character.Attack,
                psr.Character.Defense,
                psr.Character.Speed,
                psr.Character.HpRegen,
                psr.Character.MpRegen,
                psr.Character.Dexterity
            };

            Pet = null;
        }

        public ClientProcessor Client
        {
            get { return psr; }
        }

        public RealmManager Manager
        {
            get { return manager; }
        }

        //Stats
        public int AccountId { get; private set; }

        public Entity Pet { get; set; }

        public int Experience { get; set; }
        public int ExperienceGoal { get; set; }
        public int Level { get; set; }

        public int CurrentFame { get; set; }
        public int Fame { get; set; }
        public int FameGoal { get; set; }
        public int Stars { get; set; }

        public string Guild { get; set; }
        public int GuildRank { get; set; }
        public bool Invited { get; set; }

        public int Credits { get; set; }
        public int zTokens { get; set; }
        public bool NameChosen { get; set; }
        public int Texture1 { get; set; }
        public int Texture2 { get; set; }

        public List<int> Locked { get; set; }
        public List<int> Ignored { get; set; }

        public bool Glowing { get; set; }
        public int MP { get; set; }

        public int Decision { get; set; }
        public Prices price { get; set; }

        public int OxygenBar { get; set; }

        //public GlobalPlayerData PlayerData { get; private set; }

        public int[] Stats { get; private set; }
        public int[] Boost { get; private set; }

        public FameCounter FameCounter
        {
            get { return fames; }
        }

        public int[] SlotTypes { get; private set; }
        public Item[] Inventory { get; private set; }

        public void Damage(int dmg, Character chr)
        {
            if (HasConditionEffect(ConditionEffects.Paused) ||
                HasConditionEffect(ConditionEffects.Stasis) ||
                HasConditionEffect(ConditionEffects.Invincible))
                return;

            dmg = (int) statsMgr.GetDefenseDamage(dmg, false);
            if (!HasConditionEffect(ConditionEffects.Invulnerable))
                HP -= dmg;
            UpdateCount++;
            Owner.BroadcastPacket(new DamagePacket
            {
                TargetId = Id,
                Effects = 0,
                Damage = (ushort) dmg,
                Killed = HP <= 0,
                BulletId = 0,
                ObjectId = chr.Id
            }, this);

            if (HP <= 0)
                Death(
                    chr.ObjectDesc.DisplayId ??
                    chr.ObjectDesc.ObjectId);
        }

        protected override void ImportStats(StatsType stats, object val)
        {
            base.ImportStats(stats, val);
            switch (stats)
            {
                case StatsType.AccountId:
                    AccountId = (int) val;
                    break;

                case StatsType.Experience:
                    Experience = (int) val;
                    break;
                case StatsType.ExperienceGoal:
                    ExperienceGoal = (int) val;
                    break;
                case StatsType.Level:
                    Level = (int) val;
                    break;

                case StatsType.Fame:
                    CurrentFame = (int) val;
                    break;
                case StatsType.CurrentFame:
                    Fame = (int) val;
                    break;
                case StatsType.FameGoal:
                    FameGoal = (int) val;
                    break;
                case StatsType.Stars:
                    Stars = (int) val;
                    break;

                case StatsType.Guild:
                    Guild = (string) val;
                    break;
                case StatsType.GuildRank:
                    GuildRank = (int) val;
                    break;

                case StatsType.Credits:
                    Credits = (int) val;
                    break;
                case StatsType.zTokens:
                    zTokens = (int) val;
                    break;
                case StatsType.NameChosen:
                    NameChosen = (int) val != 0 ? true : false;
                    break;
                case StatsType.Texture1:
                    Texture1 = (int) val;
                    break;
                case StatsType.Texture2:
                    Texture2 = (int) val;
                    break;

                case StatsType.Glowing:
                    Glowing = IsUserInLegends() ? psr.Account.Rank == 6 : false;; //(int) val != 0 ? true : false;
                    break;
                case StatsType.HP:
                    HP = (int) val;
                    break;
                case StatsType.MP:
                    MP = (int) val;
                    break;

                case StatsType.Inventory0:
                    Inventory[0] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory1:
                    Inventory[1] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory2:
                    Inventory[2] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory3:
                    Inventory[3] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory4:
                    Inventory[4] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory5:
                    Inventory[5] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory6:
                    Inventory[6] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory7:
                    Inventory[7] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory8:
                    Inventory[8] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory9:
                    Inventory[9] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory10:
                    Inventory[10] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;
                case StatsType.Inventory11:
                    Inventory[11] = (int) val == -1 ? null : XmlDatas.ItemDescs[(short) (int) val];
                    break;

                case StatsType.MaximumHP:
                    Stats[0] = (int) val;
                    break;
                case StatsType.MaximumMP:
                    Stats[1] = (int) val;
                    break;
                case StatsType.Attack:
                    Stats[2] = (int) val;
                    break;
                case StatsType.Defense:
                    Stats[3] = (int) val;
                    break;
                case StatsType.Speed:
                    Stats[4] = (int) val;
                    break;
                case StatsType.Vitality:
                    Stats[5] = (int) val;
                    break;
                case StatsType.Wisdom:
                    Stats[6] = (int) val;
                    break;
                case StatsType.Dexterity:
                    Stats[7] = (int) val;
                    break;
            }
        }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            base.ExportStats(stats);
            stats[StatsType.AccountId] = AccountId;

            stats[StatsType.Experience] = Experience - GetLevelExp(Level);
            stats[StatsType.ExperienceGoal] = ExperienceGoal;
            stats[StatsType.Level] = Level;

            stats[StatsType.CurrentFame] = CurrentFame;
            stats[StatsType.Fame] = Fame;
            stats[StatsType.FameGoal] = FameGoal;
            stats[StatsType.Stars] = Stars;

            stats[StatsType.Guild] = Guild;
            stats[StatsType.GuildRank] = GuildRank;

            stats[StatsType.Credits] = Credits;
            stats[StatsType.zTokens] = zTokens;
            stats[StatsType.NameChosen] = NameChosen ? 1 : 0;
            stats[StatsType.Texture1] = Texture1;
            stats[StatsType.Texture2] = Texture2;

            if (Glowing)
                stats[StatsType.Glowing] = 1;
            stats[StatsType.HP] = HP;
            stats[StatsType.MP] = MP;

            stats[StatsType.Inventory0] = (Inventory[0] != null ? Inventory[0].ObjectType : -1);
            stats[StatsType.Inventory1] = (Inventory[1] != null ? Inventory[1].ObjectType : -1);
            stats[StatsType.Inventory2] = (Inventory[2] != null ? Inventory[2].ObjectType : -1);
            stats[StatsType.Inventory3] = (Inventory[3] != null ? Inventory[3].ObjectType : -1);
            stats[StatsType.Inventory4] = (Inventory[4] != null ? Inventory[4].ObjectType : -1);
            stats[StatsType.Inventory5] = (Inventory[5] != null ? Inventory[5].ObjectType : -1);
            stats[StatsType.Inventory6] = (Inventory[6] != null ? Inventory[6].ObjectType : -1);
            stats[StatsType.Inventory7] = (Inventory[7] != null ? Inventory[7].ObjectType : -1);
            stats[StatsType.Inventory8] = (Inventory[8] != null ? Inventory[8].ObjectType : -1);
            stats[StatsType.Inventory9] = (Inventory[9] != null ? Inventory[9].ObjectType : -1);
            stats[StatsType.Inventory10] = (Inventory[10] != null ? Inventory[10].ObjectType : -1);
            stats[StatsType.Inventory11] = (Inventory[11] != null ? Inventory[11].ObjectType : -1);

            if (Boost == null) CalcBoost();

            stats[StatsType.MaximumHP] = Stats[0] + Boost[0];
            stats[StatsType.MaximumMP] = Stats[1] + Boost[1];
            stats[StatsType.Attack] = Stats[2] + Boost[2];
            stats[StatsType.Defense] = Stats[3] + Boost[3];
            stats[StatsType.Speed] = Stats[4] + Boost[4];
            stats[StatsType.Vitality] = Stats[5] + Boost[5];
            stats[StatsType.Wisdom] = Stats[6] + Boost[6];
            stats[StatsType.Dexterity] = Stats[7] + Boost[7];

            stats[StatsType.HPBoost] = Boost[0];
            stats[StatsType.MPBoost] = Boost[1];
            stats[StatsType.AttackBonus] = Boost[2];
            stats[StatsType.DefenseBonus] = Boost[3];
            stats[StatsType.SpeedBonus] = Boost[4];
            stats[StatsType.VitalityBonus] = Boost[5];
            stats[StatsType.WisdomBonus] = Boost[6];
            stats[StatsType.DexterityBonus] = Boost[7];

            stats[StatsType.Size] = Size;

            if (Owner != null && Owner.Name == "Ocean Trench")
                stats[StatsType.OxygenBar] = OxygenBar;
        }

        public void SaveToCharacter()
        {
            Char chr = psr.Character;
            chr.Exp = Experience;
            chr.Level = Level;
            chr.Tex1 = Texture1;
            chr.Tex2 = Texture2;
            chr.Pet = (Pet != null ? Pet.ObjectType : -1);
            chr.CurrentFame = Fame;
            chr.HitPoints = HP;
            chr.MagicPoints = MP;
            chr.Equipment = Inventory.Select(_ => _ == null ? (short) -1 : _.ObjectType).ToArray();
            chr.MaxHitPoints = Stats[0];
            chr.MaxMagicPoints = Stats[1];
            chr.Attack = Stats[2];
            chr.Defense = Stats[3];
            chr.Speed = Stats[4];
            chr.HpRegen = Stats[5];
            chr.MpRegen = Stats[6];
            chr.Dexterity = Stats[7];
        }

        private void CalcBoost()
        {
            if (Boost == null) Boost = new int[12];
            else
                for (int i = 0; i < Boost.Length; i++) Boost[i] = 0;
            for (int i = 0; i < 4; i++)
            {
                if (Inventory[i] == null) continue;
                foreach (var b in Inventory[i].StatsBoost)
                {
                    switch ((StatsType) b.Key)
                    {
                        case StatsType.MaximumHP:
                            Boost[0] += b.Value;
                            break;
                        case StatsType.MaximumMP:
                            Boost[1] += b.Value;
                            break;
                        case StatsType.Attack:
                            Boost[2] += b.Value;
                            break;
                        case StatsType.Defense:
                            Boost[3] += b.Value;
                            break;
                        case StatsType.Speed:
                            Boost[4] += b.Value;
                            break;
                        case StatsType.Vitality:
                            Boost[5] += b.Value;
                            break;
                        case StatsType.Wisdom:
                            Boost[6] += b.Value;
                            break;
                        case StatsType.Dexterity:
                            Boost[7] += b.Value;
                            break;
                    }
                }
            }
        }

        public override void Init(World owner)
        {
            var rand = new Random();
            int x, y;
            do
            {
                x = rand.Next(0, owner.Map.Width);
                y = rand.Next(0, owner.Map.Height);
            } while (owner.Map[x, y].Region != TileRegion.Spawn);
            Move(x + 0.5f, y + 0.5f);
            tiles = new byte[owner.Map.Width, owner.Map.Height];
            fames = new FameCounter(this);
            SetNewbiePeriod();
            if (!psr.CheckAccountInUse(AccountId))
                base.Init(owner);
            else
                psr.Disconnect();
            if (psr.Character.Pet >= 0)
                GivePet((short) psr.Character.Pet);
            try
            {
                SendAccountList(Locked, LOCKED_LIST_ID);
            }
            catch
            {
            }
            try
            {
                SendAccountList(Ignored, IGNORED_LIST_ID);
            }
            catch
            {
            }
            if (!psr.ConnectedBuild.StartsWith(psr.clientVer))
            {
                psr.SendPacket(new TextBoxPacket
                {
                    Title = "Outdated Client!",
                    Message =
                        "You are running an outdated client version.\nGet or play the latest one on the\nup-to-date <u><a href='http://forum.zerorealms.com'>web client</a></u>.",
                    Button1 = "Okay"
                });
                psr.Disconnect();
            }

            WorldTimer pingTimer = null;
            owner.Timers.Add(pingTimer = new WorldTimer(PING_PERIOD, (w, t) =>
            {
                if (Client.Stage == ProtocalStage.Ready)
                {
                    Client.SendPacket(new PingPacket { Serial = pingSerial++ });
                    pingTimer.Reset();
                    RealmManager.Logic.AddPendingAction(_ => w.Timers.Add(pingTimer), PendingPriority.Creation);
                }
            }));
        }

        public override void Tick(RealmTime time)
        {
            try
            {
                if (psr.Stage == ProtocalStage.Disconnected)
                {
                    if (Owner != null)
                        Owner.LeaveWorld(this);
                    else
                        Owner.LeaveWorld(this);
                    return;
                }
            }
            catch
            {
            }
            if (!KeepAlive(time)) return;

            if (Boost == null) CalcBoost();

            TakeTick(time);
            TradeTick(time);
            HandleRegen(time);
            HandleQuest(time);
            HandleGround(time);
            HandleEffects(time);
            fames.Tick(time);

            /* try
                * {
                *     psr.Database.SaveCharacter(psr.Account, psr.Character);
                *     UpdateCount++;
                * }
                * catch
                * {
                *     Console.WriteLine("Error at line 312 of Player.cs");
                * }
            */

            try
            {
                if (Owner != null)
                {
                    SendUpdate(time);
                    if (!Owner.IsPassable((int)X, (int)Y) && Client.Account.Rank < 2)
                    {
                        Console.WriteLine("Player {0} No-Cliped at position: {1}, {2}", Name, X, Y);
                        Client.Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                SendNewTick(time);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (HP < 0 && !dying)
            {
                Death("Unknown");
                return;
            }

            base.Tick(time);
        }

        private void HandleRegen(RealmTime time)
        {
            if (HP == Stats[0] + Boost[0] || !CanHpRegen())
                hpRegenCounter = 0;
            else
            {
                hpRegenCounter += statsMgr.GetHPRegen()*time.thisTickTimes/1000f;
                var regen = (int) hpRegenCounter;
                if (regen > 0)
                {
                    HP = Math.Min(Stats[0] + Boost[0], HP + regen);
                    hpRegenCounter -= regen;
                    UpdateCount++;
                }
            }

            if (MP == Stats[1] + Boost[1] || !CanMpRegen())
                mpRegenCounter = 0;
            else
            {
                mpRegenCounter += statsMgr.GetMPRegen()*time.thisTickTimes/1000f;
                var regen = (int) mpRegenCounter;
                if (regen > 0)
                {
                    MP = Math.Min(Stats[1] + Boost[1], MP + regen);
                    mpRegenCounter -= regen;
                    UpdateCount++;
                }
            }
        }

        public void Move(RealmTime time, MovePacket pkt)
        {
            if (pkt.Position.X == -1 || pkt.Position.Y == -1) return;

            double newX = X;
            double newY = Y;
            if (newX != pkt.Position.X)
            {
                newX = pkt.Position.X;
                UpdateCount++;
            }
            if (newY != pkt.Position.Y)
            {
                newY = pkt.Position.Y;
                UpdateCount++;
            }

            if (HasConditionEffect(ConditionEffects.Paused))
            {
                ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = -1
                });
            }
            else if (HasConditionEffect(ConditionEffects.Paused) == false)
            {
                ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = 0
                });
            }
            Move((float) newX, (float) newY);
        }

        public void UsePortal(RealmTime time, UsePortalPacket pkt)
        {
            Entity entity = Owner.GetEntity(pkt.ObjectId);
            Console.WriteLine("[usePortal:" + nName + "] " + pkt + " -> " + entity.ObjectType);
            if (entity == null || !entity.Usable) return;
            World world = null;
            Player player = this;
            Portal p = null;
            if (entity is Portal)
            {
                p = entity as Portal;
                world = p.WorldInstance;
            }
            if (world == null)
            {
                if (p != null)
                {
                    bool setWorldInstance = true;
                    string d = "";
                    if (XmlDatas.IdToDungeon.TryGetValue(entity.ObjectType, out d))
                    {
                        world = RealmManager.AddWorld(new XmlWorld(XmlDatas.DungeonDescs[d]));
                    }
                    else
                    {
                        switch (entity.ObjectType) //handling default case for not found. Add more as implemented
                        {
                            case 0x0703: //portal of cowardice
                                if (RealmManager.PlayerWorldMapping.ContainsKey(AccountId))
                                    //may not be valid, realm recycled?
                                    world = RealmManager.PlayerWorldMapping[AccountId];
                                    //also reconnecting to vault is a little unexpected
                                else if (world.Id == -5 || world.Id == -11)
                                    world = RealmManager.GetWorld(World.NEXUS_ID);
                                else
                                    world = RealmManager.GetWorld(World.NEXUS_ID);
                                break;
                            case 0x0d40:
                                if (RealmManager.PlayerWorldMapping.ContainsKey(AccountId))
                                    //may not be valid, realm recycled?
                                    world = RealmManager.PlayerWorldMapping[AccountId];
                                    //also reconnecting to vault is a little unexpected
                                else if (world.Id == -5 || world.Id == -11)
                                    world = RealmManager.GetWorld(World.NEXUS_ID);
                                else
                                    world = RealmManager.GetWorld(World.NEXUS_ID);
                                break;
                            case 0x0712:
                                world = RealmManager.GetWorld(World.NEXUS_ID);
                                break;
                            case 0x071d:
                                world = RealmManager.GetWorld(World.NEXUS_ID);
                                break;
                            case 0x071c:
                                world = RealmManager.Monitor.GetRandomRealm();
                                break;
                            case 0x0720:
                                world = RealmManager.PlayerVault(psr);
                                setWorldInstance = false;
                                break;
                            case 0x071e:
                                world = RealmManager.AddWorld(new Kitchen());
                                break;
                            case 0x0ffa: //these need to match IDs
                                //world = RealmManager.GetWorld(World.GauntletMap); break; //this creates a singleton dungeon
                                world = RealmManager.AddWorld(new Island());
                                break; //this allows each dungeon to be unique
                            case 0x0ffc:
                                world = RealmManager.AddWorld(new WineCellarMap());
                                break;
                            case 0x1900:
                                world = RealmManager.AddWorld(new ArenaMap());
                                break;
                            case 0x0730:
                                world = RealmManager.AddWorld(new OceanTrench());
                                break;
                            case 0x070c:
                                world = RealmManager.AddWorld(new SpriteWorld());
                                break;
                            case 0x071b:
                                world = RealmManager.AddWorld(new Abyss());
                                break;
                            case 0x071a:
                                world = RealmManager.AddWorld(new UndeadLair());
                                break;
                            case 0x1901:
                                world = RealmManager.AddWorld(new VoidWorld());
                                break;
                            case 0x072c:
                                world = RealmManager.AddWorld(new TombMap());
                                break;
                            case 0x1905:
                                psr.SendPacket(new TextBoxPacket
                                {
                                    Button1 = "Enter",
                                    Button2 = "Cancel",
                                    Message = "Join the arena at the price of 150 fame?",
                                    Title = "Arena Confirmation",
                                    Type = "EnterArena1"
                                });
                                break;
                            case 0x1908:
                                psr.SendPacket(new TextBoxPacket
                                {
                                    Button1 = "Enter",
                                    Button2 = "Cancel",
                                    Message = "Join the free arena?",
                                    Title = "Arena Confirmation",
                                    Type = "EnterArena2"
                                });
                                break;
                            case 0x0742:
                                world = RealmManager.AddWorld(new BeachZone());
                                break;
                            case 0x0718:
                                world = RealmManager.AddWorld(new SnakePit());
                                break;
                            case 0x0890:
                                world = RealmManager.AddWorld(new MadLabMap());
                                break;
                            case 0x1919:
                                world = RealmManager.AddWorld(new Secret());
                                break;
                            case 0x1923:
                                world = RealmManager.AddWorld(new ZombieMap());
                                break;
                            case 0x195d:
                                world = RealmManager.AddWorld(new MarketMap());
                                break;
                            case 0x070d:
                                world = RealmManager.Monitor.GetRandomRealm();
                                break;
                            case 0x070e:
                                world = RealmManager.Monitor.GetRandomRealm();
                                break;
                            case 0x0704:
                                world = RealmManager.Monitor.GetRandomRealm();
                                break;
                            case 0x753E:
                                world = RealmManager.AddWorld(new LairofDraconis());
                                break;
                            case 0x7002:
                                world = RealmManager.AddWorld(new PartyCellarMap());
                                break;
                            case 0x701c:
                                psr.SendPacket(new TextBoxPacket
                                {
                                    Button1 = "Enter",
                                    Button2 = "Cancel",
                                    Message = "Join sheep herding at the price of 500 fame?",
                                    Title = "Sheep Herding",
                                    Type = "SheepHerding"
                                });
                                break;
                            case 0x7024:
                                psr.SendPacket(new TextBoxPacket
                                {
                                    Button1 = "Yes",
                                    Button2 = "Cancel",
                                    Message = "Join zombies at the price of 100 fame?",
                                    Title = "Zombies",
                                    Type = "Zombies"
                                });
                                break;
                            case 0x4a2b:
                                psr.SendPacket(new TextBoxPacket
                                {
                                    Button1 = "Yes",
                                    Button2 = "Cancel",
                                    Message = "Sorry, this isn't quite done yet. Look forward to it, though.",
                                    Title = "Nexus Defense",
                                    Type = "Nexus Defense"
                                });
                                break;
                            case 0x7025:
                                world = RealmManager.AddWorld(new ForestMap());
                                break;
                            case 0x4a29:
                                world = RealmManager.AddWorld(new ChristmasCellarMap());
                                break;
                            case 0x4a2a:
                                world = RealmManager.AddWorld(new TurkeyMap());
                                break;
                            case 0x5050:
                                world = RealmManager.AddWorld(new Shatters());
                                break;
                            case 0x2fbb:
                                world = RealmManager.AddWorld(new EC());
                                break;
                            case 0x0d7b:
                                world = RealmManager.AddWorld(new OryxChamberMap());
                                break;
                            default:
                                SendError("Portal Not Implemented!");
                                break;
                        }
                    }
                    if (setWorldInstance)
                        p.WorldInstance = world;
                }
                else
                {
                    switch (entity.ObjectType) // Special Portals that cannot be the portal class
                    {
                        case 0x072f:
                            world = RealmManager.GuildHallWorld(Guild);
                            break;
                        default:
                            psr.SendPacket(new TextPacket
                            {
                                BubbleTime = 0,
                                Stars = -1,
                                Name = "",
                                Text = "Semi-Portal Not Implemented!"
                            });
                            break;
                    }
                }
            }

            //used to match up player to last realm they were in, to return them to it. Sometimes is odd, like from Vault back to Vault...

            if (RealmManager.PlayerWorldMapping.ContainsKey(AccountId))
            {
                World tempWorld;
                RealmManager.PlayerWorldMapping.TryRemove(AccountId, out tempWorld);
            }
            RealmManager.PlayerWorldMapping.TryAdd(AccountId, Owner);

            if (world != null)
                psr.Reconnect(new ReconnectPacket
                {
                    Host = "",
                    Port = 2050,
                    GameId = world.Id,
                    Name = world.Name,
                    Key = Empty<byte>.Array,
                });
        }

        public void Teleport(RealmTime time, TeleportPacket pkt)
        {
            Teleport(time, pkt.ObjectId);
        }

        public void Teleport(RealmTime time, int objId)
        {
            Entity obj = Owner.GetEntity(objId);
            if (obj == null) return;
            if (!TPCooledDown())
            {
                SendError("Too soon to teleport again!");
                return;
            }
            if (obj.HasConditionEffect(ConditionEffects.Invisible))
            {
                SendError("Cannot teleport to invisible players");
                return;
            }

            SetTPDisabledPeriod();
            if (Pet != null)
                Pet.Move(X, Y);
            Move(obj.X, obj.Y);
            fames.Teleport();
            SetNewbiePeriod();
            UpdateCount++;
            
            Owner.BroadcastPacket(new GotoPacket
            {
                ObjectId = Id,
                Position = new Position
                {
                    X = X,
                    Y = Y
                }
            }, null);
            Owner.BroadcastPacket(new ShowEffectPacket
            {
                EffectType = EffectType.Teleport,
                TargetId = Id,
                PosA = new Position
                {
                    X = X,
                    Y = Y
                },
                Color = new ARGB(0xFFFFFFFF)
            }, null);
        }

        public void GotoAck(RealmTime time, GotoAckPacket pkt)
        {
        }

        public void AOEAck(RealmTime time, AOEAckPacket pkt)
        {
        }

        public void VisibulletHit(VisibulletPacket pkt)
        {
            //Possible bug I can see right now:
            //  If enemy does not exist, no condition effects can be applied.
            if (pkt.EnemyId == -2)
            {
                HP = 0;
                UpdateCount++;
                Death("Unknown");
                return;
            }
            Entity enemy = Owner.GetEntity(pkt.EnemyId);
            string killer = "Unknown";
            if (!HasConditionEffect(ConditionEffects.Invulnerable))
                HP -= pkt.Damage;
            ConditionEffects? ceffects = null;
            if (enemy != null)
            {
                killer = enemy.ObjectDesc.DisplayId ??
                         enemy.ObjectDesc.ObjectId;
                Projectile proj = (enemy as IProjectileOwner).Projectiles[pkt.BulletId];
                if (proj != null)
                {
                    if (!HasConditionEffect(ConditionEffects.Invincible))
                        ceffects = proj.ConditionEffects;
                    if (!HasConditionEffect(ConditionEffects.Blessed))
                    {
                        ApplyConditionEffect(proj.Descriptor.Effects);
                    }
                }
            }
            UpdateCount++;

            Owner.BroadcastPacket(new DamagePacket
            {
                TargetId = Id,
                Effects = (!ceffects.HasValue) ? 0 : (ConditionEffects) ceffects,
                Damage = (ushort) pkt.Damage,
                Killed = HP <= 0,
                BulletId = pkt.BulletId,
                ObjectId = pkt.EnemyId
            }, this);

            if (HP <= 0) Death(killer);
        }

        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            if (projectile.ProjectileOwner is Player ||
                HasConditionEffect(ConditionEffects.Paused) ||
                HasConditionEffect(ConditionEffects.Stasis) ||
                HasConditionEffect(ConditionEffects.Invincible))
                return false;

            return base.HitByProjectile(projectile, time);
        }

        private bool CheckResurrection()
        {
            for (int i = 0; i < 4; i++)
            {
                Item item = Inventory[i];
                if (item == null || !item.Resurrects) continue;

                HP = Stats[0] + Stats[0];
                MP = Stats[1] + Stats[1];
                Inventory[i] = null;
                Owner.BroadcastPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = string.Format("{0}'s {1} breaks and he disappears", Name, item.ObjectId)
                }, null);
                psr.Reconnect(new ReconnectPacket
                {
                    Host = "",
                    Port = 2050,
                    GameId = World.NEXUS_ID,
                    Name = "Nexus",
                    Key = Empty<byte>.Array,
                });

                resurrecting = true;
                return true;
            }
            return false;
        }

        private void GenerateGravestone()
        {
            int maxed = 0;
            foreach (XElement i in XmlDatas.TypeToElement[ObjectType].Elements("LevelIncrease"))
            {
                int limit = int.Parse(XmlDatas.TypeToElement[ObjectType].Element(i.Value).Attribute("max").Value);
                int idx = StatsManager.StatsNameToIndex(i.Value);
                if (Stats[idx] >= limit)
                    maxed++;
            }

            short objType;
            int? time;
            switch (maxed)
            {
                case 8:
                    objType = 0x0735;
                    time = null;
                    /*
                    if (player.objType = 782) //Wizard
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 784) //Priest
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 768) //Rogue
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 801) //Necromancer
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 798) //Knight
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 800) //Assassin
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 802) //Huntress
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 804) //Trickster
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 775)  //Warrior
                    {
                        objType = 0x0723; time = null;
                    }
                     * 
                    else if (player.objType = 782)
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 782)
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 782)
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 782)
                    {
                        objType = 0x0723; time = null;
                    }
                    else if (player.objType = 782)
                    {
                        objType = 0x0723; time = null;
                    }
                    */
                    break;
                case 7:
                    objType = 0x0734;
                    time = null;
                    break;
                case 6:
                    objType = 0x072b;
                    time = null;
                    break;
                case 5:
                    objType = 0x072a;
                    time = null;
                    break;
                case 4:
                    objType = 0x0729;
                    time = null;
                    break;
                case 3:
                    objType = 0x0728;
                    time = null;
                    break;
                case 2:
                    objType = 0x0727;
                    time = null;
                    break;
                case 1:
                    objType = 0x0726;
                    time = null;
                    break;
                default:
                    if (Level <= 1)
                    {
                        objType = 0x0723;
                        time = 30*1000;
                    }
                    else if (Level < 20)
                    {
                        objType = 0x0724;
                        time = 60*1000;
                    }
                    else
                    {
                        objType = 0x0725;
                        time = 5*60*1000;
                    }
                    break;
            }
            var con = new Container(objType, null, false);
            var con2 = new Container(0x0502, null, false);
            //Item[] equips = { new Item(XmlDatas.TypeToElement[Inventory[0].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[1].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[2].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[3].ObjectType]), null, null, null, null};
            //Item[] inv = { new Item(XmlDatas.TypeToElement[Inventory[4].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[5].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[6].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[7].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[8].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[9].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[10].ObjectType]), new Item(XmlDatas.TypeToElement[Inventory[11].ObjectType])};
            for (var i = 0; i < 4; i++)
            {
                con.Inventory[i] = Inventory[i];
            }

            for (var i = 4; i < 12; i++)
            {
                con2.Inventory[i - 4] = Inventory[i];
            }

            con.Move(X, Y);
            con2.Move(X + 1, Y);
            con.Name = Name;

            Owner.EnterWorld(con);
            Owner.EnterWorld(con2);
            //var obj = new StaticObject(objType, time, true, time != null, false);
            //obj.Move(X, Y);
            //obj.Name = Name;
            //Owner.EnterWorld(obj);
        }

        public void GivePet(short petId)
        {
            if (Pet != null)
            {
                Owner.LeaveWorld(Pet);
            }
            Pet = Resolve(petId);
            Pet.PlayerOwner = this;
            Pet.Move(X, Y);
            Pet.isPet = true;
            Owner.EnterWorld(Pet);
        }

        public bool CompareName(string name)
        {
            string rn = name.ToLower();
            if (rn.Split(' ')[0].StartsWith("[") || Name.Split(' ').Length == 1)
                if (Name.ToLower().StartsWith(rn)) return true;
                else return false;
            if (Name.Split(' ')[1].ToLower().StartsWith(rn)) return true;
            return false;
        }


        public void Death(string killer)
        {
            if (psr.Stage == ProtocalStage.Disconnected || resurrecting)
                return;

            if (!dying)
            {
                dying = true;
                switch (Owner.Name)
                {
                    case "Free Battle Arena":
                    case "Battle Arena":
                    case "Arena":
                        Owner.BroadcastPacket(new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = Name + " was eliminated by " + killer //removed XP as max packet length reached!
                        }, null);
                        psr.Reconnect(new ReconnectPacket
                        {
                            Host = "",
                            Port = 2050,
                            GameId = World.NEXUS_ID,
                            Name = "Nexus",
                            Key = Empty<byte>.Array,
                        });
                        HP = 1;
                        resurrecting = true;
                        return;
                    case "Zombies":
                        Owner.BroadcastPacket(new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = Name + " has become one with the infected" //cinematic!
                        }, null);
                        psr.Reconnect(new ReconnectPacket
                        {
                            Host = "",
                            Port = 2050,
                            GameId = World.NEXUS_ID,
                            Name = "Nexus",
                            Key = Empty<byte>.Array,
                        });
                        HP = 1;
                        resurrecting = true;
                        return;
                    case "Nexus":
                        Owner.BroadcastPacket(new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = Name + " was saved by the holy powers of the Nexus."
                        }, null);
                        HP = psr.Character.MaxHitPoints;
                        SaveToCharacter();
                        psr.Save();
                        psr.Disconnect();
                        return;
                }
                if (Client.Account.Rank > 3)
                {
                    HP = psr.Character.MaxHitPoints;
                    SaveToCharacter();
                    psr.Save();
                    psr.Disconnect();
                    return;
                }
                if (CheckResurrection())
                    return;

                if (psr.Character.Dead)
                {
                    SaveToCharacter();
                    psr.Save();
                    psr.Disconnect();
                    return;
                }
                GenerateGravestone();
                switch (killer)
                {
                    case "":
                    case "Unknown":
                        break;
                    default:
                        Owner.BroadcastPacket(new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text =
                                Name + " died at Level " + Level + ", with " + Fame + " Fame" + ", killed by " + killer
                        }, null);
                        break;
                }
            }

            try
            {
                List<string> maxed = (from i in XmlDatas.TypeToElement[ObjectType].Elements("LevelIncrease")
                    let limit = int.Parse(XmlDatas.TypeToElement[ObjectType].Element(i.Value).Attribute("max").Value)
                    let idx = StatsManager.StatsNameToIndex(i.Value)
                    where Stats[idx] >= limit
                    select StatsManager.StatsIndexToPotName(idx)).ToList();

                psr.Character.Dead = true;
                SaveToCharacter();
                psr.Save();
                if (Owner.Id != -6)
                {
                    psr.SaveDeath(killer);
                    psr.SendPacket(new DeathPacket
                    {
                        AccountId = AccountId,
                        CharId = psr.Character.CharacterId,
                        Killer = killer
                    });
                    Owner.Timers.Add(new WorldTimer(1000, (w, t) => psr.Disconnect()));
                    Owner.LeaveWorld(this);

                    int mcount = maxed.Count;
                    if (mcount >= 2)
                    {
                        MaxPotionOnDeath(maxed);
                        if (mcount >= 6)
                        {
                            MaxPotionOnDeath(maxed);
                            if (mcount == 8)
                            {
                                MaxPotionOnDeath(maxed);
                            }
                        }
                    }
                }
                else
                    psr.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MaxPotionOnDeath(List<string> maxed)
        {
            maxed.Shuffle();
            psr.Account.Bonuses.Add(XmlDatas.IdToType["Potion of Max " + maxed[0]]);
            new Database().SetBonuses(psr.Account.AccountId, psr.Account.Bonuses);
            maxed.RemoveAt(0);
        }
    }
}