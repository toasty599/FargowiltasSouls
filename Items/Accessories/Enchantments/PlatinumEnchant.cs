using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PlatinumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Platinum Enchantment");
            string tooltip =
@"Increases luck by 10
'Its value is immeasurable'";
            Tooltip.SetDefault(tooltip);

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铂金魔石");
            string tooltip_ch = 
@"敌人死亡时掉落的战利品有20%几率翻倍
'价值无法估量'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(83, 103, 143);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            player.luckMaximumCap = 10;
            player.luck = 10;
            //modPlayer.PlatinumEnchant = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumHelmet)
                .AddIngredient(ItemID.PlatinumChainmail)
                .AddIngredient(ItemID.PlatinumGreaves)
                .AddIngredient(ItemID.GardenGnome)
                .AddIngredient(ItemID.GemSquirrelDiamond)
                .AddIngredient(ItemID.LadyBug)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
