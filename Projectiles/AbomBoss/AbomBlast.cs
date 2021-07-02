using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomBlast : BossWeapons.PhantasmalBlast
    {
        public override string Texture => "FargowiltasSouls/Projectiles/AbomBoss/AbomBlast";

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibble>(), 300);
            target.immune[projectile.owner] = 3;
        }
    }
}

