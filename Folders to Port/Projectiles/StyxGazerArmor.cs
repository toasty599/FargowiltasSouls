using Terraria;

namespace FargowiltasSouls.Projectiles
{
    public class StyxGazerArmor : BossWeapons.StyxGazer
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/StyxGazer";

        public override void AI()
        {
            projectile.damage = Main.player[projectile.owner].GetModPlayer<FargoSoulsPlayer>().HighestDamageTypeScaling(666);

            base.AI();

            Main.player[projectile.owner].itemTime = 0;
            Main.player[projectile.owner].itemAnimation = 0;
            if (Main.player[projectile.owner].reuseDelay < 17)
                Main.player[projectile.owner].reuseDelay = 17;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.localNPCImmunity[target.whoAmI] >= 15)
                return false;
            return null;
        }
    }
}