using System;
using wServer.logic.attack;
using wServer.logic.loot;
using wServer.logic.movement;
using wServer.logic.taunt;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ EternalCrucible = Behav()
            .Init(0x2f6f, Behaves("Armored Guardian",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Chasing.Instance(15, 25, 1, null),
                            If.Instance(IsEntityPresent.Instance(1, null), new SetKey(-1, 2))
                            )
                        ),
                    IfGreater.Instance(-1, 1, SimpleWandering.Instance(0, 0)),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Rand.Instance(
                                new SetKey(-1, 3),
                                new SetKey(-1, 4),
                                new SetKey(-1, 6)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Flashing.Instance(100, 0xff00ff00),
                            new QueuedBehavior(
                                CooldownExact.Instance(800),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 6, 0, projectileIndex: 0),
                                MultiAttack.Instance(30, 25*(float) Math.PI/360, 2, 0, 1),
                                MultiAttack.Instance(30, 10*(float) Math.PI/360, 3, 180*(float) Math.PI/180, 1),
                                CooldownExact.Instance(800),
                                new SetKey(-1, 1)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            Flashing.Instance(100, 0xff0000ff),
                            new QueuedBehavior(
                                CooldownExact.Instance(800),
                                new SetKey(-1, 5)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            Flashing.Instance(100, 0xff0000ff),
                            InfiniteSpiralAttack.Instance(200, 3, 20, projectileIndex: 0),
                            Cooldown.Instance(999, SimpleAttack.Instance(20, 1)),
                            new QueuedBehavior(
                                CooldownExact.Instance(3000),
                                new SetKey(-1, 1)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            Flashing.Instance(100, 0xffff0000),
                            new QueuedBehavior(
                                CooldownExact.Instance(800),
                                MultiAttack.Instance(25, 15*(float) Math.PI/360, 4, 180*(float) Math.PI/180, 1),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 3, 0, 2),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 3, 0, 3),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 3, 0, 4),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 3, 0, 5),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 3, 0, 6),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 3, 0, 7),
                                MultiAttack.Instance(20, 10*(float) Math.PI/360, 3, 0, 8),
                                CooldownExact.Instance(800),
                                new SetKey(-1, 1)
                                )
                            )
                        )
                    )
                ))
            .Init(0x2f70, Behaves("Surprise Carrier",
                new RunBehaviors(
                    Chasing.Instance(20, 20, 0, null),
                    SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                    If.Instance(IsEntityPresent.Instance(1, null), Once.Instance(new SetKey(-1, 1))),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            RingAttack.Instance(8),
                            SpawnMinionImmediate.Instance(0x2f6f, 2, 3, 3),
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2f75, Behaves("Scorching Crucible Monolith",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),

                    //Phase One
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            new QueuedBehavior(
                                TossEnemy.Instance(45*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(135*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(215*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(305*(float) Math.PI/180, 2, 0x2f6f),
                                CooldownExact.Instance(1600),
                                new SetKey(-1, 2)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            If.Instance(EntityLesserThan.Instance(80, 1, 0x2f6f), new SetKey(-1, 3))
                            )
                        ),
                    //End of Phase One

                    //Phase Two
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            new QueuedBehavior(
                                TossEnemy.Instance(45*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(135*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(215*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(305*(float) Math.PI/180, 2, 0x2f6f),
                                CooldownExact.Instance(1600),
                                new SetKey(-1, 4)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            If.Instance(EntityLesserThan.Instance(80, 1, 0x2f6f), new SetKey(-1, 5))
                            )
                        ),
                    //End of Phase Two

                    //Phase Three
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            new QueuedBehavior(
                                new SimpleTaunt("*hummm*"),
                                MonsterSetPiece.Instance("GroundHeat1", 15),
                                TossEnemy.Instance(45*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(135*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(215*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(305*(float) Math.PI/180, 2, 0x2f6f),
                                CooldownExact.Instance(1600),
                                new SetKey(-1, 6)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            If.Instance(EntityLesserThan.Instance(80, 1, 0x2f6f), new SetKey(-1, 7))
                            )
                        ),
                    //End of Phase 3

                    //Start of Phase 4
                    IfEqual.Instance(-1, 7,
                        new RunBehaviors(
                            new QueuedBehavior(
                                TossEnemy.Instance(45*(float) Math.PI/180, 5, 0x2f76),
                                TossEnemy.Instance(135*(float) Math.PI/180, 5, 0x2f76),
                                TossEnemy.Instance(215*(float) Math.PI/180, 5, 0x2f76),
                                TossEnemy.Instance(305*(float) Math.PI/180, 5, 0x2f76),
                                CooldownExact.Instance(1501),
                                new SetKey(-1, 8)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 8,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Cooldown.Instance(1500, MultiAttack.Instance(40, 15*(float) Math.PI/360, 2, 0, 4)),
                            HpLesser.Instance(5001, new SetKey(-1, 9))
                            )
                        ),
                    //End of Phase 4

                    //Start of Phase 5
                    IfEqual.Instance(-1, 9,
                        new RunBehaviors(
                            new QueuedBehavior(
                                Heal.Instance(1, 9999, 0x2f75),
                                OrderAllEntity.Instance(50, 0x2f76, new SetKey(-1, 3)),
                                new SetKey(-1, 10)
                                )
                            )
                        ),
                    //End of Phase 5

                    //Start of Phase 6
                    IfEqual.Instance(-1, 10,
                        new RunBehaviors(
                            new QueuedBehavior(
                                RingAttack.Instance(10, 0, 0, 4),
                                new SetKey(-1, 11)
                                )
                            )
                        ),
                    IfBetween.Instance(-1, 10, 16,
                        new RunBehaviors(
                            InfiniteSpiralAttack.Instance(275, 4, 15, 3),
                            Cooldown.Instance(750, MultiAttack.Instance(25, 16*(float) Math.PI/360, 4, 0, 2)),
                            Cooldown.Instance(1000, RingAttack.Instance(10, 0, 0, 4)),
                            HpLesser.Instance(3001, new SetKey(-1, 16))
                            )
                        ),
                    IfEqual.Instance(-1, 11,
                        new RunBehaviors(
                            Rand.Instance(
                                new SetKey(-1, 12),
                                new SetKey(-1, 13),
                                new SetKey(-1, 14),
                                new SetKey(-1, 15)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 12,
                        new RunBehaviors(
                            AngleMove.Instance(4, 0*(float) Math.PI/180, 5),
                            new QueuedBehavior(
                                CooldownExact.Instance(2250),
                                new SetKey(-1, 11)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 13,
                        new RunBehaviors(
                            AngleMove.Instance(4, 90*(float) Math.PI/180, 5),
                            new QueuedBehavior(
                                CooldownExact.Instance(2250),
                                new SetKey(-1, 11)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 14,
                        new RunBehaviors(
                            AngleMove.Instance(4, 180*(float) Math.PI/180, 5),
                            new QueuedBehavior(
                                CooldownExact.Instance(2250),
                                new SetKey(-1, 11)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 15,
                        new RunBehaviors(
                            AngleMove.Instance(4, 270*(float) Math.PI/180, 5),
                            new QueuedBehavior(
                                CooldownExact.Instance(2250),
                                new SetKey(-1, 11)
                                )
                            )
                        ),
                    //End of Phase 6

                    //Start of Phase 7
                    IfEqual.Instance(-1, 16,
                        new RunBehaviors(
                            ReturnSpawn.Instance(17),
                            Heal.Instance(1, 5000, 0x2f75),
                            new QueuedBehavior(
                                CooldownExact.Instance(6000),
                                new SetKey(-1, 17)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 17, new RunBehaviors(Flashing.Instance(75, 0xffff0000))),
                    IfEqual.Instance(-1, 17,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            Cooldown.Instance(900, RingAttack.Instance(10, 0, 0, 4)),
                            Cooldown.Instance(15000, RingAttack.Instance(20, 0, 0, 1)),
                            InfiniteSpiralAttack.Instance(200, 4, 5, 3),
                            new QueuedBehavior(
                                SpawnMinionImmediate.Instance(0x2f76, 0, 1, 1),
                                TossEnemy.Instance(45*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(135*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(215*(float) Math.PI/180, 2, 0x2f6f),
                                TossEnemy.Instance(305*(float) Math.PI/180, 2, 0x2f6f),
                                CooldownExact.Instance(999999)
                                )
                            )
                        )
                    //End of Phase 7
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 3, 0, 8,
                        Tuple.Create(0.025, (ILoot) new TierLoot(11, ItemType.Weapon)),
                        Tuple.Create(0.025, (ILoot) new TierLoot(11, ItemType.Armor)),
                        Tuple.Create(0.025, (ILoot) new TierLoot(5, ItemType.Ring)),
                        Tuple.Create(0.035, (ILoot) new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.035, (ILoot) new TierLoot(10, ItemType.Armor)),
                        Tuple.Create(0.045, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.045, (ILoot) new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.045, (ILoot) new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.055, (ILoot) new TierLoot(4, ItemType.Ring))
                        )),
                    Tuple.Create(360, new LootDef(0, 2, 0, 3,
                        Tuple.Create(0.999, (ILoot) new ItemLoot("Potion of the Inferno"))
                        ))
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        new RunBehaviors(
                            OrderAllEntity.Instance(50, 0x2f76, new SetKey(-1, 3)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2f79, 0, 1, 1))
                            )
                        )
                }
                ))
            .Init(0x2f76, Behaves("ScorchMonolithLava",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            MonsterSetPiece.Instance("TempLava1", 1),
                            CooldownExact.Instance(750),
                            MonsterSetPiece.Instance("TempLava2", 2),
                            CooldownExact.Instance(750),
                            MonsterSetPiece.Instance("TempLava3", 3),
                            CooldownExact.Instance(750),
                            MonsterSetPiece.Instance("TempLava4", 3),
                            CooldownExact.Instance(750),
                            new SetKey(-1, 2)
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            If.Instance(IsEntityNotPresent.Instance(100, 0x2f75), new SetKey(-1, 3)),
                            Cooldown.Instance(3000,
                                If.Instance(EntityLesserThan.Instance(80, 2, 0x2f77),
                                    SpawnMinionImmediate.Instance(0x2f77, 1, 1, 1)))
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            new QueuedBehavior(
                                MonsterSetPiece.Instance("TempLava9", 3),
                                CooldownExact.Instance(750),
                                MonsterSetPiece.Instance("TempLava10", 3),
                                CooldownExact.Instance(750),
                                MonsterSetPiece.Instance("TempLava11", 2),
                                CooldownExact.Instance(750),
                                MonsterSetPiece.Instance("TempLava12", 1),
                                CooldownExact.Instance(750),
                                Despawn.Instance
                                )
                            )
                        )
                    )
                ))
            .Init(0x2f77, Behaves("Lava Creature",
                new RunBehaviors(
                    Chasing.Instance(3, 40, 2, null),
                    Cooldown.Instance(1250, RingAttack.Instance(5, 0, 0, 1)),
                    Cooldown.Instance(900, MultiAttack.Instance(20, 20*(float) Math.PI/360, 3, 0, projectileIndex: 0))
                    )
                ))
            .Init(0x2f79, Behaves("Scorching Voice",
                new RunBehaviors(
                    new QueuedBehavior(
                        new QuietTaunt(
                            "You have proven your determination. I have allowed access across the volcano by removing the lava. Go now, and don't come back."),
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x2f7a, Behaves("Eternal Crucible Monolith Spawn",
                new RunBehaviors(
                    MagicEye.Instance,
                    If.Instance(IsEntityNotPresent.Instance(999, 0x2f75), Once.Instance(new SetKey(-1, 1))),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            SpawnMinionImmediate.Instance(0x2f7b, 0, 1, 1),
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2f7b, Behaves("Eternal Crucible Monolith",
                new RunBehaviors(
                    MagicEye.Instance,
                    Once.Instance(new SetKey(-1, 1)),

                    //Start of Phase 0
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            If.Instance(IsEntityPresent.Instance(35, null),
                                Once.Instance(
                                    new SimpleTaunt(
                                        "Come, adventurer. This crucible will end with your encounter with me."))),
                            If.Instance(IsEntityPresent.Instance(20, null), Once.Instance(new SetKey(-1, 2)))
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            new QueuedBehavior(
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    //End of Phase 0

                    //Start of Phase 1
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Flashing.Instance(75, 0xfffaf200),
                            new QueuedBehavior(
                                CooldownExact.Instance(2000),
                                TossEnemyAtPlayer.Instance(30, 0x2f80),
                                CooldownExact.Instance(2001),
                                TossEnemyAtPlayer.Instance(30, 0x2f80),
                                new SetKey(-1, 4)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            Cooldown.Instance(1000, MultiAttack.Instance(30, 25*(float) Math.PI/360, 14, 0, 1)),
                            If.Instance(IsEntityNotPresent.Instance(120, 0x2f80), Once.Instance(new SetKey(-1, 5)))
                            )
                        ),
                    //End of Phase 1

                    //Start of Phase 2
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack.Instance(300, 5, 25, 2),
                            Cooldown.Instance(400, MultiAttack.Instance(30, 23*(float) Math.PI/360, 3, 0, 1)),
                            HpLesser.Instance(40001, Once.Instance(new SetKey(-1, 6)))
                            )
                        ),
                    //End of Phase 2

                    //Start of Phase 3
                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            InfiniteSpiralAttack.Instance(250, 5, 30, 2),
                            Once.Instance(TossEnemyAtPlayer.Instance(30, 0x2f80)),
                            Cooldown.Instance(5000, Heal.Instance(1, 600, 0x2f7b)),
                            HpLesser.Instance(30001, Once.Instance(new SetKey(-1, 7)))
                            )
                        ),
                    //End of Phase 3

                    //Start of Phase 4
                    IfEqual.Instance(-1, 7,
                        new RunBehaviors(
                            Cooldown.Instance(350, RingAttack.Instance(4, 0, 0, 2)),
                            Cooldown.Instance(550, MultiAttack.Instance(30, 20*(float) Math.PI/360, 4, 0, 1)),
                            Cooldown.Instance(400, MultiAttack.Instance(30, 12*(float) Math.PI/360, 5, 0, 3)),
                            Cooldown.Instance(450, MultiAttack.Instance(30, 17*(float) Math.PI/360, 4, 0, 4)),
                            Cooldown.Instance(5000, Heal.Instance(1, 250, 0x2f7b)),
                            HpLesser.Instance(20001, Once.Instance(new SetKey(-1, 8)))
                            )
                        ),
                    //End of Phase 4

                    //Start of Phase 5
                    IfBetween.Instance(-1, 7, 18,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            Cooldown.Instance(4000, Heal.Instance(1, 500, 0x2f7b)),
                            HpLesser.Instance(10001, Once.Instance(new SetKey(-1, 18)))
                            )
                        ),
                    IfEqual.Instance(-1, 8,
                        new RunBehaviors(
                            new QueuedBehavior(
                                RingAttack.Instance(6, 0, 150*(float) Math.PI/180, 2),
                                CooldownExact.Instance(200),
                                RingAttack.Instance(6, 0, 125*(float) Math.PI/180, 2),
                                CooldownExact.Instance(200),
                                RingAttack.Instance(6, 0, 100*(float) Math.PI/180, 2),
                                CooldownExact.Instance(200),
                                RingAttack.Instance(6, 0, 75*(float) Math.PI/180, 2),
                                CooldownExact.Instance(200),
                                RingAttack.Instance(6, 0, 25*(float) Math.PI/180, 2),
                                CooldownExact.Instance(200),
                                RingAttack.Instance(6, 0, 0*(float) Math.PI/180, 2),
                                new SetKey(-1, 9)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 9,
                        new RunBehaviors(
                            InfiniteSpiralAttack.Instance(200, 6, 5, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 10)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 10,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 9, 25, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 11)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 11,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 15, 85, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 12)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 12,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 28, 200, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 13)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 13,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 41, 20, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(3003),
                                new SetKey(-1, 14)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 14,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 28, 155, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 15)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 15,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 15, 335, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 16)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 16,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 9, 90, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 17)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 17,
                        new RunBehaviors(
                            InfiniteSpiralAttack2.Instance(200, 6, 5, 150, 2),
                            new QueuedBehavior(
                                CooldownExact.Instance(1001),
                                new SetKey(-1, 8)
                                )
                            )
                        ),
                    //End of Phase 5

                    //Final Phase Start
                    IfEqual.Instance(-1, 18,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(75, 0xfffaf200),
                            Once.Instance(
                                new SimpleTaunt(
                                    "Life. Magic. Moral. What does this mean, to a being who has existed before time itself?")),
                            new QueuedBehavior(
                                CooldownExact.Instance(7001),
                                new SetKey(-1, 19)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 19,
                        new RunBehaviors(
                            Flashing.Instance(75, 0xfffaf200),
                            Once.Instance(new SimpleTaunt("This is it! Defeat me now, or forever hold your peace!")),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack.Instance(200, 4, 25, 1),
                            Cooldown.Instance(5000, Heal.Instance(1, 500, 0x2f7b)),
                            Once.Instance(TossEnemy.Instance(45*(float) Math.PI/180, 2, 0x2f7c)),
                            Once.Instance(TossEnemy.Instance(135*(float) Math.PI/180, 2, 0x2f7d)),
                            Once.Instance(TossEnemy.Instance(215*(float) Math.PI/180, 2, 0x2f7e)),
                            Once.Instance(TossEnemy.Instance(305*(float) Math.PI/180, 2, 0x2f7f))
                            )
                        )
                    ),
                //End of Final Phase


                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 3, 0, 8,
                        Tuple.Create(0.99, (ILoot) new ItemLoot("Potion of the Immortals")),
                        Tuple.Create(0.01, (ILoot) new TierLoot(11, ItemType.Weapon)),
                        Tuple.Create(0.01, (ILoot) new TierLoot(11, ItemType.Armor)),
                        Tuple.Create(0.01, (ILoot) new TierLoot(5, ItemType.Ring)),
                        Tuple.Create(0.02, (ILoot) new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.02, (ILoot) new TierLoot(10, ItemType.Armor)),
                        Tuple.Create(0.03, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.03, (ILoot) new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.03, (ILoot) new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.05, (ILoot) new TierLoot(4, ItemType.Ring)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(8, ItemType.Armor)),
                        Tuple.Create(0.2, (ILoot) new TierLoot(8, ItemType.Weapon)),
                        Tuple.Create(0.2, (ILoot) new TierLoot(7, ItemType.Armor)),
                        Tuple.Create(0.2, (ILoot) new TierLoot(3, ItemType.Ring))
                        )),
                    Tuple.Create(360, new LootDef(0, 2, 0, 3,
                        Tuple.Create(1.00, (ILoot) new StatPotionsLoot(1, 2))
                        ))
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        new RunBehaviors(
                            OrderGroup.Instance(25, "Shock Elements", new SetKey(-1, 2)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2f82, 0, 1, 1))
                            )
                        )
                }
                ))
            .Init(0x2f7c, Behaves("Shock Element C",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack.Instance(200, 2, 10, projectileIndex: 0),
                            If.Instance(IsEntityNotPresent.Instance(20, 0x2f7b), new SetKey(-1, 2))
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2f7d, Behaves("Shock Element D",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack.Instance(250, 2, 25, projectileIndex: 0),
                            InfiniteSpiralAttack2.Instance(250, 2, -25, 90, projectileIndex: 0),
                            If.Instance(IsEntityNotPresent.Instance(20, 0x2f7b), new SetKey(-1, 2))
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2f7e, Behaves("Shock Element E",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack2.Instance(200, 3, 10, 0, projectileIndex: 0),
                            InfiniteSpiralAttack2.Instance(200, 3, 10, 5, projectileIndex: 0),
                            InfiniteSpiralAttack2.Instance(200, 3, 10, -5, projectileIndex: 0),
                            InfiniteSpiralAttack.Instance(200, 3, -9, projectileIndex: 0),
                            If.Instance(IsEntityNotPresent.Instance(20, 0x2f7b), new SetKey(-1, 2))
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2f7f, Behaves("Shock Element F",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack2.Instance(200, 3, 20, 0, projectileIndex: 0),
                            InfiniteSpiralAttack2.Instance(200, 3, 20, 5, projectileIndex: 0),
                            InfiniteSpiralAttack2.Instance(200, 3, 20, -5, projectileIndex: 0),
                            InfiniteSpiralAttack.Instance(200, 3, -19, projectileIndex: 0),
                            If.Instance(IsEntityNotPresent.Instance(20, 0x2f7b), new SetKey(-1, 2))
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2f80, Behaves("Supreme Guardian",
                new RunBehaviors(
                    Chasing.Instance(14, 25, 5, null),
                    MaintainDist.Instance(8, 25, 4f, null),
                    SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                    Cooldown.Instance(700, MultiAttack.Instance(20, 30*(float) Math.PI/360, 4, 0, projectileIndex: 0)),
                    InfiniteSpiralAttack2.Instance(850, 4, 45, 0, 1)
                    )
                ))
            .Init(0x2f82, Behaves("Eternal Voice",
                new RunBehaviors(
                    new QueuedBehavior(
                        new QuietTaunt(
                            "You have proven that you have true skill and determination. The final crucible lies at the top of the mountain, where the true god lies."),
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x2f99, Behaves("Phoenix God Spawn",
                new RunBehaviors(
                    MagicEye.Instance,
                    If.Instance(IsEntityNotPresent.Instance(999, 0x2f75),
                        If.Instance(IsEntityNotPresent.Instance(999, 0x2f7a),
                            If.Instance(IsEntityNotPresent.Instance(999, 0x2f7b),
                                Once.Instance(new SetKey(-1, 1))
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            SpawnMinionImmediate.Instance(0x2fa0, 0, 1, 1),
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fa0, Behaves("Phoenix God Shielded",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 0)),

                    //Start of Initial Phase
                    IfEqual.Instance(-1, 0,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            If.Instance(IsEntityPresent.Instance(45, null),
                                Once.Instance(
                                    new SimpleTaunt(
                                        "It's you again! Though I shouldn't be surprised. I sensed a rogue, evil presence when you arrived."))),
                            If.Instance(IsEntityPresent.Instance(35, null),
                                Once.Instance(new SimpleTaunt("I am not a god to be trifled with!"))),
                            If.Instance(IsEntityPresent.Instance(25, null),
                                Once.Instance(new SimpleTaunt("Come and meet your demise!"))),
                            If.Instance(IsEntityPresent.Instance(15, null), Once.Instance(new SetKey(-1, 1)))
                            )
                        ),
                    //End of Initial Phase

                    //Start of Phase One
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            HpLesser.Instance(45001, new SetKey(-1, 2)),
                            new QueuedBehavior(
                                Once.Instance(RingAttack.Instance(30, 0, 0, projectileIndex: 0)),
                                CooldownExact.Instance(500),
                                TossEnemyNull.Instance(0, 1.5f, 0x2fa2),
                                CooldownExact.Instance(125),
                                TossEnemyNull.Instance(180*(float) Math.PI/180, 1.5f, 0x2fa3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(75, 0xff0000ff),
                            SmoothWandering.Instance(2, 2),
                            new QueuedBehavior(
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                OrderGroup.Instance(20, "Phoenix God E1", new SetKey(-1, 1)),
                                CooldownExact.Instance(3500),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    //End of Phase One

                    //Start of Phase Two
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Once.Instance(TossEnemyNull.Instance(270*(float) Math.PI/180, 8, 0x2fa5)),
                            Cooldown.Instance(1100, MultiAttack.Instance(35, 20*(float) Math.PI/360, 12, 0, 6)),
                            HpLesser.Instance(30001, new SetKey(-1, 4))
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(75, 0xff0000ff),
                            SmoothWandering.Instance(2, 4),
                            new QueuedBehavior(
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                OrderGroup.Instance(20, "Phoenix God E2", new SetKey(-1, 1)),
                                CooldownExact.Instance(3500),
                                new SetKey(-1, 5)
                                )
                            )
                        ),
                    //End of Phase Two

                    //Start of Phase Three
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack.Instance(200, 5, 7, 4),
                            InfiniteSpiralAttack.Instance(250, 9, 15, 2),
                            HpLesser.Instance(20001, new SetKey(-1, 6))
                            )
                        ),
                    //End of Phase Three

                    //Start of Phase Four
                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fa8, 3, 6, 6)),
                            HpLesser.Instance(10001, new SetKey(-1, 7))
                            )
                        ),
                    IfEqual.Instance(-1, 7,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            SmoothWandering.Instance(2, 4),
                            new QueuedBehavior(
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                OrderGroup.Instance(20, "Phoenix God E3", new SetKey(-1, 1)),
                                CooldownExact.Instance(3500),
                                new SetKey(-1, 8)
                                )
                            )
                        ),
                    //End of Phase Four

                    //Final Phase (woaoooah nellyyy)
                    IfEqual.Instance(-1, 8,
                        new RunBehaviors(
                            Flashing.Instance(75, 0xffff0000),
                            new QueuedBehavior(
                                Once.Instance(new SimpleTaunt("I will crush you!")),
                                CooldownExact.Instance(4500),
                                new SetKey(-1, 9)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 9,
                        new RunBehaviors(
                            CooldownExact.Instance(300, RingAttack.Instance(8, 0, 0, 3)),
                            new QueuedBehavior(
                                TossEnemyNull.Instance(180*(float) Math.PI/180, 1, 0x2fa9),
                                TossEnemyNull.Instance(0*(float) Math.PI/180, 1, 0x2fa9),
                                TossEnemyNull.Instance(180*(float) Math.PI/180, 2, 0x2fa9),
                                TossEnemyNull.Instance(0*(float) Math.PI/180, 2, 0x2fa9),
                                CooldownExact.Instance(2102),
                                new SetKey(-1, 10)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 10,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack.Instance(300, 8, 8, 3)
                            )
                        )
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        new RunBehaviors(
                            Once.Instance(OrderGroup.Instance(20, "Phoenix God E4", new SetKey(-1, 1))),
                            Once.Instance(OrderAllEntity.Instance(20, 0x2fa9, new SetKey(-1, 1))),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2faa, 0, 1, 1))
                            )
                        )
                }
                ))
            .Init(0x2fa2, Behaves("Red Fiery Vortex",
                new RunBehaviors(
                    MagicEye.Instance,
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    InfiniteSpiralAttack.Instance(250, 12, 8, projectileIndex: 0),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fa0), new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x2fa3, Behaves("Blue Fiery Vortex",
                new RunBehaviors(
                    MagicEye.Instance,
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    InfiniteSpiralAttack.Instance(250, 12, -8, projectileIndex: 0),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fa0), new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x2fa5, Behaves("Fiery Streams",
                new RunBehaviors(
                    MagicEye.Instance,
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    Cooldown.Instance(600, AngleAttack.Instance(90*(float) Math.PI/180, projectileIndex: 0)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fa0), new SetKey(-1, 1)),
                    Once.Instance(TossEnemyNull.Instance(360*(float) Math.PI/180, 6, 0x2fa6)),
                    Once.Instance(TossEnemyNull.Instance(180*(float) Math.PI/180, 6, 0x2fa7)),
                    Once.Instance(new SetKey(-1, 2)),
                    IfEqual.Instance(-1, 1,
                        Despawn.Instance
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Chasing.Instance(8, 30, 1, 0x2fa6),
                            If.Instance(IsEntityPresent.Instance(1, 0x2fa6), new SetKey(-1, 3))
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Chasing.Instance(8, 30, 1, 0x2fa7),
                            If.Instance(IsEntityPresent.Instance(1, 0x2fa7), new SetKey(-1, 2))
                            )
                        )
                    )
                ))
            .Init(0x2fa6, Behaves("FSA",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fa5), new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x2fa7, Behaves("FSB",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fa5), new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x2fa8, Behaves("Fiery Barrage",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    SmoothWandering.Instance(4, 6),
                    MagicEye.Instance,
                    RandomDelay2.Instance(250, 400,
                        MultiAttack.Instance(35, 30*(float) Math.PI/360, 3, 0, projectileIndex: 0)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fa0), new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        Despawn.Instance
                        )
                    )
                ))
            .Init(0x2fa9, Behaves("Phoenix Space Vortex",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    Once.Instance(new SetKey(-1, 2)),
                    IfEqual.Instance(-1, 1,
                        Despawn.Instance
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Cooldown.Instance(300, RingAttack.Instance(4, 0, 0, projectileIndex: 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(2102),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            If.Instance(IsEntityNotPresent.Instance(100, 0x2fa0), new SetKey(-1, 1)),
                            InfiniteSpiralAttack.Instance(300, 4, 8, projectileIndex: 0)
                            )
                        )
                    )
                ))
            .Init(0x2faa, Behaves("Phoenix God Egg",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            new QueuedBehavior(
                                CooldownExact.Instance(5000),
                                new SetKey(-1, 2)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Flashing.Instance(75, 0xffff0000),
                            SetSize.Instance(150),
                            new QueuedBehavior(
                                CooldownExact.Instance(2500),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            Flashing.Instance(75, 0xffff0000),
                            SmoothWandering.Instance(3, 3),
                            InfiniteSpiralAttack.Instance(300, 3, 14, 1),
                            InfiniteSpiralAttack2.Instance(300, 3, 14, 120, 2),
                            Cooldown.Instance(10000, RingAttack.Instance(25, 0, 0, projectileIndex: 0)),
                            Cooldown.Instance(2500, RingAttack.Instance(12, 0, 0, 3)),
                            Cooldown.Instance(1750, MultiAttack.Instance(35, 3*(float) Math.PI/360, 3, 0, 4))
                            )
                        )
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        new RunBehaviors(
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fab, 0, 1, 1))
                            )
                        )
                }
                ))
            .Init(0x2fab, Behaves("Phoenix God Reborn",
                new RunBehaviors(
                    SetConditionEffect.Instance(ConditionEffectIndex.StunImmume),
                    Once.Instance(new SetKey(-1, 1)),

                    //Start of Initial Phase
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            new QueuedBehavior(
                                CooldownExact.Instance(1500),
                                new SimpleTaunt("How many 'gods' have you killed to get here?"),
                                CooldownExact.Instance(3900),
                                new SimpleTaunt("Tens? Hundreds? Thousands?"),
                                CooldownExact.Instance(3900),
                                new SimpleTaunt(
                                    "It makes me laugh to think that such weaklings would pride themselves as gods..."),
                                CooldownExact.Instance(6900),
                                new SimpleTaunt(
                                    "...and to think that you self-proclaimed heroes claim ownership to the Realm."),
                                CooldownExact.Instance(5900),
                                new SimpleTaunt("It is time you stared down a true god."),
                                CooldownExact.Instance(1900),
                                new SimpleTaunt("Mortals, attempt to defeat me, the one true god of the Realm!"),
                                CooldownExact.Instance(1900),
                                new SetKey(-1, 2)
                                )
                            )
                        ),
                    //End of Initial Phase

                    //Start of Phase One
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Once.Instance(RingAttack.Instance(25, 0, 0, projectileIndex: 0)),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            InfiniteSpiralAttack.Instance(200, 4, 10, 1),
                            InfiniteSpiralAttack.Instance(200, 12, 7, 2),
                            Cooldown.Instance(6000, TossEnemyAtPlayer.Instance(40, 0x2fac)),
                            HpLesser.Instance(50001, new SetKey(-1, 3))
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Dazed),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(100, 0xff0000ff),
                            new QueuedBehavior(
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                CooldownExact.Instance(3900),
                                new SetKey(-1, 4)
                                )
                            )
                        ),
                    //End of Phase One

                    //Start of Phase Two
                    IfBetween.Instance(-1, 3, 6,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            HpLesser.Instance(42001, new SetKey(-1, 6)),
                            Cooldown.Instance(7000, TossEnemyAtPlayer.Instance(40, 0x2fac))
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fad, 0, 3, 3)),
                            new QueuedBehavior(
                                TossEnemyNull.Instance(0, 8, 0x2fae),
                                TossEnemyNull.Instance(180*(float) Math.PI/180, 8, 0x2fae),
                                CooldownExact.Instance(2701),
                                new SetKey(-1, 5)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            new QueuedBehavior(
                                CooldownExact.Instance(4500),
                                new SetKey(-1, 4)
                                )
                            )
                        ),
                    //End of Phase Two

                    //Start of Phase Three
                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Dazed),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(100, 0xff0000ff),
                            new QueuedBehavior(
                                OrderAllEntity.Instance(30, 0x2fad, new SetKey(-1, 1)),
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                CooldownExact.Instance(3900),
                                new SetKey(-1, 7)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 7,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2faf, 2, 5, 6)),
                            Cooldown.Instance(5000, TossEnemyAtPlayer.Instance(40, 0x2fac)),
                            Cooldown.Instance(200,
                                AngleMultiAttack.Instance(45*(float) Math.PI/180, 15*(float) Math.PI/360, 3, 7)),
                            Cooldown.Instance(200,
                                AngleMultiAttack.Instance(135*(float) Math.PI/180, 15*(float) Math.PI/360, 3, 7)),
                            HpLesser.Instance(32001, new SetKey(-1, 8))
                            )
                        ),
                    IfEqual.Instance(-1, 8,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Dazed),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(100, 0xff0000ff),
                            new QueuedBehavior(
                                OrderAllEntity.Instance(30, 0x2faf, new SetKey(-1, 1)),
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                CooldownExact.Instance(3900),
                                new SetKey(-1, 9)
                                )
                            )
                        ),
                    //End of Phase Three

                    //Start of Phase Four
                    IfEqual.Instance(-1, 9,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Once.Instance(TossEnemyNull.Instance(0*(float) Math.PI/180, 2, 0x2fb0)),
                            Once.Instance(TossEnemyNull.Instance(180*(float) Math.PI/180, 2, 0x2fb1)),
                            Cooldown.Instance(5000, TossEnemyAtPlayer.Instance(40, 0x2fac)),
                            Once.Instance(new SimpleTaunt("You're all gluttons for punishment. Now here, eat this!")),
                            HpLesser.Instance(20001, new SetKey(-1, 10))
                            )
                        ),
                    IfEqual.Instance(-1, 10,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Dazed),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(100, 0xff0000ff),
                            new QueuedBehavior(
                                OrderGroup.Instance(20, "Phoenix God Minions", new SetKey(-1, 1)),
                                CooldownExact.Instance(100),
                                new SetKey(-1, 11)
                                )
                            )
                        ),
                    //End of Phase Four

                    //Start of Phase Five
                    IfEqual.Instance(-1, 11,
                        new RunBehaviors(
                            Once.Instance(new SimpleTaunt("Amateur hour is over.")),
                            Heal.Instance(1, 5000, 0x2fab),
                            Flashing.Instance(100, 0xffff0000),
                            new QueuedBehavior(
                                CooldownExact.Instance(6000),
                                RingAttack.Instance(25, 0, 0, projectileIndex: 0),
                                new SetKey(-1, 12)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 12,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb2, 0, 1, 1)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb3, 0, 1, 1)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb4, 0, 1, 1)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb5, 0, 1, 1)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb6, 0, 1, 1)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb7, 0, 1, 1)),
                            new QueuedBehavior(
                                CooldownExact.Instance(2501),
                                new SetKey(-1, 13)
                                )
                            )
                        ),
                    IfBetween.Instance(-1, 12, 15,
                        new RunBehaviors(
                            Cooldown.Instance(2300, RingAttack.Instance(8, 0, 0, 8)),
                            HpLesser.Instance(40001, new SetKey(-1, 15))
                            )
                        ),
                    IfEqual.Instance(-1, 13,
                        new RunBehaviors(
                            new QueuedBehavior(
                                OrderGroup.Instance(20, "Phoenix God Spirals B", new SetKey(-1, 2)),
                                CooldownExact.Instance(1201),
                                new SetKey(-1, 14)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 14,
                        new RunBehaviors(
                            new QueuedBehavior(
                                OrderGroup.Instance(20, "Phoenix God Spirals A", new SetKey(-1, 2)),
                                CooldownExact.Instance(1201),
                                new SetKey(-1, 13)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 15,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Dazed),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(100, 0xff0000ff),
                            new QueuedBehavior(
                                OrderGroup.Instance(20, "Phoenix God Spirals A", new SetKey(-1, 1)),
                                OrderGroup.Instance(20, "Phoenix God Spirals B", new SetKey(-1, 1)),
                                CooldownExact.Instance(3000),
                                new SetKey(-1, 16)
                                )
                            )
                        ),
                    //End of Phase Five

                    //Start of Phase Six
                    IfEqual.Instance(-1, 16,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Once.Instance(new SimpleTaunt("Any mortal can die at the will of a true god!")),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb8, 0, 1, 1)),
                            Once.Instance(SpawnMinionImmediate.Instance(0x2fb9, 0, 1, 1)),
                            Cooldown.Instance(5000, TossEnemyAtPlayer.Instance(40, 0x2fac)),
                            HpLesser.Instance(30001, new SetKey(-1, 17)),
                            new QueuedBehavior(
                                CooldownExact.Instance(1000),
                                Once.Instance(OrderGroup.Instance(20, "Phoenix God Spirits", new SetKey(-1, 2)))
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 17,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Dazed),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(100, 0xffff0000),
                            new QueuedBehavior(
                                OrderGroup.Instance(20, "Phoenix God Spirits", new SetKey(-1, 1)),
                                CooldownExact.Instance(3000),
                                new SetKey(-1, 18)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 18,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            Flashing.Instance(100, 0xffff0000),
                            Once.Instance(new SimpleTaunt("This crucible will be your LAST!")),
                            CooldownExact.Instance(150, RingAttack.Instance(7, 0, 180*(float) Math.PI/180, 9)),
                            Cooldown.Instance(5000, TossEnemyNull.Instance(0, 2, 0x2fba)),
                            Cooldown.Instance(5000, TossEnemyAtPlayer.Instance(40, 0x2fac)),
                            Cooldown.Instance(5000, TossEnemyNull.Instance(51.428f*(float) Math.PI/180, 2, 0x2fba)),
                            Cooldown.Instance(5000, TossEnemyNull.Instance(102.85f*(float) Math.PI/180, 2, 0x2fba)),
                            Cooldown.Instance(5000, TossEnemyNull.Instance(154.28f*(float) Math.PI/180, 2, 0x2fba)),
                            Cooldown.Instance(5000, TossEnemyNull.Instance(205.71f*(float) Math.PI/180, 2, 0x2fba)),
                            Cooldown.Instance(5000, TossEnemyNull.Instance(257.14f*(float) Math.PI/180, 2, 0x2fba)),
                            Cooldown.Instance(5000, TossEnemyNull.Instance(308.57f*(float) Math.PI/180, 2, 0x2fba))
                            )
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 3, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Phoenix Hide Armor")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Robe of the Fire Bird")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Armor of Everlasting Life")),
                        Tuple.Create(0.99, (ILoot) new ItemLoot("Potion of the Immortals")),
                        Tuple.Create(0.99, (ILoot) new ItemLoot("Potion of the Inferno")),
                        Tuple.Create(0.001, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(1.00, (ILoot) new StatPotionsLoot(1, 2)),
                        Tuple.Create(0.07, (ILoot) new TierLoot(11, ItemType.Weapon)),
                        Tuple.Create(0.07, (ILoot) new TierLoot(11, ItemType.Armor)),
                        Tuple.Create(0.07, (ILoot) new TierLoot(5, ItemType.Ring)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(10, ItemType.Armor)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.25, (ILoot) new TierLoot(4, ItemType.Ring))
                        ))
                    )
                ))
            .Init(0x2fac, Behaves("Phoenix God Spell Bomb",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    new QueuedBehavior(
                        RingAttack.Instance(30, 35, 0, projectileIndex: 0),
                        new SetKey(-1, 1)
                        ),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fad, Behaves("Phoenix God Tri-Shot",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    Circling.Instance(3, 20, 5, 0x2fab),
                    RandomDelay2.Instance(400, 600,
                        MultiAttack.Instance(35, 15*(float) Math.PI/360, 3, 0, projectileIndex: 0)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fae, Behaves("Phoenix God Mountain Laser",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    Chasing.Instance(4f, 20, 2, 0x2fab),
                    Cooldown.Instance(449, AngleAttack.Instance(90*(float) Math.PI/180, projectileIndex: 0)),
                    new QueuedBehavior(
                        CooldownExact.Instance(2701),
                        new SetKey(-1, 1)
                        ),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2faf, Behaves("Phoenix God Nuclear Orb",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    SmoothWandering.Instance(3, 6),
                    RandomDelay2.Instance(500, 600, Rand.Instance(
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 4, 0, projectileIndex: 0),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 3, 0, projectileIndex: 0),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 2, 0, projectileIndex: 0),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 1, 0, projectileIndex: 0),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 4, 0, 1),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 3, 0, 1),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 2, 0, 1),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 1, 0, 1),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 4, 0, 2),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 3, 0, 2),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 2, 0, 2),
                        MultiAttack.Instance(25, 25*(float) Math.PI/360, 1, 0, 2)
                        )),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb0, Behaves("Phoenix God Fire Stream",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    Cooldown.Instance(500, Rand.Instance(
                        MultiAttack.Instance(30, 20*(float) Math.PI/360, 2, 0, projectileIndex: 0),
                        MultiAttack.Instance(30, 20*(float) Math.PI/360, 3, 0, projectileIndex: 0))),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb1, Behaves("Phoenix God Fire Burst",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    Cooldown.Instance(650,
                        PredictiveMultiAttack.Instance(30, 20*(float) Math.PI/360, 9, 5, projectileIndex: 0)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb2, Behaves("Phoenix God Spiral A",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    StrictCircling2.Instance(4, 4, 0, 0x2fab),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            CooldownExact.Instance(150, AimedRingAttack.Instance(5, 20, 0, 0x2fab, projectileIndex: 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(1501),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            NullBehavior.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb3, Behaves("Phoenix God Spiral B",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    StrictCircling2.Instance(4, 4, 0.1666, 0x2fab),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            CooldownExact.Instance(150, AimedRingAttack.Instance(5, 20, 0, 0x2fab, projectileIndex: 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(1501),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            NullBehavior.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb4, Behaves("Phoenix God Spiral C",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    StrictCircling2.Instance(4, 4, 0.333, 0x2fab),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            CooldownExact.Instance(150, AimedRingAttack.Instance(5, 20, 0, 0x2fab, projectileIndex: 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(1501),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            NullBehavior.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb5, Behaves("Phoenix God Spiral D",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    StrictCircling2.Instance(4, 4, 0.5, 0x2fab),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            CooldownExact.Instance(150, AimedRingAttack.Instance(5, 20, 0, 0x2fab, projectileIndex: 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(1501),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            NullBehavior.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb6, Behaves("Phoenix God Spiral E",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    StrictCircling2.Instance(4, 4, 0.666, 0x2fab),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            CooldownExact.Instance(150, AimedRingAttack.Instance(5, 20, 0, 0x2fab, projectileIndex: 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(1501),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            NullBehavior.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb7, Behaves("Phoenix God Spiral F",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(100, 0x2fab), new SetKey(-1, 1)),
                    StrictCircling2.Instance(4, 4, 0.8333, 0x2fab),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            CooldownExact.Instance(150, AimedRingAttack.Instance(5, 20, 0, 0x2fab, projectileIndex: 0)),
                            new QueuedBehavior(
                                CooldownExact.Instance(1501),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            NullBehavior.Instance
                            )
                        )
                    )
                ))
            .Init(0x2fb8, Behaves("Scorching Spirit",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(200, 0x2fab), new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            StrictCircling2.Instance(5, 10, 0.5, null),
                            Cooldown.Instance(2000, RingAttack.Instance(12, 0, 0, projectileIndex: 0)),
                            InfiniteSpiralAttack.Instance(500, 5, 78, 2),
                            Cooldown.Instance(300, SimpleAttack.Instance(30, 1))
                            )
                        )
                    )
                ))
            .Init(0x2fb9, Behaves("Eternal Spirit",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(200, 0x2fab), new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            StrictCircling2.Instance(5, 10, 0, null),
                            Cooldown.Instance(2000, RingAttack.Instance(12, 0, 0, projectileIndex: 0)),
                            InfiniteSpiralAttack.Instance(500, 5, 78, 2),
                            Cooldown.Instance(300, SimpleAttack.Instance(30, 1))
                            )
                        )
                    )
                ))
            .Init(0x2fba, Behaves("Phoenix God Final Divination",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    If.Instance(IsEntityNotPresent.Instance(10, 0x2fab), new SetKey(-1, 1)),
                    InfiniteSpiralAttack.Instance(100, 1, 7, projectileIndex: 0),
                    MaintainDist.Instance(2, 30, 10, 0x2fab),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        )
                    )
                ));
    }
}