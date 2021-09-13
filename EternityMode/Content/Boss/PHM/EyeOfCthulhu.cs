using Fargowiltas.Items.Summons;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class EyeOfCthulhu : EModeNPCMod
    {
        public override void CreateMatcher() => Matcher = new NPCMatcher().MatchType(NPCID.EyeofCthulhu);

        public int AITimer;
        public int ScytheSpawnTimer;
        public int FinalPhaseDashCD;
        public int FinalPhaseDashStageDuration;

        public bool IsInFinalPhase;
        public bool FinalPhaseBerserkDashesComplete;
        public bool FinalPhaseDashHorizSpeedSet;

        public bool DroppedSummon;

        // TODO Better collection format for this that takes less characters?
        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(AITimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(ScytheSpawnTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(FinalPhaseDashCD), IntStrategies.CompoundStrategy },
                { new Ref<object>(FinalPhaseDashStageDuration), IntStrategies.CompoundStrategy },

                { new Ref<object>(IsInFinalPhase), BoolStrategies.CompoundStrategy },
                { new Ref<object>(FinalPhaseBerserkDashesComplete), BoolStrategies.CompoundStrategy },
                { new Ref<object>(FinalPhaseDashHorizSpeedSet), BoolStrategies.CompoundStrategy },
            };

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.eyeBoss = npc.whoAmI;

            /*Counter0++;
            if (Counter0 >= 600)
            {
                Counter0 = 0;
                if (npc.life <= npc.lifeMax * 0.65 && NPC.CountNPCS(NPCID.ServantofCthulhu) < 6 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = new Vector2(2, 2);
                    for (int i = 0; i < 4; i++)
                    {
                        int n = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), NPCID.ServantofCthulhu);
                        if (n != 200)
                        {
                            Main.npc[n].velocity = vel.RotatedBy(Math.PI / 2 * i);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                }
            }*/

            npc.dontTakeDamage = npc.alpha > 50;
            if (npc.dontTakeDamage)
                Lighting.AddLight(npc.Center, 0.75f, 1.35f, 1.5f);

            if (ScytheSpawnTimer > 0)
            {
                if (ScytheSpawnTimer % (IsInFinalPhase ? 2 : 6) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (IsInFinalPhase)
                    {
                        int p = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<BloodScythe>(), npc.damage / 4, 1f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 45;
                    }
                    else
                    {
                        Projectile.NewProjectile(new Vector2(npc.Center.X + Main.rand.Next(-15, 15), npc.Center.Y), npc.velocity * 0.1f, ModContent.ProjectileType<BloodScythe>(), npc.damage / 4, 1f, Main.myPlayer);
                    }
                }
                ScytheSpawnTimer--;
            }

            if (npc.ai[1] == 3f && !IsInFinalPhase) //during dashes in phase 2
            {
                ScytheSpawnTimer = 30;
                //Flag0 = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    FargoSoulsUtil.XWay(8, npc.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, npc.damage / 4, 0);
            }

            if (npc.life < npc.lifeMax / 2)
            {
                if (IsInFinalPhase) //final phase
                {
                    if (npc.HasValidTarget && !Main.dayTime)
                    {
                        if (npc.timeLeft < 300)
                            npc.timeLeft = 300;
                    }
                    else //despawn and retarget
                    {
                        npc.TargetClosest(false);
                        npc.velocity.X *= 0.98f;
                        npc.velocity.Y -= npc.velocity.Y > 0 ? 1f : 0.25f;

                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        AITimer = 90;
                        FinalPhaseDashCD = 0;
                        FinalPhaseBerserkDashesComplete = true;
                        FinalPhaseDashHorizSpeedSet = false;

                        npc.alpha = 0;

                        const float PI = (float)Math.PI;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;

                        float targetRotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2;
                        if (targetRotation > PI)
                            targetRotation -= 2 * PI;
                        if (targetRotation < -PI)
                            targetRotation += 2 * PI;
                        npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.07f);
                    }

                    if (++AITimer == 1) //teleport to random position
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = Main.player[npc.target].Center;
                            npc.position.X += Main.rand.Next(2) == 0 ? -600 : 600;
                            npc.position.Y += Main.rand.Next(2) == 0 ? -400 : 400;
                            npc.TargetClosest(false);
                            npc.netUpdate = true;
                            NetSync(npc);
                        }
                    }
                    else if (AITimer < 90) //fade in, moving into position
                    {
                        npc.alpha -= 4;
                        if (npc.alpha < 0)
                            npc.alpha = 0;

                        const float PI = (float)Math.PI;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;

                        float targetRotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2;
                        if (targetRotation > PI)
                            targetRotation -= 2 * PI;
                        if (targetRotation < -PI)
                            targetRotation += 2 * PI;
                        npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.07f);

                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }

                        Vector2 target = Main.player[npc.target].Center;
                        target.X += npc.Center.X < target.X ? -600 : 600;
                        target.Y += npc.Center.Y < target.Y ? -400 : 400;

                        float speedModifier = 0.3f;
                        if (npc.Center.X < target.X)
                        {
                            npc.velocity.X += speedModifier;
                            if (npc.velocity.X < 0)
                                npc.velocity.X += speedModifier * 2;
                        }
                        else
                        {
                            npc.velocity.X -= speedModifier;
                            if (npc.velocity.X > 0)
                                npc.velocity.X -= speedModifier * 2;
                        }
                        if (npc.Center.Y < target.Y)
                        {
                            npc.velocity.Y += speedModifier;
                            if (npc.velocity.Y < 0)
                                npc.velocity.Y += speedModifier * 2;
                        }
                        else
                        {
                            npc.velocity.Y -= speedModifier;
                            if (npc.velocity.Y > 0)
                                npc.velocity.Y -= speedModifier * 2;
                        }
                        if (Math.Abs(npc.velocity.X) > 24)
                            npc.velocity.X = 24 * Math.Sign(npc.velocity.X);
                        if (Math.Abs(npc.velocity.Y) > 24)
                            npc.velocity.Y = 24 * Math.Sign(npc.velocity.Y);
                    }
                    else if (!FinalPhaseBerserkDashesComplete) //berserk dashing phase
                    {
                        AITimer = 90;

                        const float xSpeed = 18f;
                        const float ySpeed = 40f;

                        if (++FinalPhaseDashCD == 1)
                        {
                            Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar

                            if (!FinalPhaseDashHorizSpeedSet) //only set this on the first dash of each set
                            {
                                FinalPhaseDashHorizSpeedSet = true;
                                npc.velocity.X = npc.Center.X < Main.player[npc.target].Center.X ? xSpeed : -xSpeed;
                            }

                            npc.velocity.Y = npc.Center.Y < Main.player[npc.target].Center.Y ? ySpeed : -ySpeed; //alternate this every dash

                            ScytheSpawnTimer = 30;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                FargoSoulsUtil.XWay(8, npc.Center, ModContent.ProjectileType<BloodScythe>(), 1f, npc.damage / 4, 0);

                            npc.netUpdate = true;
                        }
                        else if (FinalPhaseDashCD > 20)
                        {
                            FinalPhaseDashCD = 0;
                        }

                        if (++FinalPhaseDashStageDuration > 600 * 3 / xSpeed + 5) //proceed
                        {
                            FinalPhaseDashStageDuration = 0;
                            FinalPhaseBerserkDashesComplete = true;
                            npc.netUpdate = true;
                        }

                        const float PI = (float)Math.PI;
                        npc.rotation = npc.velocity.ToRotation() - PI / 2;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;
                    }
                    else if (AITimer < 180) //fade out
                    {
                        npc.velocity *= 0.98f;
                        npc.alpha += 4;
                        if (npc.alpha > 255)
                            npc.alpha = 255;

                        const float PI = (float)Math.PI;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;

                        float targetRotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2;
                        if (targetRotation > PI)
                            targetRotation -= 2 * PI;
                        if (targetRotation < -PI)
                            targetRotation += 2 * PI;
                        npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.07f);

                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }
                    }
                    else //reset
                    {
                        AITimer = 0;
                        FinalPhaseDashCD = 0;
                        FinalPhaseBerserkDashesComplete = false;
                        FinalPhaseDashHorizSpeedSet = false;
                    }

                    if (npc.netUpdate)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetSync(npc);
                        }
                        npc.netUpdate = false;
                    }
                    return false;
                }
                else if (npc.life <= npc.lifeMax * 0.1) //go into final phase
                {
                    npc.velocity *= 0.98f;
                    npc.alpha += 4;
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                        IsInFinalPhase = true;

                        Main.PlaySound(SoundID.Roar, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                    }
                    return false;
                }
                else if (npc.ai[0] == 3 && (npc.ai[1] == 0 || npc.ai[1] == 5))
                {
                    if (npc.ai[2] < 2)
                    {
                        npc.ai[2]--;
                        npc.alpha += 4;
                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }
                        if (npc.alpha > 255)
                        {
                            npc.alpha = 255;
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                            {
                                npc.ai[2] = 60;
                                npc.ai[1] = 5f;

                                Vector2 distance = Main.player[npc.target].Center - npc.Center;
                                if (Math.Abs(distance.X) > 1200)
                                    distance.X = 1200 * Math.Sign(distance.X);
                                else if (Math.Abs(distance.X) < 600)
                                    distance.X = 600 * Math.Sign(distance.X);
                                if (distance.Y > 0) //always ensure eoc teleports above player
                                    distance.Y *= -1;
                                if (Math.Abs(distance.Y) > 450)
                                    distance.Y = 450 * Math.Sign(distance.Y);
                                if (Math.Abs(distance.Y) < 150)
                                    distance.Y = 150 * Math.Sign(distance.Y);
                                npc.Center = Main.player[npc.target].Center + distance;

                                npc.netUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        npc.alpha -= 4;
                        if (npc.alpha < 0)
                        {
                            npc.alpha = 0;
                        }
                        else
                        {
                            npc.ai[2]--;
                            npc.position -= npc.velocity / 2;
                            for (int i = 0; i < 3; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].noLight = true;
                                Main.dust[d].velocity *= 4f;
                            }
                        }
                    }
                }

                /*if (++Timer > 600)
                {
                    Timer = 0;
                    if (npc.HasValidTarget)
                    {
                        Player player = Main.player[npc.target];
                        Main.PlaySound(SoundID.Item9, (int)player.position.X, (int)player.position.Y, 104, 1f, 0f);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = player.Center;
                            int direction;
                            if (player.velocity.X == 0f)
                                direction = player.direction;
                            else
                                direction = Math.Sign(player.velocity.X);
                            spawnPos.X += 600 * direction;
                            spawnPos.Y -= 600;
                            Vector2 speed = Vector2.UnitY;
                            for (int i = 0; i < 30; i++)
                            {
                                Projectile.NewProjectile(spawnPos, speed, ModContent.ProjectileType<BloodScythe>(), npc.damage / 4, 1f, Main.myPlayer);
                                spawnPos.X += 72 * direction;
                                speed.Y += 0.15f;
                            }
                        }
                    }
                }*/
            }
            else
            {
                npc.alpha = 0;
                npc.dontTakeDamage = false;
            }

            // Drop summon
            EModeUtils.DropSummon(npc, ModContent.ItemType<SuspiciousEye>(), NPC.downedBoss1, ref DroppedSummon);

            return true;
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ItemID.FallenStar, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.WoodenCrate, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<AgitatingLens>());
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 120);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 0);
            LoadBossHeadSprite(recolor, 1);
            LoadGoreRange(recolor, 6, 10);
        }
    }
}
