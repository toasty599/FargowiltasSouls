using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class MonkEnchant : BaseEnchant
    {

        protected override Color nameColor => new(146, 5, 32);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            MonkFields monkFields = player.GetEffectFields<MonkFields>();
            monkFields.MonkEnchantActive = true;

            if (!player.HasBuff(ModContent.BuffType<MonkBuff>()) && player.AddEffect<MonkDashEffect>(item))
            {
                if (modPlayer.FargoDash != DashManager.DashType.Shinobi)
                    modPlayer.FargoDash = DashManager.DashType.Monk;
                modPlayer.HasDash = true;

                monkFields.monkTimer++;

                if (monkFields.monkTimer >= 120)
                {
                    player.AddBuff(ModContent.BuffType<MonkBuff>(), 2);
                    monkFields.monkTimer = 0;

                    //dust
                    double spread = 2 * Math.PI / 36;
                    for (int i = 0; i< 36; i++)
                    {
                        Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                        int index2 = Dust.NewDust(player.Center, 0, 0, DustID.GoldCoin, velocity.X, velocity.Y, 100);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].noLight = true;
                    }
}
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MonkBrows)
            .AddIngredient(ItemID.MonkShirt)
            .AddIngredient(ItemID.MonkPants)
            //.AddIngredient(ItemID.MonkBelt);
            .AddIngredient(ItemID.DD2LightningAuraT2Popper)
            //meatball
            //blue moon
            //valor
            .AddIngredient(ItemID.DaoofPow)
            .AddIngredient(ItemID.MonkStaffT2)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class MonkDashEffect : AccessoryEffect
    {
        public override bool HasToggle => true;
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
    }
    public class MonkFields : EffectFields
    {
        public bool MonkEnchantActive = false;
        public bool ShinobiEnchantActive = false;
        public int monkTimer = 0;
        public override void ResetEffects()
        {
            if (!MonkEnchantActive)
                Player.ClearBuff(ModContent.BuffType<MonkBuff>());
            MonkEnchantActive = false;
            ShinobiEnchantActive = false;
        }
    }
}
