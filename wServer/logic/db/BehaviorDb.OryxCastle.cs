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
    private static _ Castle = Behav() 
        .Init(0x0d78, Behaves("Oryx Stone Guardian Right", 
          new RunBehaviors(
            Once.Instance(new SetKey(-1, 0)),

            IfEqual.Instance(-1, 0,
              new RunBehaviors(
                Once.Instance(SetAltTexture.Instance(1)),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)
              )
            ),

            IfEqual.Instance(-1, 1, 
              new RunBehaviors(
                Once.Instance(OrderEntity.Instance(10, 0xd79, new SetKey(-1, 1))),
                Cooldown.Instance(1500, RingAttack.Instance(12, 0, 0, projectileIndex: 2)),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  Cooldown.Instance(3500),
                  new SetKey(-1, 2)
                )
              )
            ),

            IfEqual.Instance(-1, 2, 
              new RunBehaviors(
                Cooldown.Instance(3000, ThrowAttack.Instance(4, 20, 130)),
                new QueuedBehavior(
                  AngleMove.Instance(12f, 90 * (float)Math.PI / 180, 13),
                  CooldownExact.Instance(2000),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance((float)Math.PI, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  CooldownExact.Instance(1500),
                  new SetKey(-1, 3)
                )
              )
            ),

            IfEqual.Instance(-1, 3,
              new RunBehaviors(
                ReturnSpawn.Instance(15),
                new QueuedBehavior(
                  Cooldown.Instance(2000),
                  TossEnemyAtPlayer.Instance(15, 0x0d7e),
                  Cooldown.Instance(750),
                  TossEnemyAtPlayer.Instance(15, 0x0d7e),
                  Cooldown.Instance(750),
                  new SetKey(-1, 4)
                )
              )
            ),

            IfEqual.Instance(-1, 4,
              new RunBehaviors(
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                Chasing.Instance(6, 40, 0, null),
                Cooldown.Instance(1000, RingAttack.Instance(12, 0, 0, projectileIndex: 2)),
                Cooldown.Instance(1800, MultiAttack.Instance(30, 25 * (float)Math.PI / 360, 6, 0, projectileIndex: 1)),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  CooldownExact.Instance(12000),
                  new SetKey(-1, 5)
                )
              )
            ),

            IfEqual.Instance(-1, 5,
              new RunBehaviors(
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                Chasing.Instance(8, 50, 0, null),
                TimedToggleTexture.Instance(1000, 3, 4),
                InfiniteSpiralAttack.Instance(500, 4, 20, projectileIndex: 4),
                InfiniteSpiralAttack.Instance(500, 4, -20, projectileIndex: 4),
                new QueuedBehavior(
                  CooldownExact.Instance(12001),
                  new SetKey(-1, 6)
                )
              )
            ),

            IfEqual.Instance(-1, 6,
              new RunBehaviors(
                ReturnSpawn.Instance(20),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  CooldownExact.Instance(5000),
                  new SetKey(-1, 7)
                )
              )
            ),

            IfEqual.Instance(-1, 7,
              new RunBehaviors(
                TimedToggleTexture.Instance(1000, 3, 4),
                TimedInfSpiralAttack.Instance(500, 250, 4, 45, 0, projectileIndex: 4),
                new QueuedBehavior(
                  AngleMove.Instance(12f, 90 * (float)Math.PI / 180, 13),
                  AngleMove.Instance(3.5f, 135 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(3.5f, 225 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(3.5f, 315 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(3.5f, 45 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(8, 0, 10),
                  CooldownExact.Instance(2000),
                  new SetKey(-1, 8)
                )
              )
            ),

            IfEqual.Instance(-1, 8,
              new RunBehaviors(
                InfiniteSpiralAttack2.Instance(500, 1, -10, 270, projectileIndex: 4),
                InfiniteSpiralAttack2.Instance(500, 1, 10, 90, projectileIndex: 4),
                InfiniteSpiralAttack2.Instance(500, 1, -10, 270, projectileIndex: 5),
                InfiniteSpiralAttack2.Instance(500, 1, 10, 90, projectileIndex: 5),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  CooldownExact.Instance(5001),
                  new SetKey(-1, 9)
                )
              )
            ),

            IfEqual.Instance(-1, 9,
              new RunBehaviors(
                UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                ReturnSpawn.Instance(20),
                new QueuedBehavior(
                  CooldownExact.Instance(3000),
                  new SetKey(-1, 2)
                )
              )
            )
          ),

          loot: new LootBehavior(LootDef.Empty,
            Tuple.Create(100, new LootDef(0, 5, 0, 9,
              Tuple.Create(0.009, (ILoot)new ItemLoot("Void Incantation")),
              Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Def)),
              Tuple.Create(0.009, (ILoot)new ItemLoot("Ancient Stone Sword")),
              Tuple.Create(0.009, (ILoot)new ItemLoot("Wine Cellar Incantation"))
            ))
          ),

          condBehaviors: new ConditionalBehavior[] {
                new OnHit(
                  new RunBehaviors(
                    IfEqual.Instance(-1,0, Once.Instance(new SetKey(-1,1))),
                    IfBetween.Instance(-1,-1, 2, Once.Instance(OrderEntity.Instance(10, 0xd79, new SetKey(-1,1))))
                  )
                )
              }
        ))

        .Init(0x0d79, Behaves("Oryx Stone Guardian Left",
          new RunBehaviors(
            Once.Instance(new SetKey(-1, 0)),

            IfEqual.Instance(-1, 0,
              new RunBehaviors(
                Once.Instance(SetAltTexture.Instance(1)),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)
              )
            ),

            IfEqual.Instance(-1, 1, 
              new RunBehaviors(
                Once.Instance(OrderEntity.Instance(10, 0xd78, new SetKey(-1, 1))),
                Cooldown.Instance(1500, RingAttack.Instance(12, 0, 0, projectileIndex: 2)),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  Cooldown.Instance(3500),
                  new SetKey(-1, 2)
                )
              )
            ),

            IfEqual.Instance(-1, 2,
              new RunBehaviors(
                Cooldown.Instance(3000, ThrowAttack.Instance(4, 20, 130)),
                new QueuedBehavior(
                  AngleMove.Instance(12f, 90 * (float)Math.PI / 180, 13),
                  CooldownExact.Instance(2000),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  Cooldown.Instance(500, AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 1)),
                  AngleMultiAttack.Instance(0, 21.25f * (float)Math.PI / 360, 8, projectileIndex: 3),
                  CooldownExact.Instance(1500),
                  new SetKey(-1, 3)
                )
              )
            ),

            IfEqual.Instance(-1, 3,
              new RunBehaviors(
                ReturnSpawn.Instance(15),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                new QueuedBehavior(
                  Cooldown.Instance(2000),
                  TossEnemyAtPlayer.Instance(15, 0x0d7e),
                  Cooldown.Instance(750),
                  TossEnemyAtPlayer.Instance(15, 0x0d7e),
                  Cooldown.Instance(750),
                  new SetKey(-1, 4)
                )
              )
            ),

            IfEqual.Instance(-1, 4,
              new RunBehaviors(
                TimedToggleTexture.Instance(1000, 3, 4),
                InfiniteSpiralAttack.Instance(500, 4, 20, projectileIndex: 4),
                InfiniteSpiralAttack.Instance(500, 4, -20, projectileIndex: 4),
                Cooldown.Instance(400, PredictiveAttack.Instance(40, 5, projectileIndex: 4)),
                new QueuedBehavior(
                  CooldownExact.Instance(12001),
                  new SetKey(-1, 5)
                )
              )
            ),

            IfEqual.Instance(-1, 5,
              new RunBehaviors(
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                Cooldown.Instance(1250, PredictiveRingAttack.Instance(30, 5, 50, projectileIndex: 4)),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  CooldownExact.Instance(12001),
                  new SetKey(-1, 6)
                )
              )
            ),

            IfEqual.Instance(-1, 6,
              new RunBehaviors(
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                ReturnSpawn.Instance(20),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  CooldownExact.Instance(5000),
                  new SetKey(-1, 7)
                )
              )
            ),

            IfEqual.Instance(-1, 7,
              new RunBehaviors(
                TimedToggleTexture.Instance(1000, 3, 4),
                TimedInfSpiralAttack.Instance(250, 1000, 4, 45, 0, projectileIndex: 4),
                new QueuedBehavior(
                  AngleMove.Instance(12f, 90 * (float)Math.PI / 180, 13),
                  AngleMove.Instance(3.5f, 315 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(3.5f, 45 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(3.5f, 135 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(3.5f, 225 * (float)Math.PI / 180, 3),
                  AngleMove.Instance(8, (float)Math.PI, 10),
                  CooldownExact.Instance(2000),
                  new SetKey(-1, 8)
                )
              )
            ),

            IfEqual.Instance(-1, 8,
              new RunBehaviors(
                InfiniteSpiralAttack2.Instance(500, 1, 10, 270, projectileIndex: 4),
                InfiniteSpiralAttack2.Instance(500, 1, -10, 90, projectileIndex: 4),
                InfiniteSpiralAttack2.Instance(500, 1, 10, 270, projectileIndex: 5),
                InfiniteSpiralAttack2.Instance(500, 1, -10, 90, projectileIndex: 5),
                new QueuedBehavior(
                  SetAltTexture.Instance(0),
                  CooldownExact.Instance(5001),
                  new SetKey(-1, 9)
                )
              )
            ),

            IfEqual.Instance(-1, 9,
              new RunBehaviors(
                UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                ReturnSpawn.Instance(20),
                new QueuedBehavior(
                  CooldownExact.Instance(3000),
                  new SetKey(-1, 2)
                )
              )
            )
          ),

          loot: new LootBehavior(LootDef.Empty,
            Tuple.Create(100, new LootDef(0, 5, 0, 9,
              Tuple.Create(0.009, (ILoot)new ItemLoot("Void Incantation")),
              Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Def)),
              Tuple.Create(0.009, (ILoot)new ItemLoot("Ancient Stone Sword")),
              Tuple.Create(0.009, (ILoot)new ItemLoot("Wine Cellar Incantation"))
            ))
          ),

          condBehaviors: new ConditionalBehavior[] {
                new OnHit(
                  new RunBehaviors(
                    IfEqual.Instance(-1,0, Once.Instance(new SetKey(-1,1))),
                    IfBetween.Instance(-1,-1, 2, Once.Instance(OrderEntity.Instance(10, 0xd78, new SetKey(-1,1))))
                  )
                )
              }
        ))

        .Init(0x0d7a, Behaves("Oryx Guardian TaskMaster",
          new RunBehaviors(
            SetConditionEffect.Instance(ConditionEffectIndex.Invisible),
            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
            If.Instance(IsEntityNotPresent.Instance(999, 0x0d78), 
              If.Instance(IsEntityNotPresent.Instance(999, 0x0d79),
                Die.Instance
              )
            )
          ),

          condBehaviors: new ConditionalBehavior[]
          {
            new DeathPortal(0x0d7b, 100, -1)
          }
        ))



        //Minions now!
        .Init(0x0d82, Behaves("Oryx Brute",
            new RunBehaviors(
                Cooldown.Instance(2500, new SetConditionEffectTimed(ConditionEffectIndex.Armored, 1250)),
                SimpleWandering.Instance(1, 1f),
                Chasing.Instance(6, 6, 0, null),
                RandomDelay2.Instance(100, 150, SimpleAttack.Instance(3, projectileIndex: 0)),
                Cooldown.Instance(12000, If.Instance(IsEntityPresent.Instance(10, null), SpawnMinionImmediate.Instance(0x0d81, 1, 4, 4)))
                ),
            loot: new LootBehavior(LootDef.Empty,
                Tuple.Create(100, new LootDef(0, 5, 0, 2,
                    Tuple.Create(0.1, (ILoot) new TierLoot(3, ItemType.Misc)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Att)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Def)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Dex)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Spd)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Vit)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Wis))
                    )))
            ))

        .Init(0x0d71, Behaves("Suit of Armor",
            loot: new LootBehavior(LootDef.Empty,
                Tuple.Create(100, new LootDef(0, 5, 0, 8,
                    Tuple.Create(0.001, (ILoot)MpPotionLoot.Instance),
                    Tuple.Create(0.001, (ILoot)HpPotionLoot.Instance)
                    )))
            ))
        .Init(0x0d72, Behaves("Suit of Armor Sm",
            loot: new LootBehavior(LootDef.Empty,
                Tuple.Create(100, new LootDef(0, 5, 0, 8,
                    Tuple.Create(0.001, (ILoot)MpPotionLoot.Instance),
                    Tuple.Create(0.001, (ILoot)HpPotionLoot.Instance)
                    )))
            ))
        .Init(0x0d7d, Behaves("Suit of Armor Spawner",
            new RunBehaviors(
              Once.Instance(new SetKey(-1, 0)),
              SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
              SetConditionEffect.Instance(ConditionEffectIndex.Invisible),
              IfEqual.Instance(-1, 0,
                new RunBehaviors(
                  SpawnMinionImmediate.Instance(0x0d7e, 0, 1, 1),
                  new SetKey(-1, 1)
                )
              ),
              IfEqual.Instance(-1, 1,
                new RunBehaviors(
                  Cooldown.Instance(2000, If.Instance(IsEntityNotPresent.Instance(1, 0x0d7e), new SetKey(-1, 2)))
                )
              ),
              IfEqual.Instance(-1, 2,
                new RunBehaviors(
                  Cooldown.Instance(35000, new SetKey(-1, 0))
                )
              )
            )
        ))
        .Init(0x0d7e, Behaves("Oryx Suit of Armor",
            new RunBehaviors(
              Once.Instance(new SetKey(-1, 0)),

              IfEqual.Instance(-1, 0,
                new RunBehaviors(
                  If.Instance(IsEntityPresent.Instance(6.5f, null), new SetKey(-1, 1))
                )
              ),

              IfEqual.Instance(-1, 1,
                new RunBehaviors(
                  SetAltTexture.Instance(1),
                  Cooldown.Instance(450, MultiAttack.Instance(12.5f, 30 * (float)Math.PI / 360, 3, 0, projectileIndex: 0)),
                  Chasing.Instance(4, 10, 1, null),
                  HpLesser.Instance(2000, Once.Instance(new SetKey(-1, 2)))
                )
              ),

              IfEqual.Instance(-1, 2,
                new RunBehaviors(
                  SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  Cooldown.Instance(999, RingAttack.Instance(6)),
                  Cooldown.Instance(999, Heal.Instance(0.1f, 1100, 0x0d7e)),
                  Cooldown.Instance(3003, new SetKey(-1, 3))
                )
              ),

              IfEqual.Instance(-1, 3,
                new RunBehaviors(
                  SetAltTexture.Instance(1),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                  Cooldown.Instance(350, MultiAttack.Instance(12.5f, 30 * (float)Math.PI / 360, 4, 0, projectileIndex: 0)),
                  Chasing.Instance(5, 10, 1, null)
                )
              )
            ),
            loot: new LootBehavior(LootDef.Empty,
                Tuple.Create(100, new LootDef(0, 5, 0, 8,
                    Tuple.Create(0.05, (ILoot)MpPotionLoot.Instance),
                    Tuple.Create(0.05, (ILoot)HpPotionLoot.Instance)
                    )))
            ))
        .Init(0x0d7f, Behaves("Oryx Insect Commander",
            new RunBehaviors(
                SimpleWandering.Instance(1, 1f),
                Cooldown.Instance(500, SimpleAttack.Instance(2, projectileIndex: 0)),
                Once.Instance(SpawnMinionImmediate.Instance(0x0d80, 1, 10, 10)),
                If.Instance(EntityLesserThan.Instance(10, 10, 0x0d80), SpawnMinionImmediate.Instance(0x0d80, 5, 4, 5))
                ),
            loot: new LootBehavior(LootDef.Empty,
                Tuple.Create(100, new LootDef(0, 5, 0, 2,
      //Tuple.Create(0.1, (ILoot) new TierLoot(3, ItemType.Misc)
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Att)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Def)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Dex)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Spd)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Vit)),
                    Tuple.Create(0.02, (ILoot)new StatPotionLoot(StatPotion.Wis))
                    )))
            ))
        .Init(0x0d80, Behaves("Oryx Insect Minion",
            new RunBehaviors(
                Cooldown.Instance(1575, PredictiveRingAttack.Instance(7, 5, 20, 0)),
                Once.Instance(new SetKey(-1, 1)),

                IfEqual.Instance(-1, 0,
                  new RunBehaviors(
                    Chasing.Instance(8.5f, 20, 0, null),
                    Cooldown.Instance(675, PredictiveRingAttack.Instance(7, 5, 20, 1)),
                    Cooldown.Instance(4000, new SetKey(-1, 1))
                  )
                ),

                IfEqual.Instance(-1, 1,
                  new RunBehaviors(
                    Circling.Instance(5.5f, 20, 8.5f, 0x0d7f),
                    Cooldown.Instance(5000, If.Instance(IsEntityPresent.Instance(15, null), new SetKey(-1, 0)))
                  )
                )
             ),
            loot: new LootBehavior(LootDef.Empty,
                Tuple.Create(100, new LootDef(0, 5, 0, 8,
                    Tuple.Create(0.008, (ILoot)MpPotionLoot.Instance),
                    Tuple.Create(0.008, (ILoot)HpPotionLoot.Instance)
                    )))
            ))
        .Init(0x0d81, Behaves("Oryx Eye Warrior",
            new RunBehaviors(
              Once.Instance(new SetKey(-1, 0)),

              IfEqual.Instance(-1, 0,
                new RunBehaviors(
                  SimpleWandering.Instance(2, 1.5f),
                  Chasing.Instance(7.5f, 6, 0, null),
                  Cooldown.Instance(400, MultiAttack.Instance(5, 45 * (float)Math.PI / 360, 3, 0, projectileIndex: 0)),
                  Cooldown.Instance(4500, new SetKey(-1, 1))
                )
              ),

              IfEqual.Instance(-1, 1,
                new RunBehaviors(
                  SimpleWandering.Instance(2, 1.5f),
                  Charge.Instance(7, 10, null),
                  Cooldown.Instance(400, MultiAttack.Instance(6, 45 * (float)Math.PI / 360, 3, 0, projectileIndex: 0)),
                  Cooldown.Instance(400, RingAttack.Instance(10, 4, 0, projectileIndex: 1)),
                  Cooldown.Instance(2200, new SetKey(-1, 0))
                )
              )
            ),
            loot: new LootBehavior(LootDef.Empty,
                Tuple.Create(100, new LootDef(0, 5, 0, 8,
                    Tuple.Create(0.045, (ILoot)MpPotionLoot.Instance),
                    Tuple.Create(0.045, (ILoot)HpPotionLoot.Instance)
                    )))
            ))
        .Init(0x0d86, Behaves("Quiet Bomb",
            new RunBehaviors(
                Cooldown.Instance(1000, RingAttack.Instance(40, 0, projectileIndex: 0)),
                Cooldown.Instance(1020, Despawn.Instance)
                )
            ))

        .Init(0x0d83, Behaves("Oryx Knight",
            new RunBehaviors(
              If.Instance(IsEntityPresent.Instance(10, null), Once.Instance(new SetKey(-1, 0))),

              IfEqual.Instance(-1, 0,
                new RunBehaviors(
                  Chasing.Instance(10.5f, 5, 1, null),
                  Cooldown.Instance(900, MultiAttack.Instance(10, 20 * (float)Math.PI / 360, 6, 0, projectileIndex: 0)),
                  Cooldown.Instance(500, SimpleAttack.Instance(0, projectileIndex: 1)),
                  Cooldown.Instance(1250, RingAttack.Instance(8, 6, 0, projectileIndex: 2)),
                  Cooldown.Instance(7501, If.Instance(IsEntityPresent.Instance(10, null), new SetKey(-1, 1)))
                )
              ),

              IfEqual.Instance(-1, 1,
                new RunBehaviors(
                  Chasing.Instance(3, 6, 1, null),
                  Cooldown.Instance(700, MultiAttack.Instance(10, 10 * (float)Math.PI / 360, 5, 0, projectileIndex: 3)),
                  Cooldown.Instance(500, SimpleAttack.Instance(0, projectileIndex: 1)),
                  Cooldown.Instance(1250, RingAttack.Instance(8, 6, 0, projectileIndex: 2)),
                  Cooldown.Instance(7401, If.Instance(IsEntityPresent.Instance(10, null), SpawnMinionImmediate.Instance(0x0d84, 1, 3, 4))),
                  Cooldown.Instance(7501, new SetKey(-1, 0))
                )
              )
            )
        ))

        .Init(0x0d84, Behaves("Oryx Pet",
            new RunBehaviors(
              SimpleWandering.Instance(2, 2f),
              Chasing.Instance(9.5f, 16, 0, null),
              Cooldown.Instance(400, RingAttack.Instance(1, 2, projectileIndex: 0))
            )
        ))
        .Init(0x0d87, Behaves("Oryx's Living Floor",
            new RunBehaviors(
                Cooldown.Instance(3500, TossEnemyAtPlayer.Instance(20, 0x0d86)),
                Cooldown.Instance(1500, RingAttack.Instance(20, 10, 0, projectileIndex: 0))
                )
            ));
  }
}