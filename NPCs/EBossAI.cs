using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.MutantBoss;
using Fargowiltas.Items.Summons;
using Fargowiltas.Items.Summons.Abom;
using Fargowiltas.Items.Summons.Mutant;
using Fargowiltas.Items.Summons.VanillaCopy;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Buffs.Souls;

namespace FargowiltasSouls.NPCs
{
    public partial class EModeGlobalNPC
    {
        private bool droppedSummon = false;

        public void KingSlimeAI(NPC npc)
        {
            slimeBoss = npc.whoAmI;
            npc.color = Main.DiscoColor * 0.3f;
            if (masoBool[1])
            {
                if (npc.velocity.Y == 0f) //attack that happens when landing
                {
                    masoBool[1] = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        /*for (int i = 0; i < 30; i++) //spike spray
                        {
                            Projectile.NewProjectile(new Vector2(npc.Center.X + Main.rand.Next(-5, 5), npc.Center.Y - 15),
                                new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-8, -5)),
                                ProjectileID.SpikedSlimeSpike, npc.damage / 5, 0f, Main.myPlayer);
                        }*/

                        if (npc.HasValidTarget)
                        {
                            Main.PlaySound(SoundID.Item21, Main.player[npc.target].Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    Vector2 spawn = Main.player[npc.target].Center;
                                    spawn.X += Main.rand.Next(-150, 151);
                                    spawn.Y -= Main.rand.Next(600, 901);
                                    Vector2 speed = Main.player[npc.target].Center - spawn;
                                    speed.Normalize();
                                    speed *= masoBool[0] ? 10f : 5f;
                                    speed = speed.RotatedByRandom(MathHelper.ToRadians(4));
                                    Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<SlimeBallHostile>(), npc.damage / 6, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
            else if (npc.velocity.Y > 0)
            {
                masoBool[1] = true;
            }

            if (npc.velocity.Y < 0) //jumping up
            {
                if (!masoBool[3])
                {
                    masoBool[3] = true;
                    //if player is well above me, jump higher and spray spikes
                    if (npc.HasValidTarget && Main.player[npc.target].Center.Y < npc.position.Y + npc.height - 240)
                    {
                        npc.velocity.Y *= 2f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const float gravity = 0.15f;
                            float time = 90f;
                            Vector2 distance = Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 30f;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            for (int i = 0; i < 15; i++)
                            {
                                Projectile.NewProjectile(npc.Center, distance + Main.rand.NextVector2Square(-1f, 1f),
                                    ModContent.ProjectileType<SlimeSpike>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }
            else
            {
                masoBool[3] = false;
            }

            if ((masoBool[0] || npc.life < npc.lifeMax * .5f) && npc.HasValidTarget)
            {
                if (--Counter[0] < 0) //spike rain
                {
                    Counter[0] = 240;
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -12; i <= 12; i++)
                        {
                            Vector2 spawnPos = Main.player[npc.target].Center;
                            spawnPos.X += 110 * i;
                            spawnPos.Y -= 500;
                            Projectile.NewProjectile(spawnPos, (masoBool[0] ? 6f : 0f) * Vector2.UnitY,
                                ModContent.ProjectileType<SlimeSpike2>(), npc.damage / 6, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            /*if (!masoBool[0]) //is not berserk
            {
                SharkCount = 0;

                if (npc.HasPlayerTarget)
                {
                    Player player = Main.player[npc.target];
                    if (player.active && !player.dead && player.Center.Y < npc.position.Y && npc.Distance(player.Center) < 1000f)
                    {
                        Counter[1]++; //timer runs if player is above me and nearby
                        if (Counter[1] >= 600 && Main.netMode != NetmodeID.MultiplayerClient) //go berserk
                        {
                            masoBool[0] = true;
                            npc.netUpdate = true;
                            NetUpdateMaso(npc.whoAmI);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("King Slime has enraged!"), new Color(175, 75, 255));
                            else
                                Main.NewText("King Slime has enraged!", 175, 75, 255);
                        }
                    }
                    else
                    {
                        Counter[1] = 0;
                    }
                }
            }
            else //is berserk
            {
                SharkCount = 1;

                if (!masoBool[2])
                {
                    masoBool[2] = true;
                    Main.PlaySound(SoundID.Roar, npc.Center, 0);
                }

                if (Counter[0] > 45) //faster slime spike rain
                    Counter[0] = 45;

                if (++Counter[2] > 30) //aimed spikes
                {
                    Counter[2] = 0;
                    const float gravity = 0.15f;
                    float time = 45f;
                    Vector2 distance = Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 30f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    for (int i = 0; i < 15; i++)
                    {
                        Projectile.NewProjectile(npc.Center, distance + Main.rand.NextVector2Square(-1f, 1f) * 2f,
                            ModContent.ProjectileType<SlimeSpike>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                }

                if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient && Main.player[npc.target].position.Y > npc.position.Y) //player went back down
                {
                    masoBool[0] = false;
                    masoBool[2] = false;
                    NetUpdateMaso(npc.whoAmI);
                }
            }*/

            //drop summon
            if (!NPC.downedSlimeKing && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<SlimyCrown>());
                droppedSummon = true;
            }
        }

        public bool EyeOfCthulhuAI(NPC npc)
        {
            eyeBoss = npc.whoAmI;

            /*Counter[0]++;
            if (Counter[0] >= 600)
            {
                Counter[0] = 0;
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

            if (Counter[1] > 0)
            {
                if (Counter[1] % (masoBool[0] ? 2 : 6) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (masoBool[0])
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
                Counter[1]--;
            }

            if (npc.ai[1] == 3f && !masoBool[0]) //during dashes in phase 2
            {
                Counter[1] = 30;
                //masoBool[0] = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    FargoGlobalProjectile.XWay(8, npc.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, npc.damage / 4, 0);
            }

            if (npc.life < npc.lifeMax / 2)
            {
                if (masoBool[0]) //final phase
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

                        Counter[0] = 90;
                        Counter[2] = 0;
                        masoBool[1] = true;
                        masoBool[2] = false;

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

                    if (++Counter[0] == 1) //teleport to random position
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = Main.player[npc.target].Center;
                            npc.position.X += Main.rand.Next(2) == 0 ? -600 : 600;
                            npc.position.Y += Main.rand.Next(2) == 0 ? -400 : 400;
                            npc.TargetClosest(false);
                            NetUpdateMaso(npc.whoAmI);
                            npc.netUpdate = true;
                        }
                    }
                    else if (Counter[0] < 90) //fade in, moving into position
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
                    else if (!masoBool[1]) //berserk dashing phase
                    {
                        Counter[0] = 90;

                        const float xSpeed = 18f;
                        const float ySpeed = 40f;

                        if (++Counter[2] == 1)
                        {
                            Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar

                            if (!masoBool[2]) //only set this on the first dash of each set
                            {
                                masoBool[2] = true;
                                npc.velocity.X = npc.Center.X < Main.player[npc.target].Center.X ? xSpeed : -xSpeed;
                            }

                            npc.velocity.Y = npc.Center.Y < Main.player[npc.target].Center.Y ? ySpeed : -ySpeed; //alternate this every dash

                            Counter[1] = 30;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                FargoGlobalProjectile.XWay(8, npc.Center, ModContent.ProjectileType<BloodScythe>(), 1f, npc.damage / 4, 0);
                            
                            npc.netUpdate = true;
                        }
                        else if (Counter[2] > 20)
                        {
                            Counter[2] = 0;
                        }

                        if (++Counter[3] > 600 * 3 / xSpeed + 5) //proceed
                        {
                            Counter[3] = 0;
                            masoBool[1] = true;
                            npc.netUpdate = true;
                        }
                        
                        const float PI = (float)Math.PI;
                        npc.rotation = npc.velocity.ToRotation() - PI / 2;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;
                    }
                    else if (Counter[0] < 180) //fade out
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
                        Counter[0] = 0;
                        Counter[2] = 0;
                        masoBool[1] = false;
                        masoBool[2] = false;
                    }

                    if (npc.netUpdate)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetUpdateMaso(npc.whoAmI);
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
                        masoBool[0] = true;

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

            //drop summon
            if (!NPC.downedBoss1 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<SuspiciousEye>());
                droppedSummon = true;
            }

            return true;
        }

        public bool EaterOfWorldsAI(NPC npc)
        {
            eaterBoss = npc.whoAmI;
            boss = npc.whoAmI;

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
            {
                npc.velocity.Y += 0.25f;
                if (npc.timeLeft > 120)
                    npc.timeLeft = 120;
            }

            //if (eaterResist > 0 && npc.whoAmI == NPC.FindFirstNPC(npc.type)) eaterResist--;

            int firstEater = NPC.FindFirstNPC(npc.type);

            if (Main.netMode != NetmodeID.MultiplayerClient && npc.whoAmI == firstEater && ++eaterTimer > 300) //only let one eater increment this
            {
                bool shoot = true;
                for (int i = 0; i < Main.maxNPCs; i++) //cancel if anyone is doing the u-turn
                {
                    if (Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().masoBool[0])
                    {
                        shoot = false;
                        eaterTimer -= 30;
                    }
                }
                
                if (shoot)
                {
                    eaterTimer = 0;

                    int counter = 0;
                    int delay = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            /*if (Main.npc[i].type == npc.type && !Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().masoBool[0])
                            {
                                Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().Counter[2] = 0; //stop others from triggering it
                            }
                            else */
                            if (Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail)
                            {
                                if (++counter > 3) //wave of redirecting flames
                                {
                                    counter = 0;
                                    Vector2 vel = (Main.player[npc.target].Center - Main.npc[i].Center) / 45;
                                    Projectile.NewProjectile(Main.npc[i].Center, vel,
                                        ModContent.ProjectileType<CursedFireballHoming>(), npc.damage / 5, 0f, Main.myPlayer, npc.target, delay);
                                    delay += 4;
                                }
                            }
                        }
                    }
                }
            }
            
            if (Main.netMode != NetmodeID.MultiplayerClient && Counter[1] % 6 == 3) //chose this number at random to avoid edge case
            {
                //die if segment behind me is invalid
                int ai0 = (int)npc.ai[0];
                if (!(ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].ai[1] == npc.whoAmI
                    && (Main.npc[ai0].type == NPCID.EaterofWorldsBody || Main.npc[ai0].type == NPCID.EaterofWorldsTail)))
                {
                    //Main.NewText("ai0 npc invalid");
                    npc.life = 0;
                    npc.HitEffect();
                    npc.checkDead();
                    npc.active = false;
                    npc.netUpdate = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    return false;
                }
            }

            if (!masoBool[0])
            {
                if (++Counter[0] >= 6)
                {
                    Counter[0] = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient) //cursed flamethrower, roughly same direction as head
                    {
                        Vector2 velocity = new Vector2(5f, 0f).RotatedBy(npc.rotation - Math.PI / 2.0 + MathHelper.ToRadians(Main.rand.Next(-15, 16)));
                        Projectile.NewProjectile(npc.Center, velocity, ProjectileID.EyeFire, npc.damage / 5, 0f, Main.myPlayer);
                    }
                }

                if (npc.whoAmI == firstEater)
                {
                    Counter[1]++;

                    if (Counter[1] == 700 - 90) //roar telegraph
                        Main.PlaySound(SoundID.Roar, Main.player[npc.target].Center, 0);

                    if (Counter[1] > 700 && Main.netMode != NetmodeID.MultiplayerClient) //initiate mass u-turn
                    {
                        Counter[1] = 0;
                        if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 2400)
                        {
                            masoBool[0] = true;

                            Counter[2] = NPC.CountNPCS(npc.type) / 2;

                            int headCounter = 0; //determine position of this head in the group
                            for (int i = 0; i < Main.maxNPCs; i++) //synchronize
                            {
                                if (Main.npc[i].active && Main.npc[i].type == npc.type)
                                {
                                    Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().Counter[1] = 0;
                                    Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().Counter[2] = Counter[2];
                                    Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().Counter[3] = headCounter;
                                    Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = true;

                                    Main.npc[i].netUpdate = true;
                                    NetUpdateMaso(i);

                                    headCounter *= -1; //alternate 0, 1, -1, 2, -2, 3, -3, etc.
                                    if (headCounter >= 0)
                                        headCounter++;
                                }
                            }

                            npc.netUpdate = true;
                        }
                    }
                }
            }
            else //flying u-turn ai
            {
                //eaterResist = 30;

                if (++Counter[1] < 120)
                {
                    Vector2 target = Main.player[npc.target].Center;
                    if (Counter[3] != 0)
                        target.X += 900f / Counter[2] * Counter[3]; //space out
                    target.Y += 600f;

                    float speedModifier = 0.6f;
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
                    
                    npc.localAI[0] = 1f;

                    if (Main.netMode == NetmodeID.Server && --npc.netSpam < 0) //manual mp sync control
                    {
                        npc.netSpam = 5;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                }
                else if (Counter[1] == 120) //fly up
                {
                    Main.PlaySound(SoundID.Roar, Main.player[npc.target].Center, 0);
                    npc.velocity = Vector2.UnitY * -15f;
                    Counter[0] = (int)Main.player[npc.target].Center.X; //store their initial location

                    npc.netUpdate = true;
                }
                else if (Counter[1] < 240) //cancel early and turn once we fly past player
                {
                    if (npc.Center.Y < Main.player[npc.target].Center.Y - 450)
                        Counter[1] = 239;
                }
                else if (Counter[1] == 240) //recalculate velocity to u-turn and dive back down in the same spacing over player
                {
                    Vector2 target;
                    target.X = Main.player[npc.target].Center.X;
                    if (Counter[3] != 0)
                        target.X += 900f / Counter[2] * Counter[3]; //space out
                    target.Y = npc.Center.Y;

                    float radius = Math.Abs(target.X - npc.Center.X) / 2;
                    npc.velocity = Vector2.Normalize(npc.velocity) * (float)Math.PI * radius / 30;

                    Counter[0] = Math.Sign(Main.player[npc.target].Center.X - Counter[0]); //which side player moved to from original pos

                    npc.netUpdate = true;
                }
                else if (Counter[1] < 270) //u-turn
                {
                    npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(6f) * Counter[0]);
                }
                else if (Counter[1] == 270)
                {
                    npc.velocity = Vector2.Normalize(npc.velocity) * 15f;
                    npc.netUpdate = true;
                }
                else if (Counter[1] > 300)
                {
                    Counter[1] = 0;
                    Counter[2] = 0;
                    Counter[3] = 0;
                    masoBool[0] = false;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            if (Main.npc[i].type == npc.type)
                            {
                                Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().Counter[1] = 0;
                                Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().Counter[2] = 0;
                                Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().Counter[3] = 0;
                                Main.npc[i].GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = false;
                                Main.npc[i].netUpdate = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetUpdateMaso(i);
                            }
                            else if (Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail)
                            {
                                Main.npc[i].netUpdate = true;
                            }
                        }
                    }

                    npc.netUpdate = true;
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;

                if (npc.netUpdate)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetUpdateMaso(npc.whoAmI);
                    }
                    npc.netUpdate = false;
                }
                return false;
            }

            //drop summon
            if (!NPC.downedBoss2 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                //eater meme
                if (!player.dead && player.GetModPlayer<FargoPlayer>().FreeEaterSummon)
                {
                    player.GetModPlayer<FargoPlayer>().FreeEaterSummon = false;

                    Item.NewItem(player.Hitbox, ModContent.ItemType<WormyFood>());
                    droppedSummon = true;
                } 
            }

            return true;
        }

