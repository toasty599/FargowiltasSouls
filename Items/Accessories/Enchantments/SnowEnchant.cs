using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SnowEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snow Enchantment");
            Tooltip.SetDefault(
@"Spawns a snowstorm at your cursor
Any projectiles or npcs in the snowstorm are slowed by 50%
'It's Burning Cold Outside'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰雪魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"在光标位置生成暴雪
暴雪中的所有弹幕和NPC的速度减缓50%（NPC包括敌人）
'冷的要死'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(37, 195, 242);
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
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().SnowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EskimoHood)
            .AddIngredient(ItemID.EskimoCoat)
            .AddIngredient(ItemID.EskimoPants)
            //hand warmer
            //fruitcake chakram
            .AddIngredient(ItemID.IceBoomerang)
            //frost daggerfish
            .AddIngredient(ItemID.FrostMinnow)
            .AddIngredient(ItemID.AtlanticCod)
            //.AddIngredient(ItemID.Fish); 

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
