using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class LihzahrdSpikyBallFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiky Ball");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SpikyBallTrap);
            AIType = ProjectileID.SpikyBallTrap;
            Projectile.hostile = false;
            Projectile.trap = false;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);
        }
    }
}