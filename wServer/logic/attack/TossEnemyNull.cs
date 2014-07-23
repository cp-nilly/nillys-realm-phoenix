#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class TossEnemyNull : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, short>, TossEnemyNull> instances =
            new Dictionary<Tuple<float, float, short>, TossEnemyNull>();

        private readonly float angle;
        private readonly short objType;
        private readonly float range;
        private Random rand = new Random();

        private TossEnemyNull(float angle, float range, short objType)
        {
            this.angle = angle;
            this.range = range;
            this.objType = objType;
        }

        public static TossEnemyNull Instance(float angle, float range, short objType)
        {
            var key = new Tuple<float, float, short>(angle, range, objType);
            TossEnemyNull ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new TossEnemyNull(angle, range, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var chr = Host as Character;
            var target = new Position
            {
                X = Host.Self.X,
                Y = Host.Self.Y
            };
            target.X += (float) Math.Cos(angle)*range;
            target.Y += (float) Math.Sin(angle)*range;
            chr.Owner.BroadcastPacket(new ShowEffectPacket
            {
                TargetId = Host.Self.Id,
                PosA = target
            }, null);
            chr.Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
            {
                var entity = Entity.Resolve(objType);
                entity.Move(target.X, target.Y);
                (entity as Enemy).Terrain = (chr as Enemy).Terrain;
                world.EnterWorld(entity);
            }));

            return true;
        }
    }
}