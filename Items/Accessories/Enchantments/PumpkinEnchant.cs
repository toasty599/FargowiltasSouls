using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PumpkinEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Enchantment");
            Tooltip.SetDefault(
@"You will grow pumpkins while walking on the ground
When fully grown, they will heal 25 HP and spawn damaging embers
Enemies that touch them will destroy them and take damage
'Your sudden pumpkin craving will never be satisfied'");
            DisplayName.AddTranslation(GameCulture.Chinese, "南瓜魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"'你对南瓜的突发渴望永远不会得到满足'
走路时会留下一道火焰路径
南瓜派会使你回满血, 并获得3分钟的抗药性
召唤一个宠物南瓜娃娃");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(227, 101, 28);
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
            player.GetModPlayer<FargoPlayer>().PumpkinEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PumpkinHelmet);
            recipe.AddIngredient(ItemID.PumpkinBreastplate);
            recipe.AddIngredient(ItemID.PumpkinLeggings);
            recipe.AddIngredient(ItemID.MolotovCocktail, 50);
            recipe.AddIngredient(ItemID.Sickle);
            //rotten eggs
            //bat hook
            recipe.AddIngredient(ItemID.PumpkinPie);
            //jack o lantern
            //recipe.AddIngredient(ItemID.MagicalPumpkinSeed);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}