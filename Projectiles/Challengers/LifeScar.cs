using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeScar : ModProjectile
	{
		bool init = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Mine");
            Main.projFrames[Projectile.type] = 3;
        }
		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
			Projectile.Opacity = 0f;
		}

		public override void AI()
		{
			if (!init)
			{
				Projectile.rotation = MathHelper.ToRadians(Main.rand.Next(360));
				init = true;
			}
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }

            Projectile.rotation += 0.5f;

            if (++Projectile.localAI[0] <= 60f)
            {
                Projectile.Opacity += 3 / 60f;
            }

            if (++Projectile.ai[0] >= Projectile.ai[1] - 30)
			{
				Projectile.Opacity -= 1 / 60f;
			}
			if (Projectile.ai[0] >= Projectile.ai[1])
			{
				Projectile.damage = 0;
			}
            if (Projectile.ai[0] > Projectile.ai[1] + 30)
			{
				Projectile.Kill();
			}
		}
	}
}
