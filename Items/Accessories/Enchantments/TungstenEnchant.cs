using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TungstenEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tungsten Enchantment");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钨魔石");
            
            string tooltip =
@"150% increased sword size
Every half second a projectile will be doubled in size
Enlarged swords and projectiles deal 10% more damage and have an additional chance to crit
'Bigger is always better'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"增加150%剑的尺寸
每过0.5秒便会使一个弹幕的尺寸翻倍
尺寸变大的剑和弹幕会额外造成10%伤害并且有额外几率暴击
'大就是好'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(176, 210, 178);
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
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().TungstenEnchant = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TungstenHelmet)
            .AddIngredient(ItemID.TungstenChainmail)
            .AddIngredient(ItemID.TungstenGreaves)
            //tungsten sword
            //ruler
            .AddIngredient(ItemID.CandyCaneSword)
            .AddIngredient(ItemID.GreenPhaseblade)
            .AddIngredient(ItemID.EmeraldStaff)
            //.AddIngredient(ItemID.Snail);
            //.AddIngredient(ItemID.Sluggy);

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
