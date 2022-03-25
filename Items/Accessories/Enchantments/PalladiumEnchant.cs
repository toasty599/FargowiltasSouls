using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PalladiumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Palladium Enchantment");
            Tooltip.SetDefault(
@"Briefly increases life regeneration after striking an enemy
You spawn an orb of damaging life energy every 80 life regenerated
'You feel your wounds slowly healing' ");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钯金魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"攻击敌人后暂时增加你的生命恢复速度
你每恢复80点生命值便会生成一个伤害性的生命能量球
'你感到你的伤口在慢慢愈合'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(245, 172, 40);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().PalladiumEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyPallaHead")
            .AddIngredient(ItemID.PalladiumBreastplate)
            .AddIngredient(ItemID.PalladiumLeggings)
            .AddIngredient(ItemID.PalladiumSword)
            .AddIngredient(ItemID.SoulDrain)
            //sanguine staff
            //.AddIngredient(ItemID.VampireKnives);
            .AddIngredient(ItemID.UndergroundReward)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
