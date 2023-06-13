using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SniperBulletShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("High Velocity Crystal Bullet");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.CrystalShard);
            AIType = ProjectileID.CrystalShard;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Defenseless>(), 1800);

            /*int buffTime = 300;
            target.AddBuff(ModContent.BuffType<Crippled>(), buffTime);
            target.AddBuff(ModContent.BuffType<ClippedWings>(), buffTime);*/
        }
    }
}