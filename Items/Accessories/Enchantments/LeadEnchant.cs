using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class LeadEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Lead Enchantment");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铅魔石");
            
            string tooltip =
@"Attacks inflict enemies with Lead Poisoning
Lead Poisoning deals damage over time and spreads to nearby enemies
'Not recommended for eating'";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"攻击有几率造成铅中毒减益
铅中毒减益持续造成伤害并且会扩散至周围的敌人
'不建议食用'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(67, 69, 88);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            LeadEffect(player);
        }

        public static void LeadEffect(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().LeadEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LeadHelmet)
                .AddIngredient(ItemID.LeadChainmail)
                .AddIngredient(ItemID.LeadGreaves)
                .AddIngredient(ItemID.LeadShortsword)
                .AddIngredient(ItemID.BlackPaint, 100)
                .AddIngredient(ItemID.GrayPaint, 100)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
