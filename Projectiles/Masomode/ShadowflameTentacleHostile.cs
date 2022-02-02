using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class ShadowflameTentacleHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_496";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Tentacle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 91;
            AIType = ProjectileID.ShadowFlame;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 3;
            Projectile.penetrate = 3;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.Shadowflame>(), 300);
        }
    }
}