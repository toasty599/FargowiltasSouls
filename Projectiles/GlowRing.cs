using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GlowRing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Ring");
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 0;
            projectile.timeLeft = 300;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public Color color = Color.White;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0]);
            if (npc != null && npc.HasPlayerTarget)
            {
                projectile.Center = npc.Center;
            }

            float scale = 12f;
            int maxTime = 30;
            bool customScaleAlpha = false;

            switch ((int)projectile.ai[1])
            {
                case -21: //default but small, devi uses this for becoming back money
                    scale = 4f;
                    maxTime = 60;
                    break;

                case -20: //eridanus punch windup
                    {
                        customScaleAlpha = true;
                        projectile.localAI[1] = 1;
                        maxTime = 200;
                        float modifier = projectile.localAI[0] / maxTime;
                        color = new Color(51, 255, 191) * modifier;
                        projectile.alpha = (int)(255f * (1f - modifier));
                        projectile.scale = 3f * 6f * (1f - modifier);
                    }
                    break;

                case -19: //abom dash
                    color = Color.Yellow;
                    scale = 18f;
                    break;

                case -18: //eridanus timestop
                    scale = 36f;
                    maxTime = 120;
                    break;

                case -17: //devi smallest pink
                    scale = 6f;
                    goto case -16;

                case -16:
                    color = new Color(255, 51, 153);
                    break;

                case -15:
                    scale = 18f;
                    goto case -16;

                case -14: //deviantt biggest pink
                    scale = 24f;
                    goto case -16;

                case -13: //wof reticle
                    color = new Color(93, 255, 241);
                    scale = 6f;
                    maxTime = 15;
                    break;

                case -12: //nature shroomite blue
                    color = new Color(0, 0, 255);
                    maxTime = 45;
                    break;

                case -11: //nature chlorophyte green
                    color = new Color(0, 255, 0);
                    maxTime = 45;
                    break;

                case -10: //nature frost cyan
                    color = new Color(0, 255, 255);
                    maxTime = 45;
                    break;

                case -9: //nature rain yellow
                    color = new Color(255, 255, 0);
                    maxTime = 45;
                    break;

                case -8: //nature molten orange
                    color = new Color(255, 127, 40);
                    maxTime = 45;
                    break;

                case -7: //nature crimson red
                    color = new Color(255, 0, 0);
                    maxTime = 45;
                    break;

                case -6: //will, spirit champ yellow
                    color = new Color(255, 255, 0);
                    scale = 18f;
                    break;

                case -5: //shadow champ purple
                    color = new Color(200, 0, 255);
                    scale = 18f;
                    break;

                case -4: //life champ yellow
                    color = new Color(255, 255, 0);
                    scale = 18f;
                    maxTime = 60;
                    break;

                case -3: //earth champ orange
                    color = new Color(255, 100, 0);
                    scale = 18f;
                    maxTime = 60;
                    break;

                case -2: //ml teal cyan
                    color = new Color(51, 255, 191);
                    scale = 18f;
                    break;

                case -1: //purple shadowbeam
                    color = new Color(200, 0, 200);
                    maxTime = 60;
                    break;

                case NPCID.EyeofCthulhu:
                    color = new Color(51, 255, 191);
                    maxTime = 45;
                    break;

                case NPCID.QueenBee:
                    color = new Color(255, 255, 100);
                    maxTime = 45;
                    break;

                case NPCID.WallofFleshEye:
                    color = new Color(93, 255, 241);
                    scale = 16f;
                    maxTime = 45;
                    break;

                case NPCID.Retinazer:
                    color = new Color(255, 0, 0);
                    scale = 24f;
                    maxTime = 60;
                    break;

                case NPCID.CultistBoss:
                    color = new Color(255, 127, 40);
                    break;

                case NPCID.MoonLordHand:
                case NPCID.MoonLordHead:
                case NPCID.MoonLordCore:
                    color = new Color(51, 255, 191);
                    scale = 12f;
                    maxTime = 60;
                    break;

                default:
                    break;
            }

            if (++projectile.localAI[0] > maxTime)
            {
                projectile.Kill();
                return;
            }

            if (!customScaleAlpha)
            {
                projectile.scale = scale * (float)Math.Sin(Math.PI / 2 * projectile.localAI[0] / maxTime);
                projectile.alpha = (int)(255f * projectile.localAI[0] / maxTime * 0.75f);
            }

            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
                projectile.alpha = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.localAI[1] == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            if (projectile.localAI[1] == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            return false;
        }
    }
}