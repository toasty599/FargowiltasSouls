using FargowiltasSouls.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class DarkArtistEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        protected override Color nameColor => new(155, 92, 176);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            DarkArtistEffect(player, Item);
            ApprenticeEnchant.ApprenticeEffect(player, Item);
        }

        public static void DarkArtistEffect(Player player, Item item)
        {
            player.DisplayToggle("DarkArt");
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] == 0)
            {
                //spawn tower boi
                if (player.whoAmI == Main.myPlayer && player.GetToggleValue("DarkArt"))
                {
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_Misc(""), player.Center, Vector2.Zero, ModContent.ProjectileType<FlameburstMinion>(), 0, 0f, player.whoAmI);
                    proj.netUpdate = true; // TODO make this proj sync meme
                }
            }

            modPlayer.DarkArtistEnchantItem = item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ApprenticeAltHead)
            .AddIngredient(ItemID.ApprenticeAltShirt)
            .AddIngredient(ItemID.ApprenticeAltPants)
            .AddIngredient(null, "ApprenticeEnchant")
            .AddIngredient(ItemID.DD2FlameburstTowerT3Popper)
            //.AddIngredient(ItemID.ShadowbeamStaff);
            .AddIngredient(ItemID.InfernoFork)
            //Razorpine
            //staff of earth

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
