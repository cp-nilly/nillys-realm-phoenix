#region

using System;
using wServer.logic.attack;
using wServer.logic.loot;
using wServer.logic.movement;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ SnakePit = Behav()
            .Init(0x0917, Behaves("Stheno the Snake Queen",
                new RunBehaviors(
                    If.Instance(EntityLesserThan.Instance(20, 4, 0x0918),
                        Cooldown.Instance(500, SpawnMinionImmediate.Instance(0x0918, 1, 1, 1))),
                    Once.Instance(If.Instance(IsEntityPresent.Instance(12, null), new SetKey(-1, 1))),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Cooldown.Instance(1000,
                                MultiAttack.Instance(15, 10*(float) Math.PI/180, 2, 135*(float) Math.PI/180,
                                    projectileIndex: 0)),
                            Cooldown.Instance(1000,
                                MultiAttack.Instance(15, 10*(float) Math.PI/180, 2, 225*(float) Math.PI/180,
                                    projectileIndex: 0)),
                            Cooldown.Instance(1000,
                                MultiAttack.Instance(15, 10*(float) Math.PI/180, 2, 45*(float) Math.PI/180,
                                    projectileIndex: 0)),
                            Cooldown.Instance(1000,
                                MultiAttack.Instance(15, 10*(float) Math.PI/180, 2, 315*(float) Math.PI/180,
                                    projectileIndex: 0)),
                            new QueuedBehavior(
                                Cooldown.Instance(2000, SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                Cooldown.Instance(4000, UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))
                                ),
                            new QueuedBehavior(
                                CooldownExact.Instance(10000),
                                new SetKey(-1, 2)
                                )
                            )),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            If.Instance(EntityLesserThan.Instance(10, 8, 0x0919),
                                Cooldown.Instance(100, SpawnMinionImmediate.Instance(0x0919, 3, 1, 1))),
                            SmoothWandering.Instance(2f, 3f),
                            Cooldown.Instance(1000, RingAttack.Instance(4, 12, 0, 0)),
                            new QueuedBehavior(
                                Cooldown.Instance(2000, SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                Cooldown.Instance(4000, UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                            new QueuedBehavior(
                                CooldownExact.Instance(15000),
                                OrderAllEntity.Instance(20, 0x0919, Despawn.Instance),
                                new SetKey(-1, 3)
                                ))),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Cooldown.Instance(500, RingAttack.Instance(15, 4, 0, 1)),
                            Cooldown.Instance(1000, ThrowAttack.Instance(4, 8, 100)),
                            new QueuedBehavior(
                                Cooldown.Instance(4000, SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                Cooldown.Instance(4000, UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                            new QueuedBehavior(
                                CooldownExact.Instance(10000),
                                new SetKey(-1, 1)
                                )))),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 2, 2, 8,
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Wand of the Bulwark")),
                        Tuple.Create(0.045, (ILoot) new ItemLoot("Snake Skin Armor")),
                        Tuple.Create(0.045, (ILoot) new ItemLoot("Snake Skin Shield")),
                        Tuple.Create(0.045, (ILoot) new ItemLoot("Snake Eye Ring")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(1.0, (ILoot) new StatPotionLoot(StatPotion.Spd)),
                        Tuple.Create(0.05, (ILoot) new StatPotionLoot(StatPotion.Def)),
                        Tuple.Create(0.25, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.35, (ILoot) new TierLoot(8, ItemType.Armor)),
                        Tuple.Create(0.25, (ILoot) new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(10, ItemType.Armor))
                        )))
                ))
            .Init(0x0e26, Behaves("Snakepit Guard",
                SetSize.Instance(100),
                SmoothWandering.Instance(2f, 2f),
                new RunBehaviors(
                    Cooldown.Instance(2000,
                        MultiAttack.Instance(15, 10*(float) Math.PI/180, 1, 120*(float) Math.PI/180, 2)),
                    Cooldown.Instance(2000,
                        MultiAttack.Instance(15, 10*(float) Math.PI/180, 1, 240*(float) Math.PI/180, 2)),
                    Cooldown.Instance(2000,
                        MultiAttack.Instance(15, 10*(float) Math.PI/180, 1, 360*(float) Math.PI/180, 2)),
                    Cooldown.Instance(500, MultiAttack.Instance(100, 30*(float) Math.PI/180, 3, 0, projectileIndex: 0)),
                    Cooldown.Instance(1000, RingAttack.Instance(6, 10, 0, 1))
                    ), new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 2, 2, 8,
                            Tuple.Create(0.003, (ILoot) new ItemLoot("Wand of the Bulwark")),
                            Tuple.Create(0.04, (ILoot) new ItemLoot("Snake Skin Armor")),
                            Tuple.Create(0.04, (ILoot) new ItemLoot("Snake Skin Shield")),
                            Tuple.Create(0.04, (ILoot) new ItemLoot("Snake Eye Ring")),
                            Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                            Tuple.Create(0.003, (ILoot) new ItemLoot("Wine Cellar Incantation")),
                            Tuple.Create(1.0, (ILoot) new StatPotionLoot(StatPotion.Spd)),
                            Tuple.Create(0.001, (ILoot) new StatPotionLoot(StatPotion.Def)),
                            Tuple.Create(0.25, (ILoot) new TierLoot(9, ItemType.Weapon)),
                            Tuple.Create(0.15, (ILoot) new TierLoot(10, ItemType.Weapon)),
                            Tuple.Create(0.35, (ILoot) new TierLoot(8, ItemType.Armor)),
                            Tuple.Create(0.25, (ILoot) new TierLoot(9, ItemType.Armor)),
                            Tuple.Create(0.15, (ILoot) new TierLoot(10, ItemType.Armor))
                            )))
                ))
            .Init(0x0918, Behaves("Stheno Pet",
                IfNot.Instance(
                    Circling.Instance(14, 20, 18, 0x0917),
                    SimpleWandering.Instance(4)
                    ),
                Chasing.Instance(8, 3, 3, null),
                Cooldown.Instance(1800, SimpleAttack.Instance(20, projectileIndex: 0))
                ))
            .Init(0x0919, Behaves("Stheno Swarm",
                IfNot.Instance(
                    Chasing.Instance(4, 100, 2, 0x0917),
                    SimpleWandering.Instance(4)
                    ),
                If.Instance(IsEntityNotPresent.Instance(10, 0x0917), Despawn.Instance),
                Cooldown.Instance(1000, SimpleAttack.Instance(20, projectileIndex: 0))
                ))
            .Init(0x0223, Behaves("Pit Snake",
                SimpleWandering.Instance(8),
                Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0))
                ))
            .Init(0x0224, Behaves("Pit Viper",
                SimpleWandering.Instance(9),
                Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0))
                ))
            .Init(0x0227, Behaves("Yellow Python",
                IfNot.Instance(
                    Chasing.Instance(10, 10, 1, null),
                    SimpleWandering.Instance(8)
                    ),
                Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0)
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Snake Oil")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Ring of Speed")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Ring of Vitality"))
                        ))
                ))
            .Init(0x0226, Behaves("Brown Python",
                SimpleWandering.Instance(5),
                Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0)
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Snake Oil")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Leather Armor")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Ring of Wisdom"))
                        ))
                ))
            .Init(0x0225, Behaves("Fire Python",
                IfNot.Instance(
                    Cooldown.Instance(2000, Charge.Instance(10, 10 /*radius*/, null)),
                    SimpleWandering.Instance(8)
                    ),
                Cooldown.Instance(1000, MultiAttack.Instance(100, 1*(float) Math.PI/30, 3, 0, projectileIndex: 0)
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Snake Oil")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Fire Bow")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Fire Nova Spell"))
                        ))
                ))
            .Init(0x0228, Behaves("Greater Pit Snake",
                IfNot.Instance(
                    Chasing.Instance(10, 10, 5, null),
                    SimpleWandering.Instance(8)
                    ),
                Cooldown.Instance(1000, MultiAttack.Instance(100, 1*(float) Math.PI/30, 3, 0, projectileIndex: 0)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Snake Oil")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Glass Sword")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Avenger Staff")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Wand of Dark Magic"))
                        )))
                ))
            .Init(0x0229, Behaves("Greater Pit Viper",
                IfNot.Instance(
                    Chasing.Instance(10, 10, 5, null),
                    SimpleWandering.Instance(8)
                    ),
                Cooldown.Instance(300, SimpleAttack.Instance(10, projectileIndex: 0)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Snake Oil")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Ring of Greater Attack")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Ring of Greater Health"))
                        )))
                ));
    }
}