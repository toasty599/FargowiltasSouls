using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class QueenSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenSlimeBoss);

        public int StompTimer;
        public int StompCounter;
        public int RainTimer;

        public float StompVelocityX;
        public float StompVelocityY;

        public bool DroppedSummon;

        private const float StompTravelTime = 40;
        private const float StompGravity = 1.6f;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(StompTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(StompCounter), IntStrategies.CompoundStrategy },
                { new Ref<object>(RainTimer), IntStrategies.CompoundStrategy },

                { new Ref<object>(StompVelocityX), FloatStrategies.CompoundStrategy },
                { new Ref<object>(StompVelocityY), FloatStrategies.CompoundStrategy },
            };

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.queenSlimeBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            //ai0
            //0 = default
            //3 = chase?
            //4 = stomp
            //5 = shooty gels

            if (npc.life < npc.lifeMax / 2) //phase 2
            {
                if (RainTimer < 0)
                    RainTimer++;

                if (StompTimer < 0)
                    StompTimer++;

                if (npc.ai[0] == 0) //basic flying ai
                {
                    if (RainTimer == 0)
                    {
                        npc.position += npc.velocity;

                        npc.ai[1] -= 1; //dont progress to next ai

                        if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center - 450 * Vector2.UnitY) < 200)
                        {
                            RainTimer = 1; //begin attack
                            NetSync(npc);

                            npc.netUpdate = true;

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, npc.Center, 0);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -16);
                        }
                        
                    }
                    else if (RainTimer > 0) //actually doing rain
                    {
                        npc.velocity *= 0.9f;

                        npc.ai[1] -= 1f; //dont progress ai

                        RainTimer++;

                        const int delay = 40;
                        const int timeBeforeStreamsMove = 60;
                        const int maxAttackTime = 360;
                        int attackTimer = RainTimer - delay - timeBeforeStreamsMove;
                        if (attackTimer < 0)
                            attackTimer = 0;

                        if (RainTimer > delay && RainTimer < delay + maxAttackTime && RainTimer % 3 == 0)
                        {
                            const float maxWavy = 160;
                            Vector2 focusPoint = npc.Center;
                            focusPoint.X += maxWavy * (float)Math.Sin(Math.PI * 2f / maxAttackTime * attackTimer * 1.5f);
                            focusPoint.Y -= 400;

                            for (int i = -4; i <= 4; i++)
                            {
                                Vector2 spawnPos = focusPoint + Main.rand.NextVector2Circular(16, 16);
                                spawnPos.X += 300 * i;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), spawnPos, 9f * Vector2.UnitY,
                                      ProjectileID.QueenSlimeMinionBlueSpike, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                }
                            }
                        }

                        bool endAttack = RainTimer > delay + maxAttackTime + 90;
                        if (npc.Distance(Main.player[npc.target].Center) > 1200)
                        {
                            endAttack = true;

                            StompTimer = 0;
                            StompCounter = -3; //enraged super stomps
                            npc.ai[0] = 4f;
                            npc.ai[1] = 0f;
                        }

                        if (endAttack)
                        {
                            RainTimer = -666 * 2;
                            npc.netUpdate = true;
                            NetSync(npc);
                        }
                    }
                    else
                    {
                        npc.ai[1] += 1; //proceed to next ais faster
                    }
                }
                else if (npc.ai[0] == 4) //stompy
                {
                    if (StompTimer == 0) //ready to super stomp
                    {
                        StompTimer = 1;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, NPCID.WallofFleshEye);

                        npc.netUpdate = true;
                        NetSync(npc);
                        return false;
                    }
                    else if (StompTimer > 0 && StompTimer < 30) //give time to react
                    {
                        npc.velocity = Vector2.Zero;
                        npc.rotation = 0;
                        StompTimer++;
                        return false;
                    }
                    else if (StompTimer == 30)
                    {
                        if (StompCounter++ < 3)
                        {
                            StompTimer++;

                            Vector2 distance = Main.player[npc.target].Top - npc.Bottom;
                            float time = StompTravelTime;
                            if (StompCounter < 0) //enraged
                            {
                                time /= 2;
                                //distance += Main.player[npc.target].velocity * time;
                            }
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * StompGravity * time;
                            StompVelocityX = distance.X;
                            StompVelocityY = distance.Y;

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                            npc.netUpdate = true;
                            NetSync(npc);
                            return false;
                        }
                        else //done enough stomps
                        {
                            StompCounter = 0;
                            StompTimer = -480;

                            npc.ai[1] = 2000f; //proceed to next thing immediately
                            npc.netUpdate = true;
                            NetSync(npc);
                        }
                    }
                    else if (StompTimer > 0)
                    {
                        npc.rotation = 0;
                        npc.noTileCollide = true;

                        float time = StompTravelTime;
                        if (StompCounter < 0) //enraged
                            time /= 2;

                        if (++StompTimer > time)
                        {
                            npc.noTileCollide = false;
                            //when landed on a surface
                            if (npc.velocity.Y == 0 || StompTimer > time * 2)
                            {
                                StompTimer = 30;
                                //if enraged
                                if (StompCounter < 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    const int max = 3;
                                    for (int i = -max; i <= max; i++)
                                        Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, 12f * -Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / max * i), ProjectileID.QueenSlimeGelAttack, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 1.25f), 0f, Main.myPlayer);
                                }
                            }
                        }

                        npc.velocity.X = StompVelocityX;
                        npc.velocity.Y = StompVelocityY;
                        StompVelocityY += StompGravity;

                        return false;
                    }
                }
                else if (npc.ai[0] == 5) //when shooting, be careful to stay above player
                {
                    if (npc.HasValidTarget && npc.Bottom.Y > Main.player[npc.target].Top.Y - 80 && npc.velocity.Y > -8f)
                        npc.velocity.Y -= 0.8f;
                }
            }

            //FargoSoulsUtil.PrintAI(npc);

            EModeUtils.DropSummon(npc, "JellyCrystal", NPC.downedQueenSlime, ref DroppedSummon, Main.hardMode);

            return true;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<QUEENSLIMEACCESSORY>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            //LoadNPCSprite(recolor, npc.type);
            //LoadBossHeadSprite(recolor, 37);
            //LoadGoreRange(recolor, 1262, 1268);
            //extra_177, 180, 185, 186
        }
    }

    public class CrystalSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenSlimeMinionBlue);

        public bool TimeToFly;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(TimeToFly), BoolStrategies.CompoundStrategy },
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.localAI[0] += 0.5f;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss))
            {
                if (TimeToFly)
                {
                    npc.velocity = npc.velocity.Length() * npc.DirectionTo(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center);
                    npc.position += 8f * npc.DirectionTo(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center);

                    if (npc.Distance(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center) < 300f)
                    {
                        TimeToFly = false;
                        NetSync(npc);

                        npc.velocity += 8f * npc.DirectionTo(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center).RotatedByRandom(MathHelper.PiOver4);
                        npc.netUpdate = true;
                    }
                }
                else if (npc.Distance(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center) > 900f)
                {
                    TimeToFly = true;
                    NetSync(npc);
                }
            }
            else
            {
                TimeToFly = false;
            }

            npc.noTileCollide = TimeToFly;

            if (npc.velocity.Y != 0)
                npc.localAI[0] = 25f;
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            //if (Main.netMode != NetmodeID.MultiplayerClient)
            //{
            //    for (int i = -2; i <= 2; i++)
            //    {
            //        Vector2 vel = -12f * Vector2.UnitY.RotatedBy(MathHelper.ToRadians(10) * i);
            //        Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, vel, ProjectileID.QueenSlimeMinionBlueSpike, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
            //    }
            //}
        }
    }

    public class BouncySlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenSlimeMinionPink);

        public bool TimeToFly;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(TimeToFly), BoolStrategies.CompoundStrategy },
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.localAI[0] += 0.5f;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss))
            {
                if (TimeToFly)
                {
                    npc.velocity = npc.velocity.Length() * npc.DirectionTo(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center);
                    npc.position += 8f * npc.DirectionTo(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center);

                    if (npc.Distance(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center) < 300f)
                    {
                        TimeToFly = false;
                        NetSync(npc);

                        npc.velocity += 8f * npc.DirectionTo(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center).RotatedByRandom(MathHelper.PiOver4);
                        npc.netUpdate = true;
                    }
                }
                else if (npc.Distance(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center) > 900f)
                {
                    TimeToFly = true;
                    NetSync(npc);
                }
            }
            else
            {
                TimeToFly = false;
            }

            npc.noTileCollide = TimeToFly;

            if (npc.velocity.Y != 0)
                npc.localAI[0] = 25f;
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            //if (Main.netMode != NetmodeID.MultiplayerClient)
            //    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, -12f * Vector2.UnitY, ProjectileID.QueenSlimeMinionPinkBall, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
        }
    }

    public class HeavenlySlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenSlimeMinionPink);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //FargoSoulsUtil.PrintAI(npc);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);


        }
    }
}
