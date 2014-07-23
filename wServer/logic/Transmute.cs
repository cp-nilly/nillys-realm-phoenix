#region

using System;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    internal class Transmute : Behavior
    {
        private readonly int maxCount;
        private readonly int minCount;
        private readonly short objType;

        private readonly Random rand = new Random();

        public Transmute(short objType, int minCount = 1, int maxCount = 1)
        {
            this.objType = objType;
            this.minCount = minCount;
            this.maxCount = maxCount;
        }

        protected override bool TickCore(RealmTime time)
        {
            var c = rand.Next(minCount, maxCount + 1);
            var parent = Host as Entity;
            for (var i = 0; i < c; i++)
            {
                var entity = Entity.Resolve(objType);
                entity.Move(parent.X, parent.Y);
                (entity as Enemy).Terrain = (Host as Enemy).Terrain;
                parent.Owner.EnterWorld(entity);
            }
            parent.Owner.LeaveWorld(parent);
            return true;
        }
    }
}