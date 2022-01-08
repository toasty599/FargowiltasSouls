using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PearlwoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearlwood Enchantment");
            Tooltip.SetDefault(
@"Projectiles may spawn a star when they hit something
'Too little, too late…'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "珍珠木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"弹幕在击中敌人或物块时有几率生成一颗星星
'既渺小无力，又慢人一步...'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(173, 154, 95);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().PearlEnchant = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.PearlwoodHelmet)
            .AddIngredient(ItemID.PearlwoodBreastplate)
            .AddIngredient(ItemID.PearlwoodGreaves)
            .AddIngredient(ItemID.PearlwoodSword)
            .AddIngredient(ItemID.UnicornonaStick)
            //dragonfruit or starfruit
            //recipe.AddIngredient(ItemID.LightningBug);
            //recipe.AddIngredient(ItemID.Prismite);
            .AddIngredient(ItemID.TheLandofDeceivingLooks)
            //butterfly pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
