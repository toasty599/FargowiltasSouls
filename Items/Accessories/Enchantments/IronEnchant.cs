using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    [AutoloadEquip(EquipType.Shield)]
    public class IronEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Enchantment");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铁魔石");
            
            string tooltip =
@"Right Click to guard with your shield
You will totally block an attack if timed correctly
You attract items from a larger range
'Strike while the iron is hot'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"右键进行盾牌格挡
如果时机正确则抵消这次伤害
扩大你的拾取范围
'趁热打铁'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(152, 142, 131);
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
            Item.value = 40000;
            //Item.shieldSlot = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.GetToggleValue("IronS"))
            {
                //shield
                //modPlayer.IronEffect();
            }
            //magnet
            if (player.GetToggleValue("IronM", false))
            {
                //modPlayer.IronEnchant = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronHelmet)
            .AddIngredient(ItemID.IronChainmail)
            .AddIngredient(ItemID.IronGreaves)
            .AddIngredient(ItemID.EmptyBucket)
            .AddIngredient(ItemID.IronBroadsword)
            //.AddIngredient(ItemID.IronBow);
            //apricot (high in iron pog)
            .AddIngredient(ItemID.ZebraSwallowtailButterfly)

            .AddTile(TileID.DemonAltar)
            .Register();

        }
    }
}
