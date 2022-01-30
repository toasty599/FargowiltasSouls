using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantEyeHoming : MutantEye
    {
        public override string Texture => "Terraria/Images/Projectile_452";

        public override void AI()
        {
            if (--projectile.ai[1] < 0 && projectile.ai[1] > -60)
            {
                Player p = FargoSoulsUtil.PlayerExists(projectile.ai[0]);
                if (p != null)
                {
                    
                    Vector2 target = p.Center;

                    if (Math.Abs(p.Center.Y - projectile.Center.Y) > 250)
                    {
                        Vector2 distance = target - projectile.Center;

                        double angle = distance.ToRotation() - projectile.velocity.ToRotation();
                        if (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        if (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        projectile.velocity = projectile.velocity.RotatedBy(angle * 0.2);
                    }
                    else
                    {
                        projectile.ai[1] = -60;
                    }
                }
                else
                {
                    projectile.ai[0] = Player.FindClosest(projectile.Center, 0, 0);
                }
            }

            if (projectile.ai[1] < 0)
                projectile.velocity = Vector2.Normalize(projectile.velocity) * MathHelper.Lerp(projectile.velocity.Length(), 10f, 0.035f);

            base.AI();
        }
    }
}