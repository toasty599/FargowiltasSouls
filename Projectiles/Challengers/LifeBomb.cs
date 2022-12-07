using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeBomb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Mine");
		}
		public override void SetDefaults()
		{
			Projectile.width = 25;
			Projectile.height = 25;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
		}

		public override bool? CanDamage() => false;

		public override void AI()
		{
			Projectile.ai[0] += 2f;
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 0.25f);
			Projectile.rotation = Projectile.rotation + (float)Math.PI / 16f;
			if (Projectile.ai[0] >= 60f)
			{
				Projectile.velocity = Projectile.velocity * 0.96f;
			}
			if (Projectile.ai[0] >= 100f)
			{
				Projectile.Kill();
			}
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
			int damage = Projectile.damage;
			int knockBack = 3;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2), 0f, 0f, ModContent.ProjectileType<LifeBombExplosion>(), damage, knockBack, Main.myPlayer);
		}
	}
}
