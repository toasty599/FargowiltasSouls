using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RichMahoganyEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rich Mahogany Enchantment");
            Tooltip.SetDefault(
@"All grappling hooks shoot, pull, and retract 1.5x as fast
'Guaranteed to keep you hooked'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "红木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"所有钩爪的抛出速度、牵引速度和回收速度x1.5
'保证钩到你'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(181, 108, 100);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().MahoganyEnchant = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.RichMahoganyHelmet)
            .AddIngredient(ItemID.RichMahoganyBreastplate)
            .AddIngredient(ItemID.RichMahoganyGreaves)
            .AddIngredient(ItemID.GrapplingHook)
            .AddIngredient(ItemID.Moonglow)
            .AddIngredient(ItemID.Pineapple)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
