using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class DicerSpray : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_484";

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.SeedlerThorn);
            aiType = ProjectileID.SeedlerThorn;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
        }
    }
}