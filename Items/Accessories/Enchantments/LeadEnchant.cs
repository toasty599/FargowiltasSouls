using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class LeadEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Enchantment");

            string tooltip =
@"Attacks may inflict enemies with Lead Poisoning
Lead Poisoning deals damage over time and spreads to nearby enemies
'Not recommended for eating'";
            string tooltip_ch =
@"'不建议食用'
攻击概率使敌人铅中毒
铅中毒随时间造成伤害,并传播给附近敌人";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "铅魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(67, 69, 88);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Blue;
            item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().LeadEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LeadHelmet);
            recipe.AddIngredient(ItemID.LeadChainmail);
            recipe.AddIngredient(ItemID.LeadGreaves);
            //recipe.AddIngredient(ItemID.LeadPickaxe);
            //lead axe
            recipe.AddIngredient(ItemID.LeadShortsword);
            //lead bow
            //black paint
            recipe.AddIngredient(ItemID.GrayPaint, 100);
            recipe.AddIngredient(ItemID.SulphurButterfly);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}