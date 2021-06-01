using FargowiltasSouls.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    class DevRocket : ModProjectile
    {
		public override string Texture => "Terraria/Projectile_616";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dev Rocket");
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.VortexBeaterRocket);
			aiType = ProjectileID.VortexBeaterRocket;

			projectile.ranged = false;
			projectile.minion = true;

            projectile.penetrate = 2;

            projectile.timeLeft = 40 * (projectile.extraUpdates + 1);
		}

        public override void AI()
        {
			//Main.NewText(projectile.ai[0] + " " + projectile.ai[1] + " " + projectile.localAI[0] + " " + projectile.localAI[1]);

			if (projectile.localAI[1] == 1)
			{
				projectile.ai[0] = 0;
				projectile.localAI[1] = 2;
			}
        }

        public override bool CanDamage()
        {
			return projectile.ai[0] == 20;
        }

        public override void Kill(int timeLeft)
        {
			if (projectile.owner == Main.myPlayer && projectile.localAI[1] == 0)
			{
				Projectile[] projs = FargoGlobalProjectile.XWay(Main.rand.Next(7, 10), projectile.Center, projectile.type, 7, projectile.damage, projectile.knockBack);

				foreach (Projectile proj in projs)
				{
					proj.localAI[1] = 1;
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.timeLeft = 0;
            target.immune[projectile.owner] = 2;
        }
    }
}
