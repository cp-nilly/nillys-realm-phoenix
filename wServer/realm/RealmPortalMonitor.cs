#region

using System;
using System.Collections.Generic;
using System.Linq;
using wServer.realm.entities;
using wServer.realm.worlds;

#endregion

namespace wServer.realm
{
    public class RealmPortalMonitor
    {
        private readonly Nexus nexus;
        private readonly Random rand = new Random();
        private readonly object worldLock = new object();
        public Dictionary<World, Portal> portals = new Dictionary<World, Portal>();

        public RealmPortalMonitor(Nexus nexus)
        {
            this.nexus = nexus;
            lock (worldLock)
                foreach (var i in RealmManager.Worlds)
                {
                    if (i.Value is GameWorld)
                        WorldAdded(i.Value);
                }
        }

        private Position GetRandPosition()
        {
            int x, y;
            do
            {
                x = rand.Next(0, nexus.Map.Width);
                y = rand.Next(0, nexus.Map.Height);
            } while (
                portals.Values.Any(_ => _.X == x && _.Y == y) ||
                nexus.Map[x, y].Region != TileRegion.Realm_Portals);
            return new Position {X = x, Y = y};
        }

        public void WorldAdded(World world)
        {
            //lock (worldLock)
            //{
            var pos = GetRandPosition();
            var portal = new Portal(0x0712, null)
            {
                Size = 80,
                WorldInstance = world,
                Name = world.Name
            };
            portal.Move(pos.X + 0.5f, pos.Y + 0.5f);
            nexus.EnterWorld(portal);
            portals.Add(world, portal);
            //}
        }

        public void WorldRemoved(World world)
        {
            //lock (worldLock)
            //{
            var portal = portals[world];
            nexus.LeaveWorld(portal);
            portals.Remove(world);
            
            //}
        }

        public void WorldClosed(World world)
        {
            lock (worldLock)
            {
                var portal = portals[world];
                nexus.LeaveWorld(portal);
                portals.Remove(world);
            }
        }

        public void WorldOpened(World world)
        {
            lock (worldLock)
            {
                var pos = GetRandPosition();
                var portal = new Portal(0x71c, null)
                {
                    Size = 150,
                    WorldInstance = world,
                    Name = world.Name
                };
                portal.Move(pos.X, pos.Y);
                nexus.EnterWorld(portal);
                portals.Add(world, portal);
            }
        }

        public World GetRandomRealm()
        {
            lock (worldLock)
            {
                var worlds = portals.Keys.ToArray();
                if (worlds.Length == 0)
                    return RealmManager.Worlds[World.NEXUS_ID];
                return worlds[Environment.TickCount%worlds.Length];
            }
        }
    }
}