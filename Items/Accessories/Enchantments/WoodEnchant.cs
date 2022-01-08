using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class WoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wood Enchantment");
            Tooltip.SetDefault(
@"Turns certain critters into weapons
Right click with them to attack
'Humble beginnings…'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"将某些动物转化为武器
右键进行攻击
'卑微的开始...'");
            //Certain critters have extra effects
            //Effects of Critter guide tm
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(151, 107, 75);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().WoodEnchant = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.WoodHelmet)
            .AddIngredient(ItemID.WoodBreastplate)
            .AddIngredient(ItemID.WoodGreaves)
            .AddIngredient(ItemID.Daybloom)
            .AddIngredient(ItemID.Bunny) //guide to critter companionship
            .AddRecipeGroup("FargowiltasSouls:AnySquirrel") //squirrel hook
            //recipe.AddRecipeGroup("FargowiltasSouls:AnyBird");

            .AddTile(TileID.DemonAltar)
            .Register();
            
        }
    }
}
