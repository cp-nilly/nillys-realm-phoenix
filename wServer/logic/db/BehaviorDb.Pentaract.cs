#region

using System;
using wServer.logic.attack;
using wServer.logic.cond;
using wServer.logic.loot;
using wServer.logic.movement;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Pentaract = Behav()
            .Init(0x0d5f, Behaves("Pentaract",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 0)),
                    IfEqual.Instance(-1, 0,
                        new RunBehaviors(
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                            CooldownExact.Instance(250, new PentaractStar()),
                            If.Instance(EntityLesserThan.Instance(50, 4, 0x0d5e), new SetKey(-1, 1))
                            )),
                    IfEqual.Instance(-1, 1, CooldownExact.Instance(250, new PentaractStar())),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            CooldownExact.Instance(17000),
                            OrderAllEntity.Instance(30, 0x0d60, new Transmute(0x0d5e)),
                            CooldownExact.Instance(500),
                            new SetKey(-1, 0),
                            CooldownExact.Instance(-1)
                            ))
                    )
                ))
            .Init(0x0d5e, Behaves("Pentaract Tower",
                Cooldown.Instance(5000, ThrowAttack.Instance(4, 8, 100)),
                Once.Instance(SpawnMinionImmediate.Instance(0x0d5d, 1, 5, 5)),
                condBehaviors: new ConditionalBehavior[]
                {
                    new Corpse(0x0d60)
                }
                ))
            .Init(0x0d60, Behaves("Pentaract Tower Corpse",
                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(1, new LootDef(0, 3, 0, 8,
                        Tuple.Create(0.002, (ILoot) new ItemLoot("Seal of Blasphemous Prayer")),
                        Tuple.Create(0.005, (ILoot) new TierLoot(11, ItemType.Weapon)),
                        Tuple.Create(0.005, (ILoot) new TierLoot(11, ItemType.Armor)),
                        Tuple.Create(0.005, (ILoot) new TierLoot(5, ItemType.Ring)),
                        Tuple.Create(0.01, (ILoot) new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.01, (ILoot) new TierLoot(10, ItemType.Armor)),
                        Tuple.Create(0.02, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.02, (ILoot) new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.02, (ILoot) new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.5, (ILoot) new StatPotionsLoot(1, 2)),
                        Tuple.Create(0.03, (ILoot) new TierLoot(4, ItemType.Ring)),
                        Tuple.Create(0.05, (ILoot) new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.05, (ILoot) new TierLoot(8, ItemType.Armor)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(8, ItemType.Weapon)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(7, ItemType.Armor)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(3, ItemType.Ring))
                        ))
                    )
                ))
            .Init(0x0d5d, Behaves("Pentaract Eye",
                Swirling.Instance(20, 20),
                Cooldown.Instance(200, SimpleAttack.Instance(10))
                ));
    }
}