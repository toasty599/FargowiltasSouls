using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpearSpin : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 116;
            projectile.height = 116;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.3f;
            projectile.alpha = 0;
            cooldownSlot = 1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        private bool predictive;
        private int direction = 1;

        public override void AI()
        {
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = Main.rand.NextBool() ? -1 : 1;
                projectile.timeLeft = (int)projectile.ai[1];
            }

            NPC mutant = Main.npc[(int)projectile.ai[0]];
            if (mutant.active && mutant.type == mod.NPCType("MutantBoss"))
            {
                projectile.Center = mutant.Center;
                direction = mutant.direction;

                if (mutant.ai[0] == 4 || mutant.ai[0] == 13 || mutant.ai[0] == 21)
                {
                    projectile.rotation += (float)Math.PI / 6.85f * projectile.localAI[1];

                    if (++projectile.localAI[0] > 8)
                    {
                        projectile.localAI[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 speed = Vector2.UnitY.RotatedByRandom(Math.PI / 2) * Main.rand.NextFloat(6f, 9f);
                            if (mutant.Center.Y < Main.player[mutant.target].Center.Y)
                                speed *= -1f;
                            float ai1 = projectile.timeLeft + Main.rand.Next(projectile.timeLeft / 2);
                            Projectile.NewProjectile(projectile.position + Main.rand.NextVector2Square(0f, projectile.width),
                                speed, ModContent.ProjectileType<MutantEyeHoming>(), projectile.damage, 0f, projectile.owner, mutant.target, ai1);
                        }
                    }

                    if (projectile.timeLeft % 20 == 0)
                    {
                        Main.PlaySound(SoundID.Item1, projectile.Center);
                    }

                    if (mutant.ai[0] == 13)
                        predictive = true;

                    projectile.alpha = 0;

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        Main.projectile.Where(x => x.active && x.friendly && !FargoSoulsUtil.IsMinionDamage(x, false)).ToList().ForEach(x => //reflect projectiles
                        {
                            if (Vector2.Distance(x.Center, mutant.Center) <= projectile.width / 2)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int dustId = Dust.NewDust(x.position, x.width, x.height, 87,
                                        x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                                    Main.dust[dustId].noGravity = true;
                                }

                                // Set ownership
                                x.hostile = true;
                                x.friendly = false;
                                x.owner = Main.myPlayer;
                                x.damage /= 4;

                                // Turn around
                                x.velocity *= -1f;

                                // Flip sprite
                                if (x.Center.X > mutant.Center.X)
                                {
                                    x.direction = 1;
                                    x.spriteDirection = 1;
                                }
                                else
                                {
                                    x.direction = -1;
                                    x.spriteDirection = -1;
                                }

                                //x.netUpdate = true;

                                if (x.owner == Main.myPlayer)
                                    Projectile.NewProjectile(x.Center, Vector2.Zero, ModContent.ProjectileType<Souls.IronParry>(), 0, 0f, Main.myPlayer);
                            }
                        });
                    }
                }
                else
                {
                    projectile.alpha = 255;
                }
            }
            else
            {
                projectile.Kill();
                return;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, mod.ProjectileType("PhantasmalBlast"), 0, 0f, projectile.owner);
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                target.AddBuff(mod.BuffType("OceanicMaul"), 5400);
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 600);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            if (FargoSoulsWorld.MasochistModeReal)
            {
                for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.1f)
                {
                    Texture2D glow = mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                    Color color27 = Color.Lerp(new Color(51, 255, 191, 210), Color.Transparent, (float)Math.Cos(projectile.ai[0]) / 3 + 0.3f);
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    float scale = projectile.scale - (float)Math.Cos(projectile.ai[0]) / 5;
                    scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    int max0 = Math.Max((int)i - 1, 0);
                    Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], (1 - i % 1));
                    float smoothtrail = i % 1 * (float)Math.PI / 6.85f;
                    bool withinangle = projectile.rotation > -Math.PI / 2 && projectile.rotation < Math.PI / 2;
                    if (withinangle && direction == 1)
                        smoothtrail *= -1;
                    else if (!withinangle && direction == -1)
                        smoothtrail *= -1;

                    center += projectile.Size / 2;

                    Vector2 offset = (projectile.Size / 4).RotatedBy(projectile.oldRot[(int)i] - smoothtrail * (-projectile.direction));
                    Main.spriteBatch.Draw(
                        glow,
                        center - offset - Main.screenPosition + new Vector2(0, projectile.gfxOffY),
                        null,
                        color27,
                        projectile.rotation,
                        glow.Size() / 2,
                        scale * 0.4f,
                        SpriteEffects.None,
                        0f);
                }
            }

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            
            if (projectile.ai[1] > 0 && !FargoSoulsWorld.MasochistModeReal)
            {
                Texture2D glow = mod.GetTexture("Projectiles/MutantBoss/MutantSpearAimGlow");
                float modifier = projectile.timeLeft / projectile.ai[1];
                Color glowColor = predictive ? new Color(0, 0, 255, 210) : new Color(51, 255, 191, 210);
                glowColor *= 1f - modifier;
                float glowScale = projectile.scale * 6f * modifier;
                Main.spriteBatch.Draw(glow, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, 0, origin2, glowScale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}