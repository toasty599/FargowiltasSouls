using FargowiltasSouls.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AdamantiteEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Enchantment");
            Tooltip.SetDefault("One of your projectiles will split into 3 every second" +
                "\n'Three degrees of seperation'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "精金魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "每秒会随机使你的一个弹幕分裂成三个" +
                "\n'一气化三清！'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
                itemNameLine.overrideColor = new Color(221, 85, 125);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().AdamantiteEnchant = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyAdamHead")
            .AddIngredient(ItemID.AdamantiteBreastplate)
            .AddIngredient(ItemID.AdamantiteLeggings)
            // Adamantite sword
            .AddIngredient(ItemID.AdamantiteGlaive)
            // Trident
            .AddIngredient(ItemID.TitaniumTrident)
            // Seedler
            .AddIngredient(ItemID.CrystalSerpent)
            //recipe.AddIngredient(ItemID.VenomStaff);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
