#region

using System;
using System.Linq;
using wServer.realm;
using wServer.svrPackets;

#endregion

namespace wServer.logic.cond
{
    internal class ShamblingSpawn : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            object obj;
            SpawnState state = null;
            if (!Host.StateStorage.TryGetValue(Key, out obj) ||
                (state = (SpawnState) obj).remainingTick <= 0)
            {
                Entity[] originalChildren = null;
                if (state != null)
                    originalChildren = state.children;
                state = new SpawnState
                {
                    remainingTick = 10000,
                    children = new Entity[5]
                };

                var angleInc = (2*Math.PI)/5;
                for (var i = 0; i < 5; i++)
                {
                    var entity = Entity.Resolve(0x0951);
                    entity.Move(
                        Host.Self.X + (float) Math.Cos(angleInc*i)*4,
                        Host.Self.Y + (float) Math.Sin(angleInc*i)*4);
                    state.children[i] = entity;
                    Host.Self.Owner.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.Throw,
                        Color = new ARGB(0xffffbf00),
                        TargetId = Host.Self.Id,
                        PosA = new Position
                        {
                            X = entity.X,
                            Y = entity.Y
                        }
                    }, null);
                }
                Host.StateStorage[Key] = state;
                Host.Self.Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                {
                    if (originalChildren != null)
                        foreach (var i in originalChildren)
                        {
                            if (i.Owner != null)
                                i.Owner.LeaveWorld(i);
                        }
                    foreach (var i in state.children)
                        world.EnterWorld(i);
                }));
                return false;
            }
            state.remainingTick -= time.thisTickTimes;
            if (state.remainingTick <= 0 ||
                (state.remainingTick < 8000 && state.children.Sum(_ => _.Owner == null ? 0 : 1) < 3))
                state.remainingTick = 0;
            return false;
        }

        private class SpawnState
        {
            public Entity[] children;
            public int remainingTick;
        }
    }
}