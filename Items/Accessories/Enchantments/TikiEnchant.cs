using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TikiEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tiki Enchantment");
            Tooltip.SetDefault(
@"You may continue to summon temporary minions and sentries after maxing out on your slots
Reduces attack speed of summon weapons when effect is activated
'Aku Aku!'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "提基魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"在召唤栏用光后你仍可以召唤临时的哨兵和仆从
'Aku Aku!'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(86, 165, 43);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().TikiEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TikiMask)
            .AddIngredient(ItemID.TikiShirt)
            .AddIngredient(ItemID.TikiPants)
            //leaf wings
            .AddIngredient(ItemID.Blowgun)
            //toxic flask
            .AddIngredient(ItemID.PygmyStaff)
            .AddIngredient(ItemID.PirateStaff)
            //kaledoscope
            //.AddIngredient(ItemID.TikiTotem);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
