using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.AbomBoss
{
    public class AbomBlast : BossWeapons.PhantasmalBlast
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/AbomBoss/AbomBlast";

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibbleBuff>(), 300);
            //target.immune[projectile.owner] = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<BossWeapons.StyxGazer>()] > 0 ? 1 : 3;
        }
    }
}

