using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MythrilEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mythril Enchantment");
            Tooltip.SetDefault(
@"15% increased weapon use speed
Taking damage temporarily removes this weapon use speed increase
'You feel the knowledge of your weapons seep into your mind'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "秘银魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"增加15%武器使用速度
受到伤害时武器使用速度增加效果会暂时失效
'你感觉你对武器的知识渗透进了你的脑海中");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(157, 210, 144);
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
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.GetToggleValue("Mythril"))
            {
                //fargoPlayer.MythrilEnchant = true;
                //if (!fargoPlayer.DisruptedFocus)
                    //fargoPlayer.AttackSpeed += fargoPlayer.WizardEnchant ? .2f : .15f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyMythrilHead")
            .AddIngredient(ItemID.MythrilChainmail)
            .AddIngredient(ItemID.MythrilGreaves)
            //flintlock pistol
            //recipe.AddIngredient(ItemID.LaserRifle);
            .AddIngredient(ItemID.ClockworkAssaultRifle)
            .AddIngredient(ItemID.Gatligator)
            .AddIngredient(ItemID.OnyxBlaster)


            .AddTile(TileID.CrystalBall)
            .Register();

        }
    }
}
