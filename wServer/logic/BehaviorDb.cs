#region

using System;
using System.Collections.Generic;
using wServer.logic.loot;
using wServer.realm;

#endregion

namespace wServer.logic
{
    #region

    using BehaviorDef = Tuple<Behavior, Behavior, Behavior, ConditionalBehavior[]>;

    #endregion

    partial class BehaviorDb
    {
        public static Dictionary<short, BehaviorDef> Behaviors;

        public static void ResolveBehavior(Entity entity)
        {
            BehaviorDef b;
            if (Behaviors.TryGetValue(entity.ObjectType, out b))
            {
                entity.MovementBehavior = b.Item1;
                entity.AttackBehavior = b.Item2;
                entity.ReproduceBehavior = b.Item3;
                entity.CondBehaviors = b.Item4;
            }
            else
            {
                entity.MovementBehavior =
                    entity.AttackBehavior =
                        entity.ReproduceBehavior = NullBehavior.Instance;
                entity.CondBehaviors = Empty<ConditionalBehavior>.Array;
            }
        }

        //Candies
        private static BehaviorDef Behaves(
            string name,
            Behavior movement = null,
            Behavior attack = null,
            Behavior reproduce = null,
            LootBehavior loot = null,
            params ConditionalBehavior[] condBehaviors)
        {
            if (loot != null)
            {
                Array.Resize(ref condBehaviors, condBehaviors.Length + 1);
                condBehaviors[condBehaviors.Length - 1] = loot;
            }

            return new BehaviorDef(
                movement ?? NullBehavior.Instance,
                attack ?? NullBehavior.Instance,
                reproduce ?? NullBehavior.Instance,
                condBehaviors);
        }

        private static _ Behav()
        {
            if (Behaviors == null)
                Behaviors = new Dictionary<short, BehaviorDef>();
            return new _();
        }

        private struct _
        {
            public _ Init(short objType, BehaviorDef b)
            {
                try
                {
                    Behaviors.Add(objType, b);
                    return this;
                }
                catch
                {
                    Console.WriteLine("Behavior Error" + objType);
                }
                return this;
            }

            public _ InitMany(short objTypeMin, short objTypeMax, Func<int, BehaviorDef> b)
            {
                int count = objTypeMax - objTypeMin;
                for (int i = 0; i <= count; i++)
                {
                    Behaviors.Add((short) (objTypeMin + i), b(objTypeMin + i));
                }
                return this;
            }
        }
    }
}