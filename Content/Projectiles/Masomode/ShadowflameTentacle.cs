using FargowiltasSouls.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class ShadowflameTentacle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_496";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Tentacle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 91;
            AIType = ProjectileID.ShadowFlame;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 3;
            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            if (Projectile.owner == Main.myPlayer)
                Main.player[Projectile.owner].GetModPlayer<FargoSoulsPlayer>().WretchedPouchCD += 8;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = Main.player[Projectile.owner].Center.X > target.Center.X ? -1 : 1;
        }
    }
}