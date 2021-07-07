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
                case 0:
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

                case 1:
                    color = Color.Yellow;
                    maxTime = 90 + 60;
                    projectile.rotation = projectile.ai[1];
                    alphaModifier = 3;
                    if (projectile.localAI[0] < 90)
                        alphaModifier = 0;
                    else
                        projectile.velocity = Vector2.Zero;
                    break;

                case 2:
                    {
                        color = Color.Yellow;
                        maxTime = 90;
                        alphaModifier = 7f;
                        projectile.rotation = projectile.ai[1];
                        int localAI1 = (int)projectile.localAI[1];
                        if (localAI1 > -1 && localAI1 < Main.maxNPCs && Main.npc[localAI1].active && Main.npc[localAI1].type == ModContent.NPCType<NPCs.AbomBoss.AbomBoss>())
                            projectile.Center = Main.npc[localAI1].Center;
                    }
                    break;

                case 3:
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
                            projectile.rotation = projectile.rotation.AngleLerp(targetRot, 0.1f);
                        }
                    }
                    break;

                case 4:
                    {
                        color = Color.Yellow;
                        maxTime = 150;
                        alphaModifier = 7f;
                        projectile.rotation = projectile.ai[1];
                        int localAI1 = (int)projectile.localAI[1];
                        if (localAI1 > -1 && localAI1 < Main.maxNPCs && Main.npc[localAI1].active && Main.npc[localAI1].type == ModContent.NPCType<NPCs.AbomBoss.AbomBoss>())
                            projectile.Center = Main.npc[localAI1].Center;
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

            projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * projectile.localAI[0]) * alphaModifier);
            if (projectile.alpha < 0)
                projectile.alpha = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
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
            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = 1000f * projectile.rotation.ToRotationVector2() * projectile.scale * (i + 0.5f);
                Main.spriteBatch.Draw(texture2D13, offset + projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}