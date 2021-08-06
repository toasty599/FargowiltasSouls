using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpearDash : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.3f;
            projectile.alpha = 0;
            cooldownSlot = 1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToMutantBomb = true;
        }

        float scaletimer;
        public override void AI()
        {
            if (projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 1f;
                Main.PlaySound(SoundID.Item1, projectile.Center);
            }

            NPC mutant = Main.npc[(int)projectile.ai[0]];
            if (mutant.active && mutant.type == mod.NPCType("MutantBoss") && (mutant.ai[0] == 6 || mutant.ai[0] == 15 || mutant.ai[0] == 23))
            {
                projectile.velocity = Vector2.Normalize(mutant.velocity);
                projectile.position -= projectile.velocity;
                projectile.rotation = mutant.velocity.ToRotation() + MathHelper.ToRadians(135f);
                projectile.Center = mutant.Center + mutant.velocity;
                if (projectile.ai[1] <= 0f && --projectile.localAI[0] < 0)
                {
                    projectile.localAI[0] = projectile.ai[1] < 0f ? 1 : 2;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (projectile.ai[1] == -2)
                        {
                            for (int i = -1; i <= 1; i += 2)
                            {
                                int p = Projectile.NewProjectile(projectile.Center, 16f * Vector2.Normalize(mutant.velocity).RotatedBy(MathHelper.PiOver2 * i),
                                  ModContent.ProjectileType<MutantSphereSmall>(), projectile.damage, 0f, projectile.owner, -1);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 15;
                            }
                        }
                        else
                        {
                            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MutantSphereSmall>(), projectile.damage, 0f, projectile.owner, mutant.target);
                        }
                    }
                }
            }
            else
            {
                projectile.Kill();
            }

            scaletimer++;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, mod.ProjectileType("PhantasmalBlast"), 0, 0f, projectile.owner);
            if (FargoSoulsWorld.MasochistMode)
            {
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                target.AddBuff(mod.BuffType("OceanicMaul"), 5400);
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glow = mod.GetTexture("Projectiles/MutantBoss/MutantEye_Glow");
            int rect1 = glow.Height / Main.projFrames[projectile.type];
            int rect2 = rect1 * projectile.frame;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(51, 255, 191, 0), Color.Transparent, 0.82f);
            Color glowcolor2 = Color.Lerp(new Color(194, 255, 242, 0), Color.Transparent, 0.6f);
            glowcolor = Color.Lerp(glowcolor, glowcolor2, 0.5f + (float)Math.Sin(scaletimer / 7) / 2); //make it shift between the 2 colors
            Vector2 drawCenter = projectile.Center + (projectile.velocity.SafeNormalize(Vector2.UnitX) * 28);

            float rotationModifier = -MathHelper.ToRadians(135f) + MathHelper.PiOver2;

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the projectile
            {
                Vector2 drawCenter2 = drawCenter + (projectile.velocity.SafeNormalize(Vector2.UnitX) * 20).RotatedBy(MathHelper.Pi / 5 - (i * MathHelper.Pi / 5)); //use a normalized version of the projectile's velocity to offset it at different angles
                drawCenter2 -= (projectile.velocity.SafeNormalize(Vector2.UnitX) * 20); //then move it backwards
                float scale = projectile.scale;
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Main.spriteBatch.Draw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, projectile.rotation + rotationModifier, gloworigin2, scale * 1.25f, SpriteEffects.None, 0f);
            }

            for (int i = ProjectileID.Sets.TrailCacheLength[projectile.type] - 1; i > 0; i--) //scaling trail
            {
                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Vector2 value4 = projectile.oldPos[i] - (projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);
                Main.spriteBatch.Draw(glow, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    projectile.oldRot[i] + rotationModifier, gloworigin2, scale * 1.25f, SpriteEffects.None, 0f);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}