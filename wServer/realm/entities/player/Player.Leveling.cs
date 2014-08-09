#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using common.data;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        private static readonly Dictionary<string, Tuple<int, int, int>> QuestDat =
            new Dictionary<string, Tuple<int, int, int>> //Priority, Min, Max
            {
                {"Scorpion Queen", Tuple.Create(1, 1, 6)},
                {"Bandit Leader", Tuple.Create(1, 1, 6)},
                {"Hobbit Mage", Tuple.Create(3, 3, 8)},
                {"Undead Hobbit Mage", Tuple.Create(3, 3, 8)},
                {"Giant Crab", Tuple.Create(3, 3, 8)},
                {"Desert Werewolf", Tuple.Create(3, 3, 8)},
                {"Sandsman King", Tuple.Create(4, 4, 9)},
                {"Goblin Mage", Tuple.Create(4, 4, 9)},
                {"Elf Wizard", Tuple.Create(4, 4, 9)},
                {"Dwarf King", Tuple.Create(5, 5, 10)},
                {"Swarm", Tuple.Create(6, 6, 11)},
                {"Shambling Sludge", Tuple.Create(6, 6, 11)},
                {"Great Lizard", Tuple.Create(7, 7, 12)},
                {"Wasp Queen", Tuple.Create(8, 7, 20)},
                {"Horned Drake", Tuple.Create(8, 7, 20)},
                {"Deathmage", Tuple.Create(5, 6, 11)},
                {"Great Coil Snake", Tuple.Create(6, 6, 12)},
                {"Lich", Tuple.Create(9, 6, 20)},
                {"Actual Lich", Tuple.Create(9, 7, 20)},
                {"Ent Ancient", Tuple.Create(10, 7, 20)},
                {"Actual Ent Ancient", Tuple.Create(10, 7, 20)},
                {"Oasis Giant", Tuple.Create(11, 8, 20)},
                {"Phoenix Lord", Tuple.Create(11, 9, 20)},
                {"Ghost King", Tuple.Create(12, 10, 20)},
                {"Actual Ghost King", Tuple.Create(12, 10, 20)},
                {"Cyclops God", Tuple.Create(13, 10, 20)},
                {"Red Demon", Tuple.Create(15, 15, 20)},
                {"Skull Shrine", Tuple.Create(16, 15, 20)},
                {"Phoenix God", Tuple.Create(16, 15, 20)},
                {"Pentaract", Tuple.Create(16, 15, 20)},
                {"Cube God", Tuple.Create(16, 15, 20)},
                {"Grand Sphinx", Tuple.Create(16, 15, 20)},
                {"Lord of the Lost Lands", Tuple.Create(16, 15, 20)},
                {"Hermit God", Tuple.Create(16, 15, 20)},
                {"Ghost Ship", Tuple.Create(16, 15, 20)},
                {"Unknown Giant Golem", Tuple.Create(16, 15, 20)},
                {"Evil Chicken God", Tuple.Create(20, 1, 20)},
                {"Bonegrind The Butcher", Tuple.Create(20, 1, 20)},
                {"Dreadstump the Pirate King", Tuple.Create(20, 1, 20)},
                {"Arachna the Spider Queen", Tuple.Create(20, 1, 20)},
                {"Stheno the Snake Queen", Tuple.Create(20, 1, 20)},
                {"Mixcoatl the Masked God", Tuple.Create(20, 1, 20)},
                {"Limon the Sprite God", Tuple.Create(20, 1, 20)},
                {"Septavius the Ghost God", Tuple.Create(20, 1, 20)},
                {"Davy Jones", Tuple.Create(20, 1, 20)},
                {"Lord Ruthven", Tuple.Create(20, 1, 20)},
                {"Archdemon Malphas", Tuple.Create(20, 1, 20)},
                {"Elder Tree", Tuple.Create(20, 1, 20)},
                {"Thessal the Mermaid Goddess", Tuple.Create(20, 1, 20)},
                {"Dr. Terrible", Tuple.Create(20, 1, 20)},
                {"Horrific Creation", Tuple.Create(20, 1, 20)},
                {"Masked Party God", Tuple.Create(20, 1, 20)},
                {"Stone Guardian Left", Tuple.Create(20, 1, 20)},
                {"Stone Guardian Right", Tuple.Create(20, 1, 20)},
                {"Oryx the Mad God 1", Tuple.Create(20, 1, 20)},
                {"Oryx the Mad God 2", Tuple.Create(20, 1, 20)},
                {"Oryx the Mad God 3", Tuple.Create(20, 1, 20)},
            };

        private Entity questEntity;

        public Entity Quest
        {
            get { return questEntity; }
        }

        private static int GetExpGoal(int level)
        {
            return 50 + (level - 1)*100;
        }

        private static int GetLevelExp(int level)
        {
            if (level == 1) return 0;
            return 50*(level - 1) + (level - 2)*(level - 1)*50;
        }

        private static int GetFameGoal(int fame)
        {
            if (fame >= 2000) return 0;
            if (fame >= 800) return 2000;
            if (fame >= 400) return 800;
            if (fame >= 150) return 400;
            if (fame >= 20) return 150;
            return 20;
        }

        public int GetStars()
        {
            int ret = 0;
            foreach (ClassStats i in psr.Account.Stats.ClassStates)
            {
                if (i.BestFame >= 2000) ret += 5;
                else if (i.BestFame >= 800) ret += 4;
                else if (i.BestFame >= 400) ret += 3;
                else if (i.BestFame >= 150) ret += 2;
                else if (i.BestFame >= 20) ret += 1;
            }
            return ret;
        }

        private float Dist(Entity a, Entity b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float) Math.Sqrt(dx*dx + dy*dy);
        }

        private Entity FindQuest()
        {
            Entity ret = null;
            try
            {
                float bestScore = 0;
                foreach (Enemy i in Owner.Quests.Values
                    .OrderBy(quest => MathsUtils.DistSqr(quest.X, quest.Y, X, Y)))
                {
                    if (i.ObjectDesc == null || !i.ObjectDesc.Quest) continue;

                    Tuple<int, int, int> x;
                    if (!QuestDat.TryGetValue(i.ObjectDesc.ObjectId, out x)) continue;

                    if ((Level >= x.Item2 && Level <= x.Item3))
                    {
                        float score = (20 - Math.Abs((i.ObjectDesc.Level ?? 0) - Level))*x.Item1 -
                                      //priority * level diff
                                      Dist(this, i)/100; //minus 1 for every 100 tile distance
                        if (score > bestScore)
                        {
                            bestScore = score;
                            ret = i;
                        }
                    }
                }
            }
            catch
            {
            }
            return ret;
        }

        private void HandleQuest(RealmTime time)
        {
            if (time.tickCount%500 == 0 || questEntity == null || questEntity.Owner == null)
            {
                Entity newQuest = FindQuest();
                if (newQuest != null && newQuest != questEntity)
                {
                    Owner.Timers.Add(new WorldTimer(100, (w, t) =>
                    {
                        psr.SendPacket(new QuestObjIdPacket
                        {
                            ObjectID = newQuest.Id
                        });
                    }));
                    questEntity = newQuest;
                }
            }
        }

        private void CalculateFame()
        {
            int newFame = 0;
            if (Experience < 200*1000) newFame = Experience/1000;
            else newFame = 200 + (Experience - 200*1000)/1000;
            if (newFame != Fame)
            {
                Owner.BroadcastPacket(new NotificationPacket
                {
                    ObjectId = Id,
                    Color = new ARGB(0xFFFF6600),
                    Text = "+" + (newFame - Fame) + " Fame"
                }, null);
                Fame = newFame;
                int newGoal;
                ClassStats state = psr.Account.Stats.ClassStates.SingleOrDefault(_ => _.ObjectType == ObjectType);
                if (state != null && state.BestFame > Fame)
                    newGoal = GetFameGoal(state.BestFame);
                else
                    newGoal = GetFameGoal(Fame);
                if (newGoal > FameGoal)
                {
                    Owner.BroadcastPacket(new NotificationPacket
                    {
                        ObjectId = Id,
                        Color = new ARGB(0xFF00FF00),
                        Text = "Class Quest Complete!"
                    }, null);
                    Stars = GetStars();
                }
                FameGoal = newGoal;
                UpdateCount++;
            }
        }

        private bool CheckLevelUp()
        {
            if (Experience - GetLevelExp(Level) >= ExperienceGoal && Level < 20)
            {
                Level++;
                ExperienceGoal = GetExpGoal(Level);
                foreach (XElement i in XmlDatas.TypeToElement[ObjectType].Elements("LevelIncrease"))
                {
                    var rand = new Random();
                    int min = int.Parse(i.Attribute("min").Value);
                    int max = int.Parse(i.Attribute("max").Value) + 1;
                    int limit = int.Parse(XmlDatas.TypeToElement[ObjectType].Element(i.Value).Attribute("max").Value);
                    int idx = StatsManager.StatsNameToIndex(i.Value);
                    Stats[idx] += rand.Next(min, max);
                    if (Stats[idx] > limit) Stats[idx] = limit;
                }
                HP = Stats[0] + Boost[0];
                MP = Stats[1] + Boost[1];

                UpdateCount++;

                if (Level == 20)
                    foreach (Player i in Owner.Players.Values)
                        i.SendInfo(Name + " achieved level 20");
                questEntity = null;
                return true;
            }
            CalculateFame();
            return false;
        }

        public bool EnemyKilled(Enemy enemy, int exp, bool killer)
        {
            if (enemy == questEntity)
                Owner.BroadcastPacket(new NotificationPacket
                {
                    ObjectId = Id,
                    Color = new ARGB(0xFF0000FF),
                    Text = "Quest Complete!"
                }, null);
            if (exp > 0)
            {
                Experience += exp;
                UpdateCount++;
                foreach (Entity i in Owner.PlayersCollision.HitTest(X, Y, 16))
                {
                    if (i != this)
                    {
                        try
                        {
                            (i as Player).Experience += exp;
                            (i as Player).UpdateCount++;
                            (i as Player).CheckLevelUp();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            fames.Killed(enemy, killer);
            return CheckLevelUp();
        }
    }
}