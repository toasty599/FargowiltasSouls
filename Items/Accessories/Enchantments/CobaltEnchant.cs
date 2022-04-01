using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CobaltEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Cobalt Enchantment");
            Tooltip.SetDefault(
@"25% chance for your projectiles to explode into shards
This can only happen once every second
'I can't believe it's not Palladium'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钴蓝魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"你的弹幕有25%几率爆裂成碎片
此效果在每秒内只会发生一次
'真不敢相信这竟然不是钯金'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(61, 164, 196);
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
            player.GetModPlayer<FargoSoulsPlayer>().CobaltEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyCobaltHead")
            .AddIngredient(ItemID.CobaltBreastplate)
            .AddIngredient(ItemID.CobaltLeggings)
            .AddIngredient(null, "AncientCobaltEnchant")
            //.AddIngredient(ItemID.Chik);
            .AddIngredient(ItemID.CrystalStorm)
            .AddIngredient(ItemID.CrystalVileShard)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
