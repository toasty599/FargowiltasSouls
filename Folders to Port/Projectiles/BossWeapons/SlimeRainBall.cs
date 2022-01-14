using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SlimeRainBall : MutantBoss.MutantSlimeBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.hostile = false;
            projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee
            projectile.penetrate = 1;
            cooldownSlot = -1;
        }

        public override void AI()
        {
            projectile.tileCollide = --projectile.ai[1] < 0;
            base.AI();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 240);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, 255, lightColor.A);
        }
    }
}