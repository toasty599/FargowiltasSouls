using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CopperEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Copper Enchantment");

            string tooltip =
@"Attacks have a chance to shock enemies with lightning
'Behold'";
            string tooltip_ch =
@"'注视'
攻击有概率用闪电打击敌人
如果敌人处于潮湿状态,增加概率和伤害
造成潮湿的攻击不能触发闪电";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "铜魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(213, 102, 23);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Orange;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().CopperEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CopperHelmet);
            recipe.AddIngredient(ItemID.CopperChainmail);
            recipe.AddIngredient(ItemID.CopperGreaves);
            recipe.AddIngredient(ItemID.CopperShortsword);
            recipe.AddIngredient(ItemID.AmethystStaff);
            //recipe.AddIngredient(ItemID.PurplePhaseblade);
            //thunder zapper
            //recipe.AddIngredient(ItemID.Wire, 20);
            //daybloom
            recipe.AddIngredient(ItemID.FirstEncounter);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}