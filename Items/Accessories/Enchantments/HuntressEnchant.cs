using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class HuntressEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //             DisplayName.SetDefault("Huntress Enchantment");
            //             Tooltip.SetDefault(
            // @"Arrows will periodically fall towards your cursor
            // The arrow type is based on the first arrow in your inventory
            // Double tap down to create a localized rain of arrows at the cursor's position for a few seconds
            // This has a cooldown of 15 seconds
            // Explosive Traps recharge faster and oil enemies
            // Set oiled enemies on fire for extra damage
            // 'The Hunt is On'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "女猎人魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"箭矢会定期落至你光标周围
            // 箭矢的种类取决于你背包中第一个箭矢
            // 双击'下'键后令箭雨倾斜在光标位置
            // 此效果有15秒冷却时间
            // 爆炸机关攻击速度更快且会造成涂油减益
            // 点燃涂油的敌人以造成额外伤害
            // '狩猎开始了'");
        }

        protected override Color nameColor => new Color(122, 192, 76);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 200000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().HuntressEffect();
        }

        public static void HuntressEffect()
        {
        }


        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.HuntressWig)
            .AddIngredient(ItemID.HuntressJerkin)
            .AddIngredient(ItemID.HuntressPants)
            //.AddIngredient(ItemID.HuntressBuckler);
            .AddIngredient(ItemID.DD2ExplosiveTrapT2Popper)
            //tendon bow
            .AddIngredient(ItemID.DaedalusStormbow)
            //shadiwflame bow
            .AddIngredient(ItemID.DD2PhoenixBow)
            //dog pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
