using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DeathSickleHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_274";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Sickle");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DeathSickle);
            AIType = ProjectileID.DeathSickle;
            Projectile.DamageType = DamageClass.Default;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.light = 0f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.timeLeft < 255 ? Projectile.timeLeft : 255);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
        }
    }
}