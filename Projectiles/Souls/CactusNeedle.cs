using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class CactusNeedle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cactus Needle");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PineNeedleFriendly);
            Projectile.aiStyle = 336;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            CooldownSlot = 1;
        }

		public override void AI()
		{
			Projectile.ai[0] += 1f;

			if (Projectile.ai[0] >= 50f)
			{
				Projectile.ai[0] = 50f;
				Projectile.velocity.Y += 0.5f;
			}
			if (Projectile.ai[0] >= 15f)
			{
				Projectile.ai[0] = 15f;
				Projectile.velocity.Y += 0.1f;
			}

			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;

			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}
		}
	}
}
