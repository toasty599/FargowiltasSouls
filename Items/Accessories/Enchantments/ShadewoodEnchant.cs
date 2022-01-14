using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "阴影木魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"一圈流血光环环绕着你
在流血光环内被攻击的敌人会喷出伤害性血液
'出奇的干净'");
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
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().ShadewoodEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.ShadewoodHelmet)
            .AddIngredient(ItemID.ShadewoodBreastplate)
            .AddIngredient(ItemID.ShadewoodGreaves)
            .AddIngredient(ItemID.ShadewoodSword)
            //shadewood bow
            //deathweed
            .AddIngredient(ItemID.ViciousMushroom)
            //blood orange/rambuttan
            //.AddIngredient(ItemID.CrimsonTigerfish);
            .AddIngredient(ItemID.DeadlandComesAlive)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
