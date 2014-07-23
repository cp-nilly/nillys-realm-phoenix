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
        private static _ Spriteworld = Behav()
            .Init(0x0d06, Behaves("Limon the Sprite God",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            Flashing.Instance(500, 0xff00ff00),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            Cooldown.Instance(2000, If.Instance(IsEntityPresent.Instance(11, null),
                                new SetKey(-1, 2))
                                ))),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Flashing.Instance(500, 0xff00ff00),
                            SimpleWandering.Instance(3, 1f),
                            new QueuedBehavior(
                                Cooldown.Instance(5000),
                                new SetKey(-1, 3)
                                ))),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Cooldown.Instance(1000, RingAttack.Instance(2, 0, 90*(float) Math.PI/180, 0)),
                            If.Instance(IsEntityPresent.Instance(10, null),
                                Cooldown.Instance(1000, MultiAttack.Instance(10, 10*(float) Math.PI/180, 2, 0, 0)),
                                Cooldown.Instance(1000, RingAttack.Instance(20, 0, 0, 0))),
                            If.Instance(IsEntityPresent.Instance(10, null),
                                Cooldown.Instance(1000, RingAttack.Instance(2, 0, 90*(float) Math.PI/180, 0))),
                            new QueuedBehavior(
                                CooldownExact.Instance(10000, new SetKey(-1, 4))),
                            new QueuedBehavior(
                                Charge.Instance(20, 10, null),
                                Timed.Instance(1000, SimpleWandering.Instance(2, 2f))),
                            new QueuedBehavior(
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                Cooldown.Instance(3000),
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                Cooldown.Instance(3000),
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                Cooldown.Instance(3000),
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                Cooldown.Instance(4000)
                                ))),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            new QueuedBehavior(
                                TossEnemy.Instance(315*(float) Math.PI/180, 9, 0x0d08),
                                TossEnemy.Instance(45*(float) Math.PI/180, 9, 0x0d07),
                                TossEnemy.Instance(135*(float) Math.PI/180, 9, 0x0d09),
                                TossEnemy.Instance(225*(float) Math.PI/180, 9, 0x0d0a),
                                TossEnemy.Instance(315*(float) Math.PI/180, 14, 0x0d08),
                                TossEnemy.Instance(45*(float) Math.PI/180, 14, 0x0d07),
                                TossEnemy.Instance(135*(float) Math.PI/180, 14, 0x0d09),
                                TossEnemy.Instance(225*(float) Math.PI/180, 14, 0x0d0a),
                                new SetKey(-1, 5)
                                ))),
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            new QueuedBehavior(
                                CooldownExact.Instance(10000),
                                OrderGroup.Instance(30, "AttackType", new SetKey(-1, 2)),
                                CooldownExact.Instance(8000),
                                OrderGroup.Instance(30, "AttackType", new SetKey(-1, 1))
                                ),
                            new QueuedBehavior(
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(375, RingAttack.Instance(3, 7, 0, 0)),
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                Cooldown.Instance(500, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(500, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(500, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(500, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(500, RingAttack.Instance(3, 7, 0, 0)),
                                Cooldown.Instance(500, RingAttack.Instance(3, 7, 0, 0))
                                ),
                            new QueuedBehavior(
                                CooldownExact.Instance(20000),
                                new SetKey(-1, 6)
                                )
                            )),
                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            SimpleWandering.Instance(4, 2f),
                            Flashing.Instance(500, 0xff00ff00),
                            new QueuedBehavior(
                                SpawnMinionImmediate.Instance(0x0d01, 1, 1, 2),
                                SpawnMinionImmediate.Instance(0x0d02, 1, 1, 2),
                                CooldownExact.Instance(3000),
                                HpLesserPercent.Instance(0.5f, new SetKey(-1, 1), new SetKey(-1, 7))),
                            new QueuedBehavior(
                                CooldownExact.Instance(2000),
                                OrderGroup.Instance(30, "AttackType", Despawn.Instance)
                                ))),
                    IfEqual.Instance(-1, 7,
                        new RunBehaviors(
                            SimpleWandering.Instance(4, 1f),
                            Cooldown.Instance(1000, MultiAttack.Instance(10, 10*(float) Math.PI/180, 3, 0, 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(10000, new SetKey(-1, 1)))
                            ))
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new DeathPortal(0x070e, 100, -1)
                },
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 1, 0, 1,
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Staff of Extreme Prejudice")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Sprite Wand")),
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Cloak of the Planewalker")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot)new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.4, (ILoot) new TierLoot(3, ItemType.Ring)),
                        Tuple.Create(0.4, (ILoot) new TierLoot(6, ItemType.Armor)),
                        Tuple.Create(0.4, (ILoot) new TierLoot(3, ItemType.Ability)),
                        Tuple.Create(0.3, (ILoot) new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.2, (ILoot) new TierLoot(5, ItemType.Ability))
                        )),
                    Tuple.Create(100, new LootDef(1, 1, 1, 2,
                        Tuple.Create(0.99, (ILoot) new StatPotionLoot(StatPotion.Dex)),
                        Tuple.Create(0.045, (ILoot) new StatPotionLoot(StatPotion.Def))
                        )))))
            .Init(0x0d07, Behaves("Limon Element 1",
                new RunBehaviors(
                    If.Instance(IsEntityNotPresent.Instance(30, 0x0d06), Despawn.Instance),
                    SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(270*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(270*(float) Math.PI/180, 0),
                                new QueuedBehavior(
                                    AngleAttack.Instance(180*(float) Math.PI/180, 0),
                                    Cooldown.Instance(250),
                                    AngleAttack.Instance(180*(float) Math.PI/180, 0)
                                    )))),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(270*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(270*(float) Math.PI/180, 0)),
                            new QueuedBehavior(
                                AngleAttack.Instance(180*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(180*(float) Math.PI/180, 0)),
                            new QueuedBehavior(
                                AngleAttack.Instance(225*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(225*(float) Math.PI/180, 0)
                                ))
                        ))))
            .Init(0x0d08, Behaves("Limon Element 2",
                new RunBehaviors(
                    If.Instance(IsEntityNotPresent.Instance(30, 0x0d06), Despawn.Instance),
                    SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(90*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(90*(float) Math.PI/180, 0)
                                ),
                            new QueuedBehavior(
                                AngleAttack.Instance(180*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(180*(float) Math.PI/180, 0)
                                ))),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(90*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(90*(float) Math.PI/180, 0)),
                            new QueuedBehavior(
                                AngleAttack.Instance(180*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(180*(float) Math.PI/180, 0)
                                ),
                            new QueuedBehavior(
                                AngleAttack.Instance(135*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(135*(float) Math.PI/180, 0)
                                ))
                        ))))
            .Init(0x0d09, Behaves("Limon Element 3",
                new RunBehaviors(
                    If.Instance(IsEntityNotPresent.Instance(30, 0x0d06), Despawn.Instance),
                    SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(0*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(0*(float) Math.PI/180, 0)
                                ),
                            new QueuedBehavior(
                                AngleAttack.Instance(270*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(270*(float) Math.PI/180, 0)
                                ))),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(0*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(0*(float) Math.PI/180, 0)),
                            new QueuedBehavior(
                                AngleAttack.Instance(270*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(270*(float) Math.PI/180, 0)
                                ),
                            new QueuedBehavior(
                                AngleAttack.Instance(315*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(315*(float) Math.PI/180, 0)
                                ))
                        ))))
            .Init(0x0d0a, Behaves("Limon Element 4",
                new RunBehaviors(
                    If.Instance(IsEntityNotPresent.Instance(30, 0x0d06), Despawn.Instance),
                    SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(0*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(0*(float) Math.PI/180, 0)
                                ),
                            new QueuedBehavior(
                                AngleAttack.Instance(90*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(90*(float) Math.PI/180, 0)
                                ))),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            new QueuedBehavior(
                                AngleAttack.Instance(0*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(0*(float) Math.PI/180, 0)),
                            new QueuedBehavior(
                                AngleAttack.Instance(90*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(90*(float) Math.PI/180, 0)
                                ),
                            new QueuedBehavior(
                                AngleAttack.Instance(45*(float) Math.PI/180, 0),
                                Cooldown.Instance(250),
                                AngleAttack.Instance(45*(float) Math.PI/180, 0)
                                ))
                        ))))
            .Init(0x0d00, Behaves("Native Fire Sprite",
                SimpleWandering.Instance(15),
                Cooldown.Instance(300, MultiAttack.Instance(10, 7*(float) Math.PI/180, 2)),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.03, (ILoot) new TierLoot(5, ItemType.Weapon)),
                        Tuple.Create(0.03, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Sprite Wand"))
                        )
                    )
                ))
            .Init(0x0d01, Behaves("Native Ice Sprite",
                SimpleWandering.Instance(15),
                Cooldown.Instance(1000, MultiAttack.Instance(10, 7*(float) Math.PI/180, 3)),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.03, (ILoot) new TierLoot(2, ItemType.Ability)),
                        Tuple.Create(0.03, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Sprite Wand"))
                        )
                    )
                ))
            .Init(0x0d02, Behaves("Native Magic Sprite",
                SimpleWandering.Instance(15),
                Cooldown.Instance(1000, MultiAttack.Instance(10, 7*(float) Math.PI/180, 4)),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.03, (ILoot) new TierLoot(5, ItemType.Armor)),
                        Tuple.Create(0.03, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Sprite Wand"))
                        )
                    )
                ))
            .Init(0x0d03, Behaves("Native Nature Sprite",
                SimpleWandering.Instance(15),
                Cooldown.Instance(1000, MultiAttack.Instance(10, 7*(float) Math.PI/180, 5)),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.03, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Sprite Wand"))
                        )
                    )
                ))
            .Init(0x0d04, Behaves("Native Darkness Sprite",
                SimpleWandering.Instance(15),
                Cooldown.Instance(1000, MultiAttack.Instance(10, 7*(float) Math.PI/180, 5)),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.3, (ILoot) new ItemLoot("Ring of Dexterity")),
                        Tuple.Create(0.03, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Sprite Wand"))
                        )
                    )
                ))
            .Init(0x0d05, Behaves("Native Sprite God",
                SimpleWandering.Instance(2),
                new RunBehaviors(
                    Cooldown.Instance(1000, PredictiveMultiAttack.Instance(12, 10*(float) Math.PI/180, 4, 1, 0)),
                    Cooldown.Instance(1000, PredictiveAttack.Instance(10, 1, 1))
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(1, CommonGodSoulBag),
                    Tuple.Create(360, new LootDef(0, 1, 0, 8,
                        Tuple.Create(PotProbability, (ILoot) new StatPotionLoot(StatPotion.Att))
                        ))
                    )
                ))
            ;
    }
}