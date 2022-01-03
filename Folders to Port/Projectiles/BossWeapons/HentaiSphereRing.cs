using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSphereRing : MutantBoss.MutantSphereRing
    {
        public override string Texture => "Terraria/Projectile_454";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            if (projectile.timeLeft % projectile.MaxUpdates == 0)
                projectile.position += Main.player[projectile.owner].position - Main.player[projectile.owner].oldPosition;

            if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<HentaiSpearWand>()] < 1)
            {
                projectile.Kill();
                return;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
            target.immune[projectile.owner] = 1;
        }
    }
}