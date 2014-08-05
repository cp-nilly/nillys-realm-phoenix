#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using db;
using db.data;
using MySql.Data.MySqlClient;
using wServer.cliPackets;
using wServer.realm;
using wServer.realm.entities.player;
using wServer.realm.worlds;
using wServer.svrPackets;
using FailurePacket = wServer.svrPackets.FailurePacket;

#endregion

namespace wServer
{
    public enum ProtocalStage
    {
        Connected,
        Handshaked,
        Ready,
        Disconnected
    }

    public class ClientProcessor
    {
        private readonly Socket skt;
        public uint UpdateAckCount = 0;
        private Account account;
        private Char character;
        public string clientVer = "1.9.3"; //Might want this
        private Database db;
        private Player entity;
        private NetworkHandler handler;
        private IP ip;

        private ProtocalStage stage;
        private int targetWorld = -1;

        public ClientProcessor(Socket skt)
        {
            this.skt = skt;
            ReceiveKey =
                new RC4(new byte[] {0x31, 0x1f, 0x80, 0x69, 0x14, 0xF1, 0xc7, 0x1b, 0x09, 0xa1, 0x3a, 0x2a, 0x6e});
            SendKey = new RC4(new byte[] {0x72, 0xc5, 0x58, 0x3c, 0xaf, 0xb6, 0x81, 0x89, 0x9F, 0xcb, 0xd7, 0x4b, 0x80});
        }

        public RC4 ReceiveKey { get; private set; }
        public RC4 SendKey { get; private set; }
        public RealmManager Manager { get; private set; }

        public Socket Socket
        {
            get { return skt; }
        }

        public Database Database
        {
            get { return db; }
        }

        public Char Character
        {
            get { return character; }
        }

        public Account Account
        {
            get { return account; }
        }

        public ProtocalStage Stage
        {
            get { return stage; }
            set { stage = value; } // bad practice?
        }

        public Player Player
        {
            get { return entity; }
        }

        public wRandom Random { get; private set; }
        public string ConnectedBuild { get; private set; }

        public IP IP
        {
            get { return ip; }
        }

        public void BeginProcess()
        {
            handler = new NetworkHandler(this, skt);
            handler.BeginHandling();
        }

        public void SendPacket(Packet pkt)
        {
            handler.SendPacket(pkt);
        }

        public void SendPackets(IEnumerable<Packet> pkts)
        {
            handler.SendPackets(pkts);
        }

        //internal bool ProcessPacket(Packet pkt)
        public bool IsReady()
        {
            if (stage == ProtocalStage.Disconnected)
                return false;
            if (stage == ProtocalStage.Ready && (entity == null || entity != null && entity.Owner == null))
                return false;
            return true;
        }

