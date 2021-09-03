using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class LightningVortexHostile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortex");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
        }

        public override bool CanDamage()
        {
            return false;
        }
        Color DrawColor = Color.Cyan;

        public override void AI()
        {
            bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.MasochistMode;
            if ((NPC.AnyNPCs(NPCID.TheDestroyer) && recolor) || NPC.AnyNPCs(mod.NPCType("MutantBoss")))
                DrawColor = new Color(231, 174, 254);

            int shadertype = (DrawColor == new Color(231, 174, 254)) ? 100 : 0; //if it's recolored, use a shader for all the dusts spawned so they're purple instead of cyan
            Player player = FargoSoulsUtil.PlayerExists(projectile.ai[1]);
            if (player == null)
                TargetEnemies();

            projectile.ai[0]++;
            if (projectile.ai[0] <= 50)
            {
                if (Main.rand.Next(4) == 0)
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 4f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
                if (Main.rand.Next(4) == 0)
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true; 
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.position = projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 2f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
            }
            else if (projectile.ai[0] <= 90)
            {
                projectile.scale = (projectile.ai[0] - 50) / 40;
                projectile.alpha = 255 - (int)(255 * projectile.scale);
                projectile.rotation = projectile.rotation - 0.1570796f;
                if (Main.rand.Next(2) == 0)
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.fadeIn = 0.5f;
                    dust.customData = projectile.Center;
                }
                if (Main.rand.Next(2) == 0)
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = projectile.Center;
                }

                
                if (projectile.ai[0] == 90 && projectile.ai[1] != -1 && Main.netMode != NetmodeID.MultiplayerClient && player != null)
                {
                    Vector2 rotationVector2 = player.Center - projectile.Center;
                    rotationVector2.Normalize();

                    Vector2 vector2_3 = rotationVector2 * 24f;
                    float ai1New = (Main.rand.Next(2) == 0) ? 1 : -1; //randomize starting direction
                    int p = Projectile.NewProjectile(projectile.Center, vector2_3,
                        mod.ProjectileType("HostileLightning"), projectile.damage, projectile.knockBack, projectile.owner,
                        rotationVector2.ToRotation(), ai1New * 0.75f);
                    Main.projectile[p].localAI[1] = shadertype; //change projectile's ai if the recolored vortex portal is being used, so that purple ones always fire purple lightning
                }
            }
            else if (projectile.ai[0] <= 120)
            {
                projectile.scale = 1f;
                projectile.alpha = 0;
                projectile.rotation = projectile.rotation - (float)Math.PI / 60f;
                if (Main.rand.Next(2) == 0)
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = projectile.Center;
                }
                else
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = projectile.Center;
                }
            }
            else
            {
                projectile.scale = (float)(1.0 - (projectile.ai[0] - 120.0) / 60.0);
                projectile.alpha = 255 - (int)(255 * projectile.scale);
                projectile.rotation = projectile.rotation - (float)Math.PI / 30f;
                if (projectile.alpha >= 255)
                    projectile.Kill();
                for (int index = 0; index < 2; ++index)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * projectile.scale;
                            Dust dust1 = Main.dust[Dust.NewDust(projectile.Center - spinningpoint1 * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                            dust1.noGravity = true;
                            dust1.position = projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                            dust1.velocity = spinningpoint1.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                            dust1.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                            dust1.scale = 0.5f + Main.rand.NextFloat();
                            dust1.fadeIn = 0.5f;
                            dust1.customData = projectile.Center;
                            break;
                        case 1:
                            Vector2 spinningpoint2 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * projectile.scale;
                            Dust dust2 = Main.dust[Dust.NewDust(projectile.Center - spinningpoint2 * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                            dust2.noGravity = true;
                            dust2.position = projectile.Center - spinningpoint2 * 30f;
                            dust2.velocity = spinningpoint2.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                            dust2.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                            dust2.scale = 0.5f + Main.rand.NextFloat();
                            dust2.fadeIn = 0.5f;
                            dust2.customData = projectile.Center;
                            break;
                    }
                }
            }
        }

        private void TargetEnemies()
        {
            float maxDistance = 1000f;
            int possibleTarget = -1;
            for (int i = 0; i < 200; i++)
            {
                Player player = Main.player[i];
                if (player.active && Collision.CanHitLine(projectile.Center, 0, 0, player.Center, 0, 0))
                {
                    float Distance = projectile.Distance(player.Center);
                    if (Distance < maxDistance)
                    {
                        maxDistance = Distance;
                        possibleTarget = i;
                    }
                }
            }
            projectile.ai[1] = possibleTarget;
            projectile.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(DrawColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}