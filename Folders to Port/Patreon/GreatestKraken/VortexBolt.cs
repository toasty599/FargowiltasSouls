using Terraria.ID;

namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexBolt : Projectiles.LightningArc
    {
		public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.CultistIsResistantTo[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.ranged = false;
            Projectile.DamageType = DamageClass.Magic;

            projectile.usesIDStaticNPCImmunity = false;
            projectile.idStaticNPCHitCooldown = 0;
            projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = false;

            projectile.timeLeft = 30 * (projectile.extraUpdates + 1);
        }
    }
}
