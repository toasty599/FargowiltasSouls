using FargowiltasSouls.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    class DevRocket : ModProjectile
    {
		private bool split = true;

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
			if (projectile.localAI[1] == 0)
			{
				Projectile[] projs = FargoGlobalProjectile.XWay(Main.rand.Next(7, 10), projectile.Center, projectile.type, 10, projectile.damage / 2, projectile.knockBack);

				foreach (Projectile proj in projs)
				{
					proj.localAI[1] = 1;
				}
			}


			
        }


    }
}
