using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ChlorophyteEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chlorophyte Enchantment");

            string tooltip =
@"Summons a ring of leaf crystals to shoot at nearby enemies
Grants a double spore jump
All herb collection is doubled
Effects of Flower Boots
Summons a pet Seedling
'The jungle's essence crystallizes around you'";

            string tooltip_ch =
@"'丛林的精华凝结在你周围'
召唤一圈叶绿水晶射击附近的敌人
受到伤害时会释放出有毒的孢子爆炸
所有草药收获翻倍
拥有植物纤维绳索指南的效果
召唤一颗宠物幼苗";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "叶绿魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(36, 137, 0);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Lime;
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //crystal and pet
            modPlayer.ChloroEffect(hideVisual);
            modPlayer.FlowerBoots();
            //modPlayer.FlowerBoots();
            modPlayer.JungleEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyChloroHead");
            recipe.AddIngredient(ItemID.ChlorophytePlateMail);
            recipe.AddIngredient(ItemID.ChlorophyteGreaves);
            recipe.AddIngredient(null, "JungleEnchant");
            //recipe.AddIngredient(ItemID.ChlorophyteWarhammer);
            recipe.AddIngredient(ItemID.ChlorophyteClaymore);
            //grape juice
            recipe.AddIngredient(ItemID.Seedling);
            //plantero pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}