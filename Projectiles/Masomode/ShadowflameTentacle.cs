using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class ShadowflameTentacle : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_496";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Tentacle");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 91;
            aiType = ProjectileID.ShadowFlame;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.MaxUpdates = 3;
            projectile.penetrate = 2;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            if (projectile.owner == Main.myPlayer)
                Main.player[projectile.owner].GetModPlayer<FargoPlayer>().WretchedPouchCD += 8;
        }
    }
}