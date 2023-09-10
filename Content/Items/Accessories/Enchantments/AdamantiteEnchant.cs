using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Projectiles;

using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AdamantiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Adamantite Enchantment");
            /* Tooltip.SetDefault("Every weapon shot will split into 2" +
                "\nAll weapon shots deal 50% damage" +
                "\nThey hit twice as fast and gain armor penetration equal to 50% damage" +
                "\n'Chaos'"); */
        }

        protected override Color nameColor => new(221, 85, 125);

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
        static int[] AdamIgnoreItems = new int[]
        {
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.TerraBlade,
            ModContent.ItemType<DecrepitAirstrikeRemote>()
        };
        public static void AdamantiteSplit(Projectile projectile, FargoSoulsPlayer modPlayer, int splitDegreeAngle)
        {
            
            if (AdamIgnoreItems.Contains(modPlayer.Player.HeldItem.type))
            {
                return;
            }
            foreach (Projectile p in FargoSoulsGlobalProjectile.SplitProj(projectile, 3, MathHelper.ToRadians(splitDegreeAngle), modPlayer.EarthForce ? 1f / 3 : 1f / 2))
            {
                if (p != null && p.active)
                {
                    p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().HuntressProj = projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().HuntressProj;
                }
            }

            if (!modPlayer.EarthForce)
            {
                projectile.type = ProjectileID.None;
                projectile.timeLeft = 0;
                projectile.active = false;
            }
            else
            {
                projectile.damage = (int)(projectile.damage / 3f);
            }
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
