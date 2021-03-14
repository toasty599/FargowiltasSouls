using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviSparklingLove : ModProjectile
    {
        public int scaleCounter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Love");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 180;
            projectile.alpha = 250;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToMutantBomb = true;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            //the important part
            int ai0 = (int)projectile.ai[0];
            if (ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].type == ModContent.NPCType<NPCs.DeviBoss.DeviBoss>())
            {
                if (projectile.localAI[0] == 0)
                {
                    projectile.localAI[0] = 1;
                    projectile.localAI[1] = projectile.DirectionFrom(Main.npc[ai0].Center).ToRotation();
                    MakeDust();
                }

                Vector2 offset = new Vector2(projectile.ai[1], 0).RotatedBy(Main.npc[ai0].ai[3] + projectile.localAI[1]);
                projectile.Center = Main.npc[ai0].Center + offset;
            }
            else
            {
                projectile.Kill();
                return;
            }

            //not important part
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 4;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
            
            //important again
            if (++projectile.localAI[0] > 31)
            {
                projectile.localAI[0] = 1;
                if (++scaleCounter < 3)
                {
                    projectile.position = projectile.Center;

                    projectile.width *= 2;
                    projectile.height *= 2;
                    projectile.scale *= 2;

                    projectile.Center = projectile.position;

                    MakeDust();
                    Main.PlaySound(SoundID.Item92, projectile.Center);
                }
            }

            if (projectile.timeLeft == 8)
            {
                Main.PlaySound(SoundID.NPCKilled, projectile.Center, 6);
                Main.PlaySound(SoundID.Item92, projectile.Center);

                MakeDust();

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    /*for (int i = 0; i < 8; i++)
                    {
                        Vector2 target = 400 * Vector2.UnitX.RotatedBy(Math.PI / 4 * i + Math.PI / 8);
                        Vector2 speed = 2 * target / 90;
                        float acceleration = -speed.Length() / 90;
                        float rotation = speed.ToRotation() + (float)Math.PI / 2;
                        Projectile.NewProjectile(projectile.Center, speed, mod.ProjectileType("DeviEnergyHeart"), projectile.damage, 0f, Main.myPlayer, rotation, acceleration);
                    }*/
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 target = 600 * Vector2.UnitX.RotatedBy(Math.PI / 4 * i);
                        Vector2 speed = 2 * target / 90;
                        float acceleration = -speed.Length() / 90;
                        float rotation = speed.ToRotation() + (float)Math.PI / 2;
                        Projectile.NewProjectile(projectile.Center, speed, mod.ProjectileType("DeviEnergyHeart"),
                            (int)(projectile.damage * 0.75), 0f, Main.myPlayer, rotation, acceleration);
                    }
                }
            }

            projectile.direction = projectile.spriteDirection = Main.npc[ai0].direction;
            projectile.rotation = Main.npc[ai0].ai[3] + projectile.localAI[1] + (float)Math.PI / 2 + (float)Math.PI / 4;
            if (projectile.spriteDirection >= 0)
                projectile.rotation -= (float)Math.PI / 2;
        }

        private void MakeDust()
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 272, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f * projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 272, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f * projectile.scale;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            for (int i = 0; i < 80; i++) //warning dust ring
            {
                Vector2 vector6 = Vector2.UnitY * 15f * projectile.scale;
                vector6 = vector6.RotatedBy((i - (80 / 2 - 1)) * 6.28318548f / 80) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 86, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }

        /*public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity.X = target.Center.X < Main.npc[(int)projectile.ai[0]].Center.X ? -15f : 15f;
            target.velocity.Y = -10f;
            target.AddBuff(mod.BuffType("Lovestruck"), 240);
        }*/

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}