using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.realm;
using wServer.logic.attack;
using wServer.logic.movement;
using wServer.logic.loot;
using wServer.logic.taunt;
using wServer.logic.cond;

namespace wServer.logic
{
  partial class BehaviorDb
  {
    static _ PhoenixGod = Behav()
      //4689799044661248 sleeping texture
      //6040272188211200 active texture
        .Init(0x2f62, Behaves("Phoenix God",
          new RunBehaviors(
            Once.Instance(new SetKey(-1, 1)),

            IfEqual.Instance(-1, 1,
              new RunBehaviors(
                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                Once.Instance(new SimpleTaunt("I have been summoned. Come, adventurers, and meet your despise."))
              )
            ),

            IfEqual.Instance(-1, 2,
              new RunBehaviors(
                Once.Instance(RingAttack.Instance(24, 0, 0, projectileIndex: 2)),
                Flashing.Instance(100, 0xffff0000),
                new QueuedBehavior(
                  CooldownExact.Instance(1200),
                  new SimpleTaunt("I have been in slumber for eons to meet you, {PLAYER}. Awe at my tremendous power as I burn you down to the ashes you shall be."),
                  CooldownExact.Instance(1100),
                  SetSize.Instance(100),
                  CooldownExact.Instance(1100),
                  SetSize.Instance(110),
                  CooldownExact.Instance(1100),
                  SetSize.Instance(120),
                  CooldownExact.Instance(1100),
                  SetSize.Instance(130),
                  CooldownExact.Instance(1100),
                  SetSize.Instance(140),
                  SetAltTexture.Instance(1),
                  RingAttack.Instance(16, 0, 0, projectileIndex: 5),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  new SetKey(-1, 3)
                )
              )),


            IfEqual.Instance(-1, 3,
              new RunBehaviors(
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                SmoothWandering.Instance(2f, 3f),
                Cooldown.Instance(800, SpawnMinionImmediate.Instance(0x2f63, 2.5f, 1, 2)),
                Cooldown.Instance(630, SpawnMinionImmediate.Instance(0x2f63, 2.5f, 1, 2)),
                new QueuedBehavior(
                  CooldownExact.Instance(12000),
                  new SetKey(-1, 4))
              )
            ),

            IfEqual.Instance(-1, 4,
              new RunBehaviors(
                Flashing.Instance(100, 0xff0000ff),
                SmoothWandering.Instance(2f, 4f),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                new QueuedBehavior(
                  SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  CooldownExact.Instance(1800),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  new SetKey(-1, 5)
                ))
            ),

          IfEqual.Instance(-1, 5,
            new RunBehaviors(
              SimpleWandering.Instance(0, 0),
              Cooldown.Instance(1000, MultiAttack.Instance(30, 8.5f * (float)Math.PI / 360, 4, 0, projectileIndex: 0)),
              InfiniteSpiralAttack.Instance(125, 5, 7.5f, projectileIndex: 4),
              UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
              SetConditionEffect.Instance(ConditionEffectIndex.Armored),
              new QueuedBehavior(
                CooldownExact.Instance(6000),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                new SetKey(-1, 6)
                )
              )),

            IfEqual.Instance(-1, 6,
              new RunBehaviors(
                Flashing.Instance(100, 0xff0000ff),
                SmoothWandering.Instance(2f, 4f),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                new QueuedBehavior(
                  CooldownExact.Instance(1800),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  CooldownExact.Instance(1),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  new SetKey(-1, 7)
                ))
            ),

            IfEqual.Instance(-1, 7,
              new RunBehaviors(
                MagicEye.Instance,
                SmoothWandering.Instance(0, 0),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                new QueuedBehavior(
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(1501),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(1501),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(1000),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(700),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(700),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(500),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(300),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(200),
                  TossEnemyAtPlayer.Instance(20, 0x2f64),
                  CooldownExact.Instance(1501),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  new SetKey(-1, 8)
                )
              )
            ),

            IfEqual.Instance(-1, 8,
              new RunBehaviors(
                Flashing.Instance(100, 0xff0000ff),
                SmoothWandering.Instance(2f, 4f),
                new QueuedBehavior(
                  CooldownExact.Instance(1800),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  CooldownExact.Instance(1),
                  UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                  new SetKey(-1, 9)
              ))
            ),

            IfEqual.Instance(-1, 9,
              new RunBehaviors(
                SmoothWandering.Instance(2f, 4f),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                InfiniteSpiralAttack.Instance(125, 3, -7.5f, projectileIndex: 1),
                InfiniteSpiralAttack.Instance(150, 3, 10f, projectileIndex: 4),
                new QueuedBehavior(
                  CooldownExact.Instance(7000),
                  new SetKey(-1, 10)
                )
              )
            ),

            IfEqual.Instance(-1, 10,
              new RunBehaviors(
                SmoothWandering.Instance(2, 3),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                Flashing.Instance(100, 0xff0000ff),
                new QueuedBehavior(
                  CooldownExact.Instance(1800),
                  new SetKey(-1, 11)
                )
              )
            ),

            IfEqual.Instance(-1, 11,
              new RunBehaviors(
                SimpleWandering.Instance(3, 5),
                Chasing.Instance(2.5f, 20, 2, null),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                InfiniteSpiralAttack.Instance(450, 10, 18, projectileIndex: 0),
                Cooldown.Instance(850, MultiAttack.Instance(20, 10 * (float)Math.PI / 360, 5, 0, projectileIndex: 3)),
                Cooldown.Instance(850, MultiAttack.Instance(20, 10 * (float)Math.PI / 360, 4, 0, projectileIndex: 6)),
                new QueuedBehavior(
                  new SimpleTaunt("Still not dead yet? I applaud your determination, but it will go no more!"),
                  CooldownExact.Instance(7800),
                  new SetKey(-1, 12)
                )
              )
            ),

            IfEqual.Instance(-1, 12,
              new RunBehaviors(
                ReturnSpawn.Instance(10),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                Flashing.Instance(100, 0xff0000ff),
                new QueuedBehavior(
                  CooldownExact.Instance(2200),
                  new SetKey(-1, 13)
                )
              )
            ),

            IfEqual.Instance(-1, 13,
              new RunBehaviors(
                UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                new QueuedBehavior(
                  new SimpleTaunt("Your powers just don't impress me."),
                  TossEnemyNull.Instance(0, 1.5f, 0x2f65),
                  CooldownExact.Instance(125),
                  TossEnemyNull.Instance(180 * (float)Math.PI / 180, 1.5f, 0x2f66),
                  CooldownExact.Instance(7000),
                  OrderGroup.Instance(99, "Phoenix God Encounter Phase 6", new SetKey(-1, 1)),
                  new SetKey(-1, 14)
                )
              )
            ),

            IfEqual.Instance(-1, 14,
              new RunBehaviors(
                SmoothWandering.Instance(2f, 3f),
                UnsetConditionEffect.Instance(ConditionEffectIndex.Armored),
                SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                Flashing.Instance(100, 0xff0000ff),
                new QueuedBehavior(
                  CooldownExact.Instance(2200),
                  new SetKey(-1, 15)
                )
              )
            ),

            IfEqual.Instance(-1, 15,
              new RunBehaviors(
                MagicEye.Instance,
                Flashing.Instance(100, 0xff00ff00),
                new QueuedBehavior(
                  new SimpleTaunt("You are no match for me. I shall bestow a worthy challenger upon you."),
                  If.Instance(EntityLesserThan.Instance(90, 3, 0x2f67), TossEnemyAtPlayer.Instance(50, 0x2f67)),
                  CooldownExact.Instance(1520),
                  new SetKey(-1, 3)
                )
              )
            )
          ),

          loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 3, 0, 8,
                        Tuple.Create(0.0035, (ILoot)new ItemLoot("Phoenix Hide Armor")),
                        Tuple.Create(0.0035, (ILoot)new ItemLoot("Robe of the Fire Bird")),
                        Tuple.Create(0.0035, (ILoot)new ItemLoot("Armor of Everlasting Life")),
                        Tuple.Create(0.0045, (ILoot)new ItemLoot("Luin of Celtchar")),
                        Tuple.Create(0.02, (ILoot)new TierLoot(11, ItemType.Weapon)),
                        Tuple.Create(0.02, (ILoot)new TierLoot(12, ItemType.Armor)),
                        Tuple.Create(0.04, (ILoot)new TierLoot(11, ItemType.Armor)),
                        Tuple.Create(0.02, (ILoot)new TierLoot(5, ItemType.Ring)),
                        Tuple.Create(0.04, (ILoot)new TierLoot(10, ItemType.Weapon)),
                        Tuple.Create(0.05, (ILoot)new TierLoot(10, ItemType.Armor)),
                        Tuple.Create(0.05, (ILoot)new TierLoot(9, ItemType.Weapon)),
                        Tuple.Create(0.06, (ILoot)new TierLoot(5, ItemType.Ability)),
                        Tuple.Create(0.08, (ILoot)new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.05, (ILoot)new TierLoot(4, ItemType.Ring)),
                        Tuple.Create(0.1, (ILoot)new TierLoot(4, ItemType.Ability)),
                        Tuple.Create(0.08, (ILoot)new TierLoot(8, ItemType.Weapon))
                        )),
                    Tuple.Create(360, new LootDef(0, 2, 0, 3,
                        Tuple.Create(1.00, (ILoot)new StatPotionsLoot(1, 2))
                    ))
                    ),

