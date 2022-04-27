using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class IceSickleHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_263";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Sickle");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.IceSickle);
            AIType = ProjectileID.IceSickle;
            Projectile.DamageType = DamageClass.Default;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.light = 0f;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, .05f, .35f, .5f);
        }

        public override void Kill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int i = 0; i < 15; ++i)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 92, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(0, 101), new Color(), 1 + Main.rand.Next(40) * 0.01f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.timeLeft < 255 ? Projectile.timeLeft : 255);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            target.AddBuff(BuffID.Chilled, 120);
        }
    }
}