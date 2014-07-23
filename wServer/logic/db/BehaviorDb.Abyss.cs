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
        private static _ Abyss = Behav()
            .Init(0x090a, Behaves("Archdemon Malphas",
                new RunBehaviors(
                    SimpleWandering.Instance(1, .5f), //Basic movement
                    If.Instance(IsEntityPresent.Instance(16, null), //Once someone gets nearby, start behaviors
                        Once.Instance(new SetKey(-1, 1))
                        ),

                    #region Shooting+Potectors

                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Cooldown.Instance(2500, SpawnMinionImmediate.Instance(0x0909, 1, 1, 1)),
                            //spawn Malphas Missile every 2.5 seconds
                            Cooldown.Instance(3000, SimpleAttack.Instance(12, 0)),
                            //shoot White Armor Pierce bullet every 3 seconds
                            Cooldown.Instance(1000,
                                If.Instance(EntityLesserThan.Instance(30, 4, 0x0908),
                                    //If there are less than 4 protectors circling, spawn 1 every second untill there are 4
                                    SpawnMinionImmediate.Instance(0x0908, 1, 1, 1))),
                            new QueuedBehavior( //Invulnerable Behaviors
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                //Set effect Invulnerable
                                Cooldown.Instance(5000), //Wait 5 seconds
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                //Unset effect Invulnerable
                                Cooldown.Instance(5000)), //Wait 5 seconds and loop back to step 1


                            new QueuedBehavior(
                                CooldownExact.Instance(15000), //after 15 seconds, shrink and run stage 2
                                SetSize.Instance(90),
                                Cooldown.Instance(200),
                                SetSize.Instance(80),
                                Cooldown.Instance(200),
                                SetSize.Instance(70),
                                Cooldown.Instance(200),
                                SetSize.Instance(60),
                                Cooldown.Instance(200),
                                SetSize.Instance(50),
                                Cooldown.Instance(200),
                                SetSize.Instance(40),
                                new SetKey(-1, 2)
                                )
                            )),

                    #endregion

                    #region Small Form
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable), //Unset Invulnerability
                            SimpleWandering.Instance(8, .5f), //Moving at a higher speed than normal
                            Cooldown.Instance(1000,
                                If.Instance(EntityLesserThan.Instance(30, 4, 0x0908),
                                    //If there are less than 4 protectors circling, spawn 1 every second untill there are 4
                                    SpawnMinionImmediate.Instance(0x0908, 1, 1, 1))),
                            Cooldown.Instance(1000, RingAttack.Instance(6, 0, 0, 1)),
                            //Shoot small silver shields every second
                            Cooldown.Instance(1000, PredictiveAttack.Instance(15, 8, 0)),
                            //Shoot white Armor Piercing bullet every second

                            new QueuedBehavior(
                                CooldownExact.Instance(10000), // After 10 second, grow and go to stage 3
                                SetSize.Instance(60),
                                Cooldown.Instance(200),
                                SetSize.Instance(80),
                                Cooldown.Instance(200),
                                SetSize.Instance(100),
                                Cooldown.Instance(200),
                                SetSize.Instance(120),
                                Cooldown.Instance(200),
                                SetSize.Instance(140),
                                Cooldown.Instance(200),
                                SetSize.Instance(160),
                                Cooldown.Instance(200),
                                SetSize.Instance(170),
                                new SetKey(-1, 3)
                                ))),

                    #endregion

                    #region Large Form
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            Cooldown.Instance(1000,
                                If.Instance(EntityLesserThan.Instance(30, 4, 0x0908),
                                    //If there are less than 4 protectors circling, spawn 1 every second untill there are 4
                                    SpawnMinionImmediate.Instance(0x0908, 1, 1, 1))),
                            Cooldown.Instance(1000, RingAttack.Instance(3, 0, 90*(float) Math.PI/180, 3)),
                            //Shoot 3 Large Silver Shields in a circle every second

                            new QueuedBehavior(
                                SimpleAttack.Instance(12, 2), //Shoot large white bullet
                                Cooldown.Instance(500), //Wait half a second
                                SimpleAttack.Instance(12, 2), //Shoot another large white bullet
                                Cooldown.Instance(1000)), //Wait 1 second and start from step one

                            new QueuedBehavior( //Invulnerable Behaviors
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                //Set effect Invulnerable
                                Cooldown.Instance(5000), //Wait 5 seconds
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                //Unset effect Invulnerable
                                Cooldown.Instance(5000) //Wait 5 seconds and loop back to step 1
                                ),
                            new QueuedBehavior( //After 10 seconds, shrink back to regular size and start stage 4
                                CooldownExact.Instance(10000),
                                SetSize.Instance(160),
                                Cooldown.Instance(200),
                                SetSize.Instance(150),
                                Cooldown.Instance(200),
                                SetSize.Instance(140),
                                Cooldown.Instance(200),
                                SetSize.Instance(130),
                                Cooldown.Instance(200),
                                SetSize.Instance(120),
                                Cooldown.Instance(200),
                                SetSize.Instance(110),
                                Cooldown.Instance(200),
                                SetSize.Instance(100),
                                new SetKey(-1, 4)
                                ))),

                    #endregion

                    #region Flamers

                    IfEqual.Instance(-1, 4,
                        new QueuedBehavior(
                            SpawnMinionImmediate.Instance(0x090b, 2, 6, 8), //Spawn 6-8 Flamers
                            new SetKey(-1, 5),
                            //Go to shooting behavior (causes problems if spawning is done in the same key)
                            Cooldown.Instance(5000))),
                    //Spacing so it doesn't loop back and spawn 8 more flamer a split second before it goes to next key

                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            Cooldown.Instance(1000,
                                If.Instance(EntityLesserThan.Instance(30, 4, 0x0908),
                                    //If there are less than 4 protectors circling, spawn 1 every second untill there are 4
                                    SpawnMinionImmediate.Instance(0x0908, 1, 1, 1))),
                            Cooldown.Instance(1000, SimpleAttack.Instance(12, 0)),
                            //Shoot white Armor Piercing Bullet every second

                            new QueuedBehavior( //Invulnerable Behaviors
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                //Set effect Invulnerable
                                Cooldown.Instance(5000), //Wait 5 seconds
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                //Unset effect Invulnerable
                                Cooldown.Instance(5000)), //Wait 5 seconds and loop back to step 1

                            new QueuedBehavior(
                                Cooldown.Instance(1000),
                                //Every Second, check if there are any flamers. If not, go to Final Stage
                                If.Instance(
                                    EntityLesserThan.Instance(30, 1, 0x090b),
                                    new SetKey(-1, 6))))),

                    #endregion

                    #region Flashing + Vulnerable

                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            //Unset Invulnerable Effect
                            Flashing.Instance(500, 000000), //Flash Black

                            new QueuedBehavior(
                                CooldownExact.Instance(5000), //After 5 seconds, go back to first stage
                                new SetKey(-1, 1)
                                )))),

                #endregion
    
                condBehaviors: new ConditionalBehavior[]
                {
                    new OnDeath(MonsterSetPiece.Instance("AbyssSafeZone", 3))
                },
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 2, 8,
                        Tuple.Create(0.5, (ILoot) new StatPotionLoot(StatPotion.Vit)),
                        Tuple.Create(0.5, (ILoot) new StatPotionLoot(StatPotion.Def)),
                        Tuple.Create(0.003, (ILoot) new ItemLoot("Demon Blade")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot)new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.35, (ILoot)new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.5, (ILoot) new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.6, (ILoot) new TierLoot(3, ItemType.Ability)),
                        Tuple.Create(0.175, (ILoot) new TierLoot(10, ItemType.Armor)),
                        Tuple.Create(0.25, (ILoot) new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.3, (ILoot) new TierLoot(8, ItemType.Armor)),
                        Tuple.Create(0.225, (ILoot) new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.325, (ILoot) new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.5, (ILoot) new TierLoot(4, ItemType.Ring)),
                        Tuple.Create(0.6, (ILoot) new TierLoot(3, ItemType.Ring))
                        )))
                ))
            .Init(0x0908, Behaves("Malphas Protector",
                new RunBehaviors(
                    StrictCircling.Instance(10, 10, 0x090a),
                    Cooldown.Instance(2000, MultiAttack.Instance(100, 1*(float) Math.PI/30, 3, 0, projectileIndex: 0))
                    )
                ))
            .Init(0x0909, Behaves("Malphas Missile",
                new RunBehaviors(
                    Chasing.Instance(10, 11, 0, null),
                    CooldownExact.Instance(2000, new SetKey(-1, 1)),
                    HpLesser.Instance(1000, new SetKey(-1, 1)),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Flashing.Instance(500, 0x01FAEBD7),
                            Cooldown.Instance(1000, RingAttack.Instance(6, 0, 0, projectileIndex: 0)),
                            Cooldown.Instance(1000, Despawn.Instance)
                            ))
                    )
                ))
            .Init(0x671, Behaves("Brute of the Abyss",
                IfNot.Instance(
                    Chasing.Instance(8, 11, 1, null),
                    SimpleWandering.Instance(4)
                    ),
                Cooldown.Instance(500, MultiAttack.Instance(100, 1*(float) Math.PI/30, 3, 0, projectileIndex: 0)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Obsidian Dagger")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Steel Helm")),
                        Tuple.Create(0.01, (ILoot) HpPotionLoot.Instance)
                        )))
                ))
            .Init(0x66d, Behaves("Imp of the Abyss",
                new RunBehaviors(
                    SimpleWandering.Instance(8f),
                    Cooldown.Instance(1000, MultiAttack.Instance(100, 1*(float) Math.PI/20, 5, 0, projectileIndex: 0))
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Cloak of the Red Agent")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Felwasp Toxin")),
                        Tuple.Create(0.01, (ILoot) PotionLoot.Instance)
                        )))
                ))
            .Init(0x672, Behaves("Brute Warrior of the Abyss",
                IfNot.Instance(
                    Chasing.Instance(8, 11, 1, null),
                    SimpleWandering.Instance(4)
                    ),
                Cooldown.Instance(500, MultiAttack.Instance(100, 1*(float) Math.PI/30, 3, 0, projectileIndex: 0)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Glass Sword")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Ring of Greater Dexterity")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Magesteel Quiver")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Spirit Salve Tome"))
                        )))
                ))
            .Init(0x670, Behaves("Demon Mage of the Abyss",
                IfNot.Instance(
                    Chasing.Instance(8, 11, 5, null),
                    SimpleWandering.Instance(4)
                    ),
                Cooldown.Instance(1000, MultiAttack.Instance(100, 1*(float) Math.PI/20, 3, 0, projectileIndex: 0)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Fire Nova Spell")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Wand of Dark Magic")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Avenger Staff")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Robe of the Invoker")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Essence Tap Skull")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Demonhunter Trap"))
                        )))
                ))
            .Init(0x66f, Behaves("Demon Warrior of the Abyss",
                IfNot.Instance(
                    Chasing.Instance(8, 11, 5, null),
                    SimpleWandering.Instance(4)
                    ),
                Cooldown.Instance(1000, MultiAttack.Instance(100, 1*(float) Math.PI/20, 3, 0, projectileIndex: 0)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Fire Sword")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Steel Shield"))
                        )))
                ))
            .Init(0x66e, Behaves("Demon of the Abyss",
                IfNot.Instance(
                    Chasing.Instance(8, 11, 5, null),
                    SimpleWandering.Instance(4)
                    ),
                Cooldown.Instance(1000,
                    PredictiveMultiAttack.Instance(100, 1*(float) Math.PI/25, 3, 0, projectileIndex: 0)
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Fire Bow")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Mithril Armor"))
                        )))
                ))
            .Init(0x0e1d, Behaves("Abyss Idol",
                HpLesserPercent.Instance(0.999f,
                    new RunBehaviors(
                        Cooldown.Instance(500, RingAttack.Instance(8, 0, 0, 0)),
                        Cooldown.Instance(1000, RingAttack.Instance(4, 0, 45*(float) Math.PI/180, 1)),
                        Cooldown.Instance(1000, MultiAttack.Instance(12, 10*(float) Math.PI/180, 3, 0, 2)),
                        Cooldown.Instance(2500,
                            If.Instance(EntityLesserThan.Instance(20, 4, 0x0e1a), TossEnemyAtPlayer.Instance(8, 0x0e1a)))
                        )),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.5, (ILoot) new ItemLoot("Potion of Defense")),
                        Tuple.Create(0.5, (ILoot) new ItemLoot("Potion of Vitality")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Void Incantation")),
                        Tuple.Create(0.003, (ILoot)new ItemLoot("Wine Cellar Incantation")),
                        Tuple.Create(0.0015, (ILoot) new ItemLoot("Demon Blade"))
                        )))))
            .Init(0x0e1a, Behaves("AbyssTreasureLavaBomb",
                new RunBehaviors(
                    SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                    new QueuedBehavior(
                        CooldownExact.Instance(2000),
                        MonsterSetPiece.Instance("TempLava1", 1),
                        CooldownExact.Instance(1000),
                        MonsterSetPiece.Instance("TempLava2", 2),
                        CooldownExact.Instance(1000),
                        MonsterSetPiece.Instance("TempLava3", 3),
                        CooldownExact.Instance(1000),
                        MonsterSetPiece.Instance("TempLava4", 3),
                        CooldownExact.Instance(10000),
                        MonsterSetPiece.Instance("TempLava5", 3),
                        CooldownExact.Instance(1000),
                        MonsterSetPiece.Instance("TempLava6", 3),
                        CooldownExact.Instance(1000),
                        MonsterSetPiece.Instance("TempLava7", 2),
                        CooldownExact.Instance(1000),
                        MonsterSetPiece.Instance("TempLava8", 1),
                        Despawn.Instance
                        )
                    ))
            )
            .Init(0x090b, Behaves("Malphas Flamer",
                new QueuedBehavior(
                    Not.Instance(Chasing.Instance(7, 10, 2, null)),
                    Timed.Instance(4000, Not.Instance(new RunBehaviors(
                        Flashing.Instance(200, 0xffffff00),
                        GrowSize.Instance(20, 130),
                        Cooldown.Instance(200, SimpleAttack.Instance(10)),
                        HpLesser.Instance(200,
                            new RunBehaviors(
                                Despawn.Instance
                                )
                            )
                        ))),
                    Not.Instance(ShrinkSize.Instance(20, 70)),
                    Not.Instance(HpLesser.Instance(200,
                        new RunBehaviors(
                            Despawn.Instance
                            )
                        ))
                    )))
            .Init(0x1733, Behaves("Archdemon Summoner",
                new QueuedBehavior(
                    SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                    Cooldown.Instance(2000),
                    new SetKey(-1, 1),
                    Cooldown.Instance(5000)),
                IfEqual.Instance(-1, 1,
                    If.Instance(IsEntityNotPresent.Instance(100, 0x090a), Die.Instance)
                    ),
                condBehaviors: new ConditionalBehavior[]
                {
                    new DeathPortal(0x0704, 100, -1)
                }
                ))
            ;
    }
}