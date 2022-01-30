using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Patreon.DemonKing
{
    public class RazorbladeTyphoonFriendly2 : Projectiles.RazorbladeTyphoonFriendly
    {
        public override string Texture => "Terraria/Images/Projectile_409";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.ranged = false;
            projectile.minion = true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.defense > 0)
                damage += target.defense / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 900);
        }
    }
}