        public void BrainOfCthulhuAI(NPC npc)
        {
            brainBoss = npc.whoAmI;

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
            {
                npc.velocity.Y += 0.75f;
                if (npc.timeLeft > 120)
                    npc.timeLeft = 120;
            }

            if (npc.alpha == 0)
            {
                npc.damage = npc.defDamage;
            }
            else
            {
                npc.damage = 0;
                if (npc.ai[0] != -2 && npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 300) //stay at a minimum distance
                {
                    npc.Center = Main.player[npc.target].Center + Main.player[npc.target].DirectionTo(npc.Center) * 300;
                }
            }

            if (!npc.dontTakeDamage) //vulnerable
            {
                if (npc.buffType[0] != 0) //constant debuff cleanse
                {
                    npc.buffImmune[npc.buffType[0]] = true;
                    npc.DelBuff(0);
                }
                if (!masoBool[0]) //spawn illusions
                {
                    masoBool[0] = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.MasochistMode;
                        int type = recolor ? ModContent.NPCType<BrainIllusion2>() : ModContent.NPCType<BrainIllusion>();
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, npc.whoAmI, -1, 1);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, npc.whoAmI, 1, -1);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, npc.whoAmI, 1, 1);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<BrainClone>(), npc.whoAmI);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);

                        for (int i = 0; i < Main.maxProjectiles; i++) //clear old golden showers
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GoldenShowerHoming>())
                                Main.projectile[i].Kill();
                        }
                    }
                }

                void MakeDust(Vector2 spawn)
                {
                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 180);
                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 200);
                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 220);
                    /*for (int i = 0; i < 80; i++) //dust ring
                    {
                        Vector2 vector6 = Vector2.UnitY * 12f;
                        vector6 = vector6.RotatedBy((i - (80 / 2 - 1)) * 6.28318548f / 80) + spawn;
                        Vector2 vector7 = vector6 - spawn;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, 90, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[d].scale = 3f;
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = vector7;
                    }*/
                };

                void LaserSpread()
                {
                    if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient) //laser spreads from each illusion
                    {
                        Vector2 offset = npc.Center - Main.player[npc.target].Center;

                        const int max = 3;
                        const int degree = 3;

                        Vector2 spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y += offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y -= offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y += offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y -= offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                };

                if (--Counter[0] < 0) //confusion timer
                {
                    Counter[0] = 300;

                    if (Main.LocalPlayer.HasBuff(BuffID.Confused))
                    {
                        Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
                        MakeDust(npc.Center);
                    }
                    else
                    {
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                        Vector2 offset = npc.Center - Main.player[npc.target].Center;

                        Vector2 spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y += offset.Y;
                        MakeDust(spawnPos);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y -= offset.Y;
                        MakeDust(spawnPos);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y += offset.Y;
                        MakeDust(spawnPos);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y -= offset.Y;
                        MakeDust(spawnPos);
                    }
                }
                else if (Counter[0] == 240) //inflict confusion after telegraph
                {
                    if (npc.Distance(Main.LocalPlayer.Center) < 3000 && !Main.LocalPlayer.HasBuff(BuffID.Confused))
                    {
                        Main.LocalPlayer.AddBuff(BuffID.Confused, Main.expertMode && Main.expertDebuffTime > 1 ? 150 + 5 : 300 + 10);
                    }

                    LaserSpread();
                }

                if (--Counter[1] < 0)
                {
                    Counter[1] = Main.rand.Next(5, 15);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawn = Main.player[npc.target].Center + Main.rand.NextVector2CircularEdge(1200f, 1200f);
                        Vector2 speed = Main.player[npc.target].Center + Main.rand.NextVector2Circular(-600f, 600f) - spawn;
                        speed = Vector2.Normalize(speed) * Main.rand.NextFloat(12f, 48f);
                        Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<BrainIllusionProj>(), 0, 0f, Main.myPlayer, npc.whoAmI);
                    }
                }
            }

            //drop summon
            if (!NPC.downedBoss2 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<GoreySpine>());
                droppedSummon = true;
            }
        }

        public void CreeperAI(NPC npc)
        {
            if (!masoBool[0])
            {
                masoBool[0] = true;
                Counter[2] = Main.rand.Next(60 * NPC.CountNPCS(NPCID.Creeper)) + Main.rand.Next(-60, 61);
            }

            if (--Counter[2] < 0)
            {
                Counter[2] = 60 * NPC.CountNPCS(NPCID.Creeper) - 30;
                if (Counter[2] > 120)
                    Counter[2] += Main.rand.Next(-60, 61);

                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.Center, 10f * npc.DirectionFrom(Main.player[npc.target].Center).RotatedByRandom(Math.PI),
                        ModContent.ProjectileType<GoldenShowerHoming>(), npc.damage / 4, 0f, Main.myPlayer, npc.target, -60f);
                    /*Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Y -= Math.Abs(speed.X) * 0.1f; //account for gravity
                    speed.X += Main.rand.Next(-10, 11);
                    speed.Y += Main.rand.Next(-30, 21);
                    speed.Normalize();
                    speed *= 10f;
                    Projectile.NewProjectile(npc.Center, speed, ProjectileID.GoldenShowerHostile, npc.damage / 4, 0f, Main.myPlayer);*/
                }

                npc.netUpdate = true;
            }
        }

        public bool QueenBeeAI(NPC npc)
        {
            beeBoss = npc.whoAmI;

            if (!masoBool[0] && npc.life < npc.lifeMax / 3 * 2 && npc.HasPlayerTarget)
            {
                masoBool[0] = true;

                Vector2 vector72 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);

                int num594 = NPC.NewNPC((int)vector72.X, (int)vector72.Y, ModContent.NPCType<RoyalSubject>(), 0, 0f, 0f, 0f, 0f, 255);
                Main.npc[num594].velocity.X = (float)Main.rand.Next(-200, 201) * 0.002f;
                Main.npc[num594].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.002f;
                Main.npc[num594].localAI[0] = 60f;
                Main.npc[num594].netUpdate = true;

                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText("Royal Subject has awoken!", 175, 75, 255);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Royal Subject has awoken!"), new Color(175, 75, 255));
            }

            if (!masoBool[1] && npc.life < npc.lifeMax / 3 && npc.HasPlayerTarget)
            {
                masoBool[1] = true;

                Vector2 vector72 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);

                int num594 = NPC.NewNPC((int)vector72.X, (int)vector72.Y, ModContent.NPCType<RoyalSubject>(), 0, 0f, 0f, 0f, 0f, 255);
                Main.npc[num594].velocity.X = (float)Main.rand.Next(-200, 201) * 0.1f;
                Main.npc[num594].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.1f;
                Main.npc[num594].localAI[0] = 60f;
                Main.npc[num594].netUpdate = true;

                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText("Royal Subject has awoken!", 175, 75, 255);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Royal Subject has awoken!"), new Color(175, 75, 255));

                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<RoyalSubject>()); //so that both dont stack for being spawned from qb
            }

            if (!masoBool[2] && npc.life < npc.lifeMax / 2) //enable new attack and roar below 50%
            {
                masoBool[2] = true;
                Main.PlaySound(SoundID.Roar, npc.Center, 0);
            }

            if (NPC.AnyNPCs(ModContent.NPCType<RoyalSubject>()))
            {
                npc.HitSound = SoundID.NPCHit4;
                npc.color = new Color(127, 127, 127);

                int dustId = Dust.NewDust(npc.position, npc.width, npc.height, 1, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustId].noGravity = true;
                int dustId3 = Dust.NewDust(npc.position, npc.width, npc.height, 1, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustId3].noGravity = true;

                npc.ai[0] = 3; //always shoot stingers mode
            }
            else
            {
                npc.HitSound = SoundID.NPCHit1;
                npc.color = default;

                if (npc.life < npc.lifeMax / 2 && Counter[0] % 2 == 0)
                    Counter[0]++; //throw hives faster when no royal subjects alive
            }

            if (npc.life > npc.lifeMax / 2)
            {
                if (npc.ai[0] == 3f || npc.ai[0] == 1f) //only when in stationary modes
                {
                    if (++Counter[0] >= 90) //stinger sprays above 50%
                    {
                        Counter[0] = 0;
                        Counter[1]++;
                        if (Counter[1] > 3)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                FargoGlobalProjectile.XWay(16, npc.Center, ProjectileID.Stinger, 6, 11, 1);
                            Counter[1] = 0;
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                FargoGlobalProjectile.XWay(8, npc.Center, ProjectileID.Stinger, 6, 11, 1);
                        }
                    }
                }
            }
            else
            {
                if (++Counter[0] > 570 && Counter[2] <= 600 && (npc.ai[0] == 3f || npc.ai[0] == 1f)) //lobs hives below 50%, not dashing
                {
                    Counter[0] = 0;
                    const float gravity = 0.25f;
                    float time = 60f;
                    Vector2 distance = Main.player[npc.target].Center - Vector2.UnitY * 16 - npc.Center + Main.player[npc.target].velocity * 30f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, distance, ModContent.ProjectileType<Beehive>(),
                            npc.damage / 4, 0f, Main.myPlayer, time - 2);
                    }
                }
            }

            //only while stationary mode
            if (npc.ai[0] == 3f || npc.ai[0] == 1f)
            {
                if (masoBool[2] && ++Counter[2] > 600)
                {
                    if (Counter[2] < 720) //slow down
                    {
                        if (!masoBool[3])
                        {
                            masoBool[3] = true;
                            npc.netUpdate = true;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                            if (npc.HasValidTarget)
                                Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar
                        }

                        if (Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        {
                            npc.velocity *= 0.975f;
                        }
                        else
                        {
                            Counter[2]--; //stall this section until has line of sight
                            return true;
                        }
                    }
                    else if (Counter[2] < 840) //spray bees
                    {
                        if (masoBool[3])
                        {
                            masoBool[3] = false;
                            npc.netUpdate = true;
                        }
                        npc.velocity = Vector2.Zero;
                        if (++Counter[0] > 1)
                        {
                            Counter[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const float rotation = 0.025f;
                                Projectile.NewProjectile(npc.Center + new Vector2(3 * npc.direction, 15), Main.rand.NextFloat(9f, 18f) * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))),
                                    ModContent.ProjectileType<Bee>(), npc.damage / 4, 0f, Main.myPlayer, npc.target, Main.rand.Next(2) == 0 ? -rotation : rotation);
                                Projectile.NewProjectile(npc.Center + new Vector2(3 * npc.direction, 15), -Main.rand.NextFloat(9f, 18f) * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))),
                                    ModContent.ProjectileType<Bee>(), npc.damage / 4, 0f, Main.myPlayer, npc.target, Main.rand.Next(2) == 0 ? -rotation : rotation);
                            }
                        }
                    }
                    else if (Counter[2] > 900) //wait for 1 second then return to normal AI
                    {
                        Counter[2] = 0;
                        npc.netUpdate = true;
                    }

                    if (npc.netUpdate)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetUpdateMaso(npc.whoAmI);
                        }
                        npc.netUpdate = false;
                    }
                    return false;
                }
            }

            //drop summon
            if (!NPC.downedQueenBee && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<Abeemination2>());
                droppedSummon = true;
            }

            return true;
        }

        public void SkeletronAI(NPC npc)
        {
            skeleBoss = npc.whoAmI;

            /*if (Counter[0] != 0)
            {
                Counter[2]++;

                if (Counter[2] >= 3600)
                {
                    Counter[2] = 0;

                    bool otherHandStillAlive = false;
                    for (int i = 0; i < 200; i++) //look for hand that belongs to me
                    {
                        if (Main.npc[i].active && Main.npc[i].type == NPCID.SkeletronHand && Main.npc[i].ai[1] == npc.whoAmI)
                        {
                            otherHandStillAlive = true;
                            break;
                        }
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.SkeletronHand, npc.whoAmI, 0f, 0f, 0f, 0f, npc.target);
                        if (n != 200)
                        {
                            Main.npc[n].ai[0] = (Counter[0] == 1) ? 1f : -1f;
                            Main.npc[n].ai[1] = npc.whoAmI;
                            Main.npc[n].life = Main.npc[n].lifeMax / 4;
                            Main.npc[n].netUpdate = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }

                    if (!otherHandStillAlive)
                    {
                        if (Counter[0] == 1)
                            Counter[0] = 2;
                        else
                            Counter[0] = 1;
                    }
                    else
                    {
                        Counter[0] = 0;
                    }
                }
            }*/

            if (npc.ai[1] == 0f)
            {
                if (npc.ai[2] == 800 - 90) //telegraph spin
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TargetingReticle>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                }
                if (npc.ai[2] < 800 - 5)
                {
                    Counter[3] = npc.target;
                }
            }

            if (npc.ai[1] == 1f || npc.ai[1] == 2f) //spinning or DG mode
            {
                if (Counter[3] > -1 && Counter[3] < Main.maxPlayers)
                {
                    npc.target = Counter[3];
                    npc.netUpdate = true;
                    Counter[3] = -1;
                    if (!npc.HasValidTarget)
                        npc.TargetClosest(false);
                }

                npc.localAI[2]++;
                float ratio = (float)npc.life / npc.lifeMax;
                float threshold = 20f + 100f * ratio;
                if (npc.localAI[2] >= threshold) //spray bones
                {
                    npc.localAI[2] = 0f;
                    if (threshold > 0 && npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 speed = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 6f;
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 vel = speed.RotatedBy(Math.PI * 2 / 8 * i);
                            vel += npc.velocity * (1f - ratio);
                            vel.Y -= Math.Abs(vel.X) * 0.2f;
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<SkeletronBone>(), npc.defDamage / 9 * 2, 0f, Main.myPlayer);
                        }
                    }
                }

                if (Counter[0] > 180)
                    Counter[0] = 180;

                if (npc.life < npc.lifeMax * .75 && npc.ai[1] == 1f && --Counter[0] < 0)
                {
                    Counter[0] = 180;

                    Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                    if (Main.netMode != NetmodeID.MultiplayerClient) //spray of baby guardian missiles
                    {
                        const int max = 30;
                        float modifier = 1f - (float)npc.life / npc.lifeMax;
                        modifier *= 4f / 3f; //scaling maxes at 25% life
                        if (modifier > 1f)
                            modifier = 1f;
                        int actualNumberToSpawn = (int)(max * modifier);
                        for (int i = 0; i < actualNumberToSpawn; i++)
                        {
                            float speed = Main.rand.NextFloat(3f, 9f);
                            Vector2 velocity = speed * npc.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI * (Main.rand.NextDouble() - 0.5));
                            float ai1 = speed / (60f + Main.rand.NextFloat(actualNumberToSpawn * 2));
                            Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<SkeletronGuardian>(), npc.damage / 5, 0f, Main.myPlayer, 0f, ai1);
                        }
                    }
                }

                if (!masoBool[0] && npc.ai[1] == 1f) //X pinch of guardians
                {
                    masoBool[0] = true;

                    for (int i = 0; i < Main.maxProjectiles; i++) //also clear leftover babies
                    {
                        if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<SkeletronGuardian2>())
                            Main.projectile[i].Kill();
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = -2; j <= 2; j++)
                            {
                                Vector2 spawnPos = new Vector2(1200, 80 * j);
                                Vector2 vel = -8 * Vector2.UnitX;
                                spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * (i + 0.5));
                                vel = vel.RotatedBy(Math.PI / 2 * (i + 0.5));
                                int p = Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<Projectiles.Champions.ShadowGuardian>(),
                                    npc.damage / 4, 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 1200 / 8 + 1;
                            }
                        }
                    }
                }
            }
            else
            {
                masoBool[0] = false;

                if (npc.life < npc.lifeMax * .75 && --Counter[0] < 0)
                {
                    Counter[0] = 240;

                    Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                    if (Main.netMode != NetmodeID.MultiplayerClient) //area denial circle spray of baby guardians
                    {
                        for (int j = -1; j <= 1; j++) //to both sides
                        {
                            if (j == 0)
                                continue;

                            const int gap = 40;
                            const int max = 14;
                            float modifier = 1f - (float)npc.life / npc.lifeMax;
                            modifier *= 4f / 3f; //scaling maxes at 25% life
                            if (modifier > 1f)
                                modifier = 1f;
                            int actualNumberToSpawn = (int)(max * modifier);
                            Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.ToRadians(gap) * j);
                            for (int k = 0; k < actualNumberToSpawn; k++) //a fan of skulls
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float velModifier = 1f + 9f * k / max;
                                    Projectile.NewProjectile(npc.Center, velModifier * baseVel.RotatedBy(MathHelper.ToRadians(10) * j * k),
                                        ModContent.ProjectileType<SkeletronGuardian2>(), npc.damage / 5, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient) //one more shot straight behind skeletron
                        {
                            float velModifier = 10f;
                            Projectile.NewProjectile(npc.Center, velModifier * npc.DirectionFrom(Main.player[npc.target].Center),
                                ModContent.ProjectileType<SkeletronGuardian2>(), npc.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            if (npc.ai[1] == 2f)
            {
                npc.defense = 9999;
                npc.damage = npc.defDamage * 15;

                if (!Main.dayTime)
                {
                    if (++Counter[1] < 120)
                    {
                        npc.position -= npc.velocity * (120 - Counter[1]) / 120;
                    }
                }
            }

            //drop summon
            if (!NPC.downedBoss3 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<SuspiciousSkull>());
                droppedSummon = true;
            }
        }

        public void SkeletronHandAI(NPC npc)
        {
            /*if (npc.life < npc.lifeMax / 2)
            {
                if (--Counter[0] < 0)
                {
                    Counter[0] = (int)(60f + 120f * npc.life / npc.lifeMax);
                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 speed = new Vector2(0f, -3f);
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 vel = speed.RotatedBy(Math.PI * 2 / 8 * i);
                            vel.Y -= Math.Abs(vel.X) * 0.2f;
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<SkeletronBone>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }
                }
                if (--Counter[1] < 0)
                {
                    Counter[1] = 300;
                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                        speed.X += Main.rand.Next(-20, 21);
                        speed.Y += Main.rand.Next(-20, 21);
                        speed.Normalize();
                        speed *= 3f;
                        Projectile.NewProjectile(npc.Center, speed, ProjectileID.Skull, npc.damage / 4, 0, Main.myPlayer, -1f, 0f);
                    }
                }
            }*/

            if (Main.npc[(int)npc.ai[1]].ai[1] == 1f || Main.npc[(int)npc.ai[1]].ai[1] == 2f) //spinning or DG mode
            {
                if (!masoBool[0])
                {
                    masoBool[0] = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget) //throw undead miner
                    {
                        float gravity = 0.4f; //shoot down
                        const float time = 60f;
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.BoneThrowingSkeleton);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].velocity = distance;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                }

                Counter[2] = 65;
            }
            else
            {
                masoBool[0] = false;

                if (Counter[2] > 0) //for a short period after ending spin
                {
                    if (--Counter[2] % 10 == 0 && npc.life < npc.lifeMax / 2 && npc.HasValidTarget) //periodic below 50%
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<SkeletronGuardian2>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                }
            }
        }

        public void WallOfFleshAI(NPC npc)
        {
            wallBoss = npc.whoAmI;

            if (npc.ai[3] == 0f) //when spawned in, make one eye invul
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.WallofFleshEye && Main.npc[i].realLife == npc.whoAmI)
                    {
                        Main.npc[i].ai[2] = -1f;
                        Main.npc[i].netUpdate = true;
                        npc.ai[3] = 1f;
                        npc.netUpdate = true;
                        break;
                    }
                }
            }

            if (masoBool[0]) //phase 2
            {
                if (++Counter[0] > 600)
                {
                    Counter[0] = 0;
                    Counter[1] = 0;
                    masoBool[1] = !masoBool[1];
                    masoBool[2] = false;
                    npc.netUpdate = true;
                }
                else if (Counter[0] < 240) //special attacks
                {
                    if (masoBool[3])
                        Counter[1]++;

                    if (masoBool[1]) //cursed inferno attack
                    {
                        if (Counter[0] == 10 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY, ModContent.ProjectileType<CursedDeathrayWOFS>(), 0, 0f, Main.myPlayer, npc.direction, npc.whoAmI);
                        }

                        if (++Counter[1] > 4)
                        {
                            Counter[1] = 0;
                            if (!masoBool[2])
                            {
                                masoBool[2] = true;
                                //Counter[2] = (int)(npc.Center.X + Math.Sign(npc.velocity.X) * 2500);
                            }
                            float xDistance = (2500f - 1800f * Counter[0] / 240f) * Math.Sign(npc.velocity.X);
                            Vector2 spawnPos = new Vector2(npc.Center.X + xDistance, npc.Center.Y);
                            Main.PlaySound(SoundID.Item34, spawnPos);
                            const int offsetY = 800;
                            const int speed = 14;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * offsetY, Vector2.UnitY * -speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * offsetY / 2, Vector2.UnitY * speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * -offsetY / 2, Vector2.UnitY * -speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * -offsetY, Vector2.UnitY * speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);

                                //Projectile.NewProjectile(spawnPos + Vector2.UnitY * offsetY, Vector2.UnitY * -speed, ProjectileID.CursedFlameHostile, npc.damage / 4, 0f, Main.myPlayer);
                                //Projectile.NewProjectile(spawnPos + Vector2.UnitY * -offsetY, Vector2.UnitY * speed, ProjectileID.CursedFlameHostile, npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    else //ichor attack
                    {
                        /*if (Counter[0] == 2)
                        {
                            for (int i = 0; i < 36; i++) //telegraphing dust ring
                            {
                                Vector2 vector6 = Vector2.UnitY * 12f;
                                vector6 = vector6.RotatedBy((i - (36 / 2 - 1)) * 6.28318548f / 36) + npc.Center;
                                Vector2 vector7 = vector6 - npc.Center;
                                int d = Dust.NewDust(vector6 + vector7, 0, 0, 87, 0f, 0f, 0, default(Color), 4f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity = vector7;
                            }
                        }*/

                        if (++Counter[1] > 8)
                        {
                            Counter[1] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                /*Vector2 target = npc.Center;
                                target.X += Math.Sign(npc.velocity.X) * 1800f * Counter[0] / 240f; //gradually targets further and further
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 speed = target - npc.Center;
                                    speed.Y -= Math.Abs(speed.X) * 0.2f; //account for gravity
                                                                         //speed.Normalize(); speed *= 8f;
                                    speed /= 45f * 3f; //ichor has 3 updates per tick
                                    speed += npc.velocity / 3f;
                                    speed.X += Main.rand.Next(-20, 21) * 0.08f;
                                    speed.Y += Main.rand.Next(-20, 21) * 0.08f;
                                    Projectile.NewProjectile(npc.Center, speed, ProjectileID.GoldenShowerHostile, npc.damage / 5, 0f, Main.myPlayer);
                                }*/

                                for (int i = 0; i < 8; i++)
                                {
                                    Vector2 target = npc.Center;
                                    target.X += Math.Sign(npc.velocity.X) * 1000f * Counter[0] / 240f; //gradually targets further and further
                                    target.X += Main.rand.NextFloat(-100, 100);
                                    target.Y += Main.rand.NextFloat(-450, 450);
                                    const float gravity = 0.5f;
                                    float time = 60f;
                                    Vector2 distance = target - npc.Center;
                                    distance.X = distance.X / time;
                                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                                    Projectile.NewProjectile(npc.Center + Vector2.UnitX * Math.Sign(npc.velocity.X) * 32f, distance, 
                                        ModContent.ProjectileType<GoldenShowerWOF>(), npc.damage / 4, 0f, Main.myPlayer, time);
                                }
                            }
                        }
                    }
                }
            }
            else if (npc.life < npc.lifeMax * .75) //enter phase 2
            {
                masoBool[0] = true;
                npc.netUpdate = true;
                //Main.PlaySound(SoundID.Roar, npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center, 0);
                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster94"),
                        npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center);
                    if (Main.LocalPlayer.active)
                        Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 90;
                }
            }
            
            if (npc.ai[3] == 2) //phase 3
            {
                if (masoBool[3])
                {
                    Counter[2]--; //increment faster

                    if (Counter[0] % 2 == 1) //always make sure its even in here
                        Counter[0]--;
                }

                if (Counter[0] >= 240 - (masoBool[3] ? 120 : 0) && Counter[0] <= 600 - 180 - (masoBool[3] ? 120 : 0))
                {
                    if (--Counter[2] < 0)
                    {
                        Counter[2] = 80;
                        if (npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn reticles for chain barrages
                            {
                                Vector2 spawnPos = Main.player[npc.target].Center;
                                spawnPos.X += Math.Sign(npc.velocity.X) * Main.rand.NextFloat(1000);
                                spawnPos.Y += Main.rand.NextFloat(-300, 300);
                                if (spawnPos.Y / 16 < Main.maxTilesY - 200) //clamp so it stays in hell
                                    spawnPos.Y = (Main.maxTilesY - 200) * 16;
                                if (spawnPos.Y / 16 >= Main.maxTilesY)
                                    spawnPos.Y = Main.maxTilesY * 16 - 16;
                                Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<WOFReticle>(), npc.damage / 6, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
                else
                {
                    Counter[2] = 0;
                }
            }
            else if (npc.ai[3] == 1 && npc.life < npc.lifeMax * .5) //enter phase 3
            {
                npc.ai[3] = 2;
                npc.netUpdate = true;
                //Main.PlaySound(SoundID.Roar, npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center, 0);
                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster94"),
                        npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center);
                    if (Main.LocalPlayer.active)
                        Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 90;
                }
            }

            if (npc.life < npc.lifeMax / 10) //final phase
            {
                Counter[0]++;

                if (Counter[3] > 0)
                    Counter[3]--;

                if (!masoBool[3])
                {
                    Counter[3] = 60;
                    masoBool[3] = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NetUpdateMaso(npc.whoAmI);
                    //Main.PlaySound(SoundID.Roar, npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center, 0);
                    if (!Main.dedServ)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster5").WithVolume(1.5f),
                            npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center);
                        if (Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 180;
                    }
                }
            }

            /*if (--Counter < 0)
            {
                Counter = 60 + (int)(120f * npc.life / npc.lifeMax);
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && Main.player[npc.target].active) //vanilla spaz p1 shoot fireball code
                {
                    Vector2 Speed = Main.player[npc.target].Center - npc.Center;
                    if (Speed.X * npc.velocity.X > 0) //don't shoot fireballs behind myself
                    {
                        Speed.Normalize();
                        int Damage;
                        Speed *= 10f;
                        Damage = npc.damage / 12;
                        Speed.X += Main.rand.Next(-40, 41) * 0.02f;
                        Speed.Y += Main.rand.Next(-40, 41) * 0.02f;
                        Speed += Main.player[npc.target].velocity / 5;
                        Projectile.NewProjectile(npc.Center + Speed * 4f, Speed, ProjectileID.CursedFlameHostile, Damage, 0f, Main.myPlayer);
                    }
                }
            }

            if (--Timer < 0) //ichor vomit
            {
                Timer = 300 + 300 * (int)((float)npc.life / npc.lifeMax);
                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient && Main.player[npc.target].active)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                        speed.Y -= Math.Abs(speed.X) * 0.2f; //account for gravity
                        speed.Normalize();
                        speed *= 8f;
                        speed += npc.velocity / 3f;
                        speed.X += Main.rand.Next(-20, 21) * 0.08f;
                        speed.Y += Main.rand.Next(-20, 21) * 0.08f;
                        Projectile.NewProjectile(npc.Center, speed, ProjectileID.GoldenShowerHostile, npc.damage / 25, 0f, Main.myPlayer);
                    }
                }
            }*/

            const float maxSpeed = 3.5f;

            if (npc.HasPlayerTarget && (Main.player[npc.target].dead || Vector2.Distance(npc.Center, Main.player[npc.target].Center) > 3000))
            {
                npc.TargetClosest(true);
                if (Main.player[npc.target].dead || Vector2.Distance(npc.Center, Main.player[npc.target].Center) > 3000)
                {
                    npc.position.X += 60 * Math.Sign(npc.velocity.X); //move faster to despawn
                }
                else if (Math.Abs(npc.velocity.X) > maxSpeed)
                {
                    npc.position.X -= (Math.Abs(npc.velocity.X) - maxSpeed) * Math.Sign(npc.velocity.X);
                }
            }
            else if (Math.Abs(npc.velocity.X) > maxSpeed)
            {
                npc.position.X -= (Math.Abs(npc.velocity.X) - maxSpeed) * Math.Sign(npc.velocity.X);
            }

            //dont do aura with swarm active
            if (Main.player[Main.myPlayer].active & !Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].ZoneUnderworldHeight && !(bool)ModLoader.GetMod("Fargowiltas").Call("SwarmActive"))
            {
                float velX = npc.velocity.X;
                if (velX > maxSpeed)
                    velX = maxSpeed;
                else if (velX < -maxSpeed)
                    velX = -maxSpeed;

                for (int i = 0; i < 10; i++) //dust
                {
                    Vector2 dustPos = new Vector2(2000 * npc.direction, 0f).RotatedBy(Math.PI / 3 * (-0.5 + Main.rand.NextDouble()));
                    int d = Dust.NewDust(npc.Center + dustPos, 0, 0, DustID.Fire);
                    Main.dust[d].scale += 1f;
                    Main.dust[d].velocity.X = velX;
                    Main.dust[d].velocity.Y = npc.velocity.Y;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                }

                if (++npc.localAI[1] > 15f)
                {
                    npc.localAI[1] = 0f; //tongue the player if they're 2000-2800 units away
                    if (Math.Abs(2400 - npc.Distance(Main.player[Main.myPlayer].Center)) < 400)
                    {
                        if (!Main.player[Main.myPlayer].tongued)
                            Main.PlaySound(SoundID.ForceRoar, Main.player[Main.myPlayer].Center, -1); //eoc roar
                        Main.player[Main.myPlayer].AddBuff(BuffID.TheTongue, 10);
                    }
                }
            }

            //drop summon
            if (!Main.hardMode && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<FleshyDoll>());
                droppedSummon = true;
            }
        }

        public bool WallOfFleshEyeAI(NPC npc)
        {
            if (masoBool[3])
                return true;

            float maxTime = 540f;

            if (npc.realLife != -1 && Main.npc[npc.realLife].GetGlobalNPC<EModeGlobalNPC>().masoBool[3])
            {
                if (npc.ai[1] < maxTime - 180) //dont lower this if it's already telegraphing laser
                    maxTime = 240f;

                npc.localAI[1] = 0f; //no more regular lasers
            }

            if (++npc.ai[1] >= maxTime)
            {
                npc.ai[1] = 0f;
                if (npc.ai[2] == 0f)
                    npc.ai[2] = 1f;
                else
                    npc.ai[2] *= -1f;

                if (npc.ai[2] > 0) //FIRE LASER
                {
                    Vector2 speed = Vector2.UnitX.RotatedBy(npc.ai[3]);
                    float ai0 = (npc.realLife != -1 && Main.npc[npc.realLife].velocity.X > 0) ? 1f : 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.npc[npc.realLife].GetGlobalNPC<EModeGlobalNPC>().Counter[3] <= 0)
                        Projectile.NewProjectile(npc.Center, speed, mod.ProjectileType("PhantasmalDeathrayWOF"), npc.damage / 4, 0f, Main.myPlayer, ai0, npc.whoAmI);
                }
                else //ring dust to denote i am vulnerable now
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                }
                npc.netUpdate = true;
            }

            if (npc.ai[2] >= 0f)
            {
                npc.alpha = 175;
                npc.dontTakeDamage = true;
                if (npc.ai[1] <= 90) //still firing laser rn
                {
                    masoBool[3] = true;
                    npc.AI();
                    masoBool[3] = false;
                    npc.localAI[1] = 0f;
                    npc.rotation = npc.ai[3];
                    return false;
                }
                else
                {
                    npc.ai[2] = 1;
                }
            }
            else
            {
                npc.alpha = 0;
                npc.dontTakeDamage = false;

                if (npc.ai[1] == maxTime - 3 * 5 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.npc[npc.realLife].GetGlobalNPC<EModeGlobalNPC>().Counter[3] <= 0)
                    {
                        float ai0 = (npc.realLife != -1 && Main.npc[npc.realLife].velocity.X > 0) ? 1f : 0f;
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("WOFBlast"), 0, 0f, Main.myPlayer, ai0, npc.whoAmI);
                    }
                }

                if (npc.ai[1] > maxTime - 180f)
                {
                    if (Main.rand.Next(4) < 3) //dust telegraphs switch
                    {
                        int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 88, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 114, default(Color), 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1.8f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                        if (Main.rand.Next(4) == 0)
                        {
                            Main.dust[dust].noGravity = false;
                            Main.dust[dust].scale *= 0.5f;
                        }
                    }

                    float stopTime = maxTime - 90f;
                    if (npc.ai[1] == stopTime) //shoot warning dust in phase 2
                    {
                        int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                        if (t != -1)
                        {
                            if (npc.Distance(Main.player[t].Center) < 3000)
                                Main.PlaySound(SoundID.Roar, (int)Main.player[t].position.X, (int)Main.player[t].position.Y, 0);
                            npc.ai[2] = -2f;
                            npc.ai[3] = (npc.Center - Main.player[t].Center).ToRotation();
                            if (npc.realLife != -1 && Main.npc[npc.realLife].velocity.X > 0)
                                npc.ai[3] += (float)Math.PI;

                            float ai0 = (npc.realLife != -1 && Main.npc[npc.realLife].velocity.X > 0) ? 1f : 0f;
                            Vector2 speed = Vector2.UnitX.RotatedBy(npc.ai[3]);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, speed, mod.ProjectileType("PhantasmalDeathrayWOFS"), 0, 0f, Main.myPlayer, ai0, npc.whoAmI);
                        }
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[1] > stopTime)
                    {
                        masoBool[3] = true;
                        npc.AI();
                        masoBool[3] = false;
                        npc.localAI[1] = 0f;
                        npc.rotation = npc.ai[3];
                        return false;
                    }
                }
            }
            
            if (npc.realLife != -1)
            {
                //dont fire during mouth's special attacks (this is at bottom to override others)
                EModeGlobalNPC wall = Main.npc[npc.realLife].GetGlobalNPC<EModeGlobalNPC>();
                if ((wall.masoBool[0] && wall.Counter[0] < 240) || wall.masoBool[3])
                {
                    npc.localAI[1] = -90f;
                }
            }

            return true;
        }

        public bool RetinazerAI(NPC npc)
        {
            retiBoss = npc.whoAmI;

            bool spazAlive = BossIsAlive(ref spazBoss, NPCID.Spazmatism);

            if (!masoBool[0]) //start phase 2
            {
                masoBool[0] = true;
                npc.ai[0] = 1f;
                npc.ai[1] = 0.0f;
                npc.ai[2] = 0.0f;
                npc.ai[3] = 0.0f;
                npc.netUpdate = true;
            }

            if (npc.life <= npc.lifeMax / 2 || npc.dontTakeDamage)
            {
                npc.dontTakeDamage = npc.life == 1 || !npc.HasValidTarget;
                if (npc.life != 1 && npc.HasValidTarget)
                    npc.dontTakeDamage = false;
                //become vulnerable again when both twins at low life
                if (npc.dontTakeDamage && npc.HasValidTarget && (!spazAlive || Main.npc[spazBoss].life == 1))
                    npc.dontTakeDamage = false;
            }

            if (Main.dayTime)
            {
                if (npc.velocity.Y > 0)
                    npc.velocity.Y = 0;

                npc.velocity.Y -= 0.5f;
                npc.dontTakeDamage = true;

                if (spazAlive)
                {
                    if (npc.timeLeft < 60)
                        npc.timeLeft = 60;

                    if (Main.npc[spazBoss].timeLeft < 60)
                        Main.npc[spazBoss].timeLeft = 60;

                    npc.TargetClosest(false);
                    Main.npc[spazBoss].TargetClosest(false);
                    if (npc.Distance(Main.player[npc.target].Center) > 2000 && Main.npc[spazBoss].Distance(Main.player[Main.npc[spazBoss].target].Center) > 2000)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            Main.npc[spazBoss].active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spazBoss);
                        }
                    }
                }

                return true;
            }

            if (npc.ai[0] < 4f) //going to phase 3
            {
                if (npc.life <= npc.lifeMax / 2)
                {
                    //npc.ai[0] = 4f;
                    npc.ai[0] = 604f; //initiate spin immediately
                    npc.netUpdate = true;
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 11, npc.whoAmI);
                }
            }
            else //in phase 3
            {

                /*if (npc.life == 1 && --Counter[1] < 0) //when brought to 1hp, begin shooting dark stars
                {
                    Counter[1] = 240;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.Normalize();
                        distance *= 10f;
                        for (int i = 0; i < 12; i++)
                            Projectile.NewProjectile(npc.Center, distance.RotatedBy(2 * Math.PI / 12 * i),
                                ModContent.ProjectileType<DarkStar>(), npc.damage / 5, 0f, Main.myPlayer);
                    }
                }*/

                //dust code
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 90, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.Next(4) == 0)
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                SharkCount = 253;

                if (npc.localAI[1] >= (npc.ai[1] == 0 ? 175 : 55)) //hijacking vanilla laser code
                {
                    npc.localAI[1] = 0;
                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center);
                    int proj = Projectile.NewProjectile(npc.Center + (npc.width - 24) * vel, vel, ModContent.ProjectileType<DarkStarHoming>(), npc.damage / 4, 0f, Main.myPlayer, -1, 1f);
                    if (proj != Main.maxProjectiles)
                        Main.projectile[proj].timeLeft = 120;
                }

                if (Counter[0] == 0 || Counter[0] == 3) //not doing deathray, grow arena
                {
                    Counter[3]--;
                    if (Counter[3] < 0)
                        Counter[3] = 0;
                }
                else //doing deathray, shrink arena
                {
                    Counter[3]++;
                    if (Counter[3] > 180)
                        Counter[3] = 180;
                }

                float auraDistance = 2000 - 1200 * Counter[3] / 180f;
                Aura(npc, auraDistance, true, DustID.Fire, default, ModContent.BuffType<Oiled>(), BuffID.OnFire, BuffID.Burning);

                Player p = Main.player[Main.myPlayer];

                //2*pi * (# of full circles) / (seconds to finish rotation) / (ticks per sec)
                const float rotationInterval = 2f * (float)Math.PI * 1.2f / 4f / 60f;

                npc.ai[0]++; //base value is 4
                switch (Counter[0]) //laser code idfk
                {
                    case 0:
                        if (!npc.HasValidTarget)
                        {
                            npc.ai[0]--; //stop counting up if player is dead
                            if (!spazAlive) //despawn REALLY fast
                                npc.velocity.Y -= 0.5f;
                        }
                        if (npc.ai[0] > 604f)
                        {
                            npc.ai[0] = 4f;
                            if (npc.HasPlayerTarget)
                            {
                                npc.rotation = npc.Center.X < Main.player[npc.target].Center.X ? 0 : (float)Math.PI;
                                npc.rotation -= (float)Math.PI / 2;

                                Counter[0]++;
                                npc.ai[3] = -npc.rotation;
                                if (--npc.ai[2] > 295f)
                                    npc.ai[2] = 295f;
                                masoBool[2] = (Main.player[npc.target].Center.X - npc.Center.X < 0);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                                Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f); //eoc roar
                            }
                            npc.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server) //synchronize counter with clients
                            {
                                var netMessage = mod.GetPacket();
                                netMessage.Write((byte)5);
                                netMessage.Write((byte)npc.whoAmI);
                                netMessage.Write(masoBool[2]);
                                netMessage.Write(Counter[0]);
                                netMessage.Send();
                            }
                        }
                        break;

                    case 1: //slowing down, beginning rotation
                        npc.velocity *= 1f - (npc.ai[0] - 4f) / 120f;
                        npc.localAI[1] = 0f;
                        //if (--npc.ai[2] > 295f) npc.ai[2] = 295f;
                        npc.ai[3] -= (npc.ai[0] - 4f) / 120f * rotationInterval * (masoBool[2] ? 1f : -1f);
                        npc.rotation = -npc.ai[3];

                        if (npc.ai[0] == 5f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 9f, npc.whoAmI);
                            }
                        }

                        if (npc.ai[0] >= 125f) //FIRE LASER
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 speed = Vector2.UnitX.RotatedBy(npc.rotation);
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<PhantasmalDeathray>(), npc.damage / 3, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                            Counter[0]++;
                            npc.ai[0] = 4f;
                            npc.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server) //synchronize counter with clients
                            {
                                var netMessage = mod.GetPacket();
                                netMessage.Write((byte)5);
                                netMessage.Write((byte)npc.whoAmI);
                                netMessage.Write(masoBool[2]);
                                netMessage.Write(Counter[0]);
                                netMessage.Send();
                            }
                        }
                        return false;

                    case 2: //spinning full speed
                        npc.velocity = Vector2.Zero;
                        npc.localAI[1] = 0f;
                        //if (--npc.ai[2] > 295f) npc.ai[2] = 295f;
                        npc.ai[3] -= rotationInterval * (masoBool[2] ? 1f : -1f);
                        npc.rotation = -npc.ai[3];

                        if (npc.ai[0] >= 244f)
                        {
                            Counter[0]++;
                            npc.ai[0] = 4f;
                            npc.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server) //synchronize counter with clients
                            {
                                var netMessage = mod.GetPacket();
                                netMessage.Write((byte)5);
                                netMessage.Write((byte)npc.whoAmI);
                                netMessage.Write(masoBool[2]);
                                netMessage.Write(Counter[0]);
                                netMessage.Send();
                            }
                        }
                        else if (!npc.HasValidTarget) //end spin immediately if player dead
                        {
                            npc.TargetClosest(false);
                            if (!npc.HasValidTarget)
                                npc.ai[0] = 244f;
                        }
                        return false;

                    case 3: //laser done, slowing down spin, moving again
                        npc.velocity *= (npc.ai[0] - 4f) / 60f;
                        npc.localAI[1] = 0f;
                        //if (--npc.ai[2] > 295f) npc.ai[2] = 295f;
                        npc.ai[3] -= (1f - (npc.ai[0] - 4f) / 60f) * rotationInterval * (masoBool[2] ? 1f : -1f);
                        npc.rotation = -npc.ai[3];

                        if (npc.ai[0] >= 64f)
                        {
                            Counter[0] = 0;
                            npc.ai[0] = 4f;
                            npc.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server) //synchronize counter with clients
                            {
                                var netMessage = mod.GetPacket();
                                netMessage.Write((byte)5);
                                netMessage.Write((byte)npc.whoAmI);
                                netMessage.Write(masoBool[2]);
                                netMessage.Write(Counter[0]);
                                netMessage.Send();
                            }
                        }
                        return false;

                    default:
                        Counter[0] = 0;
                        npc.ai[0] = 4f;
                        npc.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server) //synchronize counter with clients
                        {
                            var netMessage = mod.GetPacket();
                            netMessage.Write((byte)5);
                            netMessage.Write((byte)npc.whoAmI);
                            netMessage.Write(masoBool[2]);
                            netMessage.Write(Counter[0]);
                            netMessage.Send();
                        }
                        break;
                }

                //npc.position += npc.velocity / 4f;

                //if (Counter == 600 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                //{
                //    Vector2 vector200 = Main.player[npc.target].Center - npc.Center;
                //    vector200.Normalize();
                //    float num1225 = -1f;
                //    if (vector200.X < 0f)
                //    {
                //        num1225 = 1f;
                //    }
                //    vector200 = vector200.RotatedBy(-num1225 * 1.04719755f, default(Vector2));
                //    Projectile.NewProjectile(npc.Center, vector200, ModContent.ProjectileType<PhantasmalDeathray>(), npc.damage / 2, 0f, Main.myPlayer, num1225 * 0.0104719755f, npc.whoAmI);
                //    npc.netUpdate = true;
                //}
            }

            /*if (!BossIsAlive(ref spazBoss, NPCID.Spazmatism) && targetAlive)
            {
                Timer--;

                if (Timer <= 0)
                {
                    Timer = 600;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spawn = NPC.NewNPC((int)npc.position.X + Main.rand.Next(-1000, 1000), (int)npc.position.Y + Main.rand.Next(-400, -100), NPCID.Spazmatism);
                        if (spawn != 200)
                        {
                            Main.npc[spawn].life = Main.npc[spawn].lifeMax / 4;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Spazmatism has been revived!"), new Color(175, 75, 255));
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                            }
                            else
                            {
                                Main.NewText("Spazmatism has been revived!", 175, 75, 255);
                            }
                        }
                    }
                }
            }*/

            //drop summon
            if (Main.hardMode && !NPC.downedMechBoss2 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<MechEye>());
                droppedSummon = true;
            }

            return true;
        }

        public bool SpazmatismAI(NPC npc)
        {
            spazBoss = npc.whoAmI;
            bool retiAlive = BossIsAlive(ref retiBoss, NPCID.Retinazer);

            canHurt = true;

            float modifier = (float)npc.life / npc.lifeMax;

            if (!masoBool[0]) //spawn in phase 2
            {
                masoBool[0] = true;
                npc.ai[0] = 1f;
                npc.ai[1] = 0.0f;
                npc.ai[2] = 0.0f;
                npc.ai[3] = 0.0f;
                npc.netUpdate = true;
            }
            
            if (npc.life <= npc.lifeMax / 2 || npc.dontTakeDamage)
            {
                npc.dontTakeDamage = npc.life == 1 || !npc.HasValidTarget;
                if (npc.life != 1 && npc.HasValidTarget)
                    npc.dontTakeDamage = false;
                //become vulnerable again when both twins low
                if (npc.dontTakeDamage && npc.HasValidTarget && (!retiAlive || Main.npc[retiBoss].life == 1))
                    npc.dontTakeDamage = false;
            }

            if (Main.dayTime)
            {
                if (npc.velocity.Y > 0)
                    npc.velocity.Y = 0;

                npc.velocity.Y -= 0.5f;
                npc.dontTakeDamage = true;

                if (retiAlive)
                {
                    if (npc.timeLeft < 60)
                        npc.timeLeft = 60;

                    if (Main.npc[retiBoss].timeLeft < 60)
                        Main.npc[retiBoss].timeLeft = 60;

                    npc.TargetClosest(false);
                    Main.npc[retiBoss].TargetClosest(false);
                    if (npc.Distance(Main.player[npc.target].Center) > 2000 && Main.npc[retiBoss].Distance(Main.player[Main.npc[retiBoss].target].Center) > 2000)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            Main.npc[retiBoss].active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, retiBoss);
                        }
                    }
                }

                return true;
            }

            if (npc.ai[0] < 4f)
            {
                if (npc.life <= npc.lifeMax / 2) //going to phase 3
                {
                    npc.ai[0] = 4f;
                    npc.netUpdate = true;
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                    int index = npc.FindBuffIndex(BuffID.CursedInferno);
                    if (index != -1)
                        npc.DelBuff(index); //remove cursed inferno debuff if i have it

                    npc.buffImmune[BuffID.CursedInferno] = true;
                    npc.buffImmune[BuffID.OnFire] = true;
                    npc.buffImmune[BuffID.ShadowFlame] = true;
                    npc.buffImmune[BuffID.Frostburn] = true;
                }
            }
            else //in phase 3
            {
                //npc.position += npc.velocity / 10f;

                //dust code
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 89, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.Next(4) == 0)
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                SharkCount = 254;

                if (npc.ai[1] == 0f) //not dashing
                {
                    if (retiAlive && (Main.npc[retiBoss].ai[0] < 4f || Main.npc[retiBoss].GetGlobalNPC<EModeGlobalNPC>().Counter[0] == 0
                        || Main.npc[retiBoss].GetGlobalNPC<EModeGlobalNPC>().Counter[0] == 3)) //reti is in normal AI
                    {
                        npc.ai[1] = 1; //switch to dashing
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                        NetUpdateMaso(npc.whoAmI);
                        return false;
                    }

                    if (npc.HasValidTarget && retiAlive)
                    {
                        Vector2 target = Main.npc[retiBoss].Center + Main.npc[retiBoss].DirectionTo(npc.Center) * 100;
                        npc.velocity = (target - npc.Center) / 60;

                        const float rotationInterval = 2f * (float)Math.PI * 1.2f / 4f / 60f * 0.65f;
                        npc.rotation += rotationInterval * (Main.npc[retiBoss].GetGlobalNPC<EModeGlobalNPC>().masoBool[2] ? 1f : -1f);

                        if (Counter[2] < 0)
                            Counter[2] = 0;

                        if (Counter[3] == 0) //i can't be bothered to figure out the formula for this rn
                        {
                            Counter[3] = 2;
                            if (modifier < 0.5f / 4 * 3)
                                Counter[3] = 3;
                            if (modifier < 0.5f / 4 * 2)
                                Counter[3] = 4;
                            if (modifier < 0.5f / 4 * 1)
                                Counter[3] = 5;
                        }

                        if (++Counter[2] < 30) //snap to reti, don't do contact damage
                        {
                            npc.rotation = npc.DirectionTo(Main.npc[retiBoss].Center).ToRotation() - (float)Math.PI / 2;
                            canHurt = false;
                        }
                        else if (++Counter[0] > 3) //rings of fire
                        {
                            Counter[0] = 0;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = 24f * Math.Min((Counter[2] - 30) / 120f, 1f); //fan out gradually
                                int timeLeft = (int)(speed / 24f * 45f);
                                float baseRotation = npc.rotation + (float)Math.PI / 2;
                                if (timeLeft > 5)
                                {
                                    for (int i = 0; i < Counter[3]; i++)
                                    {
                                        int p = Projectile.NewProjectile(npc.Center, speed * (baseRotation + MathHelper.TwoPi / Counter[3] * i).ToRotationVector2(),
                                            ModContent.ProjectileType<DarkStar>(), npc.damage / 4, 0f, Main.myPlayer);
                                        if (p != Main.maxProjectiles)
                                            Main.projectile[p].timeLeft = timeLeft;
                                    }
                                }
                            }
                        }

                        /*if (++Counter[0] > 40)
                        {
                            Counter[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget) //vanilla spaz p1 shoot fireball code
                            {
                                Vector2 Speed = Main.player[npc.target].Center - npc.Center;
                                Speed.Normalize();
                                int Damage;
                                if (Main.expertMode)
                                {
                                    Speed *= 14f;
                                    Damage = 22;
                                }
                                else
                                {
                                    Speed *= 12f;
                                    Damage = 25;
                                }
                                Projectile.NewProjectile(npc.Center + Speed * 4f, Speed, ProjectileID.CursedFlameHostile, Damage, 0f, Main.myPlayer);
                            }
                        }*/

                        return false;
                    }
                }
                else //dashing
                {
                    if (retiAlive && Main.npc[retiBoss].ai[0] >= 4f && Main.npc[retiBoss].GetGlobalNPC<EModeGlobalNPC>().Counter[0] != 0
                        && Main.npc[retiBoss].GetGlobalNPC<EModeGlobalNPC>().Counter[0] != 3) //reti is doing the spin
                    {
                        npc.ai[1] = 0; //switch to not dashing
                        npc.netUpdate = true;
                        NetUpdateMaso(npc.whoAmI);
                        return false;
                    }

                    Counter[3] = 0;

                    if (Counter[2] > 75) //cooldown before attacking again
                        Counter[2] = 75;
                    if (Counter[2] > 0)
                    {
                        Counter[2]--;
                        if (npc.HasValidTarget)
                        {
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
                        return false;
                    }

                    if (npc.ai[2] > 50)
                    {
                        npc.ai[2] -= modifier;
                    }
                    else
                    {
                        if (npc.HasValidTarget && ++Counter[0] > 3) //cursed flamethrower when dashing
                        {
                            Counter[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = (1f - modifier) * 0.8f;
                                float rotationVariance = 9f * modifier * 2;
                                Projectile.NewProjectile(npc.Center, speed * npc.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-rotationVariance, rotationVariance))), ProjectileID.EyeFire, npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                /*if (npc.life == 1 && --Counter[1] < 0) //when brought to 1hp, begin shooting dark stars
                {
                    Counter[1] = 120;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.Normalize();
                        distance *= 14f;
                        for (int i = 0; i < 8; i++)
                            Projectile.NewProjectile(npc.Center, distance.RotatedBy(2 * Math.PI / 8 * i),
                                ModContent.ProjectileType<DarkStar>(), npc.damage / 5, 0f, Main.myPlayer);
                    }
                }*/
            }

            /*if (!retiAlive && npc.HasPlayerTarget && Main.player[npc.target].active)
            {
                Timer--;

                if (Timer <= 0)
                {
                    Timer = 600;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spawn = NPC.NewNPC((int)npc.position.X + Main.rand.Next(-1000, 1000), (int)npc.position.Y + Main.rand.Next(-400, -100), NPCID.Retinazer);
                        if (spawn != 200)
                        {
                            Main.npc[spawn].life = Main.npc[spawn].lifeMax / 4;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Retinazer has been revived!"), new Color(175, 75, 255));
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                            }
                            else
                            {
                                Main.NewText("Retinazer has been revived!", 175, 75, 255);
                            }
                        }
                    }
                }
            }*/

            return true;
        }

        public bool DestroyerAI(NPC npc)
        {
            destroyBoss = npc.whoAmI;

            const int p2AttackTime = 480;
            
            if (!masoBool[0])
            {
                npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;
                npc.buffImmune[BuffID.Chilled] = false;
                //npc.buffImmune[BuffID.Darkness] = false;

                if (npc.life < (int)(npc.lifeMax * .75))
                {
                    masoBool[0] = true;
                    Counter[0] = p2AttackTime * 3;
                    npc.netUpdate = true;
                    if (npc.HasPlayerTarget)
                        Main.PlaySound(SoundID.Roar, (int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, 0);
                }
            }
            else
            {
                int projDamage = npc.damage / 9;

                if (npc.HasValidTarget && !Main.dayTime)
                {
                    npc.timeLeft = 600; //don't despawn

                    if (masoBool[1]) //spinning
                    {
                        npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = true;

                        npc.netUpdate = true;
                        npc.velocity = Vector2.Normalize(npc.velocity) * 20f;
                        npc.velocity += npc.velocity.RotatedBy(Math.PI / 2) * npc.velocity.Length() / Counter[1];
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;

                        if (Counter[0] == 0)
                            Counter[2] = 0;

                        if (npc.life < npc.lifeMax / 10) //permanent coil phase 3
                        {
                            if (npc.localAI[2] >= 0)
                            {
                                npc.localAI[2] = 0;
                                Counter[0] = 0; //for edge case where destroyer coils, then goes below 10% while coiling, make sure DR behaves right
                            }

                            if (--npc.localAI[2] < -120)
                            { 
                                npc.localAI[2] = -120 + 5;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 distance = npc.DirectionTo(Main.player[npc.target].Center) * 14f;
                                    int type = ModContent.ProjectileType<DarkStarHoming>();
                                    Projectile.NewProjectile(npc.Center, distance, type, projDamage, 0f, Main.myPlayer, npc.target);
                                }
                            }

                            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
                            {
                                npc.TargetClosest(false);
                                if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
                                {
                                    Counter[0] = 0;
                                    Counter[1] = 0;
                                    masoBool[1] = false;
                                    masoBool[2] = false;
                                    masoBool[3] = Main.rand.NextBool();
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        NetUpdateMaso(npc.whoAmI);
                                }
                            }

                            Counter[0]++;
                        }
                        else
                        {
                            if (--npc.localAI[2] < 0) //shoot star spreads into the circle
                            {
                                npc.localAI[2] = Main.player[npc.target].HasBuff(ModContent.BuffType<LightningRod>()) ? 110 : 65;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 distance = Main.player[npc.target].Center - npc.Center;
                                    double angleModifier = MathHelper.ToRadians(30);
                                    distance.Normalize();
                                    distance *= 14f;
                                    int type = ModContent.ProjectileType<DarkStarHoming>();
                                    int delay = -5;
                                    Projectile.NewProjectile(npc.Center, distance.RotatedBy(-angleModifier), type, projDamage, 0f, Main.myPlayer, npc.target, delay);
                                    Projectile.NewProjectile(npc.Center, distance, type, projDamage, 0f, Main.myPlayer, npc.target, delay);
                                    Projectile.NewProjectile(npc.Center, distance.RotatedBy(angleModifier), type, projDamage, 0f, Main.myPlayer, npc.target, delay);
                                }
                            }

                            if (++Counter[0] > 300) //go back to normal AI
                            {
                                Counter[0] = 0;
                                Counter[1] = 0;
                                masoBool[1] = false;
                                masoBool[2] = false;
                                masoBool[3] = Main.rand.NextBool();
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetUpdateMaso(npc.whoAmI);
                            }
                        }

                        Vector2 pivot = npc.Center;
                        pivot += Vector2.Normalize(npc.velocity.RotatedBy(Math.PI / 2)) * 600;
                        
                        if (++Counter[2] > 95)
                        {
                            Counter[2] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int max = (int)(14f - 10f * npc.life / npc.lifeMax);
                                if (max % 2 != 0) //always shoot even number
                                    max++;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 speed = npc.DirectionTo(pivot).RotatedBy(2 * Math.PI / max * i);
                                    Vector2 spawnPos = pivot - speed * 600;
                                    Projectile.NewProjectile(spawnPos, 0.2f * speed, ModContent.ProjectileType<DestroyerLaser>(), projDamage, 0f, Main.myPlayer, 1f);
                                }
                            }
                        }

                        for (int i = 0; i < 20; i++) //arena dust
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 600);
                            offset.Y += (float)(Math.Cos(angle) * 600);
                            Dust dust = Main.dust[Dust.NewDust(pivot + offset - new Vector2(4, 4), 0, 0, 112, 0, 0, 100, Color.White, 1f)];
                            dust.velocity = Vector2.Zero;
                            if (Main.rand.Next(3) == 0)
                                dust.velocity += Vector2.Normalize(offset) * 5f;
                            dust.noGravity = true;
                        }

                        Player target = Main.player[npc.target];
                        if (target.active && !target.dead) //arena effect
                        {
                            float distance = target.Distance(pivot);
                            if (distance > 600 && distance < 3000)
                            {
                                Vector2 movement = pivot - target.Center;
                                float difference = movement.Length() - 600;
                                movement.Normalize();
                                movement *= difference < 34f ? difference : 34f;
                                target.position += movement;

                                for (int i = 0; i < 20; i++)
                                {
                                    int d = Dust.NewDust(target.position, target.width, target.height, 112, 0f, 0f, 0, default(Color), 2f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity *= 5f;
                                }
                            }
                        }
                    }
                    else
                    {
                        npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;

                        float maxSpeed = 16f;    //max speed?
                        float num15 = 0.1f;   //turn speed?
                        float num16 = 0.15f;   //acceleration?

                        bool fastStart = Counter[0] < 120;

                        float flySpeedModifierRatio = (float)npc.life / npc.lifeMax;
                        if (flySpeedModifierRatio > 0.5f) //prevent it from subtracting speed
                            flySpeedModifierRatio = 0.5f;
                        if (fastStart) //if just entered this stage, max out ratio
                            flySpeedModifierRatio = 0;

                        if (npc.HasValidTarget)
                        {
                            if (!fastStart) //after fast start to uncoil
                            {
                                float distance = npc.Distance(Main.player[npc.target].Center);
                                if (distance < 600) //slower nearby
                                {
                                    maxSpeed *= 0.5f;
                                }
                                else if (distance > 800) //come at you really hard when out of range
                                {
                                    num15 *= 2f;
                                    num16 *= 2f;
                                }
                            }

                            float comparisonSpeed = Main.player[npc.target].velocity.Length() * 1.5f;
                            float rotationDifference = MathHelper.WrapAngle(npc.velocity.ToRotation() - npc.DirectionTo(Main.player[npc.target].Center).ToRotation());
                            bool inFrontOfMe = Math.Abs(rotationDifference) < MathHelper.ToRadians(90 / 2);
                            if (maxSpeed < comparisonSpeed && inFrontOfMe) //player is moving faster than my top speed
                            {
                                maxSpeed = comparisonSpeed; //outspeed them
                            }
                        }

                        Vector2 target = Main.player[npc.target].Center;
                        if (masoBool[2]) //move MUCH faster, approach a position nearby
                        {
                            num15 = 0.4f;
                            num16 = 0.5f;

                            target += Main.player[npc.target].DirectionTo(npc.Center) * 600;

                            if (++Counter[0] > 120) //move way faster if still not in range
                                maxSpeed *= 2f;

                            if (npc.Distance(target) < 50)
                            {
                                Counter[0] = 0;
                                Counter[1] = (int)npc.Distance(Main.player[npc.target].Center);
                                masoBool[1] = true;
                                masoBool[3] = Main.rand.NextBool();
                                npc.localAI[2] = 0;
                                npc.velocity = 20 * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(-Math.PI / 2);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetUpdateMaso(npc.whoAmI);
                                Main.PlaySound(SoundID.Roar, Main.player[npc.target].Center, 0);
                                if (npc.life < npc.lifeMax / 10)
                                    Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < Main.maxProjectiles; i++)
                                    {
                                        if (Main.projectile[i].active && (
                                            Main.projectile[i].type == ModContent.ProjectileType<DarkStarHoming>() ||
                                            Main.projectile[i].type == ModContent.ProjectileType<DarkStarDestroyer>() ||
                                            Main.projectile[i].type == ModContent.ProjectileType<DestroyerLaser>() ||
                                            Main.projectile[i].type == ProjectileID.DeathLaser))
                                        {
                                            Main.projectile[i].Kill();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (npc.life < npc.lifeMax / 10)
                            {
                                if (Counter[0] < p2AttackTime * 3) //force begin desperation
                                {
                                    Counter[0] = p2AttackTime * 3;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        NetUpdateMaso(npc.whoAmI);
                                    Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar
                                }
                            }
                            else
                            {
                                int darkStarThreshold = masoBool[3] ? p2AttackTime : p2AttackTime * 2;
                                int laserThreshold = masoBool[3] ? p2AttackTime * 2 : p2AttackTime;

                                int maxDarkStarIntervals = 3;
                                if (npc.life < npc.lifeMax * 0.66)
                                    maxDarkStarIntervals = 5;
                                if (npc.life < npc.lifeMax * 0.33)
                                    maxDarkStarIntervals = 7;
                                const int darkStarPause = 60;
                                int upperDarkStarTime = darkStarThreshold + maxDarkStarIntervals * darkStarPause;
                                if (Counter[0] == darkStarThreshold)
                                    Counter[3] = 0;
                                if (Counter[0] >= darkStarThreshold && Counter[0] <= upperDarkStarTime + 90) //spaced star spread attack
                                {
                                    if (npc.Distance(target) < 500) //get away from player at high speed
                                    {
                                        target += npc.DirectionFrom(target) * 1000;

                                        num15 = 0.4f;
                                        num16 = 0.5f;
                                    }
                                    else
                                    {
                                        target += npc.DirectionTo(target).RotatedBy(MathHelper.PiOver2) * 1000;

                                        if (npc.Distance(target) < 1200)
                                        {
                                            maxSpeed *= 0.5f;
                                        }
                                        else //stop running
                                        {
                                            num15 *= 2f;
                                            num16 *= 2f;
                                        }
                                    }

                                    if (Counter[0] < upperDarkStarTime && Counter[0] % darkStarPause == 0)
                                    {
                                        List<int> segments = new List<int>(npc.whoAmI);
                                        foreach (NPC n in Main.npc.Where(n => n.active && n.realLife == npc.whoAmI))
                                            segments.Add(n.whoAmI);

                                        NPC segment = Main.npc[Main.rand.Next(segments)]; //use a random segment for each star spray

                                        Vector2 targetPos = Main.player[npc.target].Center;
                                        targetPos += segment.DirectionFrom(targetPos) * Math.Min(300, segment.Distance(targetPos)); //slightly between player and npc

                                        float accelerationAngle = segment.DirectionTo(targetPos).ToRotation();
                                        
                                        double maxStarModifier = 0.5 + 0.5 * Math.Sin(MathHelper.Pi / (maxDarkStarIntervals - 1) * Counter[3]++);
                                        int maxStarsInOneWave = (int)(maxStarModifier * (9.0 - 8.0 * npc.life / npc.lifeMax));
                                        if (maxStarsInOneWave > 6)
                                            maxStarsInOneWave = 6;
                                        //Main.NewText($"{Counter[3]} {maxStarModifier} {maxStarsInOneWave} {maxDarkStarIntervals}");
                                        for (int i = -maxStarsInOneWave; i <= maxStarsInOneWave; i++)
                                        {
                                            Vector2 offset = segment.DirectionTo(targetPos).RotatedBy(MathHelper.PiOver2);
                                            float offsetLength = 1000 / maxStarsInOneWave * i;
                                            int travelTime = 30 + Math.Abs(i) * 5;
                                            Vector2 vel = (targetPos + offset * offsetLength - segment.Center) / travelTime;
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(segment.Center, vel * 2, ModContent.ProjectileType<DarkStarDestroyer>(), projDamage, 0f, Main.myPlayer, accelerationAngle, -travelTime);
                                        }
                                    }
                                }

                                if (Counter[0] == laserThreshold - 120) //tell for hyper dash for light show
                                {
                                    Counter[3] = 0;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 9, npc.whoAmI);
                                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 9, npc.whoAmI);
                                    }
                                }
                                if (Counter[0] > laserThreshold && Counter[0] < laserThreshold + 420)
                                {
                                    flySpeedModifierRatio /= 2;

                                    if (Counter[3] == 0) //fly at player but deflect at last second
                                    {
                                        if (maxSpeed < 16)
                                            maxSpeed = 16;
                                        maxSpeed *= 1.5f;

                                        num15 *= 10;
                                        num16 *= 10;

                                        if (npc.Distance(target) < 400)
                                        {
                                            Counter[3] = 1;
                                            npc.velocity = 20f * npc.DirectionTo(target).RotatedBy(MathHelper.ToRadians(30) * (Main.rand.NextBool() ? -1 : 1));
                                            npc.netUpdate = true;
                                            if (Main.netMode == NetmodeID.Server)
                                                NetUpdateMaso(npc.whoAmI);
                                        }
                                    }
                                    else
                                    {
                                        if (maxSpeed > 4)
                                            maxSpeed = 4;
                                        if (npc.velocity.Length() > maxSpeed)
                                            npc.velocity *= 0.9875f;
                                        num15 /= 15; //garbage turning
                                        num16 /= 15;

                                        //curve very slightly towards player
                                        double angle = npc.DirectionTo(target).ToRotation() - npc.velocity.ToRotation();
                                        while (angle > Math.PI)
                                            angle -= 2.0 * Math.PI;
                                        while (angle < -Math.PI)
                                            angle += 2.0 * Math.PI;
                                        npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(0.3f) * Math.Sign(angle));

                                        if (Counter[0] < laserThreshold + 300 && ++Counter[3] % 90 == 20)
                                        {
                                            bool flip = Main.rand.NextBool();
                                            bool spawn = true;
                                            foreach (NPC n in Main.npc.Where(n => n.active && n.realLife == npc.whoAmI))
                                            {
                                                spawn = !spawn;
                                                if (!spawn)
                                                    continue;

                                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                                {
                                                    if (Main.rand.NextFloat() > npc.life / npc.lifeMax)
                                                    {
                                                        float range = MathHelper.ToRadians(10);
                                                        float ai1 = n.rotation + (flip ? 0 : MathHelper.Pi) + Main.rand.NextFloat(-range, range);
                                                        int p = Projectile.NewProjectile(n.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), projDamage, 0f, Main.myPlayer, 11, n.whoAmI);
                                                        if (p != Main.maxProjectiles)
                                                        {
                                                            Main.projectile[p].localAI[1] = ai1;
                                                            if (Main.netMode == NetmodeID.Server)
                                                                NetMessage.SendData(MessageID.SyncProjectile, number: p);
                                                        }
                                                    }
                                                }
                                                if (Main.rand.NextBool(3))
                                                    flip = !flip;
                                            }
                                        }
                                    }
                                }
                            }

                            if (++Counter[0] > p2AttackTime * 3) //change state
                            {
                                Counter[0] = 0;
                                masoBool[2] = true;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NetUpdateMaso(npc.whoAmI);
                            }
                            else if (Counter[0] == p2AttackTime * 3 - 120) //telegraph with roar
                            {
                                Main.PlaySound(SoundID.Roar, Main.player[npc.target].Center, 0);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 6, npc.whoAmI);
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 6, npc.whoAmI);
                                }
                            }
                        }
                        float num17 = target.X;
                        float num18 = target.Y;

                        float num21 = num17 - npc.Center.X;
                        float num22 = num18 - npc.Center.Y;
                        float num23 = (float)Math.Sqrt((double)num21 * (double)num21 + (double)num22 * (double)num22);

                        //ground movement code but it always runs
                        float num2 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
                        float num3 = Math.Abs(num21);
                        float num4 = Math.Abs(num22);
                        float num5 = maxSpeed / num2;
                        float num6 = num21 * num5;
                        float num7 = num22 * num5;
                        if ((npc.velocity.X > 0f && num6 > 0f || npc.velocity.X < 0f && num6 < 0f) && (npc.velocity.Y > 0f && num7 > 0f || npc.velocity.Y < 0f && num7 < 0f))
                        {
                            if (npc.velocity.X < num6)
                                npc.velocity.X += num16;
                            else if (npc.velocity.X > num6)
                                npc.velocity.X -= num16;
                            if (npc.velocity.Y < num7)
                                npc.velocity.Y += num16;
                            else if (npc.velocity.Y > num7)
                                npc.velocity.Y -= num16;
                        }
                        if (npc.velocity.X > 0f && num6 > 0f || npc.velocity.X < 0f && num6 < 0f || npc.velocity.Y > 0f && num7 > 0f || npc.velocity.Y < 0f && num7 < 0f)
                        {
                            if (npc.velocity.X < num6)
                                npc.velocity.X += num15;
                            else if (npc.velocity.X > num6)
                                npc.velocity.X -= num15;
                            if (npc.velocity.Y < num7)
                                npc.velocity.Y += num15;
                            else if (npc.velocity.Y > num7)
                                npc.velocity.Y -= num15;

                            if (Math.Abs(num7) < maxSpeed * 0.2f && (npc.velocity.X > 0f && num6 < 0f || npc.velocity.X < 0f && num6 > 0f))
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += num15 * 2f;
                                else
                                    npc.velocity.Y -= num15 * 2f;
                            }
                            if (Math.Abs(num6) < maxSpeed * 0.2f && (npc.velocity.Y > 0f && num7 < 0f || npc.velocity.Y < 0f && num7 > 0f))
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += num15 * 2f;
                                else
                                    npc.velocity.X -= num15 * 2f;
                            }
                        }
                        else if (num3 > num4)
                        {
                            if (npc.velocity.X < num6)
                                npc.velocity.X += num15 * 1.1f;
                            else if (npc.velocity.X > num6)
                                npc.velocity.X -= num15 * 1.1f;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5f)
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += num15;
                                else
                                    npc.velocity.Y -= num15;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num7)
                                npc.velocity.Y += num15 * 1.1f;
                            else if (npc.velocity.Y > num7)
                                npc.velocity.Y -= num15 * 1.1f;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5f)
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += num15;
                                else
                                    npc.velocity.X -= num15;
                            }
                        }
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
                        npc.netUpdate = true;
                        npc.localAI[0] = 1f;
                        
                        npc.position += npc.velocity * (.5f - flySpeedModifierRatio);
                    }

                    if (Main.netMode == NetmodeID.Server && npc.netUpdate && --npc.netSpam < 0) //manual mp sync control
                    {
                        npc.netUpdate = false;
                        npc.netSpam = 5;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                    return false;
                }
                else
                {
                    npc.velocity.Y++;
                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;
                    return true;
                }
            }

            //drop summon
            if (Main.hardMode && !NPC.downedMechBoss1 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ItemID.MechanicalWorm);
                droppedSummon = true;
            }

            return true;
        }

        public void DestroyerSegmentAI(NPC npc)
        {
            if (npc.realLife > -1 && npc.realLife < Main.maxNPCs && Main.npc[npc.realLife].life > 0 && npc.life > 0)
            {
                npc.defense = npc.defDefense;
                npc.localAI[0] = 0f; //disable vanilla lasers

                EModeGlobalNPC destroyer = Main.npc[npc.realLife].GetGlobalNPC<EModeGlobalNPC>();

                masoBool[0] = destroyer.masoBool[1];

                if (masoBool[0]) //spinning
                {
                    npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = true;

                    npc.defense = 0;

                    Counter[1] = 180;
                    if (Counter[2] < 0)
                        Counter[2] = 0; //cancel startup on any imminent projectiles
                    Vector2 pivot = Main.npc[npc.realLife].Center;
                    pivot += Vector2.Normalize(Main.npc[npc.realLife].velocity.RotatedBy(Math.PI / 2)) * 600;
                    if (npc.Distance(pivot) < 600) //make sure body doesnt coil into the circling zone
                        npc.Center = pivot + npc.DirectionFrom(pivot) * 600;

                    //enrage if player is outside the ring
                    /*if (Main.npc[npc.realLife].HasValidTarget && destroyer.Counter > 30 && Main.player[Main.npc[npc.realLife].target].Distance(pivot) > 600 && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.Next(120) == 0)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.X += Main.rand.Next(-200, 201);
                        distance.Y += Main.rand.Next(-200, 201);

                        double angleModifier = MathHelper.ToRadians(10) * distance.Length() / 1200.0;
                        distance.Normalize();
                        distance *= Main.rand.NextFloat(20f, 30f);

                        int type = ModContent.ProjectileType<DarkStar>();
                        int p = Projectile.NewProjectile(npc.Center, distance.RotatedBy(-angleModifier), type, npc.damage / 3, 0f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 150;
                        p = Projectile.NewProjectile(npc.Center, distance, type, npc.damage / 3, 0f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 150;
                        p = Projectile.NewProjectile(npc.Center, distance.RotatedBy(angleModifier), type, npc.damage / 3, 0f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 150;
                    }*/
                }

                if (destroyer.masoBool[0])
                    Counter[2] = 0; //just shut it off, fuck it

                canHurt = true;
                if (destroyer.masoBool[1] ? destroyer.Counter[0] < 15 : destroyer.Counter[0] >= 1080 - 15)
                    canHurt = false;

                if (Counter[1] > 0) //no lasers or stars while or shortly after spinning
                {
                    Counter[1]--;
                    if (Counter[2] > 1000)
                        Counter[2] = 1000;
                }
                /*else if (Counter[2] < 0) //preparing to fire something
                {
                    if (--Counter[2] <= -30)
                    {
                        Counter[2] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.ai[2] == 0) //light in, death laser
                            {
                                
                            }
                            else //light out, dark star
                            {
                                
                            }
                            NetUpdateMaso(npc.whoAmI);
                        }
                    }
                }
                else*/ if (npc.ai[2] == 0) //shoot lasers
                {
                    if (++Counter[3] > 60)
                    {
                        Counter[3] = -Main.rand.Next(360);
                        if (Main.npc[npc.realLife].life < Main.npc[npc.realLife].lifeMax * 0.5 && NPC.CountNPCS(NPCID.Probe) < 10 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Main.rand.NextBool(10))
                            {
                                npc.ai[2] = 1;
                                npc.netUpdate = true;

                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Probe);
                                if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }
                    }

                    Counter[2] += Main.rand.Next(6);
                    if (Counter[2] > Main.rand.Next(1200, 22000)) //replacement for vanilla lasers
                    {
                        Counter[2] = 0;
                        npc.TargetClosest();
                        if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            /*Counter[2] = -1;
                            NetUpdateMaso(npc.whoAmI);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 9, npc.whoAmI);*/

                            float speed = 2f + npc.Distance(Main.player[npc.target].Center) / 360;
                            if (speed > 16f)
                                speed = 16f;
                            int p = Projectile.NewProjectile(npc.Center, speed * npc.DirectionTo(Main.player[npc.target].Center), ProjectileID.DeathLaser, npc.damage / 4, 0f, Main.myPlayer);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = Math.Min((int)(npc.Distance(Main.player[npc.target].Center) / speed) + 180, 600);
                        }
                        npc.netUpdate = true;
                    }
                }
                else //light is out, shoot dark star
                {
                    int cap = Main.npc[npc.realLife].lifeMax / Main.npc[npc.realLife].life;
                    if (cap > 20) //prevent meme scaling at super low life
                        cap = 20;
                    Counter[2] += Main.rand.Next(2 + cap) + 1;
                    if (Counter[2] >= Main.rand.Next(3600, 36000))
                    {
                        Counter[2] = 0;
                        npc.TargetClosest();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            /*Counter[2] = -1;
                            NetUpdateMaso(npc.whoAmI);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 9, npc.whoAmI);*/

                            Vector2 distance = Main.player[npc.target].Center - npc.Center;

                            float modifier = 28f * (1f - (float)Main.npc[npc.realLife].life / Main.npc[npc.realLife].lifeMax);
                            if (modifier < 7)
                                modifier = 7;

                            int delay = (int)(distance.Length() / modifier) / 2;
                            if (delay < 0)
                                delay = 0;

                            int type = ModContent.ProjectileType<DarkStarHoming>();
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(distance) * modifier, type, npc.damage / 4, 0f, Main.myPlayer, npc.target, -delay);
                        }
                    }
                }

                if (!destroyer.masoBool[1])
                {
                    npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.life = 0;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                npc.active = false;
                //npc.checkDead();
                return;
            }

            npc.buffImmune[BuffID.Chilled] = false;
            //npc.buffImmune[BuffID.Darkness] = false;

            if (npc.buffType[0] != 0 && npc.buffType[0] != BuffID.Chilled && npc.buffType[0] != ModContent.BuffType<TimeFrozen>())
                npc.DelBuff(0);
        }

        public void SkeletronPrimeAI(NPC npc)
        {
            primeBoss = npc.whoAmI;
            
            if (npc.ai[1] == 0f)
            {
                masoBool[2] = false;
                
                if (npc.ai[2] == 600 - 90) //telegraph spin
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TargetingReticle>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                }
                if (npc.ai[2] < 600 - 5)
                {
                    Counter[3] = npc.target;
                }
            }

            if (npc.ai[1] == 1f)
            {
                if (Counter[3] > -1 && Counter[3] < Main.maxPlayers)
                {
                    npc.target = Counter[3];
                    npc.netUpdate = true;
                    Counter[3] = -1;
                    if (!npc.HasValidTarget)
                        npc.TargetClosest(false);
                }
            }

            if (npc.ai[0] != 2f) //in phase 1
            {
                if (!masoBool[2] && npc.ai[1] == 1f && npc.ai[2] > 2f) //spinning, do wave of guardians
                {
                    masoBool[2] = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = -2; j <= 2; j++)
                            {
                                Vector2 spawnPos = new Vector2(1200, 80 * j);
                                Vector2 vel = -10 * Vector2.UnitX;
                                spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * i);
                                vel = vel.RotatedBy(Math.PI / 2 * i);
                                int p = Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<PrimeGuardian>(), npc.damage / 4, 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 1200 / 10 + 1;
                            }
                        }
                    }
                }

                if (npc.life < npc.lifeMax * .75) //enter phase 2
                {
                    npc.ai[0] = 2f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!NPC.AnyNPCs(NPCID.PrimeLaser)) //revive all dead limbs
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeLaser, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                        if (!NPC.AnyNPCs(NPCID.PrimeSaw))
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeSaw, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                        if (!NPC.AnyNPCs(NPCID.PrimeCannon))
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeCannon, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                        if (!NPC.AnyNPCs(NPCID.PrimeVice))
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeVice, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }

                    Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);
                    return;
                }

                /*if (npc.ai[0] != 1f) //limb is dead and needs reviving
                {
                    npc.ai[3]++;
                    if (npc.ai[3] > 1800f) //revive a dead limb
                    {
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = 200;
                            switch ((int)npc.ai[0])
                            {
                                case 3: //laser
                                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeLaser, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                                    break;
                                case 4: //cannon
                                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeCannon, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);
                                    break;
                                case 5: //saw
                                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeSaw, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                                    break;
                                case 6: //vice
                                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeVice, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                                    break;
                                default:
                                    break;
                            }
                            if (n < 200)
                            {
                                Main.npc[n].life = Main.npc[n].lifeMax / 4;
                                Main.npc[n].netUpdate = true;
                            }
                        }

                        if (!NPC.AnyNPCs(NPCID.PrimeLaser)) //look for any other dead limbs
                            npc.ai[0] = 3f;
                        else if (!NPC.AnyNPCs(NPCID.PrimeCannon))
                            npc.ai[0] = 4f;
                        else if (!NPC.AnyNPCs(NPCID.PrimeSaw))
                            npc.ai[0] = 5f;
                        else if (!NPC.AnyNPCs(NPCID.PrimeVice))
                            npc.ai[0] = 6f;
                        else
                            npc.ai[0] = 1f;
                    }
                }*/

                if (++Counter[2] >= 360)
                {
                    Counter[2] = 0;

                    if (npc.HasPlayerTarget) //skeleton commando rockets LUL
                    {
                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                        speed.X += Main.rand.Next(-20, 21);
                        speed.Y += Main.rand.Next(-20, 21);
                        speed.Normalize();

                        int damage = npc.damage / 4;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, 3f * speed, ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(5f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(-5f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                        }

                        Main.PlaySound(SoundID.Item11, npc.Center);
                    }
                }
            }
            else //in phase 2
            {
                npc.dontTakeDamage = false;
                
                if (npc.ai[1] == 1f && npc.ai[2] > 2f) //spinning
                {
                    if (npc.HasValidTarget)
                        npc.position += npc.DirectionTo(Main.player[npc.target].Center) * 5;

                    if (++Counter[2] > 90) //projectile attack
                    {
                        Counter[2] = -30;
                        int damage = npc.defDamage / 3;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 105, 2f, -0.3f);
                            
                            int starMax = (int)(7f - 6f * npc.life / npc.lifeMax);

                            const int max = 8;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 speed = 12f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(2 * Math.PI / max * i);
                                for (int j = -starMax; j <= starMax; j++)
                                {
                                    int p = Projectile.NewProjectile(npc.Center, speed.RotatedBy(MathHelper.ToRadians(2f) * j),
                                        ModContent.ProjectileType<DarkStar>(), damage, 0f, Main.myPlayer);
                                    Main.projectile[p].soundDelay = -1; //dont play sounds
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 480;
                                }
                            }
                        }
                    }

                    masoBool[0] = true;
                }
                else if (npc.ai[1] == 2f) //dg phase
                {
                    while (npc.buffType[0] != 0)
                    {
                        npc.buffImmune[npc.buffType[0]] = true;
                        npc.DelBuff(0);
                    }

                    if (!Main.dayTime)
                    {
                        npc.position -= npc.velocity * 0.1f;

                        if (++Counter[1] < 120)
                        {
                            npc.position -= npc.velocity * (120 - Counter[1]) / 120 * 0.9f;
                        }
                    }
                }
                else //not spinning
                {
                    Counter[2] = 0; //buffer this for spin

                    npc.position += npc.velocity / 4f;

                    if (masoBool[0]) //switch hands
                    {
                        masoBool[0] = false;

                        /*int rangedArm = Main.rand.Next(2) == 0 ? NPCID.PrimeCannon : NPCID.PrimeLaser;
                        int meleeArm = Main.rand.Next(2) == 0 ? NPCID.PrimeSaw : NPCID.PrimeVice;

                        foreach (NPC l in Main.npc.Where(l => l.active && l.ai[1] == npc.whoAmI && !l.GetGlobalNPC<EModeGlobalNPC>().masoBool[1]))
                        {
                            switch (l.type) //change out attacking limbs
                            {
                                case NPCID.PrimeCannon:
                                case NPCID.PrimeLaser:
                                case NPCID.PrimeSaw:
                                case NPCID.PrimeVice:
                                    l.GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = npc.type == rangedArm || npc.type == meleeArm;
                                    l.GetGlobalNPC<EModeGlobalNPC>().NetUpdateMaso(l.whoAmI);
                                    l.netUpdate = true;
                                    break;
                                default:
                                    break;
                            }
                        }*/
                    }
                }

                if (!masoBool[1] && npc.ai[3] >= 0f) //spawn 4 more limbs
                {
                    npc.ai[3]++;
                    if (npc.ai[3] == 60f) //first set of limb management
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            foreach (NPC l in Main.npc.Where(l => l.active && l.ai[1] == npc.whoAmI))
                            {
                                switch (l.type) //my first four limbs become swipers
                                {
                                    case NPCID.PrimeCannon:
                                    case NPCID.PrimeLaser:
                                    case NPCID.PrimeSaw:
                                    case NPCID.PrimeVice:
                                        l.GetGlobalNPC<EModeGlobalNPC>().masoBool[1] = true;
                                        l.GetGlobalNPC<EModeGlobalNPC>().NetUpdateMaso(l.whoAmI);
                                        l.ai[2] = 0;
                                        l.netUpdate = true;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            //now spawn the other four
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeLaser, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeSaw, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeCannon, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeVice, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    /*else if (npc.ai[3] == 120f)
                    {
                        //now spawn the last four
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeLaser, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeSaw, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeCannon, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeVice, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }*/
                    else if (npc.ai[3] >= 180f)
                    {
                        masoBool[1] = true;
                        npc.ai[3] = -1f;
                        npc.netUpdate = true;

                        Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int rangedArm = Main.rand.Next(2) == 0 ? NPCID.PrimeCannon : NPCID.PrimeLaser;
                            int meleeArm = Main.rand.Next(2) == 0 ? NPCID.PrimeSaw : NPCID.PrimeVice;

                            foreach (NPC l in Main.npc.Where(l => l.active && l.ai[1] == npc.whoAmI && !l.GetGlobalNPC<EModeGlobalNPC>().masoBool[1]))
                            {
                                switch (l.type) //change out attacking limbs
                                {
                                    case NPCID.PrimeCannon:
                                    case NPCID.PrimeLaser:
                                    case NPCID.PrimeSaw:
                                    case NPCID.PrimeVice:
                                        l.GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = npc.type == rangedArm || npc.type == meleeArm;
                                        l.GetGlobalNPC<EModeGlobalNPC>().NetUpdateMaso(l.whoAmI);
                                        l.netUpdate = true;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            //drop summon
            if (Main.hardMode && !NPC.downedMechBoss3 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<MechSkull>());
                droppedSummon = true;
            }
        }

        public bool PrimeLimbAI(NPC npc)
        {
            int ai1 = (int)npc.ai[1];
            if (!(ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active && Main.npc[ai1].type == NPCID.SkeletronPrime))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.life = 0; //die if prime gone
                    npc.HitEffect();
                    npc.checkDead();
                }
                return false;
            }

            if (!Main.npc[ai1].HasValidTarget || Main.npc[ai1].ai[1] == 3) //return to default ai when death
                return true;

            if (npc.timeLeft < 600)
                npc.timeLeft = 600;

            if (npc.type == NPCID.PrimeCannon)
            {
                if (npc.ai[2] != 0f) //dark stars instead of cannonballs during super fast fire
                {
                    if (npc.localAI[0] > 30f)
                    {
                        npc.localAI[0] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 speed = new Vector2(16f, 0f).RotatedBy(npc.rotation + Math.PI / 2);
                            Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<DarkStar>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }
                }
            }
            else if (npc.type == NPCID.PrimeLaser)
            {
                if (npc.localAI[0] > 180) //vanilla threshold is 200
                {
                    npc.localAI[0] = 0;

                    Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center);
                    for (int j = -2; j <= 2; j++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, 7f * baseVel.RotatedBy(MathHelper.ToRadians(1f) * j),
                                ProjectileID.DeathLaser, Main.npc[ai1].defDamage / 4, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            npc.damage = Main.npc[ai1].ai[0] == 2f ? (int)(Main.npc[ai1].defDamage * 1.25) : npc.defDamage;

            if (Main.npc[ai1].ai[0] == 2f) //phase 2
            {
                npc.target = Main.npc[ai1].target;
                if (Main.npc[ai1].ai[1] == 3 || !npc.HasValidTarget) //return to normal AI
                    return true;

                canHurt = true;
                if (Counter[3] > 0)
                {
                    canHurt = false;
                    Counter[3]--;
                }

                if (masoBool[1]) //swipe AI
                {
                    npc.damage = (int)(Main.npc[ai1].defDamage * 1.25);

                    if (!masoBool[3])
                    {
                        masoBool[3] = true;
                        switch (npc.type)
                        {
                            case NPCID.PrimeCannon: Counter[0] = -1; Counter[1] = -1; break;
                            case NPCID.PrimeLaser: Counter[0] = 1; Counter[1] = -1; break;
                            case NPCID.PrimeSaw: Counter[0] = -1; Counter[1] = 1; break;
                            case NPCID.PrimeVice: Counter[0] = 1; Counter[1] = 1; break;
                            default: break;
                        }
                    }
                    if (++npc.ai[2] < 180)
                    {
                        Vector2 target = Main.player[npc.target].Center;
                        target.X += 400 * Counter[0];
                        target.Y += 400 * Counter[1];
                        npc.velocity = (target - npc.Center) / 30;
                        if (npc.ai[2] == 140)
                        {
                            float pitch = -0.3f + Main.rand.Next(-20, 21) / 100;
                            Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 15, 1.5f, pitch);
                            for (int i = 0; i < 20; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, 112, npc.velocity.X * .4f, npc.velocity.Y * .4f, 0, Color.White, 2);
                                Main.dust[d].scale += 1f;
                                Main.dust[d].velocity *= 3f;
                                Main.dust[d].noGravity = true;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 0f);
                        }

                        npc.damage = 0;
                    }
                    else if (npc.ai[2] == 180)
                    {
                        Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 18, 1.25f, 1f);
                        npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * 20f;
                        npc.netUpdate = true;
                        Counter[0] *= -1;
                        Counter[1] *= -1;
                    }
                    else if (npc.ai[2] < 210)
                    {
                        
                    }
                    else
                    {
                        npc.ai[2] = Main.npc[ai1].ai[1] == 1 || Main.npc[ai1].ai[1] == 2 ? 0 : -90;
                        npc.netUpdate = true;
                    }
                    npc.rotation = Main.npc[ai1].DirectionTo(npc.Center).ToRotation() - (float)Math.PI / 2;
                    if (npc.netUpdate)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            var netMessage = mod.GetPacket();
                            netMessage.Write((byte)13);
                            netMessage.Write((byte)npc.whoAmI);
                            netMessage.Write(Counter[0]);
                            netMessage.Write(Counter[1]);
                            netMessage.Send();
                        }
                        npc.netUpdate = false;
                    }
                    return false;
                }
                else if (Main.npc[ai1].ai[1] == 1 || Main.npc[ai1].ai[1] == 2) //other limbs while prime spinning
                {
                    //int d = Dust.NewDust(npc.position, npc.width, npc.height, 112, npc.velocity.X * .4f, npc.velocity.Y * .4f, 0, Color.White, 2); Main.dust[d].noGravity = true;
                    if (!masoBool[2]) //AND STRETCH HIS ARMS OUT JUST FOR YOU
                    {
                        Counter[3] = 2; //no damage while moving into position

                        int rotation = 0;
                        switch (npc.type)
                        {
                            case NPCID.PrimeCannon: rotation = 0; break;
                            case NPCID.PrimeLaser: rotation = 1; break;
                            case NPCID.PrimeSaw: rotation = 2; break;
                            case NPCID.PrimeVice: rotation = 3; break;
                            default: break;
                        }
                        Vector2 offset = Main.player[Main.npc[ai1].target].Center - Main.npc[ai1].Center;
                        offset = offset.RotatedBy(Math.PI / 2 * rotation + Math.PI / 4);
                        offset = Vector2.Normalize(offset) * (offset.Length() + 200);
                        if (offset.Length() < 600)
                            offset = Vector2.Normalize(offset) * 600;
                        Vector2 target = Main.npc[ai1].Center + offset;

                        npc.velocity = (target - npc.Center) / 20;

                        if (Counter[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 1f);

                        if (++Counter[0] > 60)
                        {
                            masoBool[2] = true;
                            Counter[0] = (int)offset.Length();
                            if (Counter[0] < 300)
                                Counter[0] = 300;
                            npc.localAI[3] = Main.npc[ai1].DirectionTo(npc.Center).ToRotation();

                            if (Main.netMode == NetmodeID.Server) //MP sync
                            {
                                var netMessage = mod.GetPacket();
                                netMessage.Write((byte)12);
                                netMessage.Write((byte)npc.whoAmI);
                                netMessage.Write(masoBool[2]);
                                netMessage.Write(Counter[0]);
                                netMessage.Write(npc.localAI[3]);
                                netMessage.Send();
                            }
                        }
                    }
                    else //spinning
                    {
                        masoBool[3] = true;

                        float range = Counter[0]; //extend further to hit player if beyond current range
                        if (Main.npc[ai1].HasValidTarget && Main.npc[ai1].Distance(Main.player[Main.npc[ai1].target].Center) > range)
                            range = Main.npc[ai1].Distance(Main.player[Main.npc[ai1].target].Center);

                        npc.Center = Main.npc[ai1].Center + new Vector2(range, 0f).RotatedBy(npc.localAI[3]);
                        const float rotation = 0.1f;
                        npc.localAI[3] += rotation;
                        if (npc.localAI[3] > (float)Math.PI)
                        {
                            npc.localAI[3] -= 2f * (float)Math.PI;
                            npc.netUpdate = true;
                        }
                    }
                    npc.rotation = Main.npc[ai1].DirectionTo(npc.Center).ToRotation() - (float)Math.PI / 2;

                    if (npc.type == NPCID.PrimeLaser)
                        npc.localAI[1] = 0;

                    if (npc.netUpdate)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetUpdateMaso(npc.whoAmI);
                        }
                        npc.netUpdate = false;
                    }
                    return false;
                }
                else //regular limb ai
                {
                    void ActiveDust()
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, -npc.velocity.X * 0.25f, -npc.velocity.Y * 0.25f, Scale: 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 2f;
                        }
                    };

                    if (masoBool[3])
                    {
                        masoBool[3] = false;

                        masoBool[0] = !masoBool[0]; //resetting variables for next spin
                        masoBool[2] = false;
                        Counter[0] = 0;

                        Counter[2] = -30; //extra delay before initiating melee attacks after spin

                        Counter[3] = 60; //disable contact damage for 1sec after spin is over

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetUpdateMaso(npc.whoAmI);
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        }
                    }

                    if (masoBool[0])
                    {
                        if (npc.type == NPCID.PrimeCannon) //vanilla movement but more aggressive projectiles
                        {
                            ActiveDust();

                            if (Counter[3] == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.UnitX, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 8, npc.whoAmI);
                            }

                            if (npc.ai[2] == 0f)
                            {
                                npc.localAI[0]++;
                                npc.ai[3]++;
                            }
                        }
                        else if (npc.type == NPCID.PrimeLaser) //vanilla movement but modified lasers
                        {
                            ActiveDust();

                            if (Counter[3] == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = -1; i <= 1; i += 2)
                                    {
                                        Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(20) * i),
                                              ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 8, npc.whoAmI);
                                    }
                                }
                            }

                            if (npc.Distance(Main.npc[ai1].Center) > 400) //drag back to prime if it drifts away
                            {
                                npc.velocity = (Main.npc[ai1].Center - npc.Center) / 30;
                            }

                            npc.localAI[0] = 0;
                            if (++npc.localAI[1] == 95) //v spread shot
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.ToRadians(20) * i);
                                    for (int j = -3; j <= 3; j++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(npc.Center, 7.5f * baseVel.RotatedBy(MathHelper.ToRadians(1f) * j),
                                                  ProjectileID.DeathLaser, npc.damage / 4, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                            else if (npc.localAI[1] > 190) //direct spread shot
                            {
                                npc.localAI[1] = 0;

                                Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center);
                                for (int j = -3; j <= 3; j++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(npc.Center, 7.5f * baseVel.RotatedBy(MathHelper.ToRadians(1f) * j),
                                            ProjectileID.DeathLaser, npc.damage / 4, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        else //fold arms when not in use
                        {
                            Vector2 distance = Main.npc[ai1].Center - npc.Center;
                            float length = distance.Length();
                            distance /= 8f;
                            npc.velocity = (npc.velocity * 23f + distance) / 24f;
                            return false;
                        }
                    }
                    else
                    {
                        if (npc.type == NPCID.PrimeSaw)
                        {
                            if (Counter[3] == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                            }

                            ActiveDust();

                            if (++Counter[2] < 90) //try to relocate to near player
                            {
                                if (!npc.HasValidTarget)
                                    npc.TargetClosest(false);
                                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                                vel.X -= 450f * Math.Sign(vel.X);
                                vel.Y -= 150f;
                                vel.Normalize();
                                vel *= 20f;
                                const float moveSpeed = 0.75f;
                                if (npc.velocity.X < vel.X)
                                {
                                    npc.velocity.X += moveSpeed;
                                    if (npc.velocity.X < 0 && vel.X > 0)
                                        npc.velocity.X += moveSpeed;
                                }
                                else if (npc.velocity.X > vel.X)
                                {
                                    npc.velocity.X -= moveSpeed;
                                    if (npc.velocity.X > 0 && vel.X < 0)
                                        npc.velocity.X -= moveSpeed;
                                }
                                if (npc.velocity.Y < vel.Y)
                                {
                                    npc.velocity.Y += moveSpeed;
                                    if (npc.velocity.Y < 0 && vel.Y > 0)
                                        npc.velocity.Y += moveSpeed;
                                }
                                else if (npc.velocity.Y > vel.Y)
                                {
                                    npc.velocity.Y -= moveSpeed;
                                    if (npc.velocity.Y > 0 && vel.Y < 0)
                                        npc.velocity.Y -= moveSpeed;
                                }
                                npc.rotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - (float)Math.PI / 2;
                            }
                            else if (Counter[2] == 90)
                            {
                                Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 18, 1.25f, -0.5f);
                                npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * 25f;
                                npc.rotation = npc.velocity.ToRotation() - (float)Math.PI / 2;
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                                    NetUpdateMaso(npc.whoAmI);
                                }
                            }
                            else if (Counter[2] > 120)
                            {
                                Counter[2] = 0;
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                                    NetUpdateMaso(npc.whoAmI);
                                }
                            }

                            return false;
                        }
                        else if (npc.type == NPCID.PrimeVice)
                        {
                            if (Counter[3] == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                            }

                            ActiveDust();

                            if (++Counter[2] < 90) //track to above player
                            {
                                if (!npc.HasValidTarget)
                                    npc.TargetClosest(false);

                                Vector2 target = Main.player[npc.target].Center;
                                target.X += npc.Center.X < target.X ? -50 : 50; //slightly to one side
                                target.Y -= 300;
                                
                                npc.velocity = (target - npc.Center) / 40;
                            }
                            else if (Counter[2] == 90) //slash down
                            {
                                Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 1, 1.25f, 0.5f);
                                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                                vel.Y += Math.Abs(vel.X) * 0.25f;
                                vel.X *= 0.75f;
                                npc.velocity = Vector2.Normalize(vel) * 20f;

                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                                    NetUpdateMaso(npc.whoAmI);
                                }
                            }
                            else if (Counter[2] > 120)
                            {
                                Counter[2] = 0;
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                                    NetUpdateMaso(npc.whoAmI);
                                }
                            }

                            npc.rotation = npc.DirectionFrom(Main.npc[ai1].Center).ToRotation() - (float)Math.PI / 2;

                            return false;
                        }
                        else //fold arms when not in use
                        {
                            Vector2 distance = Main.npc[ai1].Center - npc.Center;
                            float length = distance.Length();
                            distance /= 8f;
                            npc.velocity = (npc.velocity * 23f + distance) / 24f;
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void PlanteraAI(NPC npc)
        {
            if (!npc.HasValidTarget)
                npc.velocity.Y++;

            const float innerRingDistance = 130f;
            const int delayForRingToss = 360 + 120;

            if (--Counter[3] < 0)
            {
                Counter[3] = delayForRingToss;
                if (Main.netMode != NetmodeID.MultiplayerClient && !Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
                {
                    const int max = 5;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = npc.Center + new Vector2(innerRingDistance, 0f).RotatedBy(rotation * i);
                        int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, innerRingDistance, 0, rotation * i);
                        if (Main.netMode == NetmodeID.Server && n != Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
            }
            else if (Counter[3] == 120)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float speed = 8f;
                    int p = Projectile.NewProjectile(npc.Center, speed * npc.DirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<MutantMark2>(), npc.defDamage / 4, 0f, Main.myPlayer);
                    if (p != Main.maxProjectiles)
                    {
                        foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance)) //my crystal leaves
                        {
                            Main.PlaySound(SoundID.Grass, n.Center);
                            Projectile.NewProjectile(n.Center, Vector2.Zero, ModContent.ProjectileType<PlanteraCrystalLeafRing>(), npc.defDamage / 4, 0f, Main.myPlayer, Main.projectile[p].identity, n.ai[3]);

                            n.life = 0;
                            n.HitEffect();
                            n.checkDead();
                            n.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        }
                    }
                }
            }

            if (npc.life > npc.lifeMax / 2)
            {
                if (--Counter[0] < 0)
                {
                    Counter[0] = 150 * 4 + 25;
                    if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Main.player[npc.target].Center, Vector2.Zero, ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 0, 0);
                        for (int i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectile(Main.player[npc.target].Center, 30f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(2 * (float)Math.PI / 3 * i),
                              ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 1, 1);
                        }
                    }
                }
            }
            else
            {
                //Aura(npc, 700, ModContent.BuffType<IvyVenom>(), true, 188);
                masoBool[1] = true;
                //npc.defense += 21;

                if (!masoBool[2])
                {
                    masoBool[2] = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
                        {
                            const int innerMax = 5;
                            float innerRotation = 2f * (float)Math.PI / innerMax;
                            for (int i = 0; i < innerMax; i++)
                            {
                                Vector2 spawnPos = npc.Center + new Vector2(innerRingDistance, 0f).RotatedBy(innerRotation * i);
                                int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, innerRingDistance, 0, innerRotation * i);
                                if (Main.netMode == NetmodeID.Server && n != Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }

                        const int max = 12;
                        const float distance = 250;
                        float rotation = 2f * (float)Math.PI / max;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                            int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, distance, 0, rotation * i);
                            if (Main.netMode == NetmodeID.Server && n != Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }

                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].hostile &&
                                (Main.projectile[i].type == ProjectileID.ThornBall
                                || Main.projectile[i].type == ModContent.ProjectileType<DicerPlantera>()
                                || Main.projectile[i].type == ModContent.ProjectileType<CrystalLeafShot>()))
                            {
                                Main.projectile[i].Kill();
                            }
                        }
                    }
                }

                //explode time * explode repetitions + spread delay * propagations
                const int delayForDicers = 150 * 4 + 25 * 8;

                if (--Counter[2] < -120)
                {
                    Counter[2] = delayForDicers + delayForRingToss + 240;
                    //Counter[3] = delayForDicers + 120; //extra compensation for the toss offset
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer);
                        for (int i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectile(npc.Center, 25f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(2 * (float)Math.PI / 3 * i),
                              ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 1, 8);
                        }
                    }
                }

                if (Counter[2] > delayForDicers || Counter[2] < 0)
                {
                    if (Counter[3] > 120) //to still respawn the leaf ring if it's missing but disable throwing it
                        Counter[3] = 120;
                }
                else if (Counter[2] < delayForDicers)
                {
                    Counter[3] -= 1;
                    if (Counter[3] % 2 == 0) //make sure plantera can get the timing for its check
                        Counter[3]--;
                }
                else if (Counter[2] == delayForDicers)
                {
                    Counter[3] = 121; //activate it immediately as the mines fade
                }

                SharkCount = 0;

                if (npc.HasPlayerTarget && Main.player[npc.target].venom)
                {
                    //npc.defense *= 2;
                    //Counter[0]++;
                    SharkCount = 1;
                    npc.position -= npc.velocity * 0.1f;
                }
                else
                {
                    npc.position -= npc.velocity * 0.2f;
                }
            }

            //drop summon
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && !NPC.downedPlantBoss 
                && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<PlanterasFruit>());
                droppedSummon = true;
            }
        }

        public void PlanterasHookAI(NPC npc)
        {
            npc.damage = 0;
            npc.defDamage = 0;

            /*if (NPC.FindFirstNPC(NPCID.PlanterasHook) == npc.whoAmI)
            {
                npc.color = Color.LightGreen;
                PrintAI(npc);
            }*/

            if (BossIsAlive(ref NPC.plantBoss, NPCID.Plantera) && Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 2 && Main.npc[NPC.plantBoss].HasValidTarget)
            {
                if (npc.Distance(Main.player[Main.npc[NPC.plantBoss].target].Center) > 600)
                {
                    Vector2 targetPos = Main.player[Main.npc[NPC.plantBoss].target].Center / 16; //pick a new target pos near player
                    targetPos.X += Main.rand.Next(-25, 26);
                    targetPos.Y += Main.rand.Next(-25, 26);

                    Tile tile = Framing.GetTileSafely((int)targetPos.X, (int)targetPos.Y);
                    npc.localAI[0] = 600; //reset vanilla timer for picking new block
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        npc.netUpdate = true;

                    npc.ai[0] = targetPos.X;
                    npc.ai[1] = targetPos.Y;
                }

                npc.position += npc.velocity;
            }
        }

        public void GolemAI(NPC npc)
        {
            /*if (npc.ai[0] == 0f && npc.velocity.Y == 0f) //manipulating golem jump ai
                        {
                            if (npc.ai[1] > 0f)
                            {
                                npc.ai[1] += 5f; //count up to initiate jump faster
                            }
                            else
                            {
                                float threshold = -2f - (float)Math.Round(18f * npc.life / npc.lifeMax);

                                if (npc.ai[1] < threshold) //jump activates at npc.ai[1] == -1
                                    npc.ai[1] = threshold;
                            }
                        }*/

            if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].Distance(npc.Center) < 2000)
                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<LowGround>(), 2);

            if (!masoBool[3]) //temple enrage, more horiz move and fast jumps
            {
                npc.position.X += npc.velocity.X / 2f;
                if (npc.velocity.Y < 0)
                {
                    npc.position.Y += npc.velocity.Y * 0.5f;
                    if (npc.velocity.Y > -2)
                        npc.velocity.Y = 20;
                }
            }

            if (npc.velocity.Y < 0) //jumping up
            {
                if (!masoBool[2])
                {
                    masoBool[2] = true;
                    npc.velocity.Y *= 1.25f;

                    if (!masoBool[3]) //temple enrage
                    {
                        if (Main.player[npc.target].Center.Y < npc.Center.Y - 16 * 30)
                            npc.velocity.Y *= 1.5f;
                    }
                }
            }
            else
            {
                masoBool[2] = false;
            }

            if (masoBool[0])
            {
                if (npc.velocity.Y == 0f)
                {
                    masoBool[0] = false;
                    masoBool[3] = Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] != null &&
                        Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall == WallID.LihzahrdBrickUnsafe;

                    if (Main.netMode != NetmodeID.MultiplayerClient) //landing attacks
                    {
                        if (masoBool[3]) //in temple
                        {
                            Counter[0]++;
                            if (Counter[0] == 1) //plant geysers
                            {
                                Vector2 spawnPos = new Vector2(npc.position.X, npc.Center.Y); //floor geysers
                                spawnPos.X -= npc.width * 7;
                                for (int i = 0; i < 6; i++)
                                {
                                    int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                    int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<GolemGeyser2>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                                }

                                spawnPos = npc.Center;
                                for (int i = -3; i <= 3; i++) //ceiling geysers
                                {
                                    int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                    int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<GolemGeyser>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                                }
                            }
                            else if (Counter[0] == 2) //empty jump
                            {

                            }
                            else if (Counter[0] == 3) //rocks fall
                            {
                                if (npc.HasPlayerTarget)
                                {
                                    for (int i = -2; i <= 2; i++)
                                    {
                                        int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                                        int tilePosY = (int)Main.player[npc.target].Center.Y / 16;// + 1;
                                        tilePosX += 4 * i;

                                        if (Main.tile[tilePosX, tilePosY] == null)
                                            Main.tile[tilePosX, tilePosY] = new Tile();

                                        //first move up through solid tiles
                                        while (Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type])
                                        {
                                            tilePosY--;
                                            if (Main.tile[tilePosX, tilePosY] == null)
                                                Main.tile[tilePosX, tilePosY] = new Tile();
                                        }
                                        //then move up through air until next ceiling reached
                                        while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type]))
                                        {
                                            tilePosY--;
                                            if (Main.tile[tilePosX, tilePosY] == null)
                                                Main.tile[tilePosX, tilePosY] = new Tile();
                                        }

                                        Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                                        Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GolemBoulder>(), npc.damage / 4, 0f, Main.myPlayer);
                                    }
                                }
                            }
                            else //empty jump
                            {
                                Counter[0] = 0;
                            }
                        }
                        else //outside temple
                        {
                            Vector2 spawnPos = new Vector2(npc.position.X, npc.Center.Y);
                            spawnPos.X -= npc.width * 7;
                            for (int i = 0; i < 6; i++)
                            {
                                int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                if (Main.tile[tilePosX, tilePosY] == null)
                                    Main.tile[tilePosX, tilePosY] = new Tile();

                                while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[(int)Main.tile[tilePosX, tilePosY].type]))
                                {
                                    tilePosY++;
                                    if (Main.tile[tilePosX, tilePosY] == null)
                                        Main.tile[tilePosX, tilePosY] = new Tile();
                                }

                                if (npc.HasPlayerTarget && Main.player[npc.target].position.Y > tilePosY * 16)
                                {
                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 6.3f, 6.3f,
                                        ProjectileID.FlamesTrap, npc.damage / 4, 0f, Main.myPlayer);
                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, -6.3f, 6.3f,
                                        ProjectileID.FlamesTrap, npc.damage / 4, 0f, Main.myPlayer);
                                }

                                Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, -8f, ProjectileID.GeyserTrap, npc.damage / 4, 0f, Main.myPlayer);

                                Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8 - 640, 0f, -8f, ProjectileID.GeyserTrap, npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8 - 640, 0f, 8f, ProjectileID.GeyserTrap, npc.damage / 4, 0f, Main.myPlayer);
                            }
                            if (npc.HasPlayerTarget)
                            {
                                for (int i = -3; i <= 3; i++)
                                {
                                    int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                                    int tilePosY = (int)Main.player[npc.target].Center.Y / 16;// + 1;
                                    tilePosX += 10 * i;

                                    if (Main.tile[tilePosX, tilePosY] == null)
                                        Main.tile[tilePosX, tilePosY] = new Tile();

                                    for (int j = 0; j < 30; j++)
                                    {
                                        if (Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type])
                                            break;
                                        tilePosY--;
                                        if (Main.tile[tilePosX, tilePosY] == null)
                                            Main.tile[tilePosX, tilePosY] = new Tile();
                                    }

                                    Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GolemBoulder>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                        }

                        //golem's anti-air fireball spray (whenever he lands while player is below)
                        /*if (npc.HasPlayerTarget && Main.player[npc.target].position.Y > npc.position.Y + npc.height)
                        {
                            float gravity = 0.2f; //shoot down
                            const float time = 60f;
                            Vector2 distance = Main.player[npc.target].Center - npc.Center;
                            distance += Main.player[npc.target].velocity * 45f;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            if (Math.Sign(distance.Y) != Math.Sign(gravity))
                                distance.Y = 0f; //cannot arc shots to hit someone on the same elevation
                            int max = masoBool[3] ? 1 : 3;
                            for (int i = -max; i <= max; i++)
                            {
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distance.X + i * 1.5f, distance.Y,
                                    ModContent.ProjectileType<GolemFireball>(), npc.damage / 5, 0f, Main.myPlayer, gravity, 0f);
                            }
                        }*/
                    }
                }
            }
            else if (npc.velocity.Y > 0)
            {
                masoBool[0] = true;
            }

            if (++Counter[1] >= 900) //spray spiky balls
            {
                if (Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] != null && //in temple
                    Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall == WallID.LihzahrdBrickUnsafe)
                {
                    if (npc.velocity.Y > 0) //only when falling, implicitly assume at peak of a jump
                    {
                        Counter[1] = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                                  Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(-10, -6), ModContent.ProjectileType<GolemSpikyBall>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }
                }
                else //outside temple
                {
                    Counter[1] = 600; //do it more often
                    for (int i = 0; i < 16; i++)
                    {
                        Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                              Main.rand.NextFloat(-1f, 1f), Main.rand.Next(-20, -9), ModContent.ProjectileType<GolemSpikyBall>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                }
            }

            /*Counter2++;
            if (Counter2 > 240) //golem's anti-air fireball spray (when player is above)
            {
                Counter2 = 0;
                if (npc.HasPlayerTarget && Main.player[npc.target].position.Y < npc.position.Y
                    && Main.netMode != NetmodeID.MultiplayerClient) //shoutouts to arterius
                {
                    bool inTemple = Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] != null && //in temple
                        Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall == WallID.LihzahrdBrickUnsafe;

                    float gravity = -0.2f; //normally floats up
                    //if (Main.player[npc.target].position.Y > npc.position.Y + npc.height) gravity *= -1f; //aim down if player below golem
                    const float time = 60f;
                    Vector2 distance = Main.player[npc.target].Center - npc.Center;
                    distance += Main.player[npc.target].velocity * 45f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    if (Math.Sign(distance.Y) != Math.Sign(gravity))
                        distance.Y = 0f; //cannot arc shots to hit someone on the same elevation
                    int max = inTemple ? 1 : 3;
                    for (int i = -max; i <= max; i++)
                    {
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distance.X + i, distance.Y,
                            ModContent.ProjectileType<GolemFireball>(), npc.damage / 5, 0f, Main.myPlayer, gravity, 0f);
                    }
                }
            }*/


            if (!npc.dontTakeDamage)
            {
                npc.life += 3; //healing stuff
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;
                Counter[2]++;
                if (Counter[2] >= 75)
                {
                    Counter[2] = Main.rand.Next(30);
                    CombatText.NewText(npc.Hitbox, CombatText.HealLife, 180);
                }
            }

            //drop summon
            if (NPC.downedPlantBoss && !NPC.downedGolemBoss && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<LihzahrdPowerCell2>());
                droppedSummon = true;
            }
        }

        public void GolemFistAI(NPC npc)
        {
            if (npc.buffType[0] != 0)
            {
                npc.buffImmune[npc.buffType[0]] = true;
                npc.DelBuff(0);
            }

            if (npc.HasValidTarget && Framing.GetTileSafely(Main.player[npc.target].Center).wall == WallID.LihzahrdBrickUnsafe)
            {
                if (npc.ai[0] == 1) //on the tick it shoots out, reset counter
                {
                    Counter[0] = 0;
                }
                else
                {
                    if (++Counter[0] < 90) //this basically tracks total time since punch started
                    {
                        npc.ai[1] = 0; //don't allow attacking until counter finishes counting up
                    }
                }

                if (npc.velocity.Length() > 10 && !Fargowiltas.Instance.MasomodeEXLoaded)
                    npc.position -= Vector2.Normalize(npc.velocity) * (npc.velocity.Length() - 10);
            }

            if (npc.ai[0] == 0f && masoBool[0] && Framing.GetTileSafely(Main.player[npc.target].Center).wall != WallID.LihzahrdBrickUnsafe)
            {
                masoBool[0] = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), npc.damage / 4, 0f, Main.myPlayer);
                }
            }
            masoBool[0] = npc.ai[0] != 0f;

            if (NPC.golemBoss > -1 && NPC.golemBoss < Main.maxNPCs && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
            {
                if (npc.ai[0] == 0) //when attached to body
                    npc.position += Main.npc[NPC.golemBoss].velocity; //stick to body better, dont get left behind during jumps

                if (npc.life < npc.lifeMax / 2)
                {
                    npc.life = npc.lifeMax; //fully heal when below half health and golem still alive
                    Counter[2] = 75; //immediately display heal
                }
            }
            npc.life += 167;
            if (npc.life > npc.lifeMax)
                npc.life = npc.lifeMax;
            Counter[2]++;
            if (Counter[2] >= 75)
            {
                Counter[2] = Main.rand.Next(30);
                CombatText.NewText(npc.Hitbox, CombatText.HealLife, 9999);
            }
        }

        public bool GolemHeadAI(NPC npc)
        {
            if (npc.type == NPCID.GolemHead)
            {
                npc.dontTakeDamage = false;
                npc.life += 3;
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;

                Counter[2]++;
                if (Counter[2] >= 75)
                {
                    Counter[2] = Main.rand.Next(30);
                    CombatText.NewText(npc.Hitbox, CombatText.HealLife, 180);
                }

                if (NPC.golemBoss > -1 && NPC.golemBoss < Main.maxNPCs && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                {
                    npc.position += Main.npc[NPC.golemBoss].velocity;
                }
            }
            //detatched head
            else
            {
                canHurt = false;

                if (!masoBool[0]) //default mode
                {
                    npc.position += npc.velocity * 0.25f;
                    npc.position.Y += npc.velocity.Y * 0.25f;

                    if (!npc.noTileCollide && npc.HasPlayerTarget && Collision.SolidCollision(npc.position, npc.width, npc.height)) //unstick from walls
                        npc.position += npc.DirectionTo(Main.player[npc.target].Center) * 4;

                    if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 350)
                    {
                        npc.velocity.Y -= 0.3f; //snap away from player
                        if (Main.player[npc.target].velocity.Y < 0)
                            npc.position.Y += Main.player[npc.target].velocity.Y / 3; //go up with player
                    }

                    if (++Counter[0] > 540)
                    {
                        Counter[0] = 0;
                        Counter[1] = 0;
                        masoBool[0] = true;
                        masoBool[2] = Framing.GetTileSafely(npc.Center).wall == WallID.LihzahrdBrickUnsafe; //is in temple
                        npc.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetUpdateMaso(npc.whoAmI);
                    }
                }
                else //deathray time
                {
                    if (!(NPC.golemBoss > -1 && NPC.golemBoss < Main.maxNPCs && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem))
                    {
                        npc.StrikeNPCNoInteraction(9999, 0f, 0); //die if golem is dead
                        return false;
                    }

                    npc.noTileCollide = true;

                    const int fireTime = 120;
                    if (++Counter[0] < fireTime) //move to above golem
                    {
                        if (Counter[0] == 1)
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);

                        Vector2 target = Main.npc[NPC.golemBoss].Center;
                        target.Y -= 250;
                        if (target.Y > Counter[1]) //counter2 stores lowest remembered golem position
                            Counter[1] = (int)target.Y;
                        target.Y = Counter[1];
                        if (npc.HasPlayerTarget && Main.player[npc.target].position.Y < target.Y)
                            target.Y = Main.player[npc.target].position.Y;
                        /*if (masoBool[2]) //in temple
                        {
                            target.Y -= 250;
                            if (target.Y > Counter2) //counter2 stores lowest remembered golem position
                                Counter2 = (int)target.Y;
                            target.Y = Counter2;
                        }
                        else if (npc.HasPlayerTarget)
                        {
                            target.Y = Main.player[npc.target].Center.Y - 250;
                        }*/
                        npc.velocity = (target - npc.Center) / 30;
                    }
                    else if (Counter[0] == fireTime) //fire deathray
                    {
                        npc.velocity = Vector2.Zero;
                        if (npc.HasPlayerTarget) //stores if player is on head's left at this moment
                            masoBool[1] = Main.player[npc.target].Center.X < npc.Center.X;
                        npc.netUpdate = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY, ModContent.ProjectileType<PhantasmalDeathrayGolem>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                    }
                    else if (Counter[0] < fireTime + 20)
                    {
                        //do nothing
                    }
                    else if (Counter[0] < fireTime + 150)
                    {
                        npc.velocity.X += masoBool[1] ? -.15f : .15f;

                        Tile tile = Framing.GetTileSafely(npc.Center); //stop if reached a wall, but only 1sec after started firing
                        if (Counter[0] > fireTime + 60 && (tile.nactive() && tile.type == TileID.LihzahrdBrick && tile.wall == WallID.LihzahrdBrickUnsafe)
                            || (masoBool[2] && tile.wall != WallID.LihzahrdBrickUnsafe)) //i.e. started in temple but has left temple, then stop
                        {
                            npc.velocity = Vector2.Zero;
                            npc.netUpdate = true;
                            Counter[0] = 0;
                            Counter[1] = 0;
                            masoBool[0] = false;
                        }
                    }
                    else
                    {
                        npc.velocity = Vector2.Zero;
                        npc.netUpdate = true;
                        Counter[0] = 0;
                        Counter[1] = 0;
                        masoBool[0] = false;
                    }

                    if (masoBool[2]) //nerf golem movement during deathray dash, provided we're in temple
                    {
                        if (Main.npc[NPC.golemBoss].HasValidTarget && Math.Abs(Main.player[Main.npc[NPC.golemBoss].target].Center.X - Main.npc[NPC.golemBoss].Center.X) < 300)
                        {
                            if (Math.Sign(Main.player[Main.npc[NPC.golemBoss].target].Center.X - Main.npc[NPC.golemBoss].Center.X) == Math.Sign(Main.npc[NPC.golemBoss].velocity.X))
                                Main.npc[NPC.golemBoss].velocity.X *= -1; //jump AWAY from player
                        }
                        //if (Main.npc[NPC.golemBoss].velocity.Y < 0) Main.npc[NPC.golemBoss].position.Y -= Main.npc[NPC.golemBoss].velocity.Y * 0.5f; //half jump height
                    }

                    if (!masoBool[0] && Main.netMode != NetmodeID.MultiplayerClient) //spray lasers after dash
                    {
                        int max = masoBool[2] ? 6 : 10;
                        int speed = masoBool[2] ? 6 : -12; //down in temple, up outside it
                        for (int i = -max; i <= max; i++)
                        {
                            int p = Projectile.NewProjectile(npc.Center, speed * Vector2.UnitY.RotatedBy(Math.PI / 2 / max * i),
                                ModContent.ProjectileType<EyeBeam2>(), npc.damage / 4, 0f, Main.myPlayer);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = 1200;
                        }
                    }

                    if (npc.netUpdate)
                    {
                        npc.netUpdate = false;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetUpdateMaso(npc.whoAmI);
                        }
                    }
                    return false;
                }
            }

            return true;
        }

        public void DukeFishronAI(NPC npc)
        {
            void SpawnRazorbladeRing(int max, float speed, int damage, float rotationModifier, bool reduceTimeleft = false)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;
                float rotation = 2f * (float)Math.PI / max;
                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                vel.Normalize();
                vel *= speed;
                int type = ModContent.ProjectileType<RazorbladeTyphoon>();
                for (int i = 0; i < max; i++)
                {
                    vel = vel.RotatedBy(rotation);
                    int p = Projectile.NewProjectile(npc.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier * npc.spriteDirection, speed);
                    if (reduceTimeleft && p < 1000)
                        Main.projectile[p].timeLeft /= 2;
                }
                Main.PlaySound(SoundID.Item84, npc.Center);
            }

            void EnrageDust()
            {
                int num22 = 7;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    int d;
                    if (npc.velocity.Length() > 10)
                    {
                        Vector2 vector2_1 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + npc.Center;
                        Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                        d = Dust.NewDust(vector2_1 + vector2_2, 0, 0, 88, vector2_2.X * 2f, vector2_2.Y * 2f, 0, default, 1.7f);
                    }
                    else
                    {
                        d = Dust.NewDust(npc.position, npc.width, npc.height, 88, npc.velocity.X * 2f, npc.velocity.Y * 2f, 0, default, 1.7f);
                    }
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity /= 4f;
                    Main.dust[d].velocity -= npc.velocity;
                }
            }

            fishBoss = npc.whoAmI;
            if (masoBool[3]) //fishron EX
            {
                npc.GetGlobalNPC<FargoSoulsGlobalNPC>().MutantNibble = false;
                npc.GetGlobalNPC<FargoSoulsGlobalNPC>().LifePrevious = int.MaxValue; //cant stop the healing
                while (npc.buffType[0] != 0)
                    npc.DelBuff(0);

                if (npc.Distance(Main.player[Main.myPlayer].Center) < 3000f)
                {
                    Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<OceanicSeal>(), 2);
                    Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Buffs.Boss.MutantPresence>(), 2); //LUL
                }
                fishBossEX = npc.whoAmI;
                npc.position += npc.velocity * 0.5f;
                switch ((int)npc.ai[0])
                {
                    case -1: //just spawned
                        if (npc.ai[2] == 2 && Main.netMode != NetmodeID.MultiplayerClient) //create spell circle
                        {
                            int ritual1 = Projectile.NewProjectile(npc.Center, Vector2.Zero,
                                ModContent.ProjectileType<FishronRitual>(), 0, 0f, Main.myPlayer, npc.lifeMax, npc.whoAmI);
                            if (ritual1 == Main.maxProjectiles) //failed to spawn projectile, abort spawn
                                npc.active = false;
                            Main.PlaySound(SoundID.Item84, npc.Center);
                        }
                        masoBool[2] = true;
                        break;

                    case 0: //phase 1
                        if (!masoBool[1])
                            npc.dontTakeDamage = false;
                        masoBool[2] = false;
                        npc.ai[2]++;
                        break;

                    case 1: //p1 dash
                        Counter[0]++;
                        if (Counter[0] > 5)
                        {
                            Counter[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int n = NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), NPCID.DetonatingBubble);
                                if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.DirectionTo(Main.player[npc.target].Center);
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        break;

                    case 2: //p1 bubbles
                        if (npc.ai[2] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                        break;

                    case 3: //p1 drop nados
                        if (npc.ai[2] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 32;
                            float rotation = 2f * (float)Math.PI / max;
                            for (int i = 0; i < max; i++)
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = Vector2.UnitY.RotatedBy(rotation * i);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }

                            SpawnRazorbladeRing(18, 10f, npc.damage / 6, 1f);
                        }
                        break;

                    case 4: //phase 2 transition
                        masoBool[1] = false;
                        masoBool[2] = true;
                        if (npc.ai[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronRitual>(), 0, 0f, Main.myPlayer, npc.lifeMax / 4, npc.whoAmI);
                        if (npc.ai[2] >= 114)
                        {
                            Counter[0]++;
                            if (Counter[0] > 6) //display healing effect
                            {
                                Counter[0] = 0;
                                int heal = (int)(npc.lifeMax * Main.rand.NextFloat(0.1f, 0.12f));
                                npc.life += heal;
                                int max = npc.ai[0] == 9 && !Fargowiltas.Instance.MasomodeEXLoaded ? npc.lifeMax / 2 : npc.lifeMax;
                                if (npc.life > max)
                                    npc.life = max;
                                CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                            }
                        }
                        break;

                    case 5: //phase 2
                        if (!masoBool[1])
                            npc.dontTakeDamage = false;
                        masoBool[2] = false;
                        npc.ai[2]++;
                        break;

                    case 6: //p2 dash
                        goto case 1;

                    case 7: //p2 spin & bubbles
                        npc.position -= npc.velocity * 0.5f;
                        Counter[0]++;
                        if (Counter[0] > 1)
                        {
                            //Counter[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                                n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(-Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        break;

                    case 8: //p2 cthulhunado
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 60)
                        {
                            Vector2 spawnPos = Vector2.UnitX * npc.direction;
                            spawnPos = spawnPos.RotatedBy(npc.rotation);
                            spawnPos *= npc.width + 20f;
                            spawnPos /= 2f;
                            spawnPos += npc.Center;
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * 2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * -2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, 0f, 2f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                            SpawnRazorbladeRing(12, 12.5f, npc.damage / 6, 0.75f);
                            SpawnRazorbladeRing(12, 10f, npc.damage / 6, -2f);
                        }
                        break;

                    case 9: //phase 3 transition
                        if (npc.ai[2] == 1f)
                        {
                            for (int i = 0; i < npc.buffImmune.Length; i++)
                                npc.buffImmune[i] = true;
                            while (npc.buffTime[0] != 0)
                                npc.DelBuff(0);
                            npc.defDamage = (int)(npc.defDamage * 1.2f);
                        }
                        goto case 4;

                    case 10: //phase 3
                             //vanilla fishron has x1.1 damage in p3. p2 has x1.2 damage...
                             //npc.damage = (int)(npc.defDamage * 1.2f * (Main.expertMode ? 0.6f * Main.damageMultiplier : 1f));
                        masoBool[2] = false;
                        //if (Timer >= 60 + (int)(540.0 * npc.life / npc.lifeMax)) //yes that needs to be a double
                        /*Counter[2]++;
                        if (Counter[2] >= 900)
                        {
                            Counter[2] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn cthulhunado
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                        }*/
                        break;

                    case 11: //p3 dash
                        if (Counter[0] > 2)
                            Counter[0] = 2;
                        if (Counter[0] == 2)
                        {
                            //Counter[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                                n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(-Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        goto case 10;

                    case 12: //p3 *teleports behind you*
                        if (npc.ai[2] == 15f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SpawnRazorbladeRing(5, 9f, npc.damage / 6, 1f, true);
                                SpawnRazorbladeRing(5, 9f, npc.damage / 6, -0.5f, true);
                            }
                        }
                        else if (npc.ai[2] == 16f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 spawnPos = Vector2.UnitX * npc.direction; //GODLUL
                                spawnPos = spawnPos.RotatedBy(npc.rotation);
                                spawnPos *= npc.width + 20f;
                                spawnPos /= 2f;
                                spawnPos += npc.Center;
                                Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * 2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * -2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                                const int max = 24;
                                float rotation = 2f * (float)Math.PI / max;
                                for (int i = 0; i < max; i++)
                                {
                                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                    if (n != Main.maxNPCs)
                                    {
                                        Main.npc[n].velocity = npc.velocity.RotatedBy(rotation * i);
                                        Main.npc[n].velocity.Normalize();
                                        Main.npc[n].netUpdate = true;
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                    }
                                }
                            }
                        }
                        goto case 10;

                    default:
                        break;
                }
            }

            npc.position += npc.velocity * 0.25f; //fishron regular
            switch ((int)npc.ai[0])
            {
                case -1: //just spawned
                         /*if (npc.ai[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient) //create spell circle
                         {
                             int p2 = Projectile.NewProjectile(npc.Center, Vector2.Zero,
                                 ModContent.ProjectileType<FishronRitual2>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                             if (p2 == 1000) //failed to spawn projectile, abort spawn
                                 npc.active = false;
                         }*/
                    if (!masoBool[3])
                        npc.dontTakeDamage = true;
                    break;

                case 0: //phase 1
                    if (!masoBool[1])
                        npc.dontTakeDamage = false;
                    if (!Main.player[npc.target].ZoneBeach)
                        npc.ai[2]++;
                    break;

                case 1: //p1 dash
                        /*Counter[0]++;
                        if (Counter[0] > 5)
                        {
                            Counter[0] = 0;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int n = NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), NPCID.DetonatingBubble);
                                if (n != 200 && Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }*/
                    break;

                case 2: //p1 bubbles
                    if (npc.ai[2] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool random = Main.rand.Next(2) == 0; //fan above or to sides
                        for (int j = -1; j <= 1; j++) //to both sides of player
                        {
                            if (j == 0)
                                continue;

                            Vector2 offset = random ? Vector2.UnitY * -450f * j : Vector2.UnitX * 600f * j;
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), npc.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                        }
                    }
                    break;

                case 3: //p1 drop nados
                    if (npc.ai[2] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SpawnRazorbladeRing(12, 10f, npc.damage / 4, 1f);
                    }
                    break;

                case 4: //phase 2 transition
                    if (masoBool[3])
                        break;
                    npc.dontTakeDamage = true;
                    masoBool[1] = false;
                    if (npc.ai[2] == 120)
                    {
                        int heal = npc.lifeMax - npc.life;
                        npc.life = npc.lifeMax;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                    }
                    break;

                case 5: //phase 2
                    if (!masoBool[1])
                        npc.dontTakeDamage = false;
                    if (!Main.player[npc.target].ZoneBeach)
                        npc.ai[2]++;
                    break;

                case 6: //p2 dash
                        /*if (npc.ai[2] == 0 && npc.ai[3] == 0)
                        {

                        }*/
                    break;

                case 7: //p2 spin & bubbles
                    npc.position -= npc.velocity * 0.25f;

                    if (++Counter[0] > 1)
                    {
                        Counter[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2),
                                ModContent.ProjectileType<RazorbladeTyphoon2>(), npc.damage / 4, 0f, Main.myPlayer, .03f);
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity).RotatedBy(-Math.PI / 2),
                                ModContent.ProjectileType<RazorbladeTyphoon2>(), npc.damage / 4, 0f, Main.myPlayer, .02f);

                            if (Fargowiltas.Instance.MasomodeEXLoaded) //lol
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubble>());
                                if (n < 200)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(Math.PI / 2);
                                    Main.npc[n].velocity *= -npc.spriteDirection;
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                    }
                    break;

                case 8: //p2 cthulhunado
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 60)
                    {
                        Vector2 spawnPos = Vector2.UnitX * npc.direction;
                        spawnPos = spawnPos.RotatedBy(npc.rotation);
                        spawnPos *= npc.width + 20f;
                        spawnPos /= 2f;
                        spawnPos += npc.Center;
                        Projectile.NewProjectile(spawnPos.X, spawnPos.Y, 0f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                        //SpawnRazorbladeRing(8, 8f, npc.damage / 4, 2f);
                        //SpawnRazorbladeRing(8, 8f, npc.damage / 4, -2f);

                        bool random = Main.rand.Next(2) == 0; //fan above or to sides
                        for (int j = -1; j <= 1; j++) //to both sides of player
                        {
                            if (j == 0)
                                continue;

                            for (int i = -1; i <= 1; i++) //fan of fishron
                            {
                                Vector2 offset = random ? Vector2.UnitY.RotatedBy(Math.PI / 3 / 3 * i) * -450f * j : Vector2.UnitX.RotatedBy(Math.PI / 3 / 3 * i) * 600f * j;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), npc.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                            }
                        }
                    }
                    break;

                case 9: //phase 3 transition
                    if (masoBool[3])
                        break;
                    npc.dontTakeDamage = true;
                    npc.defDefense = 0;
                    npc.defense = 0;
                    masoBool[1] = false;
                    if (npc.ai[2] == 90) //first purge the bolts
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<RazorbladeTyphoon2>();
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && (Main.projectile[i].type == ProjectileID.SharknadoBolt || Main.projectile[i].type == type))
                                {
                                    Main.projectile[i].Kill();
                                }
                            }
                        }
                    }
                    if (npc.ai[2] == 120)
                    {
                        int max = Fargowiltas.Instance.MasomodeEXLoaded ? npc.lifeMax : npc.lifeMax / 2; //heal
                        int heal = max - npc.life;
                        npc.life = max;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);

                        if (Main.netMode != NetmodeID.MultiplayerClient) //purge nados
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && (Main.projectile[i].type == ProjectileID.Sharknado || Main.projectile[i].type == ProjectileID.Cthulunado))
                                {
                                    Main.projectile[i].Kill();
                                }
                            }

                            for (int i = 0; i < Main.maxNPCs; i++) //purge sharks
                            {
                                if (Main.npc[i].active && (Main.npc[i].type == NPCID.Sharkron || Main.npc[i].type == NPCID.Sharkron2))
                                {
                                    Main.npc[i].life = 0;
                                    Main.npc[i].HitEffect();
                                    Main.npc[i].active = false;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
                                }
                            }
                        }
                    }
                    break;

                case 10: //phase 3
                    if (!Main.player[npc.target].ZoneBeach || (npc.ai[3] > 5 && npc.ai[3] < 8))
                    {
                        npc.position += npc.velocity;
                        npc.ai[2]++;
                        EnrageDust();
                    }

                    if (npc.ai[3] == 1) //after 1 dash, before teleporting
                    {
                        if (++Counter[1] < 180)
                        {
                            npc.ai[2] = 0; //stay in this ai mode for a bit
                            npc.position.Y -= npc.velocity.Y * 0.5f;
                            if (Counter[1] == 30 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 4;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 offset = 450 * -Vector2.UnitY.RotatedBy(MathHelper.TwoPi / max * (i + Main.rand.NextFloat()));
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), npc.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                                }
                            }
                        }
                    }
                    else if (npc.ai[3] == 5)
                    {
                        if (npc.ai[2] == 0)
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        npc.ai[2] -= 0.5f;
                        npc.velocity *= 0.5f;
                        EnrageDust();
                    }

                    /*if (npc.ai[0] == 10)
                    {
                        if (++Counter[1] == 15)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const float delay = 15;
                                Vector2 baseVel = 100f / delay * npc.DirectionTo(Main.player[npc.target].Center);

                                const int max = 10;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, baseVel.RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<FishronBubble>(), npc.damage / 5, 0f, Main.myPlayer, delay);
                                }
                            }
                        }
                    }*/
                    break;

                case 11: //p3 dash
                    if (!Main.player[npc.target].ZoneBeach || npc.ai[3] >= 5)
                    {
                        if (npc.ai[2] == 0 && !Main.dedServ)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster70"), npc.Center);

                        if (Main.player[npc.target].ZoneBeach)
                        {
                            npc.position += npc.velocity * 0.5f;
                        }
                        else
                        {
                            npc.position += npc.velocity;
                            npc.ai[2]++;
                        }
                        EnrageDust();
                    }

                    Counter[1] = 0;
                    if (--Counter[0] < 0)
                    {
                        Counter[0] = 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.ai[3] == 2 || npc.ai[3] == 3) //spawn destructible bubbles on 2-dash
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    for (int j = 1; j <= 2; j++)
                                    {
                                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubble>());
                                        if (n < Main.maxNPCs)
                                        {
                                            Main.npc[n].velocity = Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i) * j * 0.5f;
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                        }
                                    }
                                }
                            }

                            if (!Main.player[npc.target].ZoneBeach) //enraged, spawn bubbles
                            {
                                Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<FishronBubble>(), npc.damage / 4, 0f, Main.myPlayer);
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i),
                                        ModContent.ProjectileType<FishronBubble>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    break;

                case 12: //p3 *teleports behind you*
                    if (!Main.player[npc.target].ZoneBeach || (npc.ai[3] > 5 && npc.ai[3] < 8))
                    {
                        if (!Main.player[npc.target].ZoneBeach)
                            npc.position += npc.velocity;
                        npc.ai[2]++;
                        EnrageDust();
                    }

                    Counter[0] = 0;
                    if (npc.ai[2] == 15f)
                    {
                        SpawnRazorbladeRing(6, 8f, npc.damage / 4, -0.75f);
                    }
                    else if (npc.ai[2] == 16f)
                    {
                        const int max = 5;
                        for (int j = -max; j <= max; j++)
                        {
                            Vector2 vel = npc.DirectionFrom(Main.player[npc.target].Center).RotatedBy(MathHelper.PiOver2 / max * j);
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<FishronBubble>(), npc.damage / 5, 0f, Main.myPlayer);
                        }

                        /*if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = Vector2.UnitX * npc.direction;
                            spawnPos = spawnPos.RotatedBy(npc.rotation);
                            spawnPos *= npc.width + 20f;
                            spawnPos /= 2f;
                            spawnPos += npc.Center;
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, 0f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                        }*/
                    }
                    break;

                default:
                    break;
            }

            if (fishBossEX == npc.whoAmI)// && npc.ai[0] >= 10 || (npc.ai[0] == 9 && npc.ai[2] > 120)) //in phase 3, do this check in all stages
            {
                if (--Counter[2] < 0)
                {
                    Counter[2] = 10 * 60;
                    for (int i = -1; i <= 1; i += 2)
                    {
                        int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                        int tilePosY = (int)Main.player[npc.target].Center.Y / 16;
                        tilePosX += 60 * i;

                        if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                            continue;

                        if (Main.tile[tilePosX, tilePosY] == null)
                            Main.tile[tilePosX, tilePosY] = new Tile();

                        //first move up through solid tiles
                        while (Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type])
                        {
                            tilePosY--;
                            if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                                break;
                            if (Main.tile[tilePosX, tilePosY] == null)
                                Main.tile[tilePosX, tilePosY] = new Tile();
                        }

                        tilePosY--;

                        //then move down through air until solid tile/platform reached
                        while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolidTop[Main.tile[tilePosX, tilePosY].type]))
                        {
                            tilePosY++;
                            if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                                break;
                            if (Main.tile[tilePosX, tilePosY] == null)
                                Main.tile[tilePosX, tilePosY] = new Tile();
                        }

                        tilePosY--;

                        Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                        Projectile.NewProjectile(spawn, Vector2.UnitX * -i * 8f, ProjectileID.Cthulunado, npc.damage / 4, 0f, Main.myPlayer, 10, 24);
                    }
                }
            }

            //drop summon
            if (!NPC.downedFishron && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<TruffleWorm2>());
                droppedSummon = true;
            }
        }

        public bool BetsyAI(NPC npc)
        {
            betsyBoss = npc.whoAmI;

            if (!masoBool[3] && npc.life < npc.lifeMax / 2)
            {
                masoBool[3] = true;
                Main.PlaySound(SoundID.Roar, npc.Center, 0);
            }

            if (npc.ai[0] == 6f) //when approaching for roar
            {
                if (npc.ai[1] == 0f)
                {
                    npc.position += npc.velocity;
                }
                else if (npc.ai[1] == 1f)
                {
                    masoBool[0] = true;
                }
            }

            if (masoBool[0])
            {
                npc.velocity = Vector2.Zero;

                if (Counter[0] == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), npc.damage / 3, 0f, Main.myPlayer, 4);
                }
                
                Counter[0]++;
                if (Counter[0] % 2 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, -Vector2.UnitY.RotatedBy(2 * Math.PI / 30 * Counter[1]), ModContent.ProjectileType<BetsyFury>(), npc.damage / 3, 0f, Main.myPlayer, npc.target);
                        Projectile.NewProjectile(npc.Center, -Vector2.UnitY.RotatedBy(2 * Math.PI / 30 * -Counter[1]), ModContent.ProjectileType<BetsyFury>(), npc.damage / 3, 0f, Main.myPlayer, npc.target);
                    }
                    Counter[1]++;
                }
                if (Counter[0] > (masoBool[3] ? 90 : 30) + 2)
                {
                    masoBool[0] = false;
                    masoBool[1] = true;
                    Counter[0] = 0;
                    Counter[1] = 0;
                }

                Aura(npc, 1200, BuffID.WitheredWeapon, true, 226);
                Aura(npc, 1200, BuffID.WitheredArmor, true, 226);
            }

            if (masoBool[1])
            {
                if (++Counter[1] > 75)
                {
                    masoBool[1] = false;
                    Counter[0] = 0;
                    Counter[1] = 0;
                }
                npc.position -= npc.velocity * 0.5f;
                if (Counter[0] % 2 == 0)
                    return false;
            }

            if (!DD2Event.Ongoing && npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead || npc.Distance(Main.player[npc.target].Center) > 3000))
            {
                int p = Player.FindClosest(npc.Center, 0, 0); //extra despawn code for when summoned outside event
                if (p < 0 || !Main.player[p].active || Main.player[p].dead || npc.Distance(Main.player[p].Center) > 3000)
                    npc.active = false;
            }

            //drop summon
            if (NPC.downedGolemBoss && !FargoSoulsWorld.downedBetsy && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<BetsyEgg>());
                droppedSummon = true;
            }

            return true;
        }

        public void CultistAI(NPC npc)
        {
            cultBoss = npc.whoAmI;

            //if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && npc.Distance(Main.LocalPlayer.Center) < 3000) Main.LocalPlayer.AddBuff(BuffID.ChaosState, 2);

            Lighting.AddLight(npc.Center, 1f, 1f, 1f);

            /*Timer++;
            Timer++;
            if (Timer >= 1200)
            {
                Timer = 0;

                if (NPC.CountNPCS(NPCID.AncientCultistSquidhead) < 4 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.AncientCultistSquidhead, 0, 0f, 0f, 0f, 0f, npc.target);
                    if (n != 200 && Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }*/

            //PrintAI(npc);
            //Main.NewText(Terraria.NPC.CountNPCS(Terraria.ID.NPCID.CultistBossClone).ToString());
            //npc.damage = npc.defDamage;

            if (npc.ai[3] == -1f)
            {
                if (Fargowiltas.Instance.MasomodeEXLoaded && npc.ai[1] >= 120f && npc.ai[1] < 419f) //skip summoning ritual LMAO
                {
                    npc.ai[1] = 419f;
                    npc.netUpdate = true;
                }

                if (npc.ai[0] == 5)
                {
                    if (npc.ai[1] == 1f)
                    {
                        Counter[3] = Main.rand.Next(360);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            var netMessage = mod.GetPacket();
                            netMessage.Write((byte)15);
                            netMessage.Write((byte)npc.whoAmI);
                            for (int i = 0; i < Counter.Length; i++)
                                netMessage.Write(Counter[i]);
                            netMessage.Send();
                        }
                    }

                    if (npc.ai[1] > 30f && npc.ai[1] < 330f)
                    {
                        if (npc.life < npc.lifeMax / 4)
                        {
                            Counter[3] += 2;
                        }
                        else if (npc.life < npc.lifeMax / 2)
                        {
                            //alternate between them every tick, effectively 1.5 rotation
                            Counter[3] -= npc.ai[1] % 2 == 0 ? 1 : 2;
                        }
                        else
                        {
                            Counter[3] += 1;
                        }
                        npc.Center = Main.player[npc.target].Center + 180f * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Counter[3]));
                        Lighting.AddLight(npc.Center, 1f, 1f, 1f);
                    }

                    /*int ritual = (int)npc.ai[2];
                    if (Main.projectile[ritual].active && Main.projectile[ritual].type == ProjectileID.CultistRitual)
                    {
                        Counter[3] += npc.life < npc.lifeMax / 2 ? -2 : 1;
                        npc.Center = Main.projectile[ritual].Center + 180f * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Counter[3]));
                    }*/
                }
            }
            else
            {
                //if (npc.ai[3] == 0) npc.damage = 0;

                //about to begin moving for ritual: 0 39 0 12
                //begin transit for ritual: 1 34 0 12
                //pause just before ritual: 0 0 0 13
                //ritual: 5 0 0 -1

                if (!masoBool[0] && npc.life < npc.lifeMax / 2) //p2 transition, force a ritual immediately
                {
                    masoBool[0] = true;
                    npc.ai[0] = 5;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = -1;
                    Main.PlaySound(SoundID.Roar, npc.Center, 0);
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.netUpdate = true;
                        NetUpdateMaso(npc.whoAmI);
                    }
                }

                int damage = 75; //necessary because calameme
                switch ((int)npc.ai[0])
                {
                    case -1:
                        if (npc.ai[1] == 419f)
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[3] = 11f;
                            npc.netUpdate = true;
                        }
                        break;

                    case 2: //ice mist, frost wave support
                        if (npc.life > npc.lifeMax / 2)
                        {
                            if (npc.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient) //single wave
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                    {
                                        Vector2 distance = Main.player[npc.target].Center - Main.npc[i].Center;
                                        distance.Normalize();
                                        distance *= Main.rand.NextFloat(6f, 9f);
                                        distance = distance.RotatedByRandom(Math.PI / 24);
                                        Projectile.NewProjectile(Main.npc[i].Center, distance,
                                            ProjectileID.FrostWave, damage / 3, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (npc.ai[1] < 60 && npc.ai[1] % 4 == 3)
                            {
                                int spacing = 14 - (int)(npc.ai[1] - 3) / 4; //start far and get closer
                                for (int j = -1; j <= 1; j += 2) //from above and below
                                {
                                    for (int i = -1; i <= 1; i += 2) //waves beside you
                                    {
                                        if (i == 0)
                                            continue;
                                        Vector2 spawnPos = Main.player[npc.target].Center;
                                        spawnPos.X += Math.Sign(i) * 125 * 2 + i * 125 * spacing;
                                        spawnPos.Y -= (700 + Math.Abs(i) * 50) * j;
                                        float speed = 6 + spacing;
                                        Projectile.NewProjectile(spawnPos, Vector2.UnitY * speed * j, ProjectileID.FrostWave, damage / 3, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        break;

                    case 3:
                        if (npc.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient) //fireballs
                        {
                            if (npc.life > npc.lifeMax / 2) //solar goop support
                            {
                                for (int i = 0; i < 200; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                    {
                                        int n = NPC.NewNPC((int)Main.npc[i].Center.X, (int)Main.npc[i].Center.Y, NPCID.SolarGoop);
                                        if (n < 200)
                                        {
                                            Main.npc[n].velocity.X = Main.rand.Next(-10, 11);
                                            Main.npc[n].velocity.Y = Main.rand.Next(-15, -4);
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                        }
                                    }
                                }
                            }
                            else //fireball ring
                            {
                                int max = NPC.CountNPCS(NPCID.CultistBossClone) * 2 + 6;

                                Vector2 baseOffset = npc.DirectionTo(Main.player[npc.target].Center);
                                const float spawnOffset = 1200f;
                                const float speed = 7f;
                                const float ai0 = spawnOffset / speed;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(Main.player[npc.target].Center + spawnOffset * baseOffset.RotatedBy(2 * Math.PI / max * i),
                                        -speed * baseOffset.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<CultistFireball>(),
                                        damage / 3, 0f, Main.myPlayer, ai0);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                            }
                        }
                        break;

                    case 4:
                        if (npc.ai[1] == 19f && npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient) //lightning orb
                        {
                            int cultistCount = 1;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                {
                                    if (npc.life > npc.lifeMax / 2) //aimed lightning
                                    {
                                        Vector2 dir = Main.player[npc.target].Center - Main.npc[i].Center;
                                        float ai1New = Main.rand.Next(100);
                                        Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 7f;
                                        Projectile.NewProjectile(Main.npc[i].Center, vel, ProjectileID.CultistBossLightningOrbArc,
                                            damage / 15 * 6, 0, Main.myPlayer, dir.ToRotation(), ai1New);
                                    }
                                    else //vortex lightning
                                    {
                                        Projectile.NewProjectile(Main.npc[i].Center, Main.rand.NextVector2Square(-15, 15), ModContent.ProjectileType<CultistVortex>(),
                                          damage / 15 * 6, 0, Main.myPlayer, 0f, cultistCount);
                                        cultistCount++;
                                    }
                                }
                            }
                        }
                        break;

                    case 7:
                        if (npc.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient) //ancient light, jellyfish support
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<Projectiles.Masomode.CultistRitual>())
                                {
                                    Projectile.NewProjectile(new Vector2(Main.projectile[i].Center.X, Main.player[npc.target].Center.Y - 700),
                                        Vector2.Zero, ModContent.ProjectileType<StardustRain>(), damage / 3, 0f, Main.myPlayer);
                                }
                            }
                        }
                        break;

                    case 8:
                        if (npc.ai[1] == 3f) //ancient doom, nebula sphere support
                        {
                            int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                            if (t != -1 && Main.player[t].active)
                            {
                                for (int i = 0; i < 200; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                        Projectile.NewProjectile(Main.npc[i].Center, Vector2.Zero,
                                            ProjectileID.NebulaSphere, damage / 15 * 6, 0f, Main.myPlayer);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            //drop summon
            if (NPC.downedGolemBoss && !NPC.downedAncientCultist && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<CultistSummon>());
                droppedSummon = true;
            }
        }

        public bool AncientLightAI(NPC npc)
        {
            npc.dontTakeDamage = true;
            npc.immortal = true;
            npc.chaseable = false;
            if (npc.buffType[0] != 0)
                npc.DelBuff(0);
            if (BossIsAlive(ref cultBoss, NPCID.CultistBoss))
            {
                if (++Counter[0] > 20 && Counter[0] < 60)
                {
                    npc.position -= npc.velocity;
                    return false;
                }
            }
            if (masoBool[0])
            {
                if (npc.HasPlayerTarget)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Normalize();
                    speed *= 9f;

                    npc.ai[2] += speed.X / 100f;
                    if (npc.ai[2] > 9f)
                        npc.ai[2] = 9f;
                    if (npc.ai[2] < -9f)
                        npc.ai[2] = -9f;
                    npc.ai[3] += speed.Y / 100f;
                    if (npc.ai[3] > 9f)
                        npc.ai[3] = 9f;
                    if (npc.ai[3] < -9f)
                        npc.ai[3] = -9f;
                }
                else
                {
                    npc.TargetClosest(false);
                }

                Counter[0]++;
                if (Counter[0] > 240)
                {
                    npc.HitEffect(0, 9999);
                    npc.active = false;
                }

                npc.velocity.X = npc.ai[2];
                npc.velocity.Y = npc.ai[3];
            }
            return true;
        }

        public void MoonLordCoreAI(NPC npc)
        {
            moonBoss = npc.whoAmI;
            //npc.defense = masoStateML >= 0 && masoStateML <= 3 ? 0 : npc.defDefense;

            if (!masoBool[3])
            {
                masoBool[3] = true;
                masoStateML = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LunarRitual>(), 25, 0f, Main.myPlayer, 0f, npc.whoAmI);
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FragmentRitual>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                }
            }

            if (!npc.dontTakeDamage && Main.netMode != NetmodeID.MultiplayerClient && Counter[0] % 2 == 0)
                Counter[0]++; //phases transition faster when core is exposed

            if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && !Main.player[Main.myPlayer].ghost && masoStateML >= 0 && masoStateML <= 3)
                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<NullificationCurse>(), 2);
            
            npc.position -= npc.velocity * 2f / 3f; //SLOW DOWN

            if (npc.dontTakeDamage)
            {
                Counter[1]++;

                if (Counter[1] == 370 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        NPC bodyPart = Main.npc[(int)npc.localAI[i]];
                        if (bodyPart.active)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, bodyPart.whoAmI, bodyPart.type);
                    }
                }

                if (Counter[1] > 400)
                {
                    Counter[1] = 0;
                    npc.netUpdate = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        switch (masoStateML)
                        {
                            case 0: //melee
                                for (int i = 0; i < 3; i++)
                                {
                                    NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                    if (bodyPart.active)
                                    {
                                        int damage = 30;
                                        for (int j = -2; j <= 2; j++)
                                        {
                                            Projectile.NewProjectile(bodyPart.Center,
                                                6f * bodyPart.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI / 2 / 4 * j),
                                                ModContent.ProjectileType<MoonLordFireball>(), damage, 0f, Main.myPlayer, 20, 20 + 60);
                                        }
                                    }
                                }
                                break;
                            case 1: //ranged
                                for (int j = 0; j < 6; j++)
                                {
                                    Vector2 spawn = Main.player[npc.target].Center;
                                    spawn.X += Main.rand.NextFloat(-50, 50);
                                    spawn.Y -= Main.rand.NextFloat(900, 1800);
                                    Vector2 speed = 24f * Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(5));
                                    float ai1New = (Main.rand.Next(2) == 0) ? 1 : -1; //randomize starting direction
                                    Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<Projectiles.Champions.CosmosLightning>(),
                                        30, 0f, Main.myPlayer, speed.ToRotation(), ai1New);
                                }
                                break;
                            case 2: //magic
                                for (int i = 0; i < 3; i++)
                                {
                                    NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                    if (bodyPart.active &&
                                        ((i == 2 && bodyPart.type == NPCID.MoonLordHead) ||
                                        bodyPart.type == NPCID.MoonLordHand))
                                    {
                                        int damage = 35;
                                        const int max = 6;
                                        for (int j = 0; j < max; j++)
                                        {
                                            int p = Projectile.NewProjectile(bodyPart.Center,
                                                2.5f * bodyPart.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI * 2 / max * (j + 0.5)),
                                                ModContent.ProjectileType<MoonLordNebulaBlaze>(), damage, 0f, Main.myPlayer);
                                            if (p != Main.maxProjectiles)
                                                Main.projectile[p].timeLeft = 1200;
                                        }
                                    }
                                }
                                break;
                            case 3: //summoner
                                for (int i = 0; i < 3; i++)
                                {
                                    NPC bodyPart = Main.npc[(int)npc.localAI[i]];

                                    if (bodyPart.active &&
                                        ((i == 2 && bodyPart.type == NPCID.MoonLordHead) ||
                                        bodyPart.type == NPCID.MoonLordHand))
                                    {
                                        Vector2 speed = Main.player[npc.target].Center - bodyPart.Center;
                                        speed.Normalize();
                                        speed *= 5f;
                                        for (int j = -1; j <= 1; j++)
                                        {
                                            Vector2 vel = speed.RotatedBy(MathHelper.ToRadians(15) * j);
                                            int n = NPC.NewNPC((int)bodyPart.Center.X, (int)bodyPart.Center.Y, NPCID.AncientLight, 0, 0f, (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f, vel.X, vel.Y);
                                            if (n != Main.maxNPCs)
                                            {
                                                Main.npc[n].velocity = vel;
                                                Main.npc[n].netUpdate = true;
                                                if (Main.netMode == NetmodeID.Server)
                                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                            }
                                        }
                                    }
                                }
                                break;
                            default: //phantasmal eye rings
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    const int max = 4;
                                    const int speed = 8;
                                    const float rotationModifier = 0.5f;
                                    int damage = 40;
                                    float rotation = 2f * (float)Math.PI / max;
                                    Vector2 vel = Vector2.UnitY * speed;
                                    int type = ModContent.ProjectileType<MutantSphereRing>();
                                    for (int i = 0; i < max; i++)
                                    {
                                        vel = vel.RotatedBy(rotation);
                                        int p = Projectile.NewProjectile(npc.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier, speed);
                                        if (p != Main.maxProjectiles)
                                            Main.projectile[p].timeLeft = 1800 - Counter[0];
                                        p = Projectile.NewProjectile(npc.Center, vel, type, damage, 0f, Main.myPlayer, -rotationModifier, speed);
                                        if (p != Main.maxProjectiles)
                                            Main.projectile[p].timeLeft = 1800 - Counter[0];
                                    }
                                    Main.PlaySound(SoundID.Item84, npc.Center);
                                }
                                break;
                        }
                    }
                }
            }
            else //only when vulnerable
            {
                if (!masoBool[0])
                {
                    masoBool[0] = true;
                    Main.PlaySound(SoundID.Roar, Main.player[Main.myPlayer].Center, 0);
                    npc.netUpdate = true;
                }

                Counter[2]++;

                Player player = Main.player[npc.target];
                switch (masoStateML)
                {
                    case 0: //melee
                        {
                            if (Counter[2] == 30)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(Main.npc[(int)npc.localAI[0]].Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSun>(),
                                        60, 0f, Main.myPlayer, npc.whoAmI, npc.localAI[0]);
                            }
                            else if (Counter[2] == 30 + 150)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(Main.npc[(int)npc.localAI[1]].Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSun>(),
                                        60, 0f, Main.myPlayer, npc.whoAmI, npc.localAI[1]);
                            }
                            else if (Counter[2] > 300)
                            {
                                Counter[2] = 0;
                            }
                        }
                        break;

                    case 1: //vortex
                        {
                            if (Counter[2] == 1) //spawn the vortex
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordVortex>(),
                                        40, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                        }
                        break;

                    case 2: //nebula
                        {
                            if (Counter[2] == 30)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    NPC bodyPart = Main.npc[(int)npc.localAI[i]];
                                    int damage = 35;
                                    for (int j = -2; j <= 2; j++)
                                    {
                                        Projectile.NewProjectile(bodyPart.Center,
                                            2.5f * bodyPart.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI / 2 / 2 * j),
                                            ModContent.ProjectileType<MoonLordNebulaBlaze2>(), damage, 0f, Main.myPlayer, npc.whoAmI);
                                    }
                                }
                            }
                            if (Counter[2] > 300)
                            {
                                Counter[2] = 0;
                            }
                        }
                        break;

                    case 3: //stardust
                        {
                            float baseRotation = MathHelper.ToRadians(50);
                            if (Counter[2] == 10)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(Main.npc[(int)npc.localAI[0]].Center, Main.npc[(int)npc.localAI[0]].DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.localAI[0]);
                            }
                            else if (Counter[2] == 20)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(Main.npc[(int)npc.localAI[1]].Center, Main.npc[(int)npc.localAI[2]].DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, -baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.localAI[1]);
                            }
                            else if (Counter[2] == 30)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(Main.npc[(int)npc.localAI[2]].Center, Main.npc[(int)npc.localAI[1]].DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.localAI[2]);
                            }
                            else if (Counter[2] == 40)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(),
                                        60, 0f, Main.myPlayer, -baseRotation * Main.rand.NextFloat(0.9f, 1.1f), npc.whoAmI);
                            }
                            else if (Counter[2] > 300)
                            {
                                Counter[2] = 0;
                            }
                        }
                        break;

                    default: //any
                        {
                            /*if (Counter[2] == 1) //spawn the moon
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordMoon>(),
                                        0, 0f, Main.myPlayer, npc.whoAmI);
                            }*/
                        }
                        break;
                }
            }

            if (npc.ai[0] == 2f) //moon lord is dead
            {
                if (!masoBool[1]) //check once when dead
                {
                    masoBool[1] = true;
                    //stop all attacks (and become intangible lol) after i die
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.MoonLordCore) == 1)
                    {
                        masoStateML = 4;
                        if (Main.netMode == NetmodeID.Server) //sync damage phase with clients
                        {
                            var netMessage = mod.GetPacket();
                            netMessage.Write((byte)4);
                            netMessage.Write((byte)npc.whoAmI);
                            netMessage.Write(Counter[0]);
                            netMessage.Write(masoStateML);
                            netMessage.Send();
                        }
                    }
                }
                Counter[0] = 0;
                Counter[1] = 0;
                Counter[2] = 0;
            }
            else //moon lord isn't dead
            {
                if (++Counter[0] > 1800)
                {
                    Counter[0] = 0;
                    Counter[1] = 0;
                    Counter[2] = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (++masoStateML > 4)
                            masoStateML = 0;
                        if (Main.netMode == NetmodeID.Server) //sync damage phase with clients
                        {
                            var netMessage = mod.GetPacket();
                            netMessage.Write((byte)4);
                            netMessage.Write((byte)npc.whoAmI);
                            netMessage.Write(Counter[0]);
                            netMessage.Write(masoStateML);
                            netMessage.Send();
                        }
                    }
                }
            }

            switch (masoStateML)
            {
                case 0: Main.monolithType = 3; break;
                case 1: Main.monolithType = 0; break;
                case 2: Main.monolithType = 1; break;
                case 3: Main.monolithType = 2;
                    if (Counter[0] < 60) //so that player isn't punished for using weapons during prior phase
                        Main.LocalPlayer.GetModPlayer<FargoPlayer>().MasomodeMinionNerfTimer = 0;
                    break;
                default: break;
            }

            //drop summon
            if (NPC.downedAncientCultist && !NPC.downedMoonlord && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, ModContent.ItemType<CelestialSigil2>());
                droppedSummon = true;
            }
        }

        public void MoonLordSocketAI(NPC npc)
        {
            //npc.defense = masoStateML >= 0 && masoStateML <= 3 ? 0 : npc.defDefense;

            /*if (npc.ai[0] == -2f) //eye socket is empty
            {
                if (Counter[2] < 5)
                    npc.localAI[0] = (Main.player[Main.npc[(int)npc.ai[3]].target].Center - npc.Center).ToRotation();

                if (npc.ai[1] == 0f //happens every 32 ticks
                    && Main.npc[(int)npc.ai[3]].ai[0] != 2f //will stop when ML dies
                    && Main.npc[(int)npc.ai[3]].GetGlobalNPC<EModeGlobalNPC>().masoBool[0]) //only during p3
                {
                    Counter[2]++;
                    if (Counter[2] == 2) //spawn telegraph
                    {
                        NetUpdateMaso(npc.whoAmI);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(npc.localAI[0]), ModContent.ProjectileType<PhantasmalDeathrayMLSmall>(), 0, 0f, Main.myPlayer, 0, npc.whoAmI);
                    }
                    else if (Counter[2] == 7) //FIRE LASER
                    {
                        NetUpdateMaso(npc.whoAmI);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float newRotation = (Main.player[Main.npc[(int)npc.ai[3]].target].Center - npc.Center).ToRotation();
                            float difference = newRotation - npc.localAI[0];
                            const float PI = (float)Math.PI;
                            float rotationDirection = PI / 4f / 120f; //positive is CW, negative is CCW
                            if (difference < -PI)
                                difference += 2f * PI;
                            if (difference > PI)
                                difference -= 2f * PI;
                            if (difference < 0f)
                                rotationDirection *= -1f;
                            Vector2 speed = Vector2.UnitX.RotatedBy(npc.localAI[0]);
                            int damage = 60;
                            Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<PhantasmalDeathrayML>(), damage, 0f, Main.myPlayer, rotationDirection, npc.whoAmI);
                        }
                    }
                    else if (Counter[2] >= 27)
                    {
                        Counter[2] = 0;
                    }
                    npc.netUpdate = true;
                }
            }*/
        }
    }
}
