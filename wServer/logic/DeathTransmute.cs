#region

using System;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic
{
    internal class DeathTransmute : ConditionalBehavior
    {
        private readonly int maxCount;
        private readonly int minCount;
        private readonly short objType;
        private readonly Random rand = new Random();

        public DeathTransmute(short objType, int minCount = 1, int maxCount = 1)
        {
            this.objType = objType;
            this.minCount = minCount;
            this.maxCount = maxCount;
        }

        public override BehaviorCondition Condition
        {
            get { return BehaviorCondition.OnDeath; }
        }

        protected override void BehaveCore(BehaviorCondition cond, RealmTime? time, object state)
        {
            var c = rand.Next(minCount, maxCount + 1);
            for (var i = 0; i < c; i++)
            {
                var entity = Entity.Resolve(objType);
                var parent = Host as Entity;
                entity.Move(parent.X, parent.Y);
                (entity as Enemy).Terrain = (Host as Enemy).Terrain;
                parent.Owner.EnterWorld(entity);
            }
        }
    }

    internal class DeathPortal : ConditionalBehavior
    {
        private readonly short objType;
        private readonly int percent;
        private readonly int timeExist;

        public DeathPortal(short objType, int percent, int timeExist)
        {
            this.objType = objType;
            this.percent = percent;
            this.timeExist = timeExist;
        }

        public override BehaviorCondition Condition
        {
            get { return BehaviorCondition.OnDeath; }
        }

        protected override void BehaveCore(BehaviorCondition cond, RealmTime? time, object state)
        {
            if (Host.Self.Owner.Name != "Battle Arena" && Host.Self.Owner.Name != "Free Battle Arena" && Host.Self.Owner.Name != "Arena" && Host.Self.Owner.Name != "Nexus")
            {
                if (new Random().Next(1, 100) <= percent)
                {
                    var entity = Entity.Resolve(objType) as Portal;
                    var parent = Host as Entity;
                    entity.Move(parent.X, parent.Y);
                    parent.Owner.EnterWorld(entity);
                    var w = RealmManager.GetWorld(Host.Self.Owner.Id);
                    w.Timers.Add(new WorldTimer(timeExist*1000, (world, t) =>
                    {
                        if (timeExist > 0)
                            try
                            {
                                w.LeaveWorld(entity);
                            }

                            catch
                            {
                            }
                    }));
                }
            }
        }
    }

    internal class Corpse : ConditionalBehavior
    {
        private readonly short objType;

        public Corpse(short objType)
        {
            this.objType = objType;
        }

        public override BehaviorCondition Condition
        {
            get { return BehaviorCondition.OnDeath; }
        }

        protected override void BehaveCore(BehaviorCondition cond, RealmTime? time, object state)
        {
            var entity = Entity.Resolve(objType) as Enemy;
            var parent = Host as Enemy;
            entity.Move(parent.X, parent.Y);
            entity.Terrain = (Host as Enemy).Terrain;
            parent.DamageCounter.Corpse = entity.DamageCounter;
            parent.Owner.EnterWorld(entity);
        }
    }
}