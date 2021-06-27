using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpazmarang : MutantRetirang
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/Spazmarang";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Spazmarang");
        }

        public override void PostAI()
        {
            int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            if (FargoSoulsWorld.MasochistMode)
                target.AddBuff(mod.BuffType("MutantFang"), 180);
        }
    }
}