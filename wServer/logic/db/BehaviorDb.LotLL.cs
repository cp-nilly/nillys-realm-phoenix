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
        private static _ LotLL = Behav()
            .Init(0x0d50,
                Behaves("Lord of the Lost Lands",
                    //Aprox Completion: 99.9999% Done , More fixes, Gathering power it NULL on lots of niggaz attackin Lord
                    new RunBehaviors(
                        HpLesser.Instance(9000, new SetKey(-1, 13)), // Actually he dies faster at app. 9k hp
                        Once.Instance(new SetKey(-1, 0)),
                        IfEqual.Instance(-1, 0,
                            new RunBehaviors(
                                Once.Instance(new RunBehaviors(
                                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                    DamageLesserEqual.Instance(100,
                                        new RunBehaviors(
                                            new SetKey(-1, 1)
                                            ),
                                        DamageLesserEqual.Instance(37500,
                                            new RunBehaviors(
                                                new SetKey(-1, 7)
                                                )
                                            )
                                        )
                                    )
                                    )
                                )),
                        IfEqual.Instance(-1, 1,
                            new QueuedBehavior(
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                SetAltTexture.Instance(0),
                                SmoothWandering.Instance(1.5f, 3f),
                                CooldownExact.Instance(900),
                                new SetKey(-1, 2)
                                )),
                        IfEqual.Instance(-1, 2, SmoothWandering.Instance(0f, 0f)
                            ),
                        IfEqual.Instance(-1, 2, Flashing.Instance(100, 0xFF7A7A)
                            ),
                        IfEqual.Instance(-1, 2,
                            new QueuedBehavior(
                                AngleAttack.Instance(0*(float) Math.PI/180, 1),
                                AngleAttack.Instance(90*(float) Math.PI/180, 1),
                                AngleAttack.Instance(180*(float) Math.PI/180, 1),
                                AngleAttack.Instance(270*(float) Math.PI/180, 1),
                                CooldownExact.Instance(1200),
                                AngleAttack.Instance(45*(float) Math.PI/180, 1),
                                AngleAttack.Instance(135*(float) Math.PI/180, 1),
                                AngleAttack.Instance(225*(float) Math.PI/180, 1),
                                AngleAttack.Instance(315*(float) Math.PI/180, 1),
                                CooldownExact.Instance(1200)
                                )),
                        IfEqual.Instance(-1, 2,
                            new QueuedBehavior(
                                new SimpleTaunt("GATHERING POWER!"),
                                SetAltTexture.Instance(3),
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                CooldownExact.Instance(6200),
                                new SetKey(-1, 6)
                                )),
                        IfEqual.Instance(-1, 6,
                            new RunBehaviors(
                                Once.Instance(CooldownExact.Instance(1500)),
                                new QueuedBehavior(
                                    AngleMultiAttack.Instance((0*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((0*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((1*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((1*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((2*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((2*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((3*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((3*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((4*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((4*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((0*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((0*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((1*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((1*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((2*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((2*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((3*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((3*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((4*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((4*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750)
                                    ))),
                        IfEqual.Instance(-1, 6, SmoothWandering.Instance(1.5f, 3f)
                            ),
                        IfEqual.Instance(-1, 6,
                            new QueuedBehavior(
                                SetAltTexture.Instance(0),
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                CooldownExact.Instance(1500),
                                SetAltTexture.Instance(0),
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                CooldownExact.Instance(8000),
                                new SetKey(-1, 3)
                                )),
                        IfEqual.Instance(-1, 3, SmoothWandering.Instance(0, 0)),
                        IfEqual.Instance(-1, 3,
                            new QueuedBehavior(
                                TossEnemy.Instance(0f*(float) Math.PI/180, 5f, 0x0d53),
                                TossEnemy.Instance(90f*(float) Math.PI/180, 5f, 0x0d53),
                                TossEnemy.Instance(180f*(float) Math.PI/180, 5f, 0x0d53),
                                TossEnemy.Instance(270f*(float) Math.PI/180, 5f, 0x0d53),
                                CooldownExact.Instance(2500),
                                TossEnemy.Instance(0f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(45f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(90f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(135f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(180f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(225f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(270f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(315f*(float) Math.PI/180, 4f, 0x0d51),
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                SetAltTexture.Instance(1),
                                new SetKey(-1, 4)
                                )),
                        IfEqual.Instance(-1, 4,
                            new QueuedBehavior(
                                CooldownExact.Instance(2000),
                                OrderAllEntity.Instance(50, 0x0d51, new SetKey(-2, 0)),
                                CooldownExact.Instance(250),
                                new SetKey(-1, 5)
                                )),
                        IfEqual.Instance(-1, 5,
                            new QueuedBehavior(
                                new QueuedBehavior(
                                    Cooldown.Instance(5600),
                                    If.Instance(EntityLesserThan.Instance(60, 1, 0x0d51),
                                        new SetKey(-1, 1)
                                        )),
                                new QueuedBehavior(
                                    SetAltTexture.Instance(2),
                                    CooldownExact.Instance(1600),
                                    SetAltTexture.Instance(1)
                                    )
                                )
                            ),
                        IfEqual.Instance(-1, 7,
                            new QueuedBehavior(
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                SetAltTexture.Instance(0),
                                SmoothWandering.Instance(1.5f, 3f),
                                CooldownExact.Instance(900),
                                new SetKey(-1, 8)
                                )),
                        IfEqual.Instance(-1, 8,
                            new QueuedBehavior(
                                CooldownExact.Instance(1500),
                                new SetKey(-1, 9)
                                )
                            ),
                        IfEqual.Instance(-1, 9,
                            new RunBehaviors(
                                Once.Instance(CooldownExact.Instance(1500)),
                                new QueuedBehavior(
                                    AngleMultiAttack.Instance((0*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((0*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((1*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((1*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((2*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((2*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((3*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((3*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750),
                                    AngleMultiAttack.Instance((4*36)*(float) Math.PI/180, 25, 7, projectileIndex: 0),
                                    AngleMultiAttack.Instance((4*36 + 180)*(float) Math.PI/180, 25, 7,
                                        projectileIndex: 0),
                                    CooldownExact.Instance(750)
                                    ))),
                        IfEqual.Instance(-1, 9, SmoothWandering.Instance(1.5f, 3f)
                            ),
                        IfEqual.Instance(-1, 9,
                            new QueuedBehavior(
                                SetAltTexture.Instance(0),
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                CooldownExact.Instance((750*5)),
                                new SetKey(-1, 10)
                                )),
                        IfEqual.Instance(-1, 10, SmoothWandering.Instance(0, 0)),
                        IfEqual.Instance(-1, 10,
                            new QueuedBehavior(
                                TossEnemy.Instance(0f*(float) Math.PI/180, 5f, 0x0d53),
                                TossEnemy.Instance(90f*(float) Math.PI/180, 5f, 0x0d53),
                                TossEnemy.Instance(180f*(float) Math.PI/180, 5f, 0x0d53),
                                TossEnemy.Instance(270f*(float) Math.PI/180, 5f, 0x0d53),
                                CooldownExact.Instance(1500),
                                TossEnemy.Instance(0f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(45f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(90f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(135f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(180f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(225f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(270f*(float) Math.PI/180, 4f, 0x0d51),
                                TossEnemy.Instance(315f*(float) Math.PI/180, 4f, 0x0d51),
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                SetAltTexture.Instance(1),
                                new SetKey(-1, 11)
                                )),
                        IfEqual.Instance(-1, 11,
                            new QueuedBehavior(
                                CooldownExact.Instance(2000),
                                OrderAllEntity.Instance(50, 0x0d51, new SetKey(-2, 0)),
                                CooldownExact.Instance(250),
                                new SetKey(-1, 12)
                                )),
                        IfEqual.Instance(-1, 12,
                            new QueuedBehavior(
                                new QueuedBehavior(
                                    Cooldown.Instance(5600),
                                    If.Instance(EntityLesserThan.Instance(60, 1, 0x0d51),
                                        new SetKey(-1, 7)
                                        )),
                                new QueuedBehavior(
                                    SetAltTexture.Instance(2),
                                    CooldownExact.Instance(1600),
                                    SetAltTexture.Instance(1)
                                    )
                                )
                            )
                        ,
                        IfEqual.Instance(-1, 13, SmoothWandering.Instance(0, 0)
                            ),
                        IfEqual.Instance(-1, 13, Flashing.Instance(300, 0xFF0055)
                            ),
                        IfEqual.Instance(-1, 13,
                            new QueuedBehavior(
                                SetAltTexture.Instance(3),
                                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                                new SimpleTaunt("NOOOOOOOOOOOO!"),
                                CooldownExact.Instance(5000),
                                RingAttack.Instance(8, 45, 45, 1),
                                CooldownExact.Instance(100),
                                Die.Instance
                                ))),
                                //condBehaviors: new ConditionalBehavior[]
                                //    {
                                //    new DeathPortal(0x5050, 10, 60)
                                //    },
                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 3, 0, 12,
                            Tuple.Create(0.005, (ILoot)new ItemLoot("Shield of Ogmur")),
                            Tuple.Create(0.025, (ILoot)new TierLoot(11, ItemType.Weapon)),
                            Tuple.Create(0.025, (ILoot)new TierLoot(12, ItemType.Armor)),
                            Tuple.Create(0.03, (ILoot)new TierLoot(11, ItemType.Armor)),
                            Tuple.Create(0.02, (ILoot)new TierLoot(5, ItemType.Ring)),
                            Tuple.Create(0.035, (ILoot)new TierLoot(10, ItemType.Weapon)),
                            Tuple.Create(0.035, (ILoot)new TierLoot(10, ItemType.Armor)),
                            Tuple.Create(0.045, (ILoot)new TierLoot(9, ItemType.Weapon)),
                            Tuple.Create(0.055, (ILoot)new TierLoot(5, ItemType.Ability)),
                            Tuple.Create(0.045, (ILoot)new TierLoot(9, ItemType.Armor)),
                            Tuple.Create(0.065, (ILoot)new TierLoot(4, ItemType.Ring)),
                            Tuple.Create(0.15, (ILoot)new TierLoot(4, ItemType.Ability)),
                            Tuple.Create(0.15, (ILoot)new TierLoot(8, ItemType.Armor)),
                            Tuple.Create(0.25, (ILoot)new TierLoot(8, ItemType.Weapon)),
                            Tuple.Create(0.25, (ILoot)new TierLoot(7, ItemType.Armor)),
                            Tuple.Create(0.25, (ILoot)new TierLoot(3, ItemType.Ring))
                            )),
                        Tuple.Create(100, new LootDef(1, 3, 1, 3,
                            Tuple.Create(0.04, (ILoot)new StatPotionsLoot(1, 2))
                            ))
                        )
                    ))
            .Init(0x0d51, Behaves("Protection Crystal",
                new RunBehaviors(
                    StrictCircling.Instance(4f, 0.9f, 0x0d50),
                    Cooldown.Instance(300, MultiAttack.Instance(8.5f, 5*(float) Math.PI/180, 4, projectileIndex: 0))
                    )
                ))
            .Init(0x0d52, Behaves("Knight of the Lost Lands",
                new RunBehaviors(
                    SimpleWandering.Instance(2),
                    Chasing.Instance(10, 9, 2, null),
                    Chasing.Instance(4, 4, 2, 0x0d53),
                    Cooldown.Instance(1500, MultiAttack.Instance(5, 5*(float) Math.PI/180, 1, projectileIndex: 0))
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(360, new LootDef(0, 2, 0, 2,
                        Tuple.Create(0.09, (ILoot) HpPotionLoot.Instance)
                        )))
                ))
            .Init(0x0d53, Behaves("Guardian of the Lost Lands",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)
                        ),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            Once.Instance(new SetKey(-2, 2)),
                            SimpleWandering.Instance(2),
                            Chasing.Instance(6, 9, 2, null),
                            Cooldown.Instance(3000, RingAttack.Instance(8, 15, 0, 1)),
                            Cooldown.Instance(1500,
                                MultiAttack.Instance(5, 5*(float) Math.PI/180, 5, projectileIndex: 0))
                            )
                        ),
                    IfEqual.Instance(-2, 2,
                        new QueuedBehavior(
                            Once.Instance(SpawnMinionImmediate.Instance(0x0d52, 2, 1, 4))
                            )))
                ));
    }
}