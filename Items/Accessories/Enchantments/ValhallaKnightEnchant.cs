using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ValhallaKnightEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valhalla Knight Enchantment");
            Tooltip.SetDefault(
@"Increases the effectiveness of healing sources by 33%
Greatly enhances Ballista effectiveness
'Valhalla calls'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "英灵殿骑士魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"增加33%受治疗量
大幅强化弩车的效果
'瓦尔哈拉的呼唤'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(147, 101, 30);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().ValhallaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireAltHead)
            .AddIngredient(ItemID.SquireAltShirt)
            .AddIngredient(ItemID.SquireAltPants)
            .AddIngredient(ItemID.VikingHelmet)
            .AddIngredient(null, "SquireEnchant")
            //recipe.AddIngredient(ItemID.ShinyStone);
            //starlight
            //shadow lance
            .AddIngredient(ItemID.DD2SquireBetsySword)
            //recipe.AddIngredient(ItemID.DD2PetDragon);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
