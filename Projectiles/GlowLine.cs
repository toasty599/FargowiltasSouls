using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GlowLine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Line");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 255;

            projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public Color color = Color.White;

        public override bool CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            int maxTime = 60;
            float alphaModifier = 3;

            switch ((int)projectile.ai[0])
            {
                case 0: //abom flaming scythe telegraph, sticks to abom and follows his line of sight to player w/ offset
                    {
                        color = Color.Yellow;
                        maxTime = 30;
                        alphaModifier = 10;
                        int localAI1 = (int)projectile.localAI[1];
                        if (localAI1 > -1 && localAI1 < Main.maxNPCs && Main.npc[localAI1].active && Main.npc[localAI1].type == ModContent.NPCType<NPCs.AbomBoss.AbomBoss>())
                        {
                            projectile.Center = Main.npc[localAI1].Center;
                            projectile.rotation = Main.npc[localAI1].DirectionTo(Main.player[Main.npc[localAI1].target].Center).ToRotation() + projectile.ai[1];
                        }
                    }
                    break;

                case 1: //abom split sickle box telegraph, hides until after the sickles split
                    {
                        color = Color.Yellow;
                        maxTime = 90 + 60;
                        projectile.rotation = projectile.ai[1];
                        alphaModifier = 1;
                        if (projectile.localAI[0] < 90)
                            alphaModifier = 0;
                        else
                            projectile.velocity = Vector2.Zero;
                    }
                    break;

                case 2: //devi sparkling love, decelerates alongside energy hearts
                    {
                        color = Color.HotPink;
                        maxTime = 90;
                        projectile.rotation = projectile.ai[1];
                        alphaModifier = 1;
                        if (projectile.velocity != Vector2.Zero)
                        {
                            if (projectile.localAI[0] == 0)
                                projectile.localAI[1] = -projectile.velocity.Length() / maxTime;

                            float speed = projectile.velocity.Length();
                            speed += projectile.localAI[1];
                            projectile.velocity = Vector2.Normalize(projectile.velocity) * speed;
                        }
                    }
                    break;

                case 3: //abom laevateinn 1&2 telegraph, swing around to where actual sword will spawn
                    {
                        color = Color.Yellow;
                        maxTime = 60;
                        alphaModifier = 6f;
                        
                        int localAI1 = (int)projectile.localAI[1];
                        if (localAI1 > -1 && localAI1 < Main.maxNPCs && Main.npc[localAI1].active && Main.npc[localAI1].type == ModContent.NPCType<NPCs.AbomBoss.AbomBoss>())
                        {
                            projectile.Center = Main.npc[localAI1].Center;
                            if (projectile.localAI[0] == 0)
                                projectile.rotation = Main.npc[localAI1].DirectionTo(Main.player[Main.npc[localAI1].target].Center).ToRotation();
                            float targetRot = Main.npc[localAI1].DirectionTo(Main.player[Main.npc[localAI1].target].Center).ToRotation() + projectile.ai[1];
                            while (targetRot < -(float)Math.PI)
                                targetRot += 2f * (float)Math.PI;
                            while (targetRot > (float)Math.PI)
                                targetRot -= 2f * (float)Math.PI;
                            projectile.rotation = projectile.rotation.AngleLerp(targetRot, 0.05f);
                        }
                    }
                    break;

                case 4: //abom laevateinn 3 telegraph, swing around to where actual sword will spawn but slower
                    {
                        color = Color.Yellow;
                        maxTime = 150;
                        alphaModifier = 7f;
                        
                        int localAI1 = (int)projectile.localAI[1];
                        if (localAI1 > -1 && localAI1 < Main.maxNPCs && Main.npc[localAI1].active && Main.npc[localAI1].type == ModContent.NPCType<NPCs.AbomBoss.AbomBoss>())
                        {
                            projectile.Center = Main.npc[localAI1].Center;
                            float targetRot = projectile.ai[1];
                            while (targetRot < -(float)Math.PI)
                                targetRot += 2f * (float)Math.PI;
                            while (targetRot > (float)Math.PI)
                                targetRot -= 2f * (float)Math.PI;
                            projectile.velocity = projectile.velocity.ToRotation().AngleLerp(targetRot, 0.05f).ToRotationVector2();
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation = projectile.velocity.ToRotation();
                    }
                    break;

                case 5: //abom cirno, slide in to a halt from outside
                    {
                        color = new Color(0, 1f, 1f);
                        maxTime = 150;
                        alphaModifier = 10f;
                        
                        int localAI1 = (int)projectile.localAI[1];
                        if (localAI1 > -1 && localAI1 < Main.maxNPCs && Main.npc[localAI1].active && Main.npc[localAI1].type == ModContent.NPCType<NPCs.AbomBoss.AbomBoss>())
                        {
                            Vector2 targetPos = Main.npc[localAI1].Center + Vector2.UnitX * projectile.ai[1];
                            projectile.Center = Vector2.Lerp(projectile.Center, targetPos, 0.025f);
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation = projectile.velocity.ToRotation();
                    }
                    break;

                case 6: //eridanus vortex lightning starting angles
                    {
                        projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;

                        color = new Color(51, 255, 191);
                        maxTime = 90;

                        if (projectile.ai[1] > -1 && projectile.ai[1] < Main.maxPlayers)
                        {
                            projectile.rotation = projectile.DirectionTo(Main.player[(int)projectile.ai[1]].Center).ToRotation();
                        }
                        else
                        {
                            projectile.ai[1] = Player.FindClosest(projectile.Center, 0, 0);
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation += projectile.velocity.ToRotation(); //yes, PLUS because rotation is set up there, velocity is the offset
                    }
                    break;

                case 7: //celestial pillar explode
                    {
                        projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;

                        switch ((int)projectile.ai[1])
                        {
                            case 0: color = Color.Magenta; break; //nebula
                            case 1: color = Color.Orange; break; //solar
                            case 2: color = new Color(51, 255, 191); break; //vortex
                            default: color = Color.Blue; break; //stardust
                        }
                        maxTime = 20;
                        alphaModifier = 2;

                        projectile.position -= projectile.velocity;
                        projectile.rotation = projectile.velocity.ToRotation();

                        if (projectile.localAI[0] == maxTime)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    Vector2 speed = (8f * (j + 1) + 4f) * projectile.velocity;
                                    Projectile.NewProjectile(projectile.Center, speed, mod.ProjectileType("CelestialFragment"), projectile.damage, 0f, Main.myPlayer, projectile.ai[1]);
                                }
                            }
                        }
                    }
                    break;

                case 8: //prime limbs
                    {
                        color = Color.Yellow;
                        maxTime = 60;

                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active)
                        {
                            projectile.Center = Main.npc[ai1].Center;
                            projectile.rotation = Main.npc[ai1].rotation + MathHelper.PiOver2;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }

                        projectile.position -= projectile.velocity;
                        projectile.rotation += projectile.velocity.ToRotation(); //yes, PLUS because rotation is set up there, velocity is the offset
                    }
                    break;

                case 9: //reti telegraph
                    {
                        color = Color.Red;
                        maxTime = 120;
                        alphaModifier = 2;

                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active)
                        {
                            Vector2 offset = new Vector2(Main.npc[ai1].width - 24, 0).RotatedBy(Main.npc[ai1].rotation + 1.57079633);
                            projectile.Center = Main.npc[ai1].Center + offset;
                            projectile.rotation = Main.npc[ai1].rotation + MathHelper.PiOver2;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;

                /*case 9: //destroyer telegraphs
                    {
                        maxTime = 30;
                        alphaModifier = 2;

                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active
                            && (Main.npc[ai1].type == NPCID.TheDestroyerBody || Main.npc[ai1].type == NPCID.TheDestroyerTail)
                            && !Main.npc[ai1].GetGlobalNPC<NPCs.EModeGlobalNPC>().masoBool[0])
                        {
                            color = Main.npc[ai1].ai[2] == 0 ? Color.Red : Color.Yellow;
                            projectile.Center = Main.npc[ai1].Center;
                            projectile.rotation = Main.npc[ai1].DirectionTo(Main.player[Main.npc[ai1].target].Center).ToRotation();
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;*/

                default:
                    break;
            }

            if (++projectile.localAI[0] > maxTime)
            {
                projectile.Kill();
                return;
            }

            if (alphaModifier >= 0)
            {
                projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * projectile.localAI[0]) * alphaModifier);
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity * (Main.mouseTextColor / 255f) * 0.95f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            float length = 1000f * projectile.scale;
            for (float i = 0; i <= 3000f; i += length)
            {
                Vector2 offset = projectile.rotation.ToRotationVector2() * (i + length / 2);
                Main.spriteBatch.Draw(texture2D13, offset + projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}