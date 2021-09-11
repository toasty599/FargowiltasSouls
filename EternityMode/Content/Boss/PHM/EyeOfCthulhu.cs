using Fargowiltas.Items.Summons;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class EyeOfCthulhu : EModeNPCMod
    {
        public override void CreateMatcher() => Matcher = new NPCMatcher().MatchType(NPCID.EyeofCthulhu);

        public int Counter0;
        public int Counter1;
        public int Counter2;
        public int Counter3;

        public bool Flag0;
        public bool Flag1;
        public bool Flag2;

        public bool DroppedSummon;

        // TODO Better collection format for this that takes less characters?
        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(Counter0), IntStrategies.CompoundStrategy },
                { new Ref<object>(Counter1), IntStrategies.CompoundStrategy },
                { new Ref<object>(Counter2), IntStrategies.CompoundStrategy },
                { new Ref<object>(Counter3), IntStrategies.CompoundStrategy },

                { new Ref<object>(Flag0), BoolStrategies.CompoundStrategy },
                { new Ref<object>(Flag1), BoolStrategies.CompoundStrategy },
                { new Ref<object>(Flag2), BoolStrategies.CompoundStrategy },
            };

        public override bool PreAI(NPC npc)
        {
            Main.NewText($"ai0: {Flag0} | ai1: {Flag1} | ai2: {Flag2} | ai3: {Counter3}");

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

            if (Counter1 > 0)
            {
                if (Counter1 % (Flag0 ? 2 : 6) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Flag0)
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
                Counter1--;
            }

            if (npc.ai[1] == 3f && !Flag0) //during dashes in phase 2
            {
                Counter1 = 30;
                //Flag0 = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    FargoSoulsUtil.XWay(8, npc.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, npc.damage / 4, 0);
            }

            if (npc.life < npc.lifeMax / 2)
            {
                if (Flag0) //final phase
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

                        Counter0 = 90;
                        Counter2 = 0;
                        Flag1 = true;
                        Flag2 = false;

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

                    if (++Counter0 == 1) //teleport to random position
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = Main.player[npc.target].Center;
                            npc.position.X += Main.rand.Next(2) == 0 ? -600 : 600;
                            npc.position.Y += Main.rand.Next(2) == 0 ? -400 : 400;
                            npc.TargetClosest(false);
                            npc.GetGlobalNPC<NewEModeGlobalNPC>().NetSync((byte)npc.whoAmI);
                            npc.netUpdate = true;
                        }
                    }
                    else if (Counter0 < 90) //fade in, moving into position
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
                    else if (!Flag1) //berserk dashing phase
                    {
                        Counter0 = 90;

                        const float xSpeed = 18f;
                        const float ySpeed = 40f;

                        if (++Counter2 == 1)
                        {
                            Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar

                            if (!Flag2) //only set this on the first dash of each set
                            {
                                Flag2 = true;
                                npc.velocity.X = npc.Center.X < Main.player[npc.target].Center.X ? xSpeed : -xSpeed;
                            }

                            npc.velocity.Y = npc.Center.Y < Main.player[npc.target].Center.Y ? ySpeed : -ySpeed; //alternate this every dash

                            Counter1 = 30;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                FargoSoulsUtil.XWay(8, npc.Center, ModContent.ProjectileType<BloodScythe>(), 1f, npc.damage / 4, 0);

                            npc.netUpdate = true;
                        }
                        else if (Counter2 > 20)
                        {
                            Counter2 = 0;
                        }

                        if (++Counter3 > 600 * 3 / xSpeed + 5) //proceed
                        {
                            Counter3 = 0;
                            Flag1 = true;
                            npc.netUpdate = true;
                        }

                        const float PI = (float)Math.PI;
                        npc.rotation = npc.velocity.ToRotation() - PI / 2;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;
                    }
                    else if (Counter0 < 180) //fade out
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
                        Counter0 = 0;
                        Counter2 = 0;
                        Flag1 = false;
                        Flag2 = false;
                    }

                    if (npc.netUpdate)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            npc.GetGlobalNPC<NewEModeGlobalNPC>().NetSync((byte)npc.whoAmI);
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
                        Flag0 = true;

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
    }
}
