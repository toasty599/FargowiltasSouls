using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using System;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class WoodEnchant : BaseEnchant
    {
        protected override Color nameColor => new Color(151, 107, 75);

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Wood Enchantment");
            Tooltip.SetDefault(
@"Bestiary entries complete twice as fast
You gain a shop discount based on bestiary completion");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"将某些动物转化为武器
右键进行攻击
'卑微的开始...'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            base.SafeModifyTooltips(list);

            double discount = (Main.GetBestiaryProgressReport().CompletionPercent / 2);
            discount *= 100;
            discount = Math.Round(discount, 2);
            list.Add(new TooltipLine(Mod, "Discount", "Current discount: " + discount + "%"));
            list.Add(new TooltipLine(Mod, "Flavor", "'Humble beginnings…'"));
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            WoodEffect(player);
        }

        public static void WoodEffect(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantActive = true;
        }

        public static void WoodDiscount(Chest shop)
        {
            BestiaryUnlockProgressReport bestiaryProgressReport = Main.GetBestiaryProgressReport();
            float discount = 1 - (bestiaryProgressReport.CompletionPercent / 2); //50% discount at 100% bestiary

            for (int i = 0; i < 40; i++)
            {
                int? originalPrice = shop.item[i].shopCustomPrice;

                if (originalPrice == null)
                {
                    originalPrice = shop.item[i].value;
                }

                shop.item[i].shopCustomPrice = (int)((float)originalPrice * discount);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.WoodHelmet)
            .AddIngredient(ItemID.WoodBreastplate)
            .AddIngredient(ItemID.WoodGreaves)
            .AddIngredient(ItemID.Daybloom)
            .AddIngredient(ItemID.Bunny) 
            .AddRecipeGroup("FargowiltasSouls:AnySquirrel") 

            .AddTile(TileID.DemonAltar)
            .Register();
            
        }
    }
}
