using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ForbiddenEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Enchantment");

            string tooltip =
@"Double tap down to call an ancient storm to the cursor location
Any projectiles shot through your storm gain 30% damage
'Walk like an Egyptian'";
            string tooltip_ch =
@"'走路像个埃及人Z(￣ｰ￣)Z'
双击'下'键可召唤一个远古风暴到光标位置
任何穿过风暴的抛射物获得额外50%伤害";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "禁忌魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(231, 178, 28);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Pink;
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().ForbiddenEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorHat);
            recipe.AddIngredient(ItemID.AncientBattleArmorShirt);
            recipe.AddIngredient(ItemID.AncientBattleArmorPants);
            //sun mask/moon mask
            recipe.AddIngredient(ItemID.DjinnsCurse);
            recipe.AddIngredient(ItemID.SpiritFlame);
            recipe.AddIngredient(ItemID.SkyFracture);
            //sky fracture
            //recipe.AddIngredient(ItemID.RainbowRod);

            //recipe.AddRecipeGroup("FargowiltasSouls:AnyScorpion");
            //fennec fox pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}