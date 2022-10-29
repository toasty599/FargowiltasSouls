using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RichMahoganyEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Rich Mahogany Enchantment");
            Tooltip.SetDefault(
@"All grappling hooks pull 1.5x as fast, shoot 2x as fast, and retract 3x as fast
While grappling you gain 10% damage resistance and a 50% thorns effect
'Guaranteed to keep you hooked'");

            //in force multiplier is 2.5x pull speed, DR increases to 50% and thorns to 500%
        }

        protected override Color nameColor => new Color(181, 108, 100);
        public override string wizardEffect => "Hooks pull 2.5x as fast, damage resistance and thorns increased greatly";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MahoganyEnchantActive = true;
        }

        public static void MahoganyHookAI(Projectile projectile, FargoSoulsPlayer modPlayer)
        {
            if (projectile.extraUpdates < 1)
                projectile.extraUpdates = 1;

            if (projectile.ai[0] == 2 && modPlayer.Player.velocity != Vector2.Zero) //grappling 
            {
                //this runs twice per frame due to extra update so its actually 2x this
                if (modPlayer.WoodForce)
                {
                    modPlayer.Player.endurance += 0.25f;
                    modPlayer.Player.thorns += 2.5f;
                }
                else
                {
                    modPlayer.Player.endurance += 0.05f;
                    modPlayer.Player.thorns += 0.25f;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.RichMahoganyHelmet)
            .AddIngredient(ItemID.RichMahoganyBreastplate)
            .AddIngredient(ItemID.RichMahoganyGreaves)
            .AddIngredient(ItemID.Moonglow)
            .AddIngredient(ItemID.Pineapple)
            .AddIngredient(ItemID.GrapplingHook)
            
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
