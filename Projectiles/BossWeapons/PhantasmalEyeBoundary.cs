using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PhantasmalEyeBoundary : PhantasmalEyeHoming
    {
        public override string Texture => "Terraria/Projectile_452";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.melee = false;
            projectile.ranged = true;
            projectile.timeLeft = 180;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (projectile.timeLeft % projectile.MaxUpdates == 0)
                projectile.position += Main.player[projectile.owner].position - Main.player[projectile.owner].oldPosition;

            projectile.rotation = projectile.velocity.ToRotation() + 1.570796f;

            if (projectile.localAI[0] < ProjectileID.Sets.TrailCacheLength[projectile.type])
                projectile.localAI[0] += 0.1f;
            else
                projectile.localAI[0] = ProjectileID.Sets.TrailCacheLength[projectile.type];

            projectile.localAI[1] += 0.25f;
        }
    }
}