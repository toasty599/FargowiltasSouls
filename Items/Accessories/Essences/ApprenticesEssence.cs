using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class ApprenticesEssence : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apprentice's Essence");
            Tooltip.SetDefault(
@"18% increased magic damage
5% increased magic crit
Increases your maximum mana by 50
'This is only the beginning..'");
//            DisplayName.AddTranslation(GameCulture.Chinese, "学徒精华");
//            Tooltip.AddTranslation(GameCulture.Chinese,
//@"增加18%魔法伤害
//增加5%魔法暴击率
//增加50点最大法力值
//'这是个开始...'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(255, 83, 255));
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 150000;
            Item.rare = ItemRarityID.LightRed;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.18f;
            player.GetCritChance(DamageClass.Magic) += 5;
            player.statManaMax2 += 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient(ItemID.ZapinatorGray)
                .AddIngredient(ItemID.Vilethorn)
                .AddIngredient(ItemID.CrimsonRod)
                .AddIngredient(ItemID.WeatherPain)
                .AddIngredient(ItemID.WaterBolt)
                .AddIngredient(ItemID.Flamelash)
                .AddIngredient(ItemID.DemonScythe)
                .AddIngredient(ItemID.SorcererEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            
        }
    }
}
