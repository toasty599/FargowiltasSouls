using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShadewoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadewood Enchantment");
            Tooltip.SetDefault(
@"You have an aura of Bleeding
Enemies struck while Bleeding spew damaging blood
'Surprisingly clean'");
            DisplayName.AddTranslation(GameCulture.Chinese, "阴影木魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"'出奇的干净'
受到伤害时会鲜血四溅
在血腥地形时,被击中会使敌人造成大出血");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(88, 104, 118);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Green;
            item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().ShadewoodEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.ShadewoodHelmet);
            recipe.AddIngredient(ItemID.ShadewoodBreastplate);
            recipe.AddIngredient(ItemID.ShadewoodGreaves);
            recipe.AddIngredient(ItemID.ShadewoodSword);
            //shadewood bow
            //deathweed
            recipe.AddIngredient(ItemID.ViciousMushroom);
            //blood orange/rambuttan
            recipe.AddIngredient(ItemID.CrimsonTigerfish);
            recipe.AddIngredient(ItemID.DeadlandComesAlive);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}