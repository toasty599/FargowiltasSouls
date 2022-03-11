using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Items.Misc;

namespace FargowiltasSouls.Items.Weapons.Misc
{
    public class TophatSquirrelWeapon : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Top Hat Squirrel");
            Tooltip.SetDefault("'Who knew this squirrel had phenomenal cosmic power?'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "高顶礼帽松鼠");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'谁能知道,这只松鼠竟然有着非凡的宇宙力量呢?'");
        }

        public override void SetDefaults()
        {
            item.damage = 22;

            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Yellow;
            item.useAnimation = 45;
            item.useTime = 45;

            Item.DamageType = DamageClass.Magic;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.Swing;
            item.knockBack = 6.6f;

            item.mana = 66;

            item.autoReuse = true;

            item.shoot = ModContent.ProjectileType<TopHatSquirrelProj>();
            item.shootSpeed = 8f;

            item.value = Item.sellPrice(0, 20);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<TopHatSquirrelCaught>(), 10)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddIngredient(ItemID.SoulofFright, 3)
            .AddIngredient(ItemID.SoulofSight, 3)
            .AddIngredient(ItemID.SoulofMight, 3)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddIngredient(ItemID.SoulofNight, 3)
            .AddTile(TileID.MythrilAnvil)

            
            .Register();
        }
    }
}
