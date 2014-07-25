using System;
using wServer.logic.attack;
using wServer.logic.loot;
using wServer.logic.movement;
using wServer.logic.taunt;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Shatters = Behav()
            .Init(0x5005, Behaves("Stone Knight",
                new RunBehaviors(
                    Chasing.Instance(4, 10, 2, null),
                    Cooldown.Instance(450,
                        PredictiveMultiAttack.Instance(10, 5*(float) Math.PI/60, 2, 0, projectileIndex: 0)),
                    Cooldown.Instance(4200, PredictiveMultiAttack.Instance(10, 10*(float) Math.PI/180, 3, 1, 1)
                        ))
                ))
            .Init(0x5007, Behaves("Stone Mage",
                new RunBehaviors(
                    Chasing.Instance(6, 10, 6, null),
                    Cooldown.Instance(100, MultiAttack.Instance(8, 5*(float) Math.PI/100, 2, 0, projectileIndex: 0))
                    )
                ))
            .Init(0x5006, Behaves("Fire Mage",
                new RunBehaviors(
                    Chasing.Instance(6, 10, 6, null),
                    Cooldown.Instance(450, MultiAttack.Instance(8, 5*(float) Math.PI/100, 2, 0, projectileIndex: 0)),
                    Cooldown.Instance(1000, MultiAttack.Instance(8, 5*(float) Math.PI/100, 4, 0, 1)),
                    Cooldown.Instance(1100, MultiAttack.Instance(8, 5*(float) Math.PI/100, 2, 0, 2))
                    )
                ))
            .Init(0x5009, Behaves("Ice Mage",
                new RunBehaviors(
                    Chasing.Instance(6, 10, 6, null),
                    Cooldown.Instance(2000, MultiAttack.Instance(8, 5*(float) Math.PI/100, 5, 0, projectileIndex: 0))
                    )
                ))
            .Init(0x5011, Behaves("Fire Adept",
                new RunBehaviors(
                    Chasing.Instance(6, 10, 6, null),
                    new QueuedBehavior(
                        Cooldown.Instance(300, MultiAttack.Instance(8, 5*(float) Math.PI/100, 1, 0, projectileIndex: 0)),
                        Cooldown.Instance(600, MultiAttack.Instance(8, 5*(float) Math.PI/100, 4, 0, 1)),
                        Cooldown.Instance(900, MultiAttack.Instance(8, 5*(float) Math.PI/100, 4, 0, 2))
                        ))
                ))
            .Init(0x5012, Behaves("Ice Adept",
                new RunBehaviors(
                    Chasing.Instance(8, 9, 5, null),
                    Cooldown.Instance(2500, MultiAttack.Instance(10, 5*(float) Math.PI/100, 8, 0, projectileIndex: 0)),
                    Cooldown.Instance(3500, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 1))
                    )
                ))
            .Init(0x5013, Behaves("Titanum",
                new RunBehaviors(
                    Cooldown.Instance(200, MultiAttack.Instance(8, 5*(float) Math.PI/100, 1, 0, projectileIndex: 0)),
                    Cooldown.Instance(3000, RingAttack.Instance(8, 0, 0, 0)),
                    If.Instance(
                        EntityGroupLesserThan.Instance(10, 10, "Titanumminions"),
                        Rand.Instance(
                            SpawnMinion.Instance(0x5005, 3, 3, 2000, 2000),
                            SpawnMinion.Instance(0x5007, 3, 3, 2000, 2000)
                            )))
                ))
            .Init(0x5015, Behaves("Bridge Sentinel",
                new RunBehaviors(
                    Cooldown.Instance(3000, RingAttack.Instance(8, 0, 0, 0)),
                    HpLesserPercent.Instance(0.2f, new SetKey(-1, 5)),
                    Once.Instance(new SetKey(-1, 1)),

                    #region Awake
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Cooldown.Instance(15000, (new SimpleTaunt("No one can cross this bridge!"))),
                            new QueuedBehavior(
                                Cooldown.Instance(100,
                                    MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, projectileIndex: 0)),
                                Cooldown.Instance(110, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 1)),
                                Cooldown.Instance(120, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 1)),
                                Cooldown.Instance(130, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 1)),
                                Cooldown.Instance(140, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 1)),
                                Cooldown.Instance(600)
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000, (SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(5000,
                                    (UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(15000),
                                new SetKey(-1, 2))
                            )
                        ),

                    #endregion


                    #region Sleepy
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            new QueuedBehavior(
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                SetAltTexture.Instance(1),
                                TossEnemy.Instance(180, 3, 0x5022),
                                TossEnemy.Instance(270, 3, 0x5022),
                                TossEnemy.Instance(90, 3, 0x5022),
                                If.Instance(IsEntityPresent.Instance(20, 0x5022),
                                    new SetKey(-1, 3)),
                                Cooldown.Instance(10000)
                                ))),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            If.Instance(
                                EntityLesserThan.Instance(20, 1, 0x5022),
                                new SetKey(-1, 4))
                            )),

                    #endregion


                    #region Awake2
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            Cooldown.Instance(15000, new SimpleTaunt("You chose the wrong way, and you will die!")),
                            new QueuedBehavior(
                                SetAltTexture.Instance(0),
                                Cooldown.Instance(100, MultiAttack.Instance(10, 5*(float) Math.PI/100, 5, 0, 2)),
                                Cooldown.Instance(120, MultiAttack.Instance(10, 5*(float) Math.PI/100, 5, 0, 3)),
                                Cooldown.Instance(600)
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000, (SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(5000,
                                    (UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(15000),
                                new SetKey(-1, 1)
                                )
                            )
                        ),

                    #endregion


                    #region NearDeath
                    IfEqual.Instance(-1, 5,
                        new QueuedBehavior(
                            Cooldown.Instance(100, MultiAttack.Instance(10, 5*(float) Math.PI/100, 5, 0, 2)),
                            Cooldown.Instance(110, MultiAttack.Instance(10, 5*(float) Math.PI/100, 5, 0, 3)),
                            Cooldown.Instance(120,
                                MultiAttack.Instance(10, 5*(float) Math.PI/100, 4, 0, projectileIndex: 0)),
                            Cooldown.Instance(130, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 1)),
                            Cooldown.Instance(140, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 1)),
                            Cooldown.Instance(150, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 1)),
                            Cooldown.Instance(800))
                        )
                    #endregion

                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.15, (ILoot) new ItemLoot("Potion of Defense")),
                        Tuple.Create(0.15, (ILoot) new ItemLoot("Potion of Vitality")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Sword of the Archpaladin")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Hallowed Shield")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Holy Breastplate")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Amulet of Vitality")),
                        Tuple.Create(0.15, (ILoot) new TierLoot(4, ItemType.Ring)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(10, ItemType.Weapon))
                        ))
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        new QueuedBehavior(
                            Once.Instance(SpawnMinionImmediate.Instance(0x5033, 5, 1, 1)),
                            Once.Instance(new SimpleTaunt("Your army was stronger.")),
                            Once.Instance(new SimpleTaunt("You deserve to cross."))))
                }
                ))
            .Init(0x5022, Behaves("Paladin Obelisk",
                new RunBehaviors(
                    Cooldown.Instance(3000, RingAttack.Instance(8, 0, 0, 0)),
                    Cooldown.Instance(600, MultiAttack.Instance(8, 5*(float) Math.PI/100, 8, 0, projectileIndex: 0)),
                    Cooldown.Instance(600, MultiAttack.Instance(8, 5*(float) Math.PI/100, 8, 0, 1)),
                    If.Instance(
                        EntityLesserThan.Instance(20, 10, 0x5020),
                        Rand.Instance(
                            Cooldown.Instance(5000, SpawnMinionImmediate.Instance(0x5020, 5, 4, 4)))
                        )),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Potion of Defense"))
                        )))
                ))
            .Init(0x5020, Behaves("Stone Paladin",
                new RunBehaviors(
                    Chasing.Instance(4, 9, 1, null),
                    Cooldown.Instance(1000, MultiAttack.Instance(4, 5*(float) Math.PI/100, 3, 0, projectileIndex: 0))
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Potion of Defense"))
                        ))
                    )))
            .Init(0x5027, Behaves("Archmage of Flame",
                new RunBehaviors(
                    Chasing.Instance(6, 10, 6, null),
                    new QueuedBehavior(
                        Cooldown.Instance(30, MultiAttack.Instance(8, 5*(float) Math.PI/100, 4, 0, projectileIndex: 0)),
                        Cooldown.Instance(60, MultiAttack.Instance(8, 5*(float) Math.PI/100, 4, 0, projectileIndex: 0)),
                        Cooldown.Instance(90, MultiAttack.Instance(8, 5*(float) Math.PI/100, 4, 0, projectileIndex: 0)),
                        Cooldown.Instance(110, MultiAttack.Instance(8, 5*(float) Math.PI/100, 4, 0, projectileIndex: 0)),
                        Cooldown.Instance(130, MultiAttack.Instance(8, 5*(float) Math.PI/100, 3, 0, 1)),
                        Cooldown.Instance(800)
                        )),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Potion of Attack"))
                        )))
                ))
            .Init(0x5028, Behaves("Glassier Archmage",
                new RunBehaviors(
                    Chasing.Instance(6, 10, 6, null),
                    new QueuedBehavior(
                        Cooldown.Instance(30, MultiAttack.Instance(8, 5*(float) Math.PI/100, 6, 0, projectileIndex: 0)),
                        Cooldown.Instance(60, MultiAttack.Instance(8, 5*(float) Math.PI/100, 6, 0, projectileIndex: 0)),
                        Cooldown.Instance(90, MultiAttack.Instance(8, 5*(float) Math.PI/100, 6, 0, projectileIndex: 0)),
                        Cooldown.Instance(110, MultiAttack.Instance(8, 5*(float) Math.PI/100, 6, 0, projectileIndex: 0)),
                        Cooldown.Instance(130, MultiAttack.Instance(8, 5*(float) Math.PI/100, 6, 0, 1)),
                        Cooldown.Instance(800)
                        )),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Potion of Wisdom"))
                        )))
                ))
            .Init(0x5033, Behaves("Bridge Despawner",
                new RunBehaviors(
                    Chasing.Instance(4, 10, 2, null),
                    OrderAllEntity.Instance(25, 0x5029, Despawn.Instance))
                ))
            .Init(0x5034, Behaves("Twilight Archmage",
                new RunBehaviors(
                    Cooldown.Instance(3000, RingAttack.Instance(8, 0, 0, 0)),
                    HpLesserPercent.Instance(0.2f, new SetKey(-1, 3)),
                    Once.Instance(new SetKey(-1, 1)),
                    Cooldown.Instance(10000, (RingAttack.Instance(6, 0, 2, 5))),

                    #region Awake
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Cooldown.Instance(15000, (new SimpleTaunt("Who are you? Leave the Shatters!"))),
                            new QueuedBehavior(
                                Cooldown.Instance(100,
                                    MultiAttack.Instance(10, 5*(float) Math.PI/100, 9, 0, projectileIndex: 0)),
                                Cooldown.Instance(110, MultiAttack.Instance(10, 5*(float) Math.PI/100, 9, 0, 1)),
                                Cooldown.Instance(2000)
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000, (SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(5000,
                                    (UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(15000),
                                new SetKey(-1, 2))
                            )
                        ),

                    #endregion


                    #region Pissed
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Cooldown.Instance(15000, (new SimpleTaunt("Feel the power of darkness!!"))),
                            new QueuedBehavior(
                                Cooldown.Instance(100, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 2)),
                                Cooldown.Instance(140, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 3)),
                                Cooldown.Instance(180, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 4)),
                                Cooldown.Instance(220, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 5)),
                                Cooldown.Instance(260, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 6)),
                                Cooldown.Instance(800)
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000, (SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(5000,
                                    (UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(15000),
                                new SetKey(-1, 1))
                            )
                        ),

                    #endregion


                    #region Extremely Pissed
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            SimpleWandering.Instance(2, 3f),
                            Cooldown.Instance(860, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 6)),
                            Cooldown.Instance(15000, (new SimpleTaunt("YOUR SOUL WILL BE FOREVER MINE!!!"))),
                            new QueuedBehavior(
                                Cooldown.Instance(100, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 2)),
                                Cooldown.Instance(140, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 3)),
                                Cooldown.Instance(180, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 4)),
                                Cooldown.Instance(220, MultiAttack.Instance(10, 5*(float) Math.PI/100, 3, 0, 5)),
                                Cooldown.Instance(260, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 6)),
                                Cooldown.Instance(600)
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000, (SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(5000,
                                    (UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(15000),
                                new SetKey(-1, 1))
                            )
                        )
                    #endregion

                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.15, (ILoot) new ItemLoot("Potion of Mana")),
                        Tuple.Create(0.001, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.15, (ILoot) new ItemLoot("Potion of Defense")),
                        Tuple.Create(0.15, (ILoot) new TierLoot(4, ItemType.Ring)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(5, ItemType.Ring)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(10, ItemType.Weapon))
                        ))
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        new QueuedBehavior(
                            Once.Instance(SpawnMinionImmediate.Instance(0x5042, 5, 1, 1)),
                            Once.Instance(new SimpleTaunt("You were strong enough...")),
                            Once.Instance(new SimpleTaunt("My Lord awaits you."))))
                }
                ))
            .Init(0x5042, Behaves("Gate Killer", //                      Heal.Instance(5000, 1000000, 0x0935),
                new RunBehaviors(
                    Chasing.Instance(4, 10, 2, null),
                    OrderAllEntity.Instance(25, 0x5029, Despawn.Instance))
                ))
            .Init(0x5047, Behaves("Royal Guardian",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                    Cooldown.Instance(10000, (RingAttack.Instance(11, 0, 2, 2))),

                    #region Birth
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Chasing.Instance(8, 10, 2, null),
                            Once.Instance((new SimpleTaunt("WE DEFEND THE KING!"))),
                            new RunBehaviors(
                                UnsetConditionEffect.Instance((ConditionEffectIndex.Invulnerable)),
                                Cooldown.Instance(800, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 1)
                                    ),
                                new QueuedBehavior(
                                    Cooldown.Instance(25000),
                                    NullBehavior.Instance,
                                    new SetKey(-1, 2))
                                )
                            )),

                    #endregion


                    #region Heal
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            SetConditionEffect.Instance((ConditionEffectIndex.Invulnerable)),
                            Chasing.Instance(40, 10, 1, 0x5030), //Middle Stone Guardian Anchor
                            Heal.Instance(5000, 1000000, 0x5047),
                            new QueuedBehavior(
                                Cooldown.Instance(5000),
                                NullBehavior.Instance,
                                new SetKey(-1, 1))
                            )
                        )
                    #endregion

                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Potion of Wisdom"))
                        )))
                ))
            .Init(0x5048, Behaves("The Forgotten King",
                new RunBehaviors(
                    HpLesserPercent.Instance(0.2f, new SetKey(-1, 4)),
                    Once.Instance(new SetKey(-1, 1)),

                    #region Awake
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Once.Instance(new SimpleTaunt("Who dares to interrupt the Shatters?")),
                            Once.Instance(
                                new SimpleTaunt("Ah, it's you, heroes... Finally, a deserved rivals to challenge with!")),
                            new QueuedBehavior(
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                SpawnMinionImmediate.Instance(0x5047, 5, 5, 5),
                                If.Instance(IsEntityPresent.Instance(80, 0x5047),
                                    new SetKey(-1, 3)),
                                Cooldown.Instance(10000)
                                ))),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            If.Instance(
                                EntityLesserThan.Instance(80, 1, 0x5047),
                                new SetKey(-1, 2))
                            )),

                    #endregion


                    #region Pissed
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            Cooldown.Instance(15000,
                                (new SimpleTaunt("Kneel before me, and I'll save your worthless lives!"))),
                            new QueuedBehavior(
                                Cooldown.Instance(100, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 3)),
                                Cooldown.Instance(140, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 6)),
                                Cooldown.Instance(180, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 4)),
                                Cooldown.Instance(220, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 3)),
                                Cooldown.Instance(260, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 5)),
                                Cooldown.Instance(800)
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000, (SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(5000,
                                    (UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(15000),
                                new SetKey(-1, 1))
                            )
                        ),

                    #endregion


                    #region Extremely Pissed
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            SimpleWandering.Instance(2, 3f),
                            Cooldown.Instance(860, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 6)),
                            Cooldown.Instance(15000, (new SimpleTaunt("YOU WILL BE BURIED HERE WITH ME!!!"))),
                            new QueuedBehavior(
                                Cooldown.Instance(100, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 1)),
                                Cooldown.Instance(140, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 6)),
                                Cooldown.Instance(180, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 4)),
                                Cooldown.Instance(220, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 3)),
                                Cooldown.Instance(260, MultiAttack.Instance(10, 5*(float) Math.PI/100, 1, 0, 5)),
                                Cooldown.Instance(600)
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000, (SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(5000,
                                    (UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                                )
                            )
                        )
                    #endregion

                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.05, (ILoot) new ItemLoot("Bow of Forgotten Souls")),
                        Tuple.Create(0.001, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.005, (ILoot) new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.05, (ILoot) new ItemLoot("Aegis of the Lost City")),
                        Tuple.Create(0.05, (ILoot) new ItemLoot("Quiver of Galactic Impact")),
                        Tuple.Create(0.05, (ILoot) new ItemLoot("Amulet of Defense")),
                        Tuple.Create(0.15, (ILoot) new TierLoot(4, ItemType.Ring)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(5, ItemType.Ring)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.1, (ILoot) new StatPotionLoot(StatPotion.Att)),
                        Tuple.Create(0.1, (ILoot) new StatPotionLoot(StatPotion.Def)),
                        Tuple.Create(0.1, (ILoot) new StatPotionLoot(StatPotion.Dex)),
                        Tuple.Create(0.1, (ILoot) new StatPotionLoot(StatPotion.Spd)),
                        Tuple.Create(0.1, (ILoot) new StatPotionLoot(StatPotion.Vit)),
                        Tuple.Create(0.1, (ILoot) new StatPotionLoot(StatPotion.Wis)),
                        Tuple.Create(0.15, (ILoot) new TierLoot(11, ItemType.Weapon))
                        ))
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        new QueuedBehavior(
                            Once.Instance(new SimpleTaunt("My Kingdom has lost again,")),
                            Once.Instance(new SimpleTaunt("My family and friends murdered,")),
                            Once.Instance(new SimpleTaunt("All so you monsters can get hands on some loot..."))))
                }
                ));
    }
}