using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class SwordBeamHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_116";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sword Beam");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SwordBeam);
            AIType = ProjectileID.SwordBeam;
            Projectile.DamageType = DamageClass.Default;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.light = 0f;
        }
    }
}