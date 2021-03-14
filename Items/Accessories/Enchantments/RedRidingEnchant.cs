using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RedRidingEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Riding Enchantment");
            Tooltip.SetDefault(
@"Arrows will periodically fall on nearby enemies
Double tap down to create a rain of arrows that follows the cursor's position for a few seconds
The arrow type is based on the first arrow in your inventory
This has a cooldown of 15 seconds
Greatly enhances Explosive Traps effectiveness
Effects of Celestial Shell
Summons a pet Puppy
'Big Bad Red Riding Hood'");
            DisplayName.AddTranslation(GameCulture.Chinese, "红色游侠魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"'大坏红帽'
满月时,攻击概率造成大出血
对低血量的敌人伤害增加
大幅加强爆炸陷阱能力
拥有天界贝壳的效果
召唤一只小狗");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(192, 27, 60);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Yellow;
            item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.RedRidingEffect(hideVisual);
            modPlayer.HuntressEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HuntressAltHead);
            recipe.AddIngredient(ItemID.HuntressAltShirt);
            recipe.AddIngredient(ItemID.HuntressAltPants);
            recipe.AddIngredient(null, "HuntressEnchant");
            recipe.AddIngredient(ItemID.MoonCharm);
            //candy corn rifle
            //celebration
            //eventide
            recipe.AddIngredient(ItemID.DD2BetsyBow);
            recipe.AddIngredient(ItemID.DogWhistle); //werewolf pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}