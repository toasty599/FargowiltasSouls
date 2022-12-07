using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeSplittingProjSmall : LifeProjSmall
	{
        public override void AI()
		{
            if (Projectile.ai[0] == 45f)
			{
				int damage = Projectile.damage;
				float knockBack = 3f;
				Vector2 shootoffset1 = Projectile.velocity.RotatedBy(-Math.PI / 3.0);
				Vector2 shootoffset2 = Projectile.velocity.RotatedBy(Math.PI / 3.0);
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset1, ModContent.ProjectileType<LifeProjSmall>(), damage, knockBack, Main.myPlayer);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset2, ModContent.ProjectileType<LifeProjSmall>(), damage, knockBack, Main.myPlayer);
				}
			}

            base.AI();
		}
    }
}
