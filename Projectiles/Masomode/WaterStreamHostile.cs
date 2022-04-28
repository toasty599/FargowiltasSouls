using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class WaterStreamHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_22";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Stream");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterStream);
            AIType = ProjectileID.WaterStream;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.light = 0f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 600);
        }
    }
}