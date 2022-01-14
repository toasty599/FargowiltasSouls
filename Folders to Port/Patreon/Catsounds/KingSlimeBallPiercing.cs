using Terraria.ID;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class KingSlimeBallPiercing : Projectiles.BossWeapons.SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.CultistIsResistantTo[projectile.type] = true;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.penetrate = 2;
            projectile.melee = false;
            projectile.minion = true;
        }
    }
}