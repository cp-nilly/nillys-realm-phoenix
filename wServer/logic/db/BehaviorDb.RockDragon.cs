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
        private static _ RockDragon = Behav()
            .Init(0xfd9, Behaves("Rock Dragon",
                new RunBehaviors(
                    If.Instance(IsEntityPresent.Instance(15, null), Once.Instance(new SetKey(-1, 1))),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            SetAltTexture.Instance(6),
                            SpawnMinion.Instance(0xf98, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf65, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf64, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf63, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf62, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf61, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf60, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf59, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf58, 1, 1, 1, 1),
                            SpawnMinion.Instance(0xf57, 1, 1, 1, 1),
                            new SetKey(-1, 2),
                            Cooldown.Instance(1000)
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            SetSize.Instance(120),
                            Swirling.Instance(20, 10),
                            SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            Cooldown.Instance(350,
                                MultiAttack.Instance(10, 5*(float) Math.PI/180, 6, 0, projectileIndex: 0)),
                            If.Instance(EntityGroupLesserThan.Instance(40, 1, "Dragon Body"), new SetKey(-1, 3))
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            SmoothWandering.Instance(10, 7),
                            Cooldown.Instance(1200, SimpleAttack.Instance(8, 2)),
                            Cooldown.Instance(1400, RingAttack.Instance(16, 75, 75, 3)),
                            UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                            new QueuedBehavior(
                                Cooldown.Instance(200), SetAltTexture.Instance(0),
                                Cooldown.Instance(100), SetAltTexture.Instance(5),
                                Cooldown.Instance(110), SetAltTexture.Instance(4),
                                Cooldown.Instance(150), SetAltTexture.Instance(1),
                                Cooldown.Instance(160), SetAltTexture.Instance(2),
                                Cooldown.Instance(170), SetAltTexture.Instance(3),
                                Cooldown.Instance(180), SetAltTexture.Instance(2),
                                Cooldown.Instance(190), SetAltTexture.Instance(1)
                                ),
                            new QueuedBehavior(
                                CooldownExact.Instance(15000),
                                new SetKey(-1, 1)
                                )
                            )
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 2, 5,
                        Tuple.Create(0.10, (ILoot) new ItemLoot("Potion of Dexterity")),
                        Tuple.Create(0.10, (ILoot) new ItemLoot("Potion of Attack")),
                        Tuple.Create(0.10, (ILoot) new ItemLoot("Potion of Defense")),
                        Tuple.Create(0.10, (ILoot) new ItemLoot("Potion of Wisdom")),
                        Tuple.Create(0.10, (ILoot) new ItemLoot("Potion of Speed")),
                        Tuple.Create(0.10, (ILoot) new ItemLoot("Potion of Vitality"))
                        )))));
    }
}