using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SquireEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squire Enchantment");
            Tooltip.SetDefault(
@"Increases the effectiveness of healing sources by 25%
Ballista pierces more targets and panics when you take damage
'Squire, will you hurry?'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "侍卫魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"增加25%受治疗量
受到伤害后使弩车可以穿透更多的敌人且会造成恐慌减益
'侍卫？你能快点吗？'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(148, 143, 140);
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
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().SquireEnchant = true;
            if (!player.GetToggleValue("SquirePanic"))
                player.buffImmune[BuffID.BallistaPanic] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireGreatHelm)
            .AddIngredient(ItemID.SquirePlating)
            .AddIngredient(ItemID.SquireGreaves)
            //recipe.AddIngredient(ItemID.SquireShield);
            .AddIngredient(ItemID.DD2BallistraTowerT2Popper)
            //rally
            //lance
            .AddIngredient(ItemID.RedPhasesaber)
            .AddIngredient(ItemID.DD2SquireDemonSword)
            //light discs

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
