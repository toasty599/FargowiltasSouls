using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AncientShadowEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Shadow Enchantment");
            Tooltip.SetDefault(
@"Your attacks may inflict Darkness on enemies
Darkened enemies occasionally fire shadowflame tentacles at other enemies
Three Shadow Orbs will orbit around you
'Archaic, yet functional'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "远古暗影魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"攻击有几率造成黑暗减益
身上带有黑暗减益的敌人有几率向其他敌人发射暗影焰触手
三颗暗影珠围绕着你旋转
'十分古老，却非常实用'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(94, 85, 220);
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
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //modPlayer.AncientShadowEffect();
            //modPlayer.ShadowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.AncientShadowHelmet)
            .AddIngredient(ItemID.AncientShadowScalemail)
            .AddIngredient(ItemID.AncientShadowGreaves)
            //.AddIngredient(ItemID.AncientNecroHelmet);
            //.AddIngredient(ItemID.AncientGoldHelmet);
            .AddIngredient(null, "ShadowEnchant")
            .AddIngredient(ItemID.ShadowFlameKnife)
            .AddIngredient(ItemID.ShadowFlameHexDoll)
            //dart rifle
            //toxicarp

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
