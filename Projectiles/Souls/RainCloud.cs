using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class RainCloud : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_238";

        private int timer = 60;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rain Cloud");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RainCloudRaining);
            aiType = ProjectileID.RainCloudRaining;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;

            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (--timer <= 0)
            {
                projectile.velocity = Vector2.Zero;
            }

            //destroy duplicates if they somehow spawn
            if (player.ownedProjectileCounts[projectile.type] > 1
                || (projectile.owner == Main.myPlayer && !player.GetToggleValue("Rain")))
            {
                projectile.Kill();
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active && proj.friendly && !proj.hostile && proj.owner == player.whoAmI && proj.damage > 0 && !proj.minion
                    && proj.type != projectile.type && proj.type != ProjectileID.RainFriendly && proj.whoAmI != Main.player[proj.owner].heldProj
                    && Array.IndexOf(FargoGlobalProjectile.noSplit, projectile.type) <= -1 && proj.type != ModContent.ProjectileType<Chlorofuck>() && proj.Hitbox.Intersects(projectile.Hitbox))
                {
                    if (projectile.scale < 3f)
                    {
                        projectile.scale *= 1.1f;
                        projectile.timeLeft += 60;
                    }
                    else
                    {
                        Vector2 rotationVector2 = (proj.Center + proj.velocity * 25) - projectile.Center;
                        rotationVector2.Normalize();

                        Vector2 vector2_3 = rotationVector2 * 8f;
                        float ai_1 = Main.rand.Next(80);
                        Projectile.NewProjectile(projectile.Center.X + vector2_3.X * 5, projectile.Center.Y + vector2_3.Y * 5, vector2_3.X, vector2_3.Y,
                            mod.ProjectileType("LightningArc"), proj.damage * 2, projectile.knockBack, projectile.owner,
                            rotationVector2.ToRotation(), ai_1);

                    }

                    proj.active = false;

                    break;
                }
            }

            //cancel normal rain
            projectile.ai[0] = 0;

            projectile.localAI[1]++;

            //bigger = more rain
            if (projectile.scale > 3f)
            {
                projectile.localAI[1] += 4;
            }
            else if (projectile.scale > 2f)
            {
                projectile.localAI[1] += 3;
            }
            else if (projectile.scale > 1.5f)
            {
                projectile.localAI[1] += 2;
            }
            else
            {
                projectile.localAI[1]++;
            }

            //do the rain
            if (projectile.localAI[1] >= 8)
            {
                projectile.localAI[1] = 0;

                int num414 = (int)(projectile.Center.X + (float)Main.rand.Next((int)(-20 * projectile.scale), (int)(20 * projectile.scale)));
                int num415 = (int)(projectile.position.Y + (float)projectile.height + 4f);
                int p = Projectile.NewProjectile((float)num414, (float)num415, 0f, 5f, ProjectileID.RainFriendly, projectile.damage / 2, 0f, projectile.owner, 0f, 0f);
                Main.projectile[p].penetrate = 1;
            }
        }
    }
}