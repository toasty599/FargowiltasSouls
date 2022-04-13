using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
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
        public int SpikeCounter;

        public float StompVelocityX;
        public float StompVelocityY;

        public bool SpawnedMinions1;
        public bool SpawnedMinions2;
        public int RainDirection;

        public bool DroppedSummon;

        private const float StompTravelTime = 60;
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

            void TrySpawnMinions(ref bool check, double threshold)
            {
                if (!check && npc.life < npc.lifeMax * threshold)
                {
                    check = true;

                    FargoSoulsUtil.PrintText("Gelatin Subjects have awoken!", new Color(175, 75, 255));

                    for (int i = 0; i < 6; i++)
                    {
                        FargoSoulsUtil.NewNPCEasy(npc.GetSpawnSourceForNPCFromNPCAI(), npc.Center, ModContent.NPCType<GelatinSubject>(), npc.whoAmI, target: npc.target, 
                            velocity: Main.rand.NextFloat(8f) * npc.DirectionFrom(Main.player[npc.target].Center).RotatedByRandom(MathHelper.PiOver2));
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                    }
                }
            }

            TrySpawnMinions(ref SpawnedMinions1, 0.75);
            TrySpawnMinions(ref SpawnedMinions2, 0.25);

            //ai0
            //0 = default
            //3 = chase?
            //4 = stomp
            //5 = shooty gels

            if (npc.ai[0] == 5 && npc.ai[1] == 45) //when shooting, p1 and p2
            {
                if (++SpikeCounter > 3) //every few shots
                {
                    SpikeCounter = 0;
                    NetSync(npc);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -4; i <= 4; i++)
                        {
                            Vector2 targetPos = Main.player[npc.target].Center;
                            targetPos.X += 300 * i;

                            float minionTravelTime = StompTravelTime / 2 + Main.rand.Next(10);
                            float minionGravity = StompGravity;
                            Vector2 vel = targetPos - npc.Center;
                            vel.X = vel.X / minionTravelTime;
                            vel.Y = vel.Y / minionTravelTime - 0.5f * minionGravity * minionTravelTime;

                            FargoSoulsUtil.NewNPCEasy(npc.GetSpawnSourceForNPCFromNPCAI(), npc.Center, ModContent.NPCType<GelatinSlime>(), npc.whoAmI, minionTravelTime, minionGravity, vel.X, vel.Y, target: npc.target, velocity: vel);
                        }
                    }
                }
            }

            if (npc.life < npc.lifeMax / 2) //phase 2
            {
                npc.defense = npc.defDefense / 2;

                if (RainTimer < 0)
                    RainTimer++;

                if (RainTimer <= 0 && StompTimer < 0) //dont run timer during rain attack
                    StompTimer++;

                if (npc.ai[0] == 0) //basic flying ai
                {
                    if (RainTimer == 0)
                    {
                        npc.position += npc.velocity;

                        npc.ai[1] -= 1; //dont progress to next ai

                        if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center - 250 * Vector2.UnitY) < 64)
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
                        npc.velocity.X *= 0.9f;

                        npc.ai[1] -= 1f; //dont progress ai

                        RainTimer++;

                        const int delay = 45;
                        const int timeBeforeStreamsMove = 45;
                        const int maxAttackTime = 480;
                        int attackTimer = RainTimer - delay - timeBeforeStreamsMove;
                        if (attackTimer < 0)
                            attackTimer = 0;

                        if (RainTimer == delay)
                            RainDirection = Math.Sign(Main.player[npc.target].Center.X - npc.Center.X);

                        if (RainTimer > delay && RainTimer < delay + maxAttackTime && RainTimer % 5 == 0)
                        {
                            const float maxWavy = 200;
                            Vector2 focusPoint = new Vector2(npc.Center.X, Math.Min(npc.Center.Y, Main.player[npc.target].Center.Y));
                            focusPoint.X += maxWavy * RainDirection * (float)Math.Sin(Math.PI * 2f / maxAttackTime * attackTimer * 1.5f);
                            focusPoint.Y -= 500;

                            for (int i = -4; i <= 4; i++)
                            {
                                Vector2 spawnPos = focusPoint + Main.rand.NextVector2Circular(32, 32);
                                spawnPos.X += 300 * i;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), spawnPos, 8f * Vector2.UnitY,
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
                        }

                        if (endAttack)
                        {
                            RainTimer = -1000;
                            npc.netUpdate = true;
                            NetSync(npc);

                            if (StompTimer == 0) //transition directly to stompy if ready
                            {
                                npc.ai[0] = 4f;
                                npc.ai[1] = 0f;
                            }
                        }
                    }
                    else
                    {
                        npc.ai[1] += 1; //proceed to next ais faster

                        if (RainTimer >= -60 && RainTimer % 6 == 0)
                        {
                            if (NPC.AnyNPCs(ModContent.NPCType<GelatinSlime>()))
                                RainTimer = -60;
                        }
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
                        npc.ai[1]++; //to control slimes

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
                            if (StompCounter == 1 || StompCounter == 2)
                                distance.X += 300f * Math.Sign(Main.player[npc.target].Center.X - npc.Center.X);
                            float time = StompTravelTime;
                            if (StompCounter < 0) //enraged
                                time /= 2;
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
                            StompTimer = -360;

                            npc.velocity.X = 0;

                            npc.ai[1] = 2000f; //proceed to next thing immediately
                            npc.netUpdate = true;
                            NetSync(npc);
                        }
                    }
                    else if (StompTimer > 30)
                    {
                        npc.rotation = 0;
                        npc.noTileCollide = true;

                        float time = StompTravelTime;
                        if (StompCounter < 0) //enraged
                            time /= 2;

                        if (++StompTimer > time + 30)
                        {
                            npc.noTileCollide = false;

                            //WHERE'S TJHE FKC IJNGI METHOD FOR HTIS? ITS NOT COLLISION.SOLKIDCOLLIOSOM ITS NOPT COLLISON.SOLDITILES I HATE 1.4 IHATE TMODLAOREI I HATE THIS FUSPTID FUCKIGN GNAME SOFU KIGN MCUCH FUCK FUCK FUCK
                            bool isInTilesIncludingPlatforms = false;
                            for (int x = 0; x < npc.width; x += 16)
                            {
                                for (float y = npc.height / 2; y < npc.height; y += 16)
                                {
                                    Tile tile = Framing.GetTileSafely((int)(npc.position.X + x) / 16, (int)(npc.position.Y + y) / 16);
                                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                    {
                                        isInTilesIncludingPlatforms = true;
                                        break;
                                    }
                                }
                            }
                            
                            //when landed on a surface
                            if ((npc.Bottom.Y > Main.player[npc.target].Top.Y && (npc.velocity.Y == 0 || isInTilesIncludingPlatforms)) || StompTimer >= time * 2 + 25)
                            {
                                StompTimer = FargoSoulsWorld.MasochistModeReal ? 25 : 20;

                                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item92, npc.Center);

                                //spray spikes
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int j = -1; j <= 1; j += 2)
                                    {
                                        Vector2 baseVel = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(10) * j));
                                        const int max = 12;
                                        for (int i = 0; i < max; i++)
                                        {
                                            Vector2 vel = Main.rand.NextFloat(9f, 15f) * j * baseVel.RotatedBy(MathHelper.PiOver4 * 0.8f / max * i * -j);
                                            Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, vel, ProjectileID.QueenSlimeMinionBlueSpike, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                        }

                        Main.NewText($"{npc.velocity} {new Vector2(StompVelocityX, StompVelocityY)} {StompGravity} {StompVelocityY - npc.oldVelocity.Y}");

                        npc.velocity.X = StompVelocityX;
                        npc.velocity.Y = StompVelocityY;
                        StompVelocityY += StompGravity;

                        FargoSoulsUtil.PrintAI(npc);

                        return false;
                    }
                }
                else if (npc.ai[0] == 5) //when shooting
                {
                    //be careful to stay above player
                    if (npc.HasValidTarget && npc.Bottom.Y > Main.player[npc.target].Top.Y - 80 && npc.velocity.Y > -8f)
                        npc.velocity.Y -= 0.8f;
                }
            }

            //FargoSoulsUtil.PrintAI(npc);

            EModeUtils.DropSummon(npc, "JellyCrystal", NPC.downedQueenSlime, ref DroppedSummon, Main.hardMode);

            return true;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.life < npc.lifeMax / 2)
                damage *= 0.5;

            if (NPC.AnyNPCs(ModContent.NPCType<GelatinSubject>()))
                damage /= 3;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Slimed, 240);
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

    public class QueenSlimeMinion : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.QueenSlimeMinionBlue,
            NPCID.QueenSlimeMinionPink,
            NPCID.QueenSlimeMinionPurple
        );

        public bool TimeToFly;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(TimeToFly), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (FargoSoulsWorld.MasochistModeReal)
            {
                npc.lifeMax *= 2;
                npc.knockBackResist = 0;
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (FargoSoulsWorld.MasochistModeReal)
            {
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss))
                {
                    Vector2 target = Main.player[Main.npc[EModeGlobalNPC.queenSlimeBoss].target].Top;
                    if (TimeToFly)
                    {

                        npc.velocity = Math.Min(npc.velocity.Length(), 20f) * npc.DirectionTo(target);
                        npc.position += 8f * npc.DirectionTo(target);

                        if (npc.Distance(target) < 300f)
                        {
                            TimeToFly = false;
                            NetSync(npc);

                            npc.velocity += 8f * npc.DirectionTo(target).RotatedByRandom(MathHelper.PiOver4);
                            npc.netUpdate = true;
                        }
                    }
                    else if (npc.Distance(target) > 900f)
                    {
                        TimeToFly = true;
                        NetSync(npc);
                    }
                }
                else
                {
                    TimeToFly = false;
                }
            }
            else
            {
                npc.localAI[0] = 30f; //prevent firing
            }

            npc.noTileCollide = TimeToFly;

            //if (npc.velocity.Y != 0)
            //    npc.localAI[0] = 25f;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Slimed, 180);
        }
    }
}
