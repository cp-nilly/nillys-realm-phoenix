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
        private static _ Trench = Behav()
            .Init(0x1706, Behaves("Thessal the Mermaid Goddess",
                new RunBehaviors(
                    Once.Instance(SpawnMinionImmediate.Instance(0x172f, 1, 1, 1)),
                    HpGreaterEqual.Instance(25000, (new SetKey(-1, 1))),
                    HpLesser.Instance(25000, new SetKey(-1, 2)),
                    Cooldown.Instance(5000, If.Instance(EntityLesserThan.Instance(20, 4, 0x1707),
                        new QueuedBehavior(
                            new RunBehaviors(
                                TossEnemy.Instance(0*(float) Math.PI/180, 10, 0x01707),
                                TossEnemy.Instance(90*(float) Math.PI/180, 10, 0x01707),
                                TossEnemy.Instance(180*(float) Math.PI/180, 10, 0x01707),
                                TossEnemy.Instance(270*(float) Math.PI/180, 10, 0x01707)),
                            Cooldown.Instance(1000),
                            OrderAllEntity.Instance(20, 0x01707, Despawn.Instance),
                            Cooldown.Instance(1000)
                            ))),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Cooldown.Instance(2500, TossEnemyAtPlayer.Instance(20, 0x1702)),
                            Cooldown.Instance(8000, new RunBehaviors(
                                TossEnemy.Instance(45*(float) Math.PI/180, 14, 0x1702),
                                TossEnemy.Instance(135*(float) Math.PI/180, 14, 0x1702),
                                TossEnemy.Instance(225*(float) Math.PI/180, 14, 0x1702),
                                TossEnemy.Instance(315*(float) Math.PI/180, 14, 0x1702))),
                            SmoothWandering.Instance(1f, 1f),
                            new QueuedBehavior(
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(1000),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(1000),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(1000),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                RingAttack.Instance(8, 0, 0, 0),
                                Cooldown.Instance(1000),
                                RingAttack.Instance(8, 0, 0, 0),
                                Cooldown.Instance(1000),
                                RingAttack.Instance(8, 0, 0, 0),
                                Cooldown.Instance(1000)
                                ))
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            SimpleWandering.Instance(1f, 1f),
                            Cooldown.Instance(550, Once.Instance(Flashing.Instance(500, 0x01ADFF2F))),
                            Cooldown.Instance(6000, new RunBehaviors(
                                TossEnemy.Instance(45*(float) Math.PI/180, 14, 0x1702),
                                TossEnemy.Instance(135*(float) Math.PI/180, 14, 0x1702),
                                TossEnemy.Instance(225*(float) Math.PI/180, 14, 0x1702),
                                TossEnemy.Instance(315*(float) Math.PI/180, 14, 0x1702))),
                            Cooldown.Instance(1500, TossEnemyAtPlayer.Instance(20, 0x1702)),
                            new QueuedBehavior(
                                ReturnSpawn.Instance(40),
                                Flashing.Instance(1000, 0x01ADFF2F),
                                RingAttack.Instance(30, 0, 0, 3),
                                Cooldown.Instance(1000),
                                RingAttack.Instance(30, 0, 0, 3),
                                Cooldown.Instance(1000),
                                RingAttack.Instance(30, 0, 0, 3),
                                Cooldown.Instance(1000),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(750),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(750),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(750),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(750),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(0*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(90*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(180*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(270*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(750),
                                new RunBehaviors(
                                    AngleMultiAttack.Instance(45*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(135*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(225*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2),
                                    AngleMultiAttack.Instance(315*(float) Math.PI/180, 20*(float) Math.PI/180, 2, 2)),
                                Cooldown.Instance(500),
                                RingAttack.Instance(16, 0, 0, 0),
                                Cooldown.Instance(1000),
                                RingAttack.Instance(16, 0, 0, 0),
                                Cooldown.Instance(1000),
                                RingAttack.Instance(16, 0, 0, 0),
                                Cooldown.Instance(1000),
                                RingAttack.Instance(16, 0, 0, 0)
                                )))
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        Once.Instance(new RandomDo(20, SpawnMinionImmediate.Instance(0x1704, 1, 1, 1))))
                },
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 2, 5,
                        Tuple.Create(1.00, (ILoot) new ItemLoot("Potion of Mana")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Coral Bow")),
                        Tuple.Create(0.008, (ILoot) new ItemLoot("Coral Silk Armor")),
                        Tuple.Create(0.015, (ILoot) new ItemLoot("Coral Venom Trap"))
                        )))
                ))
            .Init(0x1700, Behaves("Fishman Warrior",
                IfNot.Instance(
                    Chasing.Instance(8, 9, 3, null),
                    IfNot.Instance(
                        Circling.Instance(3, 10, 6, null),
                        SimpleWandering.Instance(4)
                        )
                    ),
                new RunBehaviors(
                    Cooldown.Instance(1000, MultiAttack.Instance(100, 1*(float) Math.PI/30, 3, 0, projectileIndex: 0)),
                    Cooldown.Instance(2000, SimpleAttack.Instance(3, 1)),
                    Cooldown.Instance(5000, RingAttack.Instance(5, 10, 0, 2))
                    )
                ))
            .Init(0x170a, Behaves("Sea Mare",
                IfNot.Instance(
                    Charge.Instance(6, 10, null),
                    SimpleWandering.Instance(4)
                    ),
                Cooldown.Instance(500, RingAttack.Instance(3, 5, 0, 0)),
                Cooldown.Instance(1500, SimpleAttack.Instance(3, 1))
                ))
            .Init(0x170c, Behaves("Giant Squid",
                Chasing.Instance(8, 25, 3, null),
                Cooldown.Instance(250, SimpleAttack.Instance(10, projectileIndex: 0)),
                SpawnMinion.Instance(0x170b, 0, 1, 10000, 10000)
                ))
            .Init(0x170b, Behaves("Ink Bubble",
                Cooldown.Instance(100, RingAttack.Instance(1, 1, 0, projectileIndex: 0))
                ))
            .Init(0x1707, Behaves("Deep Sea Beast",
                SetSize.Instance(100),
                new QueuedBehavior(
                    Cooldown.Instance(50, SimpleAttack.Instance(3, projectileIndex: 0)),
                    Cooldown.Instance(100, SimpleAttack.Instance(3, 1)),
                    Cooldown.Instance(150, SimpleAttack.Instance(3, 2)),
                    Cooldown.Instance(200, SimpleAttack.Instance(3, 3)),
                    CooldownExact.Instance(300)
                    )
                ))
            .Init(0x1709, Behaves("Sea Horse",
                IfNot.Instance(
                    Chasing.Instance(12, 9, 1, 0x170a),
                    SimpleWandering.Instance(4, 1)
                    ),
                Cooldown.Instance(660, MultiAttack.Instance(7, 2*(float) Math.PI/180, 2, 0, projectileIndex: 0))
                ))
            .Init(0x170e, Behaves("Grey Sea Slurp",
                IfNot.Instance(
                    Chasing.Instance(12, 9, 2, 0x170d),
                    SimpleWandering.Instance(4)
                    ),
                Cooldown.Instance(500, SimpleAttack.Instance(8, projectileIndex: 0)),
                Cooldown.Instance(500, RingAttack.Instance(8, 4, 0, 1))
                ))
            .Init(0x170d, Behaves("Sea Slurp Home",
                new QueuedBehavior(
                    Cooldown.Instance(500, RingAttack.Instance(8, 4, 0, projectileIndex: 0)),
                    Cooldown.Instance(500, RingAttack.Instance(8, 2, 0, 1))
                    )
                ))
            .Init(0x172f, Behaves("Thessal Summoner",
                new QueuedBehavior(
                    SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                    Cooldown.Instance(2000),
                    new SetKey(-1, 1),
                    Cooldown.Instance(5000)),
                IfEqual.Instance(-1, 1,
                    If.Instance(IsEntityNotPresent.Instance(100, 0x1706), Die.Instance)
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new DeathPortal(0x0704, 100, -1)
                }
                ))
            //nostealing
            .Init(0x1705, Behaves("Coral Gift",
                NullBehavior.Instance,
                new QueuedBehavior(
                    SetAltTexture.Instance(1),
                    CooldownExact.Instance(500),
                    SetAltTexture.Instance(2),
                    CooldownExact.Instance(500),
                    SetAltTexture.Instance(0),
                    CooldownExact.Instance(500)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(360, new LootDef(0, 3, 0, 8,
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Coral Bow")),
                        Tuple.Create(0.007, (ILoot) new ItemLoot("Coral Silk Armor")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.5, (ILoot) new ItemLoot("Coral Juice")),
                        Tuple.Create(0.5, (ILoot) new ItemLoot("Potion of Mana"))
                        )))
                ))
            .Init(0x1702, Behaves("Coral Bomb Big",
                new RunBehaviors(
                    new QueuedBehavior(
                        CooldownExact.Instance(500),
                        new RunBehaviors(
                            TossEnemy.Instance(30*(float) Math.PI/180, 1.5f, 0x1703),
                            TossEnemy.Instance(90*(float) Math.PI/180, 1.5f, 0x1703),
                            TossEnemy.Instance(150*(float) Math.PI/180, 1.5f, 0x1703),
                            TossEnemy.Instance(210*(float) Math.PI/180, 1.5f, 0x1703),
                            TossEnemy.Instance(270*(float) Math.PI/180, 1.5f, 0x1703),
                            TossEnemy.Instance(330*(float) Math.PI/180, 1.5f, 0x1703)
                            ),
                        RingAttack.Instance(5, 0, 30*(float) Math.PI/180, 0),
                        CooldownExact.Instance(500),
                        Despawn.Instance,
                        CooldownExact.Instance(1000)
                        ))
                ))
            .Init(0x1703, Behaves("Coral Bomb Small",
                new RunBehaviors(
                    new QueuedBehavior(
                        CooldownExact.Instance(500),
                        RingAttack.Instance(5, 0, 30*(float) Math.PI/180, 0),
                        CooldownExact.Instance(500),
                        Despawn.Instance,
                        CooldownExact.Instance(1000)
                        ))
                ))
            .Init(0x1704, Behaves("Thessal the Mermaid Goddess Wounded",
                new RunBehaviors(
                    Once.Instance(new SimpleTaunt("Is King Alexander alive?")),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                    Once.Instance(new SetKey(-1, 1)
                        ),
                    new QueuedBehavior(
                        SetAltTexture.Instance(0),
                        Cooldown.Instance(500),
                        SetAltTexture.Instance(1)
                        ),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            new QueuedBehavior(
                                CooldownExact.Instance(12000),
                                new SetKey(-1, 3)
                                ))),
                    IfEqual.Instance(-1, 2,
                        new QueuedBehavior(
                            Once.Instance(new SimpleTaunt("Thank you kind sailor!")),
                            TossEnemy.Instance(60*(float) Math.PI/180, 4, 0x1705),
                            TossEnemy.Instance(180*(float) Math.PI/180, 4, 0x1705),
                            TossEnemy.Instance(300*(float) Math.PI/180, 4, 0x1705),
                            CooldownExact.Instance(4500),
                            Die.Instance
                            )),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Cooldown.Instance(1000, TossEnemy.Instance(0*(float) Math.PI/180, 10, 0x01707)),
                            Cooldown.Instance(1000, TossEnemy.Instance(90*(float) Math.PI/180, 10, 0x01707)),
                            Cooldown.Instance(1000, TossEnemy.Instance(180*(float) Math.PI/180, 10, 0x01707)),
                            Cooldown.Instance(1000, TossEnemy.Instance(270*(float) Math.PI/180, 10, 0x01707)),
                            new QueuedBehavior(
                                Cooldown.Instance(500, RingAttack.Instance(12, 20, projectileIndex: 2)),
                                Cooldown.Instance(500, RingAttack.Instance(6, 20, 60, projectileIndex: 0))
                                ),
                            Cooldown.Instance(1500, RingAttack.Instance(20, 20, projectileIndex: 3)),
                            new QueuedBehavior(
                                Cooldown.Instance(10000),
                                Die.Instance
                                )
                            )
                        )
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new ChatEvent(
                        IfEqual.Instance(-1, 1, new SetKey(-1, 2))
                        ).SetChats(
                            "He lives and reigns and conquers the world.",
                            "He lives and reigns and conquers the world"
                        )
                }
                ))
            ;
    }
}