using FargowiltasSouls.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Projectiles;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AdamantiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Adamantite Enchantment");
            Tooltip.SetDefault("Every other weapon projectile you spawn will split into 3" +
                "\nPiercing projectiles hit twice as fast" +
                "\nAll projectiles deal 50% damage" +
                "\n'Chaos'");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "精金魔石");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "每秒会随机使你的一个弹幕分裂成三个" +
            //     "\n'一气化三清！'");
        }

        protected override Color nameColor => new Color(221, 85, 125);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AdamantiteEffect(player);
        }

        public static void AdamantiteEffect(Player player)
        {
            FargoSoulsPlayer modplayer = player.GetModPlayer<FargoSoulsPlayer>();
            modplayer.AdamantiteEnchantActive = true;
        }

        public static void AdamantiteSplit(Projectile projectile)
        {
            FargoSoulsGlobalProjectile.SplitProj(projectile, 3, MathHelper.Pi / 16, 1f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyAdamHead")
                .AddIngredient(ItemID.AdamantiteBreastplate)
                .AddIngredient(ItemID.AdamantiteLeggings)
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.QuadBarrelShotgun)
                .AddIngredient(ItemID.DarkLance)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