        internal void ProcessPacket(Packet pkt)
        {
            try
            {
                if (pkt.ID == PacketID.Hello)
                    ProcessHelloPacket(pkt as HelloPacket);
                else if (pkt.ID == PacketID.Create)
                    ProcessCreatePacket(pkt as CreatePacket);
                else if (pkt.ID == PacketID.Load)
                    ProcessLoadPacket(pkt as LoadPacket);
                else if (pkt.ID == PacketID.Broadcast)
                    ProcessBroadcastPacket(pkt as BroadcastPacket);
                else if (pkt.ID == PacketID.Move)
                    RealmManager.Logic.AddPendingAction(t => entity.Move(t, pkt as MovePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.PlayerShoot)
                    RealmManager.Logic.AddPendingAction(t => entity.PlayerShoot(t, pkt as PlayerShootPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.EnemyHit)
                    RealmManager.Logic.AddPendingAction(t => entity.EnemyHit(t, pkt as EnemyHitPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.OtherHit)
                    RealmManager.Logic.AddPendingAction(t => entity.OtherHit(t, pkt as OtherHitPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.SquareHit)
                    RealmManager.Logic.AddPendingAction(t => entity.SquareHit(t, pkt as SquareHitPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.PlayerHit)
                    RealmManager.Logic.AddPendingAction(t => entity.PlayerHit(t, pkt as PlayerHitPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.ShootAck) //Spam
                    RealmManager.Logic.AddPendingAction(t => entity.ShootAck(t, pkt as ShootAckPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.InvSwap)
                    RealmManager.Logic.AddPendingAction(t => entity.InventorySwap(t, pkt as InvSwapPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.InvDrop)
                    RealmManager.Logic.AddPendingAction(t => entity.InventoryDrop(t, pkt as InvDropPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.UseItem)
                    RealmManager.Logic.AddPendingAction(t => entity.UseItem(t, pkt as UseItemPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.UsePortal)
                    RealmManager.Logic.AddPendingAction(t => entity.UsePortal(t, pkt as UsePortalPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.PlayerText)
                    RealmManager.Logic.AddPendingAction(t => entity.PlayerText(t, pkt as PlayerTextPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.ChooseName)
                    ProcessChooseNamePacket(pkt as ChooseNamePacket);
                else if (pkt.ID == PacketID.Escape)
                    ProcessEscapePacket(pkt as EscapePacket);
                else if (pkt.ID == PacketID.Teleport)
                    RealmManager.Logic.AddPendingAction(t => entity.Teleport(t, pkt as TeleportPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.GotoAck)
                    RealmManager.Logic.AddPendingAction(t => entity.GotoAck(t, pkt as GotoAckPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.EditAccountList)
                    RealmManager.Logic.AddPendingAction(t => entity.EditAccountList(t, pkt as EditAccountListPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.Buy)
                    RealmManager.Logic.AddPendingAction(t => entity.Buy(t, pkt as BuyPacket), PendingPriority.Networking);
                else if (pkt.ID == PacketID.RequestTrade)
                    RealmManager.Logic.AddPendingAction(t => entity.RequestTrade(t, pkt as RequestTradePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.ChangeTrade)
                    RealmManager.Logic.AddPendingAction(t => entity.ChangeTrade(t, pkt as ChangeTradePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.AcceptTrade)
                    RealmManager.Logic.AddPendingAction(t => entity.AcceptTrade(t, pkt as AcceptTradePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.CancelTrade)
                    RealmManager.Logic.AddPendingAction(t => entity.CancelTrade(t, pkt as CancelTradePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.AOEAck)
                    RealmManager.Logic.AddPendingAction(t => entity.AOEAck(t, pkt as AOEAckPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.GroundDamage)
                    RealmManager.Logic.AddPendingAction(t => entity.GroundDamage(t, pkt as GroundDamagePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.CheckCredits)
                    RealmManager.Logic.AddPendingAction(t => entity.CheckCredits(t, pkt as CheckCreditsPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.GuildRemove)
                    RealmManager.Logic.AddPendingAction(t => entity.GuildRemove(t, pkt as GuildRemovePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.CreateGuild)
                    RealmManager.Logic.AddPendingAction(t => entity.CreateGuild(t, pkt as CreateGuildPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.JoinGuild)
                    RealmManager.Logic.AddPendingAction(t => entity.JoinGuild(t, pkt as JoinGuildPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.GuildInvite)
                    RealmManager.Logic.AddPendingAction(t => entity.InviteToGuild(t, pkt as GuildInvitePacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.ChangeGuildRank)
                    RealmManager.Logic.AddPendingAction(t => entity.ChangeGuildRank(t, pkt as ChangeGuildRankPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.Visibullet)
                    RealmManager.Logic.AddPendingAction(t => entity.VisibulletHit(pkt as VisibulletPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.TextBoxButton)
                    RealmManager.Logic.AddPendingAction(t => entity.TextBoxButton(pkt as TextBoxButtonPacket),
                        PendingPriority.Networking);
                else if (pkt.ID == PacketID.UpdateAck)
                    RealmManager.Logic.AddPendingAction(t => HandleUpdateAck(), PendingPriority.Networking);
                else if (pkt.ID == PacketID.Pong)
                    entity.Pong((pkt as PongPacket).Time, pkt as PongPacket);
                else
                {
                    Console.WriteLine(@"Unhandled packet: " + pkt);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(pkt.ToString());
                Console.Out.WriteLine(e);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (stage == ProtocalStage.Disconnected)
                return;

            try
            {
                skt.Close();

                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }

                if (account != null)
                    DisconnectFromRealm();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Save()
        {
            try
            {
                if (character == null) return;
                using (var dbx = new Database())
                {
                    entity.SaveToCharacter();
                    if (entity.Owner.Id != -6)
                        dbx.SaveCharacter(account, character);
                    dbx.Dispose();
                }
            }
            catch
            {
            }
        }

        public void SaveDeath(string killer) // temp fix
        {
            try
            {
                if (character == null) return;
                using (var dbx = new Database())
                {
                    if (entity.Owner.Id != -6)
                        dbx.Death(account, character, killer);
                    dbx.Dispose();
                }
            }
            catch
            {
            }
        }

        private void ConnectionFailed(string msg)
        {
            SendPacket(new FailurePacket
            {
                Message = msg
            });
        }

        /* Makes various checks to see if client is allowed to connect.
         * Returns null on success, otherwise return value contains
         * discription of error.
         */
        private string OkToConnect(HelloPacket pkt)
        {
            string retMsg = null;
            ConnectedBuild = pkt.BuildVersion;

            // check client version
            if (!ConnectedBuild.StartsWith(clientVer))
                ConnectionFailed(retMsg = "Wrong build version.");

            // has valid account?
            else if ((account = db.Verify(pkt.GUID, pkt.Password)) == null)
                ConnectionFailed(retMsg = "Invalid account.");

            // ip banned?
            else if (ip.Banned)
                ConnectionFailed(retMsg = "IP banned.");

            // account banned?
            else if (account.Banned)
                ConnectionFailed(retMsg = "Account banned.");

            // server full?
            else if (RealmManager.Clients.Count >= RealmManager.MAX_CLIENT)
                ConnectionFailed(retMsg = "Server full.");

            // valid gameId?
            else if (RealmManager.GetWorld(pkt.GameId) == null) 
            {
                // invalid world... send to nexus instead
                pkt.GameId = World.NEXUS_ID;
            }
            
            // account already connected? disconnect if so
            else if (AccountConnected(account.AccountId))
            {
                ConnectionFailed(retMsg = "Account in use... ");
                ClientProcessor target = RealmManager.Clients[account.AccountId];
                target.Disconnect();
            }

            return retMsg;
        }

        private void ProcessHelloPacket(HelloPacket pkt)
        {
            // connect to database
            db = new Database();
            account = db.Verify(pkt.GUID, pkt.Password);
            ip = db.CheckIp(skt.RemoteEndPoint.ToString().Split(':')[0]);

            Console.Write(@"Connecting " + ((account == null)? "null":account.Name) + 
                                "@" + ip.Address + "... ");

            // check if ok to connect
            string msg;
            if ((msg = OkToConnect(pkt)) != null)
            {
                Console.WriteLine(msg);
                account = null;
                Disconnect();
                return;
            }

            // ok to connect, add client to client list
            RealmManager.Clients.TryAdd(account.AccountId, this);

            // setup client world
            World world = RealmManager.GetWorld(pkt.GameId);
            if (world.Id == -6) //Test World
                (world as Test).LoadJson(pkt.MapInfo);
            else if (world.IsLimbo)
                world = world.GetInstance(this);
            uint seed = (uint)((long)Environment.TickCount * pkt.GUID.GetHashCode()) % uint.MaxValue;
            Random = new wRandom(seed);
            targetWorld = world.Id;

            // connection successful, send MapInfo packet
            SendPacket(new MapInfoPacket
            {
                Width = world.Map.Width,
                Height = world.Map.Height,
                Name = world.Name,
                Seed = seed,
                Background = world.Background,
                AllowTeleport = world.AllowTeleport,
                ShowDisplays = world.ShowDisplays,
                Music = world.GetMusic(Random),
                ClientXML = world.ClientXML,
                ExtraXML = world.ExtraXML,
                SendMusic = true
            });
            stage = ProtocalStage.Handshaked;

            Console.WriteLine("Joined " + world.Name + " (" + world.Id + ").");
        }

        private void ProcessCreatePacket(CreatePacket pkt)
        {
            int nextCharId = 1;
            int maxChar = 2;
            nextCharId = db.GetNextCharId(account);
            MySqlCommand cmd = db.CreateQuery();
            cmd.CommandText = "SELECT maxCharSlot FROM accounts WHERE id=@accId;";
            cmd.Parameters.AddWithValue("@accId", account.AccountId);
            try
            {
                maxChar = (int) cmd.ExecuteScalar();
            }
            catch
            {
            }
            cmd = db.CreateQuery();
            cmd.CommandText = "SELECT COUNT(id) FROM characters WHERE accId=@accId AND dead = FALSE;";
            cmd.Parameters.AddWithValue("@accId", account.AccountId);
            var currChar = (int) (long) cmd.ExecuteScalar();

            if (currChar >= maxChar)
            {
                Disconnect();
                return;
            }
            if (CheckAccountInUse(account.AccountId))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("CreatePacket: " + account.Name + ", account in use.");
                Console.ForegroundColor = ConsoleColor.White;
                SendPacket(new FailurePacket
                {
                    Message = "Account in use! Retrying..."
                });
                Disconnect();
                return;
            }

            character = Database.CreateCharacter(pkt.ObjectType, nextCharId);
            var stats = new[]
            {
                character.MaxHitPoints,
                character.MaxMagicPoints,
                character.Attack,
                character.Defense,
                character.Speed,
                character.Dexterity,
                character.HpRegen,
                character.MpRegen
            };

            bool ok = true;
            cmd = db.CreateQuery();
            cmd.Parameters.AddWithValue("@accId", account.AccountId);
            cmd.Parameters.AddWithValue("@charId", nextCharId);
            cmd.Parameters.AddWithValue("@charType", pkt.ObjectType);
            cmd.Parameters.AddWithValue("@items", Utils.GetCommaSepString(character.EquipSlots()));
            cmd.Parameters.AddWithValue("@stats", Utils.GetCommaSepString(stats));
            cmd.Parameters.AddWithValue("@fameStats", character.FameStats.ToString());
            cmd.CommandText =
                "INSERT INTO characters (accId, charId, charType, level, exp, fame, items, hp, mp, stats, dead, pet, fameStats) VALUES (@accId, @charId, @charType, 1, 0, 0, @items, 100, 100, @stats, FALSE, -1, @fameStats);";
            int v = cmd.ExecuteNonQuery();
            ok = v > 0;

            if (ok)
            {
                if (account.Bonuses != null)
                    if (account.Bonuses.Count > 0)
                    {
                        short[] chrEquip = character.Equipment;
                        for (int i = 4; i < 12; i++)
                        {
                            if (chrEquip[i] == -1)
                            {
                                chrEquip[i] = account.Bonuses.First();
                                account.Bonuses.Remove(account.Bonuses.First());
                                SendPacket(new TextPacket
                                {
                                    Name = "",
                                    BubbleTime = 0,
                                    Stars = -1,
                                    Text = "You received " + XmlDatas.TypeToId[chrEquip[i]] + "."
                                });
                                if (account.Bonuses.Count == 0)
                                    break;
                            }
                        }
                        db.SetBonuses(account.AccountId, account.Bonuses);
                        character.Equipment = chrEquip;
                    }
                    else
                    {
                    }
                else
                {
                    account.Bonuses = new List<short>();
                }
                if (IP.Gifts != null)
                    if (IP.Gifts.Count > 0)
                    {
                        short[] chrEquip = character.Equipment;
                        for (int i = 4; i < 12; i++)
                        {
                            if (chrEquip[i] == -1)
                            {
                                chrEquip[i] = IP.Gifts.First();
                                IP.Gifts.Remove(IP.Gifts.First());
                                SendPacket(new TextPacket
                                {
                                    Name = "",
                                    BubbleTime = 0,
                                    Stars = -1,
                                    Text = "You received " + XmlDatas.TypeToId[chrEquip[i]] + "."
                                });
                                if (IP.Gifts.Count == 0)
                                    break;
                            }
                        }
                        db.SetBonuses(IP.Address, IP.Gifts);
                        character.Equipment = chrEquip;
                    }
                    else
                    {
                    }
                else
                {
                    IP.Gifts = new List<short>();
                }
                db.SaveBackpacks(character, account);
            }

            if (ok)
            {
                World target = RealmManager.Worlds[targetWorld];
                //Delay to let client load remote texture
                /*target.Timers.Add(new WorldTimer(2000, (w, t) => SendPacket(new CreateResultPacket
                {
                    CharacterID = character.CharacterId,
                    ObjectID = RealmManager.Worlds[targetWorld].EnterWorld(entity = new Player(this))
                })));*/
                // ^^ above can cause a crash. textures will be embedded at some point so this isn't necessary
                SendPacket(new CreateResultPacket
                {
                    CharacterID = character.CharacterId,
                    ObjectID = RealmManager.Worlds[targetWorld].EnterWorld(entity = new Player(this))
                });
                stage = ProtocalStage.Ready;
            }
            else
                SendPacket(new FailurePacket
                {
                    Message = "Failed to Load character."
                });
        }

        private void ProcessLoadPacket(LoadPacket pkt)
        {
            character = db.LoadCharacter(account, pkt.CharacterId);
            if (character != null)
            {
                if (character.Dead)
                {
                    SendPacket(new FailurePacket
                    {
                        Message = "Character is dead."
                    });
                    Disconnect();
                }
                else if (CheckAccountInUse(account.AccountId))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("LoadPacket: " + account.Name + ", account in use.");
                    Console.ForegroundColor = ConsoleColor.White;
                    SendPacket(new FailurePacket
                    {
                        Message = "Account in use! Retrying..."
                    });
                    Disconnect();
                }
                else
                {
                    if (account.Bonuses != null)
                        if (account.Bonuses.Count > 0)
                        {
                            short[] chrEquip = character.Equipment;
                            for (int i = 4; i < 12; i++)
                            {
                                if (chrEquip[i] == -1)
                                {
                                    chrEquip[i] = account.Bonuses.First();
                                    account.Bonuses.Remove(account.Bonuses.First());
                                    SendPacket(new TextPacket
                                    {
                                        Name = "",
                                        BubbleTime = 0,
                                        Stars = -1,
                                        Text = "You received " + XmlDatas.TypeToId[chrEquip[i]] + "."
                                    });
                                    if (account.Bonuses.Count == 0)
                                        break;
                                }
                            }
                            db.SetBonuses(account.AccountId, account.Bonuses);
                            character.Equipment = chrEquip;
                        }
                        else
                        {
                        }
                    else
                    {
                        account.Bonuses = new List<short>();
                    }
                    if (IP.Gifts != null)
                        if (IP.Gifts.Count > 0)
                        {
                            short[] chrEquip = character.Equipment;
                            for (int i = 4; i < 12; i++)
                            {
                                if (chrEquip[i] == -1)
                                {
                                    chrEquip[i] = IP.Gifts.First();
                                    IP.Gifts.Remove(IP.Gifts.First());
                                    SendPacket(new TextPacket
                                    {
                                        Name = "",
                                        BubbleTime = 0,
                                        Stars = -1,
                                        Text = "You received " + XmlDatas.TypeToId[chrEquip[i]] + "."
                                    });
                                    if (IP.Gifts.Count == 0)
                                        break;
                                }
                            }
                            db.SetBonuses(IP.Address, IP.Gifts);
                            character.Equipment = chrEquip;
                        }
                        else
                        {
                        }
                    else
                    {
                        IP.Gifts = new List<short>();
                    }

                    World target = RealmManager.Worlds[targetWorld];
                    //Delay to let client load remote texture <-- remote textures will be embedded at some point so this isn't necessary
                    /*target.Timers.Add(new WorldTimer(2000, (w, t) => SendPacket(new CreateResultPacket
                    {
                        CharacterID = character.CharacterId,
                        ObjectID = RealmManager.Worlds[targetWorld].EnterWorld(entity = new Player(this))
                    })));*/
                    // the previous delay was also taken out to fix a null account variable that happens on occation
                    SendPacket(new CreateResultPacket
                    {
                        CharacterID = character.CharacterId,
                        ObjectID = RealmManager.Worlds[targetWorld].EnterWorld(entity = new Player(this))
                    });
                    stage = ProtocalStage.Ready;
                }
            }
            else
            {
                Player.SendInfo("Failed to Load character.");
                Disconnect();
            }
        }

        private void ProcessChooseNamePacket(ChooseNamePacket pkt)
        {
            MySqlCommand cmdx = db.CreateQuery();
            cmdx.CommandText = "SELECT namechosen FROM accounts WHERE id=@accId";
            cmdx.Parameters.AddWithValue("@accId", account.AccountId);
            object execx = cmdx.ExecuteScalar();
            bool namechosen = bool.Parse(execx.ToString());

            if (string.IsNullOrEmpty(pkt.Name) ||
                pkt.Name.Length > 10)
            {
                SendPacket(new NameResultPacket
                {
                    Success = false,
                    Message = "Invalid name"
                });
            }
            else
            {
                MySqlCommand cmd = db.CreateQuery();
                cmd.CommandText = "SELECT COUNT(name) FROM accounts WHERE name=@name;";
                cmd.Parameters.AddWithValue("@name", pkt.Name);
                object x = cmd.ExecuteScalar();
                if (int.Parse(x.ToString()) > 0)
                    SendPacket(new NameResultPacket
                    {
                        Success = false,
                        Message = "Duplicated name"
                    });
                else
                {
                    db.ReadStats(account);
                    if (account.Credits < 1000 && namechosen)
                        SendPacket(new NameResultPacket
                        {
                            Success = false,
                            Message = "Not enough credits"
                        });
                    else
                    {
                        if (account.NameChosen == false)
                        {
                            cmd = db.CreateQuery();
                            cmd.CommandText = "UPDATE accounts SET name=@name, namechosen=TRUE WHERE id=@accId;";
                            cmd.Parameters.AddWithValue("@accId", account.AccountId);
                            cmd.Parameters.AddWithValue("@name", pkt.Name);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                entity.Name = pkt.Name;
                                entity.NameChosen = true;
                                entity.UpdateCount++;
                                SendPacket(new NameResultPacket
                                {
                                    Success = true,
                                    Message = "Success!"
                                });
                            }
                            else
                                SendPacket(new NameResultPacket
                                {
                                    Success = false,
                                    Message = "Internal Error"
                                });
                        }
                        else
                        {
                            cmd = db.CreateQuery();
                            cmd.CommandText = "UPDATE accounts SET name=@name, namechosen=TRUE WHERE id=@accId;";
                            cmd.Parameters.AddWithValue("@accId", account.AccountId);
                            cmd.Parameters.AddWithValue("@name", pkt.Name);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                entity.Credits = db.UpdateCredit(account, -1000);
                                entity.Name = pkt.Name;
                                entity.NameChosen = true;
                                entity.UpdateCount++;
                                SendPacket(new NameResultPacket
                                {
                                    Success = true,
                                    Message = "Success!"
                                });
                            }
                            else
                                SendPacket(new NameResultPacket
                                {
                                    Success = false,
                                    Message = "Internal Error"
                                });
                        }
                    }
                }
            }
        }

        private void ProcessEscapePacket(EscapePacket pkt)
        {
            try
            {
                World world = RealmManager.GetWorld(Player.Owner.Id);
                if (world.Id == World.NEXUS_ID)
                {
                    SendPacket(new TextPacket
                    {
                        Stars = -1,
                        BubbleTime = 0,
                        Name = "",
                        Text = "You are already at the Nexus!"
                    });
                    return;
                }
                /*Reconnect(new ReconnectPacket
                {
                    Host = "",
                    Port = 2050,
                    GameId = World.NEXUS_ID,
                    Name = "Nexus",
                    Key = Empty<byte>.Array,
                });*/
                Disconnect();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.WriteLine("Error while sending EscapePacket, Check ClientProcessor.cs");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void ProcessBroadcastPacket(BroadcastPacket pkt)
        {
            Account acc;
            string msg = pkt.Message;

            if ((acc = new Database().Verify(pkt.Username, pkt.Password)) != null)
            {
                if (acc.Rank >= 5)
                {
                    foreach (ClientProcessor i in RealmManager.Clients.Values)
                        i.SendPacket(new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "#" + acc.Name,
                            Text = " " + msg
                        });
                }
            }
        }

        //Following must execute, network loop will discard disconnected client, so logic loop
        private void DisconnectFromRealm()
        {
            RealmManager.Logic.AddPendingAction(t => 
                RealmManager.Disconnect(this), PendingPriority.Destruction);
        }

        public void Reconnect(ReconnectPacket pkt)
        {
            RealmManager.Logic.AddPendingAction(t =>
            {
                RealmManager.Disconnect(this); // saves player on exit
                SendPacket(pkt);
            }, PendingPriority.Destruction);
        }

        public bool AccountConnected(int accId)
        {
            if (RealmManager.Clients.ContainsKey(account.AccountId))
                return true;
            return false;
        }

        public bool CheckAccountInUse(int accId)
        {
            foreach (World w in RealmManager.Worlds.Values)
                foreach (Player p in w.Players.Values)
                {
                    if (p.AccountId == accId) return true;
                }

            return false;
        }

        public void HandleUpdateAck()
            //each time the server sends an UpdatePacket, the client sends back an UpdateAckPacket: if they're out of sync, we can disconnect the client
        {
            /*this.UpdateAckCount++;
            if ((this.Player.UpdateCount - (int)this.UpdateAckCount) > 5)
            {
                Console.WriteLine("Possible custom client (UpdateAck count lesser than UpdateCount): " + this.Account.Name);
            }
            if ((this.Player.UpdateCount - (int)this.UpdateAckCount) > 10)
            {
                Console.WriteLine("Probable custom client (UpdateAck count lesser than UpdateCount): " + this.Account.Name+", disconnecting it");
                psr.SendPacket(new svrPackets.FailurePacket() { Message = "Probable hacked client detected, disconnecting..." });
                psr.Disconnect();
            }*/
        }
    }
}