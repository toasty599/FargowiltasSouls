using Terraria.ID;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class KingSlimeBallPiercing : Projectiles.BossWeapons.SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 2;
            Projectile.DamageType = Terraria.ModLoader.DamageClass.Summon;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }
    }
}