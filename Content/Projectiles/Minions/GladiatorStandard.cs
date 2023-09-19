using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class GladiatorStandard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 74;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 60 * 15;
        }

        public override bool? CanDamage()
        {
            return Projectile.velocity != Vector2.Zero; //only while travelling
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            int AuraSize = modPlayer.ForceEffect(ModContent.ItemType<GladiatorEnchant>()) ? 800 : 400;

            FargoSoulsUtil.AuraDust(Projectile, AuraSize, DustID.GoldCoin);
            if (FargoSoulsUtil.ClosestPointInHitbox(Main.LocalPlayer.Hitbox, Projectile.Center).Distance(Projectile.Center) < AuraSize)
            {
                player.AddBuff(ModContent.BuffType<GladiatorBuff>(), 2);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            const int rootAmount = 8;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.position += Projectile.velocity;
            Projectile.position += Vector2.UnitY * rootAmount;
            Projectile.velocity = Vector2.Zero;

            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Projectile.Center + (Vector2.UnitY * Projectile.height / 2), 0, 0, DustID.Gold, Main.rand.NextFloat(-30, 30), -Main.rand.NextFloat(4, 8));
            }
            Projectile.tileCollide = false;
            return false;
        }
    }
}
