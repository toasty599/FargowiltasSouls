using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PalmWoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Palm Wood Enchantment");
            Tooltip.SetDefault(
@"Double tap down to spawn a palm tree sentry that throws nuts at enemies
'Alarmingly calm'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "棕榈木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"双击'下'键会召唤一个会向敌人扔橡子的棕榈树哨兵
'出奇的宁静'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(183, 141, 86);
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
            //player.GetModPlayer<FargoSoulsPlayer>().PalmEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.PalmWoodHelmet)
            .AddIngredient(ItemID.PalmWoodBreastplate)
            .AddIngredient(ItemID.PalmWoodGreaves)
            .AddIngredient(ItemID.PalmWoodSword)
            .AddIngredient(ItemID.BreathingReed)
            //.AddIngredient(ItemID.BlackInk);
            //coconut
            //seagull
            .AddIngredient(ItemID.Tuna)
            //shark pet

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
