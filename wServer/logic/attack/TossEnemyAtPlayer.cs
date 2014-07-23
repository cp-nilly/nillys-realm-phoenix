#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class TossEnemyAtPlayer : Behavior
    {
        private static readonly Dictionary<Tuple<float, short>, TossEnemyAtPlayer> instances =
            new Dictionary<Tuple<float, short>, TossEnemyAtPlayer>();

        private readonly short objType;
        private readonly float range;
        private Random rand = new Random();

        private TossEnemyAtPlayer(float range, short objType)
        {
            this.range = range;
            this.objType = objType;
        }

        public static TossEnemyAtPlayer Instance(float range, short objType)
        {
            var key = new Tuple<float, short>(range, objType);
            TossEnemyAtPlayer ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new TossEnemyAtPlayer(range, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var dist = range;
            var player = GetNearestEntity(ref dist, null);
            if (player != null)
            {
                var chr = Host as Character;
                var target = new Position
                {
                    X = player.X,
                    Y = player.Y
                };

                chr.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Throw,
                    Color = new ARGB(0xffffbf00),
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
            }
            return true;
        }
    }
}