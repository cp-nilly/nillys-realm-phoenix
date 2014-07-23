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
        private static _ ForbiddenJungle = Behav()
            .Init(0x0dc3, Behaves("Mixcoatl the Masked God",
                new RunBehaviors(
                    Cooldown.Instance(5000, ReturnSpawn.Instance(3)),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    If.Instance(IsEntityPresent.Instance(10, null), Once.Instance(new SetKey(-1, 1))),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            new QueuedBehavior(
                                SetAltTexture.Instance(3),
                                Cooldown.Instance(200),
                                SetAltTexture.Instance(4),
                                Cooldown.Instance(200),
                                SetAltTexture.Instance(5),
                                Cooldown.Instance(200),
                                SetAltTexture.Instance(6),
                                Cooldown.Instance(200)
                                ),
                            new QueuedBehavior(
                                CooldownExact.Instance(5000),
                                new SetKey(-1, 2)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                            Once.Instance(SetAltTexture.Instance(0)),
                            SimpleWandering.Instance(2, 2f),
                            new QueuedBehavior(
                                SimpleAttack.Instance(10, 2),
                                Cooldown.Instance(250),
                                SimpleAttack.Instance(10, 2),
                                Cooldown.Instance(1000),
                                SimpleAttack.Instance(10, 2),
                                Cooldown.Instance(250),
                                SimpleAttack.Instance(10, 2)
                                ),
                            new QueuedBehavior(
                                CooldownExact.Instance(5000),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            new QueuedBehavior(
                                ReturnSpawn.Instance(8),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, -90*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, -45*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 0*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 45*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 90*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 135*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 180*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 225*(float) Math.PI/180, 1),
                                Cooldown.Instance(200)
                                ),
                            new QueuedBehavior(
                                CooldownExact.Instance(3000),
                                new SetKey(-1, 4)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            SimpleWandering.Instance(2, 2f),
                            new QueuedBehavior(
                                ReturnSpawn.Instance(5),
                                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Paralyzed)),
                                SetAltTexture.Instance(1),
                                Cooldown.Instance(500),
                                SetAltTexture.Instance(2),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                SetAltTexture.Instance(0)
                                ),
                            new QueuedBehavior(
                                CooldownExact.Instance(3000),
                                new SetKey(-1, 5)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            Chasing.Instance(2, 5, 2, null),
                            Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed)),
                            SetAltTexture.Instance(0),
                            new QueuedBehavior(
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, -90*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, -45*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 0*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 45*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 90*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 135*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 180*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                MultiAttack.Instance(20, 10*(float) Math.PI/180, 3, 225*(float) Math.PI/180, 1),
                                Cooldown.Instance(200),
                                RingAttack.Instance(8, 5, 0, 0),
                                Cooldown.Instance(400)
                                )
                            )
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 6, 0, 10,
                        Tuple.Create(0.001, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot)new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.4, (ILoot) new ItemLoot("Crystal Bone Ring")),
                        Tuple.Create(0.25, (ILoot) new ItemLoot("Staff of the Crystal Serpent")),
                        Tuple.Create(0.3, (ILoot) new ItemLoot("Cracked Crystal Skull")),
                        Tuple.Create(0.4, (ILoot) new ItemLoot("Robe of the Tlatoani"))
                        )))))
            .Init(0x0dc2, Behaves("Great Coil Snake",
                new QueuedBehavior(
                    Timed.Instance(5000, False.Instance(SimpleWandering.Instance(2))),
                    Timed.Instance(1500, Not.Instance(ReturnSpawn.Instance(2)))
                    ),
                new QueuedBehavior(
                    Cooldown.Instance(1000, PredictiveAttack.Instance(10, 1, projectileIndex: 0)),
                    Cooldown.Instance(1000, RingAttack.Instance(10, 10, projectileIndex: 1)),
                    Cooldown.Instance(1000, Rand.Instance(
                        TossEnemy.Instance(0*(float) Math.PI/180, 4, 0x0dc1),
                        TossEnemy.Instance(90*(float) Math.PI/180, 4, 0x0dc1),
                        TossEnemy.Instance(180*(float) Math.PI/180, 4, 0x0dc1),
                        TossEnemy.Instance(270*(float) Math.PI/180, 4, 0x0dc1)
                        )),
                    Cooldown.Instance(1000, Rand.Instance(
                        TossEnemy.Instance(0*(float) Math.PI/180, 4, 0x0dc1),
                        TossEnemy.Instance(90*(float) Math.PI/180, 4, 0x0dc1),
                        TossEnemy.Instance(180*(float) Math.PI/180, 4, 0x0dc1),
                        TossEnemy.Instance(270*(float) Math.PI/180, 4, 0x0dc1)
                        ))
                    )
                ))
            .Init(0x0dc1, Behaves("Great Snake Egg",
                new QueuedBehavior(
                    Cooldown.Instance(5000),
                    new Transmute(0x0dc0, 1, 2)
                    )
                ))
            .Init(0x0dc0, Behaves("Great Temple Snake",
                False.Instance(SimpleWandering.Instance(3)),
                new QueuedBehavior(
                    Cooldown.Instance(1000, MultiAttack.Instance(4, 7*(float) Math.PI/180, 2, projectileIndex: 0)),
                    Cooldown.Instance(1000, RingAttack.Instance(6, 4, projectileIndex: 1))
                    )));
    }
}