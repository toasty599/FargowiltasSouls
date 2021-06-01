using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexBolt : ModProjectile
    {
		public override string Texture => "Terraria/Projectile_255";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vortex Bolt");
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.MagnetSphereBolt);
			aiType = ProjectileID.MagnetSphereBolt;

			projectile.extraUpdates = 100;
			projectile.timeLeft = 300;
		}
	}
}
