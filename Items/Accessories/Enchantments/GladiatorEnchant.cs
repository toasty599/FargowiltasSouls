using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class GladiatorEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator Enchantment");
            Tooltip.SetDefault(
@"Spears will rain down on struck enemies
'Are you not entertained?'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "角斗士魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"长矛将倾泄在被攻击的敌人身上
'难道你不高兴吗？'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(156, 146, 78);
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
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().GladiatorEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.GladiatorHelmet)
            .AddIngredient(ItemID.GladiatorBreastplate)
            .AddIngredient(ItemID.GladiatorLeggings)
            .AddIngredient(ItemID.Spear) //gladius
            .AddIngredient(ItemID.Javelin, 300)
            .AddIngredient(ItemID.BoneJavelin, 300)

            //.AddIngredient(ItemID.TartarSauce);

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
