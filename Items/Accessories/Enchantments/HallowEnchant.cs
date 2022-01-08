using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class HallowEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hallowed Enchantment");

            Tooltip.SetDefault(
@"You gain a shield that can reflect projectiles
Summons an Enchanted Sword familiar that scales with minion damage
Drastically increases minion speed
Certain minion attacks do reduced damage to compensate for increased speed
'Hallowed be your sword and shield'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神圣魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"使你获得一面可以反弹弹幕的盾牌
召唤一柄附魔剑，附魔剑的伤害取决于你的召唤伤害
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
            //player.GetModPlayer<FargoSoulsPlayer>().HallowEffect(hideVisual); //new effect
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddRecipeGroup("FargowiltasSouls:AnyHallowHead")//add summon helm here
            .AddIngredient(ItemID.HallowedPlateMail)
            .AddIngredient(ItemID.HallowedGreaves)
            .AddIngredient(ModContent.ItemType<SilverEnchant>())
            .AddIngredient(ItemID.Gungnir)
            .AddIngredient(ItemID.RainbowRod)
            //hallow lance
            //hallowed repeater
            //any caught fairy
            //any horse mount
            //recipe.AddIngredient(ItemID.FairyBell);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
