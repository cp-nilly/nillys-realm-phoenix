#region

using System;
using wServer.logic.attack;
using wServer.logic.loot;
using wServer.logic.movement;
using wServer.logic.taunt;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Sanctuary = Behav()
            .Init(0x700c, Behaves("Ent Minion",
                Chasing.Instance(5, 15, 1, null),
                SimpleWandering.Instance(4),
                Cooldown.Instance(2500, RingAttack.Instance(10)), new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.05, (ILoot) HpPotionLoot.Instance)
                        )
                    )
                ))
            .Init(0x700e, Behaves("Elder Tree Roots",
                new RunBehaviors(
                    CooldownExact.Instance(100, RingAttack.Instance(8))
                    ),
                new QueuedBehavior(
                    RingAttack.Instance(8, 0, 0, 1),
                    CooldownExact.Instance(75, SetAltTexture.Instance(1)),
                    CooldownExact.Instance(75, SetAltTexture.Instance(2)),
                    CooldownExact.Instance(75, SetAltTexture.Instance(3)),
                    CooldownExact.Instance(3000, SetAltTexture.Instance(2)),
                    CooldownExact.Instance(75, SetAltTexture.Instance(1)),
                    CooldownExact.Instance(75, SetAltTexture.Instance(0)),
                    CooldownExact.Instance(75, Despawn.Instance)
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.1, (ILoot) HpPotionLoot.Instance)
                        )
                    )
                ))
            .Init(0x700d, Behaves("Elder Tree",
                new RunBehaviors(
                    new State("idle",
                        HpLesser.Instance(12500, SetState.Instance("transition1"))
                        ),
                    new State("transition1",
                        Once.Instance(
                            new SimpleTaunt(
                                "I have lived for thousands of years. Prepare to meet the end of your short life!")),
                        StateOnce.Instance(new SetConditionEffectTimed(ConditionEffectIndex.Invulnerable, 2990)),
                        new QueuedBehavior(
                            Flashing.Instance(200, 0xFF00FF00)
                            ),
                        CooldownExact.Instance(3000, SetState.Instance("spawning"))
                        ),
                    new State("spawning",
                        StateOnce.Instance(SpawnMinionImmediate.Instance(0x700c, 5, 10, 15)),
                        If.Instance(EntityLesserThan.Instance(20, 20, 0x700c),
                            SpawnMinion.Instance(0x700c, 5, 1, 700, 1200)
                            ),
                        If.Instance(EntityLesserThan.Instance(50, 1, 0x700c),
                            SetState.Instance("transition2")
                            ),
                        CooldownExact.Instance(3000, MultiAttack.Instance(10, 20*(float) Math.PI/180, 7)),
                        CooldownExact.Instance(12000, SetState.Instance("transition2"))
                        ),
                    new State("transition2",
                        StateOnce.Instance(new SetConditionEffectTimed(ConditionEffectIndex.Invulnerable, 2990)),
                        new QueuedBehavior(
                            Flashing.Instance(200, 0xFF00FF00)
                            ),
                        CooldownExact.Instance(3000, SetState.Instance("roots"))
                        ),
                    new State("roots",
                        Cooldown.Instance(100, SpawnRoot.Instance(0x700e, 10, 1f)),
                        CooldownExact.Instance(2000, MultiAttack.Instance(10, 20*(float) Math.PI/180, 7)),
                        CooldownExact.Instance(16000, SetState.Instance("transition3"))
                        ),
                    new State("transition3",
                        StateOnce.Instance(new SetConditionEffectTimed(ConditionEffectIndex.Invulnerable, 2990)),
                        new QueuedBehavior(
                            Flashing.Instance(200, 0xFF00FF00)
                            ),
                        CooldownExact.Instance(3000, SetState.Instance("attack"))
                        ),
                    new State("attack",
                        new QueuedBehavior(
                            CooldownExact.Instance(1000, MultiAttack.Instance(10, 20*(float) Math.PI/180, 7)),
                            CooldownExact.Instance(1000, RingAttack.Instance(14))
                            ),
                        CooldownExact.Instance(12000, SetState.Instance("transition1"))
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(500, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.8, (ILoot) new ItemLoot("Potion of Speed")),
                        Tuple.Create(0.15, (ILoot) new ItemLoot("Potion of Vitality")),
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Staff of Natural Essence")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot)new ItemLoot("Wine Cellar Incantation"))
                        )))
                ))
            .Init(0x2f08, Behaves("Green Forest Wisp",
                new RunBehaviors(
                    new RunBehaviors(
                        Cooldown.Instance(2000, PredictiveRingAttack.Instance(5, 1, 10, projectileIndex: 0)),
                        Once.Instance(new SetKey(-1, 1))
                        ),
                    IfEqual.Instance(-1, 1,
                        SmoothWandering.Instance(8f, 2)
                        ),
                    IfEqual.Instance(-1, 2,
                        Chasing.Instance(3, 30, 2, null)
                        )
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.5, (ILoot) HpPotionLoot.Instance),
                        Tuple.Create(0.5, (ILoot) MpPotionLoot.Instance)
                        )
                    )
                ))
            .Init(0x2f09, Behaves("Blue Forest Wisp",
                new RunBehaviors(
                    new RunBehaviors(
                        Cooldown.Instance(2000, PredictiveRingAttack.Instance(5, 1, 10, projectileIndex: 0)),
                        Once.Instance(new SetKey(-1, 1))
                        ),
                    IfEqual.Instance(-1, 1,
                        SmoothWandering.Instance(8f, 2)
                        ),
                    IfEqual.Instance(-1, 2,
                        Chasing.Instance(3, 30, 2, null)
                        )
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.5, (ILoot) HpPotionLoot.Instance),
                        Tuple.Create(0.5, (ILoot) MpPotionLoot.Instance)
                        )
                    )
                ))
            .Init(0x2f10, Behaves("Red Forest Wisp",
                new RunBehaviors(
                    new RunBehaviors(
                        Cooldown.Instance(2000, PredictiveRingAttack.Instance(5, 1, 10, projectileIndex: 0)),
                        Once.Instance(new SetKey(-1, 1))
                        ),
                    IfEqual.Instance(-1, 1,
                        SmoothWandering.Instance(8f, 2)
                        ),
                    IfEqual.Instance(-1, 2,
                        Chasing.Instance(3, 30, 2, null)
                        )
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.5, (ILoot) HpPotionLoot.Instance),
                        Tuple.Create(0.5, (ILoot) MpPotionLoot.Instance)
                        )
                    )
                ))
            .Init(0x2f11, Behaves("Green Guardian Wisp",
                new RunBehaviors(
                    Cooldown.Instance(2000,
                        MultiAttack.Instance(15, 40*(float) Math.PI/360, 3, 180*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(1200, PredictiveMultiAttack.Instance(15, 2*(float) Math.PI/360, 4, 5, 1)),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Circling.Instance(2, 10, 5, 0x2f14)
                        ),
                    IfEqual.Instance(-1, 1,
                        Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))
                        ),
                    IfEqual.Instance(-1, 2,
                        Circling.Instance(4, 20, 3, 0x2f14)
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Armored))
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Flashing.Instance(100, 0xffff0000),
                            SmoothWandering.Instance(1, 2)
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new QueuedBehavior(
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                            CooldownExact.Instance(1500),
                            RingAttack.Instance(8, 0, 0, projectileIndex: 0),
                            CooldownExact.Instance(100),
                            RingAttack.Instance(8, 0, 40*(float) Math.PI/180, 1),
                            Despawn.Instance
                            )
                        ),
                    If.Instance(IsEntityNotPresent.Instance(21, 0x2f14),
                        new SetKey(-1, 3)
                        )
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 5,
                        Tuple.Create(0.5, (ILoot) HpPotionLoot.Instance),
                        Tuple.Create(0.5, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Dexterity")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Life")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Mana"))
                        )
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        IfEqual.Instance(-1, 2,
                            new Transmute(0x2f12, 1, 1)
                            )
                        )
                }
                ))
            .Init(0x2f12, Behaves("Blue Guardian Wisp",
                new RunBehaviors(
                    Cooldown.Instance(2000,
                        MultiAttack.Instance(15, 40*(float) Math.PI/360, 3, 180*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(1200, PredictiveMultiAttack.Instance(15, 2*(float) Math.PI/360, 4, 5, 1)),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Circling.Instance(2, 10, 5, 0x2f14)
                        ),
                    IfEqual.Instance(-1, 1,
                        Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))
                        ),
                    IfEqual.Instance(-1, 2,
                        Circling.Instance(4, 20, 3, 0x2f14)
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Armored))
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Flashing.Instance(100, 0xffff0000),
                            SmoothWandering.Instance(1, 2)
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new QueuedBehavior(
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                            CooldownExact.Instance(1500),
                            RingAttack.Instance(8, 0, 0, projectileIndex: 0),
                            CooldownExact.Instance(100),
                            RingAttack.Instance(8, 0, 40*(float) Math.PI/180, 1),
                            Despawn.Instance
                            )
                        ),
                    If.Instance(IsEntityNotPresent.Instance(21, 0x2f14),
                        new SetKey(-1, 3)
                        )
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 5,
                        Tuple.Create(0.5, (ILoot) HpPotionLoot.Instance),
                        Tuple.Create(0.5, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Dexterity")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Life")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Mana"))
                        )
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        IfEqual.Instance(-1, 2,
                            new Transmute(0x2f13, 1, 1)
                            )
                        )
                }
                ))
            .Init(0x2f13, Behaves("Red Guardian Wisp",
                new RunBehaviors(
                    Cooldown.Instance(2000,
                        MultiAttack.Instance(15, 40*(float) Math.PI/360, 3, 180*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(1200, PredictiveMultiAttack.Instance(15, 2*(float) Math.PI/360, 4, 5, 1)),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Circling.Instance(2, 10, 5, 0x2f14)
                        ),
                    IfEqual.Instance(-1, 1,
                        Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))
                        ),
                    IfEqual.Instance(-1, 2,
                        Circling.Instance(4, 20, 3, 0x2f14)
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Armored))
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Flashing.Instance(100, 0xffff0000),
                            SmoothWandering.Instance(1, 2)
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new QueuedBehavior(
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                            CooldownExact.Instance(1500),
                            RingAttack.Instance(8, 0, 0, projectileIndex: 0),
                            CooldownExact.Instance(100),
                            RingAttack.Instance(8, 0, 40*(float) Math.PI/180, 1),
                            Despawn.Instance
                            )
                        ),
                    If.Instance(IsEntityNotPresent.Instance(21, 0x2f14),
                        new SetKey(-1, 3)
                        )
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 2, 0, 5,
                        Tuple.Create(0.5, (ILoot) HpPotionLoot.Instance),
                        Tuple.Create(0.5, (ILoot) MpPotionLoot.Instance),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Dexterity")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Life")),
                        Tuple.Create(0.1, (ILoot) new ItemLoot("Tincture of Mana"))
                        )
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        IfEqual.Instance(-1, 2,
                            new Transmute(0x2f11, 1, 1)
                            )
                        )
                }
                ))
            .Init(0x2f14, Behaves("Wisp Queen",
                new RunBehaviors(
                    MagicEye.Instance,
                    If.Instance(IsEntityPresent.Instance(10, null),
                        Once.Instance(new SetKey(-1, 1))
                        ),
                    IfEqual.Instance(-1, 1,
                        Flashing.Instance(200, 0xff0000ff)
                        ),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                            TossEnemy.Instance(0*(float) Math.PI/180, 2, 0x2f11),
                            TossEnemy.Instance(120*(float) Math.PI/180, 2, 0x2f12),
                            TossEnemy.Instance(240*(float) Math.PI/180, 2, 0x2f13),
                            CooldownExact.Instance(1501),
                            TossEnemyAtPlayer.Instance(30, 0x2f08),
                            TossEnemyAtPlayer.Instance(30, 0x2f08),
                            TossEnemyAtPlayer.Instance(30, 0x2f09),
                            TossEnemyAtPlayer.Instance(30, 0x2f09),
                            TossEnemyAtPlayer.Instance(30, 0x2f10),
                            TossEnemyAtPlayer.Instance(30, 0x2f10),
                            CooldownExact.Instance(501),
                            RingAttack.Instance(10, 30, 0, projectileIndex: 0),
                            CooldownExact.Instance(500),
                            RingAttack.Instance(10, 30, 0, 2),
                            CooldownExact.Instance(501),
                            OrderGroup.Instance(15, "Forest Wisps",
                                new SetKey(-1, 2)
                                ),
                            RingAttack.Instance(10, 30, 0, 4),
                            CooldownExact.Instance(500),
                            RingAttack.Instance(3, 0, 0, projectileIndex: 0),
                            RingAttack.Instance(3, 0, 40*(float) Math.PI/360, 2),
                            RingAttack.Instance(3, 0, 80*(float) Math.PI/360, 4),
                            new SetKey(-1, 2)
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new QueuedBehavior(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            SetAltTexture.Instance(1),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, projectileIndex: 0),
                            CooldownExact.Instance(1000),
                            SetAltTexture.Instance(2),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, 2),
                            CooldownExact.Instance(1000),
                            SetAltTexture.Instance(3),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, 4),
                            CooldownExact.Instance(750),
                            SetAltTexture.Instance(1),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, projectileIndex: 0),
                            CooldownExact.Instance(750),
                            SetAltTexture.Instance(2),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, 2),
                            CooldownExact.Instance(600),
                            SetAltTexture.Instance(3),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, 4),
                            CooldownExact.Instance(550),
                            SetAltTexture.Instance(1),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, projectileIndex: 0),
                            CooldownExact.Instance(275),
                            SetAltTexture.Instance(2),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, 2),
                            CooldownExact.Instance(275),
                            SetAltTexture.Instance(3),
                            CooldownExact.Instance(10),
                            MultiAttack.Instance(25, 15*(float) Math.PI/360, 5, 0, 4),
                            CooldownExact.Instance(300),
                            Rand.Instance(
                                new SetKey(-1, 3), //Red Coloration
                                new SetKey(-1, 4), //Blue Coloration
                                new SetKey(-1, 5) //Green Coloration
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        Flashing.Instance(2500, 0xff0000ff)
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            SetAltTexture.Instance(1),
                            Cooldown.Instance(30000,
                                new SimpleTaunt(
                                    "My children have differentiated for my protection. Their segregation makes me invincible!")),
                            OrderGroup.Instance(20, "Guardian Wisps",
                                new SetKey(-1, 2)
                                ),
                            Cooldown.Instance(2000, PredictiveRingAttack.Instance(10, 5, 25, projectileIndex: 0)),
                            Cooldown.Instance(1500, MultiAttack.Instance(25, 10*(float) Math.PI/360, 5, 0, 1)),
                            Cooldown.Instance(25000, SpawnMinionImmediate.Instance(0x2f10, 3, 0, 2)),
                            If.Instance(IsEntityNotPresent.Instance(30, 0x2f11),
                                If.Instance(IsEntityNotPresent.Instance(30, 0x2f13),
                                    new SetKey(-1, 6)
                                    )
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        Flashing.Instance(2500, 0xff00ff00)
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            SetAltTexture.Instance(2),
                            Cooldown.Instance(30000,
                                new SimpleTaunt(
                                    "My children have differentiated for my protection. Their segregation makes me invincible!")),
                            OrderGroup.Instance(20, "Guardian Wisps",
                                new SetKey(-1, 2)
                                ),
                            Cooldown.Instance(2000, PredictiveRingAttack.Instance(10, 5, 25, 2)),
                            Cooldown.Instance(1500, MultiAttack.Instance(25, 10*(float) Math.PI/360, 5, 0, 3)),
                            Cooldown.Instance(25000, SpawnMinionImmediate.Instance(0x2f09, 3, 0, 2)),
                            If.Instance(IsEntityNotPresent.Instance(30, 0x2f12),
                                If.Instance(IsEntityNotPresent.Instance(30, 0x2f13),
                                    new SetKey(-1, 7)
                                    )
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 5,
                        Flashing.Instance(2500, 0xffff0000)
                        ),
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            SetAltTexture.Instance(3),
                            Cooldown.Instance(30000,
                                new SimpleTaunt(
                                    "My children have differentiated for my protection. Their segregation makes me invincible!")),
                            OrderGroup.Instance(20, "Guardian Wisps",
                                new SetKey(-1, 2)
                                ),
                            Cooldown.Instance(2000, PredictiveRingAttack.Instance(10, 5, 25, 4)),
                            Cooldown.Instance(1500, MultiAttack.Instance(25, 10*(float) Math.PI/360, 5, 0, 5)),
                            Cooldown.Instance(25000, SpawnMinionImmediate.Instance(0x2f08, 3, 0, 2)),
                            If.Instance(IsEntityNotPresent.Instance(30, 0x2f12),
                                If.Instance(IsEntityNotPresent.Instance(30, 0x2f11),
                                    new SetKey(-1, 8)
                                    )
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 6,
                        SmoothWandering.Instance(2, 2)
                        ),
                    IfEqual.Instance(-1, 6,
                        new QueuedBehavior(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(1),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(1),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(1),
                            Cooldown.Instance(1000),
                            SetAltTexture.Instance(0),
                            new SetKey(-1, 9)
                            )
                        ),
                    IfEqual.Instance(-1, 7,
                        SmoothWandering.Instance(2, 2)
                        ),
                    IfEqual.Instance(-1, 7,
                        new QueuedBehavior(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(2),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(2),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(2),
                            Cooldown.Instance(1000),
                            SetAltTexture.Instance(0),
                            new SetKey(-1, 9)
                            )
                        ),
                    IfEqual.Instance(-1, 8,
                        SmoothWandering.Instance(2, 2)
                        ),
                    IfEqual.Instance(-1, 8,
                        new QueuedBehavior(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(3),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(3),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(0),
                            Cooldown.Instance(800),
                            SetAltTexture.Instance(3),
                            Cooldown.Instance(1000),
                            SetAltTexture.Instance(0),
                            new SetKey(-1, 9)
                            )
                        ),
                    IfEqual.Instance(-1, 9,
                        ReturnSpawn.Instance(3)
                        ),
                    IfEqual.Instance(-1, 9,
                        new QueuedBehavior(
                            CooldownExact.Instance(500),
                            new SetKey(-1, 2)
                            )
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 2, 0, 8,
                        Tuple.Create(0.8, (ILoot) new StatPotionLoot(StatPotion.Att)),
                        Tuple.Create(0.2, (ILoot) new StatPotionLoot(StatPotion.Mana)),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Will-o-Wisp Spell"))
                        )
                        ))
                ));
    }
}