using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrimsonEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Enchantment");
            Tooltip.SetDefault(
@"After taking a hit, regen is greatly increased until the half the hit is healed off
If you take another hit before it's healed, you lose the heal in addition to normal damage
'The blood of your enemy is your rebirth'");
            DisplayName.AddTranslation(GameCulture.Chinese, "血腥魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"'你从敌人的血中重生'
大幅度增加生命回复速度
召唤巨脸怪宝宝和血腥心脏");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(200, 54, 75);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Orange;
            item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().CrimsonEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimsonHelmet);
            recipe.AddIngredient(ItemID.CrimsonScalemail);
            recipe.AddIngredient(ItemID.CrimsonGreaves);
            //blood axe tging
            recipe.AddIngredient(ItemID.TheUndertaker);
            recipe.AddIngredient(ItemID.TheMeatball);
            recipe.AddIngredient(ItemID.CrimsonHeart);

            //blood rain bow
            //flesh catcher rod
            //recipe.AddIngredient(ItemID.BoneRattle);
            //recipe.AddIngredient(ItemID.CrimsonHeart);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}