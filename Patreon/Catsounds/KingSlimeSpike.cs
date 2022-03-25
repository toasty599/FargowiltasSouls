using Terraria.ID;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class KingSlimeSpike : Projectiles.BossWeapons.SlimeSpikeFriendly
    {
        public override string Texture => "Terraria/Images/Projectile_605";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 300;
            Projectile.DamageType = Terraria.ModLoader.DamageClass.Summon;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }
    }
}