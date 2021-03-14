namespace FargowiltasSouls.Projectiles.ChallengerItems
{
    public class TheLightning : LightningArc
    {
        public override string Texture => "Terraria/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.tileCollide = false;
            projectile.ranged = false;
            projectile.melee = true;

            projectile.usesIDStaticNPCImmunity = false;
            projectile.idStaticNPCHitCooldown = 0;
        }
    }
}