          condBehaviors: new ConditionalBehavior[]
        	        {
        	          new OnHit(
        	        	new RunBehaviors(
        	        	  IfEqual.Instance(-1,1,
        	        		Once.Instance(new SetKey(-1,2))
        	        	  ))),
                          
                        new DeathPortal(0x2fbb, 30, 30),
                          
                      new OnDeath(
                        new RunBehaviors(
                          OrderGroup.Instance(90, "Phoenix God Encounter Phase 6", new SetKey(-1,1)),
                          Once.Instance(new SimpleTaunt("Oh, and to think that I was getting started...")))
                        )
        	        }
              ))

        .Init(0x2f63, Behaves("PhoenixGodPhase1",
           new RunBehaviors(
             SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
             new QueuedBehavior(
               CooldownExact.Instance(10),
               RingAttack.Instance(10, 20, 0, projectileIndex: 0),
               If.Instance(IsEntityNotPresent.Instance(25, null), RingAttack.Instance(10, 0, 0, projectileIndex: 0)),
               Despawn.Instance))
          ))

        .Init(0x2f64, Behaves("PhoenixGodPhase3",
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
                 If.Instance(IsEntityNotPresent.Instance(100, 0x2f62), new SetKey(-1, 3)),
                 new QueuedBehavior(
                   CooldownExact.Instance(40000),
                   new SetKey(-1, 3))
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

         .Init(0x2f65, Behaves("PhoenixGodPhase6Red",
            new RunBehaviors(
              MagicEye.Instance,
              Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
              InfiniteSpiralAttack.Instance(250, 12, 5, projectileIndex: 0),
              If.Instance(IsEntityNotPresent.Instance(100, 0x2f62), new SetKey(-1, 1)),

              IfEqual.Instance(-1, 1,
                Despawn.Instance
              )
            )
          ))

          .Init(0x2f66, Behaves("PhoenixGodPhase6Blue",
            new RunBehaviors(
              MagicEye.Instance,
              Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
              InfiniteSpiralAttack.Instance(250, 12, -5, projectileIndex: 0),
              If.Instance(IsEntityNotPresent.Instance(100, 0x2f62), new SetKey(-1, 1)),

              IfEqual.Instance(-1, 1,
                Despawn.Instance
              )
            )
          ))

          .Init(0x2f67, Behaves("Unstable Fire Bird",
             new RunBehaviors(
               Once.Instance(new SetKey(-1, 1)),

               IfEqual.Instance(-1, 1,
                 new RunBehaviors(
                   SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                   Flashing.Instance(75, 0xffffffff),
                   new QueuedBehavior(
                     new SimpleTaunt("I have been chosen by my god. Fear me, for I am mighty."),
                     SetSize.Instance(60),
                     CooldownExact.Instance(200),
                     SetSize.Instance(70),
                     CooldownExact.Instance(200),
                     SetSize.Instance(80),
                     CooldownExact.Instance(200),
                     SetSize.Instance(90),
                     CooldownExact.Instance(200),
                     SetSize.Instance(100),
                     CooldownExact.Instance(200),
                     SetSize.Instance(110),
                     CooldownExact.Instance(200),
                     SetSize.Instance(125),
                     CooldownExact.Instance(200),
                     new SetKey(-1, 2)
                   )
                 )
               ),

               IfEqual.Instance(-1, 2,
                 new RunBehaviors(
                   UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                   SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                   new QueuedBehavior(
                     CooldownExact.Instance(1),
                     new SetKey(-1, 3)
                   )
                 )
               ),

               IfEqual.Instance(-1, 3,
                 new RunBehaviors(
                   Flashing.Instance(100, 0xff0000ff),
                   InfiniteSpiralAttack.Instance(250, 2, 24, projectileIndex: 1),
                   new QueuedBehavior(
                     CooldownExact.Instance(3751),
                     new SetKey(-1, 4)
                   )
                 )
               ),

               IfEqual.Instance(-1, 4,
                 new RunBehaviors(
                   Flashing.Instance(100, 0xff00ff00),
                   Chasing.Instance(3.5f, 30, 2, null),
                   Cooldown.Instance(900, MultiAttack.Instance(20, 12.5f * (float)Math.PI / 360, 5, 0, projectileIndex: 0)),
                   new QueuedBehavior(
                     CooldownExact.Instance(8000),
                     new SetKey(-1, 3)
                   )
                 )
               )

             )
           ));
  }
}
