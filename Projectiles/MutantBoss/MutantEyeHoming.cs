using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantEyeHoming : MutantEye
    {
        public override string Texture => "Terraria/Images/Projectile_452";

        public override void AI()
        {
            if (--Projectile.ai[1] < 0 && Projectile.ai[1] > -60)
            {
                Player p = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
                if (p != null)
                {
                    
                    Vector2 target = p.Center;

                    if (Math.Abs(p.Center.Y - Projectile.Center.Y) > 250)
                    {
                        Vector2 distance = target - Projectile.Center;

                        double angle = distance.ToRotation() - Projectile.velocity.ToRotation();
                        if (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        if (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        Projectile.velocity = Projectile.velocity.RotatedBy(angle * 0.2);
                    }
                    else
                    {
                        Projectile.ai[1] = -60;
                    }
                }
                else
                {
                    Projectile.ai[0] = Player.FindClosest(Projectile.Center, 0, 0);
                }
            }

            if (Projectile.ai[1] < 0)
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * MathHelper.Lerp(Projectile.velocity.Length(), 10f, 0.035f);

            base.AI();
        }
    }
}