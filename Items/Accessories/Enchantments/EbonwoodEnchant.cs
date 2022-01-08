using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class EbonwoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebonwood Enchantment");
            Tooltip.SetDefault(
@"You have an aura of Shadowflame
'Untapped potential'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "乌木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"一圈暗影焰光环环绕着你
'未开发的潜力'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(100, 90, 141);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().EbonEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EbonwoodHelmet)
            .AddIngredient(ItemID.EbonwoodBreastplate)
            .AddIngredient(ItemID.EbonwoodGreaves)
            .AddIngredient(ItemID.EbonwoodSword)
            //recipe.AddIngredient(ItemID.EbonwoodBow);
            //recipe.AddIngredient(ItemID.Deathweed);
            .AddIngredient(ItemID.VileMushroom)
            //elderberry/blackcurrant
            //recipe.AddIngredient(ItemID.Ebonkoi);
            .AddIngredient(ItemID.LightlessChasms)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
