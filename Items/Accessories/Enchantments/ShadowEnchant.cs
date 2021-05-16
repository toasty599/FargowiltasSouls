using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShadowEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Enchantment");
            Tooltip.SetDefault(
@"Two Shadow Orbs will orbit around you
Attacking a Shadow Orb will cause it to release a burst of homing shadow energy
'You feel your body slip into the deepest of shadows'");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"'你感觉身体陷入了最深的阴影中'
攻击概率造成黑暗
陷入黑暗的敌人偶尔会向其他敌人发射暗影烈焰触手
召唤一只噬魂者宝宝和阴影珍珠");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(66, 53, 111);
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
            player.GetModPlayer<FargoPlayer>().ShadowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ShadowHelmet);
            recipe.AddIngredient(ItemID.ShadowScalemail);
            recipe.AddIngredient(ItemID.ShadowGreaves);
            recipe.AddIngredient(ItemID.Musket);
            recipe.AddIngredient(ItemID.WarAxeoftheNight);
            recipe.AddIngredient(ItemID.ShadowOrb);

            //ball o hurt
            //demon bow
            //recipe.AddIngredient(ItemID.PurpleClubberfish);
            //fisher of souls
            //recipe.AddIngredient(ItemID.EatersBone);
            

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}