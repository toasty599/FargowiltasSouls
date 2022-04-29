using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AncientCobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ancient Cobalt Enchantment");
            Tooltip.SetDefault(
@"20% chance for your projectiles to explode into stingers
This can only happen once every second
'The jungle of old empowers you'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "远古钴魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"你的弹幕有20%几率爆裂成毒刺
            // 此效果在每秒内只会发生一次
            // '古老的丛林赋予你力量'");
        }

        protected override Color nameColor => new Color(53, 76, 116);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().AncientCobaltEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.AncientCobaltHelmet)
            .AddIngredient(ItemID.AncientCobaltBreastplate)
            .AddIngredient(ItemID.AncientCobaltLeggings)
            //.AddIngredient(ItemID.AncientIronHelmet);
            .AddIngredient(ItemID.Blowpipe)
            .AddIngredient(ItemID.PoisonDart, 300)
            .AddIngredient(ItemID.PoisonedKnife, 300)
            //moon glow
            //buggy /grubby whoever isnt used
            //variegated lardfish

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
