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
        private static _ Oryx = Behav()
            .Init(0x0932, Behaves("Oryx the Mad God 2",
                new RunBehaviors(
                    new State("idle",
                        Cooldown.Instance(850, MultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 0, 1)),
                        Cooldown.Instance(800, MultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 0, 2)),
                        Cooldown.Instance(700, MultiAttack.Instance(25, 10*(float) Math.PI/180, 2, 0, 3)),
                        Cooldown.Instance(600, MultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 0, 4)),
                        Cooldown.Instance(550, MultiAttack.Instance(25, 10*(float) Math.PI/180, 2, 0, 5)),
                        Cooldown.Instance(500, MultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 0, 6)),
                        Cooldown.Instance(6000, new SimpleTaunt("Puny mortals! My {HP} HP will annihilate you!"))
                        )
                    ),
                new RunBehaviors(
                    new State("idle",
                        HpGreaterEqual.Instance(15000,
                            new RunBehaviors(
                                MaintainDist.Instance(1, 5, 15, null),
                                Cooldown.Instance(3600,
                                    MultiAttack.Instance(25, 36*(float) Math.PI/180, 10, 0, projectileIndex: 0)),
                                If.Instance(EntityLesserThan.Instance(6, 5, 0x0944),
                                    SpawnMinion.Instance(0x0944, 2, 3, 12000, 12000))
                                )
                            ),
                        HpLesserPercent.Instance(0.2f,
                            new RunBehaviors(
                                Chasing.Instance(3, 25, 2, null),
                                Cooldown.Instance(2200,
                                    MultiAttack.Instance(25, 45*(float) Math.PI/180, 8, 0, projectileIndex: 0)),
                                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                Once.Instance(new SimpleTaunt("Can't... keep... henchmen... alive... anymore! ARGHHH!!!")),
                                Cooldown.Instance(8000,
                                    Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(30, 0, 2, 7))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(30, projectileIndex: 8))),
                                Cooldown.Instance(1000, TossEnemyAtPlayer.Instance(8, 0x0de2))
                                )
                            )
                        )
                    ),
                new RunBehaviors(
                    new State("dance",
                        Once.Instance(new SimpleTaunt("I will dance for you!")),
                        Cooldown.Instance(500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 5, 1)),
                        Cooldown.Instance(500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 5, 2)),
                        Cooldown.Instance(500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 2, 5, 3)),
                        Cooldown.Instance(500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 5, 4)),
                        Cooldown.Instance(500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 2, 5, 5)),
                        Cooldown.Instance(500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 5, 6))
                        )
                    ), new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 5, 0, 15,
                            Tuple.Create(0.07, (ILoot) new TierLoot(5, ItemType.Ability)),
                            Tuple.Create(0.05, (ILoot) new TierLoot(6, ItemType.Ability)),
                            Tuple.Create(0.07, (ILoot) new TierLoot(11, ItemType.Armor)),
                            Tuple.Create(0.06, (ILoot) new TierLoot(12, ItemType.Armor)),
                            Tuple.Create(0.05, (ILoot) new TierLoot(13, ItemType.Armor)),
                            Tuple.Create(0.07, (ILoot) new TierLoot(10, ItemType.Weapon)),
                            Tuple.Create(0.06, (ILoot) new TierLoot(11, ItemType.Weapon)),
                            Tuple.Create(0.05, (ILoot) new TierLoot(12, ItemType.Weapon)),
                            Tuple.Create(0.05, (ILoot) new TierLoot(5, ItemType.Ring)),
                            Tuple.Create(0.4, (ILoot) new StatPotionLoot(StatPotion.Att)),
                            Tuple.Create(0.4, (ILoot) new StatPotionLoot(StatPotion.Wis)),
                            Tuple.Create(0.4, (ILoot) new StatPotionLoot(StatPotion.Vit)),
                            Tuple.Create(0.4, (ILoot) new StatPotionLoot(StatPotion.Spd))
                            ))),
                    new ConditionalBehavior[] {
                        new DeathPortal(0x7040, 100, 30)
                    }
                ))
            .Init(0x0944, Behaves("Henchman of Oryx",
                new QueuedBehavior(
                    Timed.Instance(2500, Circling.Instance(2, 20, 3, 0x0932)),
                    Timed.Instance(2000, Chasing.Instance(2, 11, 4, null)),
                    Timed.Instance(1500, SimpleWandering.Instance(2))
                    ),
                new RunBehaviors(
                    Cooldown.Instance(1500, PredictiveAttack.Instance(12, 1, projectileIndex: 0)),
                    Cooldown.Instance(1500, MultiAttack.Instance(12, 20*(float) Math.PI/180, 3, projectileIndex: 1))
                    ),
                new RunBehaviors(
                    SpawnMinion.Instance(0x0de3, 20, 1, 7000, 7000),
                    SpawnMinion.Instance(0x0de1, 20, 1, 7000, 7000)
                    ), new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 1, 0, 2,
                            Tuple.Create(0.1, (ILoot) new TierLoot(3, ItemType.Misc)
                                )))
                        )
                ))
            .Init(0x0de3, Behaves("Aberrant of Oryx",
                new RunBehaviors(
                    SimpleWandering.Instance(3),
                    Chasing.Instance(7, 9, 12, null),
                    Circling.Instance(4, 15, 5, 0x0944)
                    ),
                reproduce: new RunBehaviors(
                    Cooldown.Instance(1500, TossEnemyAtPlayer.Instance(8, 0x0de4))
                    )
                ))
            .Init(0x0de4, Behaves("Aberrant Blaster",
                attack: new RunBehaviors(
                    If.Instance(
                        IsEntityPresent.Instance(5, null),
                        new QueuedBehavior(
                            MultiAttack.Instance(8, 10*(float) Math.PI/180, 6),
                            Die.Instance
                            )
                        ),
                    CooldownExact.Instance(1500, Die.Instance)
                    )
                ))
            .Init(0x0de1, Behaves("Monstrosity of Oryx",
                new RunBehaviors(
                    SimpleWandering.Instance(3),
                    Chasing.Instance(7, 9, 12, null),
                    Circling.Instance(4, 15, 5, 0x0944)
                    ),
                reproduce: new RunBehaviors(
                    If.Instance(
                        IsEntityPresent.Instance(8, null),
                        Cooldown.Instance(1000, SpawnMinionImmediate.Instance(0x0de2, 0, 1, 1))
                        )
                    )
                ))
            .Init(0x0de2, Behaves("Monstrosity Scarab",
                attack: new RunBehaviors(
                    If.Instance(
                        IsEntityPresent.Instance(8, null),
                        new RunBehaviors(
                            Chasing.Instance(25, 10, 1, null),
                            If.Instance(
                                IsEntityPresent.Instance(3, null),
                                new QueuedBehavior(
                                    RingAttack.Instance(12),
                                    Die.Instance
                                    )
                                )
                            )
                        ),
                    CooldownExact.Instance(1500, Die.Instance)
                    )
                ))
            .Init(0x2001, Behaves("Oryx the Mad God 3",
                new RunBehaviors(
                    new State("idle",
                        SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                        IfNot.Instance(
                            IsEntityPresent.Instance(20000, 0x1902),
                            SetState.Instance("flash")
                            )
                        ),
                    new State("flash",
                        Once.Instance(new PlayMusic("Oryx 3")),
                        SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                        Once.Instance(
                            new RunBehaviors(new SimpleTaunt("Mischevious runts. I will destroy you once and for all."))),
                        Timed.Instance(24000, False.Instance(Flashing.Instance(250, 0xffff0000))),
                        CooldownExact.Instance(5000, Once.Instance(new SimpleTaunt("You have killed me many times."))),
                        CooldownExact.Instance(10000,
                            Once.Instance(new SimpleTaunt("I have been humiliated over and over again."))),
                        CooldownExact.Instance(15000,
                            Once.Instance(new SimpleTaunt("Now, I will slaughter you into a million pieces."))),
                        CooldownExact.Instance(20000, Once.Instance(new SimpleTaunt("Your life is worthless!"))),
                        CooldownExact.Instance(22000, Once.Instance(new SimpleTaunt("Prepare to die!"))),
                        CooldownExact.Instance(24000, UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                        CooldownExact.Instance(24000, SetState.Instance("battle")),
                        new QueuedBehavior(
                            PlaySound.Instance(1),
                            CooldownExact.Instance(5000, PlaySound.Instance()),
                            CooldownExact.Instance(5000, PlaySound.Instance()),
                            CooldownExact.Instance(5000, PlaySound.Instance()),
                            CooldownExact.Instance(5000, PlaySound.Instance()),
                            CooldownExact.Instance(2000, PlaySound.Instance()),
                            CooldownExact.Instance(2000, PlaySound.Instance(2))
                            )
                        ),
                    new State("battle",
                        Chasing.Instance(1, 10, 5, null),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 1, 1)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 1, 2)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 2, 1, 3)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 1, 4)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 1, 9)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 2, 1, 5)),
                        Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 1, 6)),
                        Cooldown.Instance(5000, RingAttack.Instance(16, projectileIndex: 9)),
                        Cooldown.Instance(4000, SpawnMinionImmediate.Instance(0x2003, 5, 1, 1)), //Knight of the Void (4 sec)
                        Cooldown.Instance(10000, SpawnMinionImmediate.Instance(0x370f, 5, 1, 1)), //Priest of the Void (10 sec)
                        Cooldown.Instance(5000, SpawnMinionImmediate.Instance(0x370e, 5, 1, 1)), //Rogue of the Void (5 sec)
                        Cooldown.Instance(15000, SpawnMinionImmediate.Instance(0x370d, 5, 1, 1)), //Wizard of the Void (15 sec)
                        If.Instance(CheckConditionEffects.Instance(new[] {ConditionEffects.Stunned}), new RunBehaviors(
                            Cooldown.Instance(4000, SpawnMinionImmediate.Instance(0x2003, 5, 1, 1)), //Knight of the Void (4 sec)
                            Cooldown.Instance(10000, SpawnMinionImmediate.Instance(0x370f, 5, 1, 1)), //Priest of the Void (10 sec)
                            Cooldown.Instance(5000, SpawnMinionImmediate.Instance(0x370e, 5, 1, 1)), //Rogue of the Void (5 sec)
                            Cooldown.Instance(15000, SpawnMinionImmediate.Instance(0x370d, 5, 1, 1)) //Wizard of the Void (15 sec)
                        )),
                        HpLesserPercent.Instance(0.1f, SetState.Instance("neardeath"))
                        ),
                    new State("neardeath",
                        Once.Instance(new SimpleTaunt("I must live on!")),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 50*(float) Math.PI/180, 3, 0, 1)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 50*(float) Math.PI/180, 3, 0, 2)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 50*(float) Math.PI/180, 2, 0, 3)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 50*(float) Math.PI/180, 3, 0, 4)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 50*(float) Math.PI/180, 3, 0, 9)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 50*(float) Math.PI/180, 2, 0, 5)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 50*(float) Math.PI/180, 3, 0, 6)),
                        Cooldown.Instance(4000, SpawnMinionImmediate.Instance(0x2003, 5, 1, 1)), //Knight of the Void (4 sec)
                        Cooldown.Instance(10000, SpawnMinionImmediate.Instance(0x370f, 5, 1, 1)), //Priest of the Void (10 sec)
                        Cooldown.Instance(5000, SpawnMinionImmediate.Instance(0x370e, 5, 1, 1)), //Rogue of the Void (5 sec)
                        Cooldown.Instance(15000, SpawnMinionImmediate.Instance(0x370d, 5, 1, 1)), //Wizard of the Void (15 sec)
                        If.Instance(CheckConditionEffects.Instance(new[] { ConditionEffects.Stunned }), new RunBehaviors(
                            Cooldown.Instance(4000, SpawnMinionImmediate.Instance(0x2003, 5, 1, 1)), //Knight of the Void (4 sec)
                            Cooldown.Instance(10000, SpawnMinionImmediate.Instance(0x370f, 5, 1, 1)), //Priest of the Void (10 sec)
                            Cooldown.Instance(5000, SpawnMinionImmediate.Instance(0x370e, 5, 1, 1)), //Rogue of the Void (5 sec)
                            Cooldown.Instance(15000, SpawnMinionImmediate.Instance(0x370d, 5, 1, 1)) //Wizard of the Void (15 sec)
                        )),
                        new QueuedBehavior(
                            Cooldown.Instance(2000, RingAttack.Instance(8, projectileIndex: 9)),
                            Cooldown.Instance(2000, RingAttack.Instance(8, offset: 90, projectileIndex: 9))
                            )
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 8,
                        Tuple.Create(0.1, (ILoot) new TierLoot(8, ItemType.Ability)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(15, ItemType.Armor)),
                        Tuple.Create(0.1, (ILoot) new TierLoot(14, ItemType.Weapon)),
                        Tuple.Create(0.001, (ILoot) new ItemLoot("Tome of Noble Assault")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Potion of Oryx"))
                        ))),
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(new RunBehaviors(new PlayMusic("clooooeebe mixex"), new SimpleTaunt("I'm such a weakling...")))
                }
                ))
            .Init(0x1902, Behaves("Magical Tree",
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(
                        If.Instance(
                            IsEntityPresent.Instance(20000, 0x1902),
                            Rand.Instance(
                                new SimpleTaunt("We protect the lord!"),
                                new SimpleTaunt("Go into battle if you wish!"),
                                new SimpleTaunt("You are food for his minions!")
                                )
                            )
                        )
                }
                ))
            .Init(0x2003, Behaves("Knight of the Void",
                new RunBehaviors(
                    Chasing.Instance(15, 10, 2, null),
                    new QueuedBehavior(
                        Cooldown.Instance(250, SimpleAttack.Instance(5)),
                        Cooldown.Instance(250, PredictiveAttack.Instance(5, 0.2f))
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.0000001, (ILoot) new ItemLoot("Potion of Maxy"))
                        //Currently rarest item in the game?
                        )))
                ))
            .Init(0x370d, Behaves("Wizard of the Void",
                new RunBehaviors(
                    SimpleWandering.Instance(1f),
                    Chasing.Instance(14, 10, 4, null),
                    Cooldown.Instance(750, PredictiveMultiAttack.Instance(8, 0, 2, 3, projectileIndex: 0)),
                    Cooldown.Instance(75, MultiAttack.Instance(8, 0, 2, 0, projectileIndex: 0))
                    )
                ))
            .Init(0x370e, Behaves("Rogue of the Void",
                new RunBehaviors(
                    MagicEye.Instance,
                    Chasing.Instance(18, 10, 2, null),
                    Cooldown.Instance(1800, PredictiveAttack.Instance(5, 3, projectileIndex: 0)),
                    Cooldown.Instance(225, SimpleAttack.Instance(5, projectileIndex: 0))
                    )
                ))
            .Init(0x370f, Behaves("Priest of the Void",
                new RunBehaviors(
                    SimpleWandering.Instance(1f),
                    MaintainDist.Instance(8, 9, 10, null),
                    Chasing.Instance(8, 12, 10, null),
                    Cooldown.Instance(1000, HealGroup.Instance(8, 40, "Void Mob")),
                    Cooldown.Instance(500, PredictiveAttack.Instance(10, 4, projectileIndex: 0))
                    )
                ))
            .Init(0x1740, Behaves("Oryx the Mad God 1",
                    new RunBehaviors(
                        IfExist.Instance(-1, NullBehavior.Instance,
                            new RunBehaviors(
                                new QueuedBehavior(
                                    CooldownExact.Instance(400)
                                ),
                                Once.Instance(new SimpleTaunt("I still have {HP} hitpoints!")),
                                new QueuedBehavior(new SetKey(-1, 1))

                            )
                        ),
                        IfEqual.Instance(-1, 1,
                            new RunBehaviors(
                            Once.Instance(SpawnMinionImmediate.Instance(0x1749, 10, 0, 4)),
                            Cooldown.Instance(100000, MultiAttack.Instance(10, 15 * (float)Math.PI / 180, 4, 0, projectileIndex: 12)),
                            Reproduce.Instance(0x1749, 10, 5000, 4),
                            new QueuedBehavior(HpLesserPercent.Instance(0.95f, new SetKey(-1, 2)))


                            )
                        ),
                        IfEqual.Instance(-1, 2,
                            new RunBehaviors(
                            Once.Instance(new SimpleTaunt("BE SILENT!")),
                            Once.Instance(RingAttack.Instance(20, 20, 0, projectileIndex: 0)),
                            Timed.Instance(2500, Flashing.Instance(200, 0xf389E13)),
                            InfiniteSpiralAttack.Instance(125, 10, 7.5f, projectileIndex: 10),
                            Cooldown.Instance(5000, RingAttack.Instance(8, 20, 0, projectileIndex: 17)),
                            Cooldown.Instance(4000, new SetConditionEffectTimed(ConditionEffectIndex.Invulnerable, 2000)),
                            new QueuedBehavior(HpLesserPercent.Instance(0.85f, new SetKey(-1, 3)))
                            )
                         ),
                         IfEqual.Instance(-1, 3,
                            new RunBehaviors(
                                Chasing.Instance(5, 20, 2, null),
                                Cooldown.Instance(1000, RingAttack.Instance(20, 20, 0, projectileIndex: 2)),
                                Cooldown.Instance(125, MultiAttack.Instance(20, 3 * (float)Math.PI / 180, 2, 0,  projectileIndex: 1)),
                                new QueuedBehavior(HpLesserPercent.Instance(0.70f, new SetKey(-1, 4)))
                            )
                        ),
                        IfEqual.Instance(-1, 4,
                            new RunBehaviors(
                                Timed.Instance(2500, Flashing.Instance(200, 0xf389E13)),
                                Once.Instance(new SimpleTaunt("I still have {HP} hitpoints!")),
                                new QueuedBehavior(
                                    CooldownExact.Instance(1000)
                                ),
                                SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                                Cooldown.Instance(4000, new SetConditionEffectTimed(ConditionEffectIndex.Invulnerable, 3000)),
                                Once.Instance(new SimpleTaunt("Minions Kill Them!!")),
                                Cooldown.Instance(2000, RingAttack.Instance(10, 20, 0, projectileIndex: 6)),
                                Reproduce.Instance(0x1749, 3, 5000, 4),
                                new QueuedBehavior(
                                    HpLesserPercent.Instance(0.40f, new SetKey(-1, 5))
                                )
                            )
                        ),
                        IfEqual.Instance(-1, 5,
                            new RunBehaviors(
                                Once.Instance(new SimpleTaunt("My Artifacts will protect me!!")),
                                Once.Instance(SpawnMinionImmediate.Instance(0x174a, 2, 1, 1)),
                                Once.Instance(SpawnMinionImmediate.Instance(0x174b, 2, 1, 1)),
                                Once.Instance(SpawnMinionImmediate.Instance(0x174c, 2, 1, 1)),
                                Once.Instance(SpawnMinionImmediate.Instance(0x174d, 2, 1, 1)),
                                Cooldown.Instance(4000, new SetConditionEffectTimed(ConditionEffectIndex.Invulnerable, 2000)),
                                new QueuedBehavior(HpLesserPercent.Instance(0.20f, new SetKey(-1, 6)))
                            )
                        ),
                        IfEqual.Instance(-1, 6,
                            new RunBehaviors(
                                Once.Instance(new SimpleTaunt("ENOUGH!")),
                                Flashing.Instance(200, 0xf389E13),
                                new QueuedBehavior(
                                    CooldownExact.Instance(1100),
                                    SetSize.Instance(100),
                                    CooldownExact.Instance(1100),
                                    SetSize.Instance(110),
                                    CooldownExact.Instance(1100),
                                    SetSize.Instance(120),
                                    CooldownExact.Instance(1100),
                                    SetSize.Instance(130),
                                    CooldownExact.Instance(1100),
                                    SetSize.Instance(140)
                                ),
                                Chasing.Instance(8, 20, 3, null),
                                Cooldown.Instance(125, MultiAttack.Instance(20, 3 * (float)Math.PI / 180, 2, 0, projectileIndex: 1)),
                                Cooldown.Instance(2000, MultiAttack.Instance(20, 10 * (float)Math.PI / 180, 2, 0, projectileIndex: 14))
                            )
                        )
                        
                    ),

                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 6, 0, 16,
                            Tuple.Create(0.001, (ILoot)new TierLoot(4, ItemType.Ability)),
                            Tuple.Create(0.005, (ILoot)new TierLoot(5, ItemType.Ability)),

                            Tuple.Create(0.05, (ILoot)new TierLoot(8, ItemType.Armor)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(9, ItemType.Armor)),
                            Tuple.Create(0.025, (ILoot)new TierLoot(10, ItemType.Armor)),
                            Tuple.Create(0.005, (ILoot)new TierLoot(11, ItemType.Armor)),
                            Tuple.Create(0.001, (ILoot)new TierLoot(12, ItemType.Armor)),

                            Tuple.Create(0.05, (ILoot)new TierLoot(8, ItemType.Weapon)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(9, ItemType.Weapon)),
                            Tuple.Create(0.005, (ILoot)new TierLoot(10, ItemType.Weapon)),
                            Tuple.Create(0.001, (ILoot)new TierLoot(11, ItemType.Weapon)),

                            Tuple.Create(0.2, (ILoot)new TierLoot(5, ItemType.Ring)),
                            Tuple.Create(0.5, (ILoot)new TierLoot(4, ItemType.Ring)),
                            Tuple.Create(0.7, (ILoot)new TierLoot(3, ItemType.Ring)),
                            Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Def)),
                            Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Att))
                    ))),
                    condBehaviors: new ConditionalBehavior[]
                    {
                        new DeathPortal(0x0721, 100, 60),
                    }       

            ))
            .Init(0x1748, Behaves("Ring Element",
                new RunBehaviors(
                    Timed.Instance(20000, Cooldown.Instance(700, RingAttack.Instance(8, 100, 0, projectileIndex: 0))),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    CooldownExact.Instance(19000,
                        new RunBehaviors(
                            Despawn.Instance
                            ))
                    ))
            )
            .Init(0x1749, Behaves("Minion of Oryx",
                new RunBehaviors(
                    SimpleWandering.Instance(3),
                    Cooldown.Instance(700, MultiAttack.Instance(10, 15*(float) Math.PI/180, 3, 0, projectileIndex: 0)),
                    Cooldown.Instance(700, MultiAttack.Instance(10, 15*(float) Math.PI/180, 3, 0, 1))
                    ))
            )
            .Init(0x174a, Behaves("Guardian Element 1",
                new RunBehaviors(
                    Circling.Instance(2, 10, 20, 0x1740),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(850, MultiAttack.Instance(10, 15*(float) Math.PI/180, 3, 0, projectileIndex: 0)),
                    Cooldown.Instance(30000, SetSize.Instance(200)),
                    CooldownExact.Instance(40000,
                        new RunBehaviors(
                            Despawn.Instance
                            ))
                    ))
            )
            .Init(0x174b, Behaves("Guardian Element 2",
                new RunBehaviors(
                    Circling.Instance(2, 10, 20, 0x1740),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(850, MultiAttack.Instance(10, 15*(float) Math.PI/180, 3, 0, projectileIndex: 0)),
                    Cooldown.Instance(30000, SetSize.Instance(200)),
                    CooldownExact.Instance(40000,
                        new RunBehaviors(
                            Despawn.Instance
                            ))
                    ))
            )
            .Init(0x174c, Behaves("Guardian Element 3",
                new RunBehaviors(
                    Circling.Instance(2, 10, 20, 0x1740),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(850, MultiAttack.Instance(10, 15*(float) Math.PI/180, 3, 0, projectileIndex: 0)),
                    Cooldown.Instance(30000, SetSize.Instance(200)),
                    CooldownExact.Instance(40000,
                        new RunBehaviors(
                            Despawn.Instance
                            ))
                    ))
            )
            .Init(0x174d, Behaves("Guardian Element 4",
                new RunBehaviors(
                    Circling.Instance(2, 10, 20, 0x1740),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(850, MultiAttack.Instance(10, 15*(float) Math.PI/180, 3, 0, projectileIndex: 0)),
                    Cooldown.Instance(30000, SetSize.Instance(200)),
                    CooldownExact.Instance(40000,
                        new RunBehaviors(
                            Despawn.Instance
                            ))
                    ))
            )
            .Init(0x174e, Behaves("Outer Guardian Element",
                new RunBehaviors(
                    Circling.Instance(12, 10, 15, 0x1740),
                    Cooldown.Instance(850, MultiAttack.Instance(10, 15*(float) Math.PI/180, 3, 0, projectileIndex: 0)),
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    CooldownExact.Instance(40000,
                        new RunBehaviors(
                            Despawn.Instance
                            ))
                    ))
            )
            .Init(0x370b, Behaves("Priest of the Void",
                new RunBehaviors(
                    SimpleWandering.Instance(4, 5),
                    Cooldown.Instance(1000, SimpleAttack.Instance(10, 0)),
                    Cooldown.Instance(2000, HealGroup.Instance(20, 40, "Void"))
                    )))
            .Init(0x370c, Behaves("Wizard of the Void",
                new RunBehaviors(
                    SimpleWandering.Instance(4, 5),
                    Cooldown.Instance(1000, SimpleAttack.Instance(10, 0))
                    )));
    }
}
