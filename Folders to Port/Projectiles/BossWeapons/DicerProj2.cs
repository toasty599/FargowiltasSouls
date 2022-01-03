using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class DicerProj2 : ModProjectile
    {
        public int Counter = 1;

        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 19;
            projectile.height = 19;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1f;
            projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (Counter >= 75)
            {
                projectile.scale += .1f;
                projectile.rotation += 0.2f;
            }

            Counter++;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int proj2 = mod.ProjectileType("DicerSpray"); //374;
                Vector2 baseVel = Main.rand.NextBool() ? Vector2.UnitX : Vector2.UnitX.RotatedBy(Math.PI * 2 / 8 * 0.5);
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(projectile.Center, 5f * baseVel.RotatedBy(Math.PI * 2 / 8 * i), proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
            }
        }
    }
}