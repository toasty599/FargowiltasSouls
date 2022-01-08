using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TitaniumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Enchantment");
            Tooltip.SetDefault(
@"Briefly become invulnerable after striking an enemy
'Hit me with your best shot'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钛金魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"攻击敌人后会使你无敌一小段时间
'Hit me with your best shot'（某歌曲名）");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(130, 140, 136);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().TitaniumEffect(); //new set bonus soon
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyTitaHead")
            .AddIngredient(ItemID.TitaniumBreastplate)
            .AddIngredient(ItemID.TitaniumLeggings)
            //recipe.AddIngredient(ItemID.TitaniumDrill);
            .AddIngredient(ItemID.TitaniumSword)
            .AddIngredient(ItemID.Rockfish)
            .AddIngredient(ItemID.WhitePhasesaber)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
