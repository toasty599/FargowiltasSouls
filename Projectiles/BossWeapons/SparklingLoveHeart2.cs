using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SparklingLoveHeart2 : SparklingLoveHeart
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.melee = false;
            projectile.minion = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Lovestruck, 300);
            target.immune[projectile.owner] = 1;

            /*if (projectile.owner == Main.myPlayer)
            {
                int healAmount = 2;
                Main.player[Main.myPlayer].HealEffect(healAmount);
                Main.player[Main.myPlayer].statLife += healAmount;

                if (Main.player[Main.myPlayer].statLife > Main.player[Main.myPlayer].statLifeMax2)
                    Main.player[Main.myPlayer].statLife = Main.player[Main.myPlayer].statLifeMax2;
            }*/
        }
    }
}