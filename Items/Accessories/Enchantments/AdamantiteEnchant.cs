using FargowiltasSouls.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Projectiles;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AdamantiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Adamantite Enchantment");
            Tooltip.SetDefault("Every weapon shot will split into 3" +
                "\nAll weapon shots deal 50% damage and have 50% less iframes" +
                "\n'Chaos'");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "精金魔石");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "每秒会随机使你的一个弹幕分裂成三个" +
            //     "\n'一气化三清！'");
        }

        protected override Color nameColor => new Color(221, 85, 125);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AdamantiteEffect(player, Item);
        }

        public static void AdamantiteEffect(Player player, Item item)
        {
            FargoSoulsPlayer modplayer = player.GetModPlayer<FargoSoulsPlayer>();
            modplayer.AdamantiteEnchantItem = item;
        }

        public static float ProjectileDamageRatio = 0.5f;

        public static void AdamantiteSplit(Projectile projectile)
        {
            float damageRatio = ProjectileDamageRatio;

            List<Projectile> projectiles = FargoSoulsGlobalProjectile.SplitProj(projectile, 3, MathHelper.Pi / 16, damageRatio);
            projectiles.Add(projectile);

            foreach (Projectile proj in projectiles)
            {
                //standard iframes is 10
                //half if they aready use local, otherwise do 5 (half of standard) ??
                if (proj.usesLocalNPCImmunity)
                {
                    proj.localNPCHitCooldown /= 2;
                }
                else
                {
                    proj.usesLocalNPCImmunity = true;
                    proj.localNPCHitCooldown = 5;
                }
            }

            projectile.damage = (int)(projectile.damage * damageRatio);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyAdamHead")
                .AddIngredient(ItemID.AdamantiteBreastplate)
                .AddIngredient(ItemID.AdamantiteLeggings)
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.QuadBarrelShotgun)
                .AddIngredient(ItemID.DarkLance)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
