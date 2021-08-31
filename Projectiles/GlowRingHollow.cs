using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GlowRingHollow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Ring");
        }

        public override void SetDefaults()
        {
            projectile.width = 1000;
            projectile.height = 1000;
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

        public override void AI()
        {
            projectile.timeLeft = 2;

            float radius = 500f;
            int maxTime = 60;
            int alphaModifier = 3;

            switch ((int)projectile.ai[0])
            {
                case 1: //mutant reti glaive
                    projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
                    color = Color.Red;
                    radius = 525;
                    maxTime = 180;
                    break;

                case 2: //mutant spaz glaive
                    projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
                    color = Color.Green;
                    radius = 350;
                    maxTime = 180;
                    break;

                case 3: //abom emode p2 dash telegraph
                    {
                        color = Color.Yellow;
                        maxTime = 120;
                        alphaModifier = 10;
                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active)
                        {
                            projectile.Center = Main.npc[ai1].Center;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                        radius = 1400f * (maxTime - projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 4: //betsy electrosphere boundary
                    color = Color.Cyan;
                    radius = 1200;
                    maxTime = 360;
                    break;

                case 5: //mutant subphase transition
                    projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
                    color = new Color(51, 255, 191);
                    maxTime = 120;
                    radius = 1200 * (float)Math.Cos(Math.PI / 2 * projectile.localAI[0] / maxTime);
                    alphaModifier = -1;
                    projectile.alpha = 0;
                    break;

                case 6: //destroyer coil tell
                    {
                        color = Color.Purple;
                        maxTime = 120;
                        alphaModifier = 10;
                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active)
                        {
                            projectile.Center = Main.npc[ai1].Center;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                        radius = 1200f * (maxTime - projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 7: //life champ dash tell
                    {
                        color = Color.Yellow;
                        alphaModifier = 10;
                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active && Main.npc[ai1].ai[3] == 0)
                        {
                            projectile.Center = Main.npc[ai1].Center;

                            maxTime = Main.npc[ai1].localAI[2] == 1 ? 30 : 60;

                            if (Main.npc[ai1].ai[1] == 0)
                                projectile.localAI[0] = 0;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                        radius = 1800f * (maxTime - projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 8: //boc confused tell
                    color = Color.Red;
                    maxTime = 60;
                    alphaModifier = 3;
                    radius = projectile.ai[1] * (float)Math.Sqrt(Math.Sin(Math.PI / 2 * projectile.localAI[0] / maxTime));
                    break;

                case 9: //destroyer light show tell
                    {
                        color = Color.Yellow;
                        maxTime = 120;
                        alphaModifier = 10;
                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active)
                        {
                            projectile.Center = Main.npc[ai1].Center;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                        radius = 1200f * (maxTime - projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 10: //nebula tower tp
                    {
                        color = Color.Violet;
                        maxTime = 90;
                        alphaModifier = 10;
                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active)
                        {
                            if (projectile.localAI[0] == maxTime)
                            {
                                Main.npc[ai1].Center = projectile.Center;
                                for (int i = 0; i < 100; i++)
                                {
                                    int d = Dust.NewDust(Main.npc[ai1].position, Main.npc[ai1].width, Main.npc[ai1].height, 86, Scale: 4f);
                                    Main.dust[d].velocity *= 4f;
                                    Main.dust[d].noGravity = true;
                                }
                            }
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                        radius = 1200f * (maxTime - projectile.localAI[0]) / maxTime; //shrink down
                    }
                    break;

                case 11: //retinazer aura
                    {
                        color = Color.Red;

                        if (projectile.localAI[0] > maxTime / 2) //NEVER fade normally
                            projectile.localAI[0] = maxTime / 2;

                        int ai1 = (int)projectile.ai[1];
                        if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active)
                        {
                            projectile.Center = Main.npc[ai1].Center;
                            radius = 2000 - 1200 * Main.npc[ai1].GetGlobalNPC<NPCs.EModeGlobalNPC>().Counter[3] / 180f;
                        }
                        else
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;

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
                projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * projectile.localAI[0])) * alphaModifier;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }

            projectile.scale = radius * 2f / 1000f;

            projectile.position = projectile.Center;
            projectile.width = projectile.height = (int)(1000 * projectile.scale);
            projectile.Center = projectile.position;
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
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}