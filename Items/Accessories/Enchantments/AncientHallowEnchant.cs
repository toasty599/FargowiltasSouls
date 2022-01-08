using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AncientHallowEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Hallowed Enchantment");
            Tooltip.SetDefault(
@"You gain a shield that can reflect projectiles
Summons an Enchanted Sword familiar that scales with minion damage
Summons a magical fairy
'Hallowed be your sword and shield'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "远古神圣魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"使你获得一面可以反弹弹幕的盾牌
召唤一柄附魔剑，附魔剑的伤害取决于你的召唤伤害
召唤一只魔法仙灵
'愿人都尊你的剑与盾为圣'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(150, 133, 100);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = 180000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().HallowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddRecipeGroup("FargowiltasSouls:AnyHallowHead") //ancient
            .AddIngredient(ItemID.HallowedPlateMail)  //ancient
            .AddIngredient(ItemID.HallowedGreaves) //ancient
            .AddIngredient(null, "SilverEnchant")
            .AddIngredient(ItemID.Excalibur)
            //durendal
            //sergent united
            //paladin hammer
            //nightglow
            //terraprisma

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
