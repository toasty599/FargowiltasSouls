using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class ShroomiteShroom : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_131";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroom");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Mushroom);
            AIType = ProjectileID.Mushroom;
            projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;

            projectile.melee = false;
            Projectile.DamageType = DamageClass.Ranged;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 20;
            projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            //dies thrice as fast
            projectile.alpha += 8;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }
    }
}