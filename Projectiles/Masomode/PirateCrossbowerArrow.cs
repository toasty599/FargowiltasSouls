using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PirateCrossbowerArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jester's Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.JestersArrow);
            AIType = ProjectileID.JestersArrow;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Default;
            Projectile.arrow = false;
            Projectile.hostile = true;
            Projectile.timeLeft *= 2;
        }

        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                int type = Main.rand.Next(new int[] { 15, 57, 58 });
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 150, default, 0.8f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.Midas>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0);
        }

        public override void Kill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 60; i++)
            {
                int Type;
                switch (Main.rand.Next(3))
                {
                    case 0: Type = 15; break;
                    case 1: Type = 57; break;
                    default: Type = 58; break;
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 150, default(Color), 1.2f);
            }
        }
    }
}