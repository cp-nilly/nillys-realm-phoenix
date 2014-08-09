#region

using System;
using System.Collections.Generic;
using System.Linq;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        public const int SightRadius = 15;
        private const int AppoxAreaOfSight = (int) (Math.PI*SightRadius*SightRadius + 1);

        public readonly HashSet<Entity> _clientEntities = new HashSet<Entity>();
        private readonly HashSet<IntPoint> _clientStatic = new HashSet<IntPoint>(new IntPointComparer());
        private readonly Dictionary<Entity, int> _lastUpdate = new Dictionary<Entity, int>();
        private List<byte[,]> Invisible = new List<byte[,]>();
        private int _mapHeight;
        private int _mapWidth;
        private int _tickId;

        private IEnumerable<Entity> GetNewEntities()
        {
            foreach (var i in Owner.Players.Where(i => _clientEntities.Add(i.Value)))
            {
                if (!i.Value.vanished || i.Value == this)
                {
                    yield return i.Value;
                }
            }
            foreach (
                Decoy i in
                    Owner.PlayersCollision.HitTest(X, Y, SightRadius).OfType<Decoy>().Where(i => _clientEntities.Add(i))
                )
            {
                yield return i;
            }

            foreach (Entity i in Owner.EnemiesCollision.HitTest(X, Y, SightRadius))
            {
                if (i is Container)
                {
                    int? owner = (i as Container).BagOwner;
                    if (owner != null && owner != AccountId) continue;
                }
                if (MathsUtils.DistSqr(i.X, i.Y, X, Y) <= SightRadius*SightRadius)
                {
                    if (_clientEntities.Add(i))
                        yield return i;
                }
            }
            if (questEntity != null && _clientEntities.Add(questEntity))
                yield return questEntity;
        }

        private IEnumerable<int> GetRemovedEntities()
        {
            foreach (Entity i in _clientEntities.Where(i => i is Player))
            {
                if ((i as Player).vanished && i != this)
                {
                    yield return i.Id;
                }
            }
            foreach (Entity i in _clientEntities.Where(i => !(i is Player) || i.Owner == null))
            {
                if (MathsUtils.DistSqr(i.X, i.Y, X, Y) > SightRadius*SightRadius &&
                    !(i is StaticObject && (i as StaticObject).Static) &&
                    i != questEntity)
                    yield return i.Id;
                else if (i.Owner == null)
                    yield return i.Id;
                if (i is Player)
                {
                    if ((i as Player).vanished && i != this)
                    {
                        yield return i.Id;
                    }
                }
            }
        }

        private IEnumerable<ObjectDef> GetNewStatics(int _x, int _y)
        {
            return (from i in Sight.GetSightCircle(SightRadius)
                let x = i.X + _x
                let y = i.Y + _y
                where x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight
                let tile = Owner.Map[x, y]
                where tile.ObjId != 0 && tile.ObjType != 0 && _clientStatic.Add(new IntPoint(x, y))
                select tile.ToDef(x, y)).ToList();
        }

        private IEnumerable<IntPoint> GetRemovedStatics(int _x, int _y)
        {
            return from i in _clientStatic
                let dx = i.X - _x
                let dy = i.Y - _y
                let tile = Owner.Map[i.X, i.Y]
                where dx*dx + dy*dy > SightRadius*SightRadius ||
                      tile.ObjType == 0
                let objId = Owner.Map[i.X, i.Y].ObjId
                where objId != 0
                select i;
        }

        private void SendUpdate(RealmTime time)
        {
            _mapWidth = Owner.Map.Width;
            _mapHeight = Owner.Map.Height;
            Wmap map = Owner.Map;
            var _x = (int) X;
            var _y = (int) Y;

            var sendEntities = new HashSet<Entity>(GetNewEntities());

            var list = new List<UpdatePacket.TileData>(AppoxAreaOfSight);
            int sent = 0;
            foreach (IntPoint i in Sight.GetSightCircle(SightRadius))
            {
                int x = i.X + _x;
                int y = i.Y + _y;
                bool sightblockedx = false;
                bool sightblockedy = false;
                bool sightblockedxy = false;
                bool sightblockedyx = false;
                WmapTile tile;
                ObjectDesc desc;

                if (x < 0 || x >= _mapWidth ||
                    y < 0 || y >= _mapHeight ||
                    tiles[x, y] >= (tile = map[x, y]).UpdateCount) continue;

                World world = Owner;
                if (world.IsDungeon())
                {
                    if (x < X)
                    {
                        for (int XX = _x; XX > x; XX--)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[XX, _y].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedx = true;
                                }
                            }
                        }
                    }
                    if (x > X)
                    {
                        for (int XX = _x; XX < x; XX++)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[XX, _y].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedx = true;
                                }
                            }
                        }
                    }
                    if (y < Y)
                    {
                        for (int YY = _y; YY > y; YY--)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[_x, YY].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedy = true;
                                }
                            }
                        }
                    }
                    if (y > Y)
                    {
                        for (int YY = _y; YY < y; YY++)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[_x, YY].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedy = true;
                                }
                            }
                        }
                    }
                    if (x < X)
                    {
                        for (int XX = _x; XX > x; XX--)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[XX, y].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedyx = true;
                                }
                            }
                        }
                    }
                    if (x > X)
                    {
                        for (int XX = _x; XX < x; XX++)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[XX, y].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedyx = true;
                                }
                            }
                        }
                    }

                    if (y < Y)
                    {
                        for (int YY = _y; YY > y; YY--)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[x, YY].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedxy = true;
                                }
                            }
                        }
                    }
                    if (y > Y)
                    {
                        for (int YY = _y; YY < y; YY++)
                        {
                            common.data.XmlDatas.ObjectDescs.TryGetValue(map[x, YY].ObjType, out desc);
                            if (desc != null)
                            {
                                if (desc.BlocksSight)
                                {
                                    sightblockedxy = true;
                                }
                            }
                        }
                    }


                    if ((sightblockedy && sightblockedxy) || (sightblockedx && sightblockedyx) ||
                        (sightblockedyx && sightblockedxy))
                    {
                        desc = null;
                        continue;
                    }
                    desc = null;
                }

                list.Add(new UpdatePacket.TileData
                {
                    X = (short) x,
                    Y = (short) y,
                    Tile = (Tile) tile.TileId
                });
                tiles[x, y] = tile.UpdateCount;
                sent++;
            }
            fames.TileSent(sent);

            int[] dropEntities = GetRemovedEntities().Distinct().ToArray();
            _clientEntities.RemoveWhere(_ => Array.IndexOf(dropEntities, _.Id) != -1);

            //purge unused entities
            List<Entity> toRemove = new List<Entity>();
            foreach (Entity i in _lastUpdate.Keys.Where(i => !_clientEntities.Contains(i)))
                toRemove.Add(i);
            toRemove.ForEach(i => _lastUpdate.Remove(i));

            foreach (Entity i in sendEntities)
            {
                _lastUpdate[i] = i.UpdateCount;
            }

            ObjectDef[] newStatics = GetNewStatics(_x, _y).ToArray();
            IntPoint[] removeStatics = GetRemovedStatics(_x, _y).ToArray();
            var removedIds = new List<int>();
            foreach (IntPoint i in removeStatics)
            {
                removedIds.Add(Owner.Map[i.X, i.Y].ObjId);
                _clientStatic.Remove(i);
            }

            if (sendEntities.Count > 0 || list.Count > 0 || dropEntities.Length > 0 ||
                newStatics.Length > 0 || removedIds.Count > 0)
            {
                var packet = new UpdatePacket
                {
                    Tiles = list.ToArray(),
                    NewObjects = sendEntities.Select(_ => _.ToDefinition()).Concat(newStatics).ToArray(),
                    RemovedObjectIds = dropEntities.Concat(removedIds).ToArray()
                };
                psr.SendPacket(packet);
            }
        }

        private void SendNewTick(RealmTime time)
        {
            var sendEntities = new List<Entity>();
            try
            {
                foreach (Entity i in _clientEntities.Where(i => i.UpdateCount > _lastUpdate[i]))
                {
                    sendEntities.Add(i);
                    _lastUpdate[i] = i.UpdateCount;
                }
            }

            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("Crash halted - Nobody likes death...");
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (questEntity != null &&
                (!_lastUpdate.ContainsKey(questEntity) || questEntity.UpdateCount > _lastUpdate[questEntity]))
            {
                sendEntities.Add(questEntity);
                _lastUpdate[questEntity] = questEntity.UpdateCount;
            }
            var p = new NewTickPacket();
            _tickId++;
            p.TickId = _tickId;
            p.TickTime = time.thisTickTimes;
            p.UpdateStatuses = sendEntities.Select(_ => _.ExportStats()).ToArray();
            psr.SendPacket(p);
        }
    }
}