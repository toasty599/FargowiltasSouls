using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Weapons.Misc
{
    public class Mahoguny : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mahoguny");
            Tooltip.SetDefault("Uses acorns as ammo\n" +
            "Fires leaves and acorns");
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            item.width = 58;
            item.height = 26;
            item.useTime = 33;
            item.useAnimation = 33;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.sellPrice(0, 8);
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item61;
            item.autoReuse = true;
            item.shoot = item.shoot = ModContent.ProjectileType<AcornProjectile>();
            item.shootSpeed = 18f;
            item.useAmmo = ItemID.Acorn;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 46f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            {
                //item.damage = 80;
                int numberProjectiles = 2 + Main.rand.Next(2);
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(30));
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<MahogunyLeafProjectile>(), (int)(damage * 0.6), knockBack, player.whoAmI);
                }
            }
            speedY -= 4f;

            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, -3);
        }
    }
}