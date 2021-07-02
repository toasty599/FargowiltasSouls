using Terraria;

namespace FargowiltasSouls.Projectiles
{
    public class StyxGazerArmor : BossWeapons.StyxGazer
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/StyxGazer";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 1;
        }

        public override void AI()
        {
            projectile.damage = Main.player[projectile.owner].GetModPlayer<FargoPlayer>().HighestDamageTypeScaling(444);

            base.AI();

            Main.player[projectile.owner].itemTime = 0;
            Main.player[projectile.owner].itemAnimation = 0;
            Main.player[projectile.owner].reuseDelay = 17;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            projectile.penetrate = -1;
        }
    }
}