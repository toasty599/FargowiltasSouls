using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class BorealWoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boreal Wood Enchantment");
            Tooltip.SetDefault(
@"Attacks will periodically be accompanied by several snowballs
'The cooler wood'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "针叶木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"攻击时定期释放雪球
'冷木'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(139, 116, 100);
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
            //player.GetModPlayer<FargoSoulsPlayer>().BorealEnchant = true;
            //player.GetModPlayer<FargoSoulsPlayer>().AdditionalAttacks = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BorealWoodHelmet)
            .AddIngredient(ItemID.BorealWoodBreastplate)
            .AddIngredient(ItemID.BorealWoodGreaves)
            //.AddIngredient(ItemID.BorealWoodSword);
            //.AddIngredient(ItemID.BorealWoodBow);
            .AddIngredient(ItemID.Snowball, 300)
            .AddIngredient(ItemID.Shiverthorn)
            //cherry/plum
            //.AddIngredient(ItemID.Penguin);
            .AddIngredient(ItemID.ColdWatersintheWhiteLand)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
