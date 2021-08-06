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
            DisplayName.SetDefault("Spazmarang");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            if (FargoSoulsWorld.MasochistMode)
                target.AddBuff(mod.BuffType("MutantFang"), 180);
        }
    }
}