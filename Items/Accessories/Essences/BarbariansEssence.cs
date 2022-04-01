using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class BarbariansEssence : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Barbarian's Essence");
            Tooltip.SetDefault(
@"18% increased melee damage
10% increased melee speed
5% increased melee crit chance
'This is only the beginning..'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "野蛮人精华");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"增加18%近战伤害
增加10%近战攻速
增加5%近战暴击率
'这只是个开始...'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(255, 111, 6));
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
            player.GetDamage(DamageClass.Melee) += 0.18f;
            player.meleeSpeed += .1f;
            player.GetCritChance(DamageClass.Melee) += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ZombieArm)
                .AddIngredient(ItemID.ChainKnife)
                .AddIngredient(ItemID.IceBlade)
                .AddIngredient(ItemID.Shroomerang)
                .AddIngredient(ItemID.JungleYoyo)
                .AddIngredient(ItemID.CombatWrench)
                .AddIngredient(ItemID.Flamarang)
                .AddIngredient(ItemID.Terragrim)
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();

        }
    }
}
