using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using FargowiltasSouls.Content.Projectiles;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class NinjaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Ninja Enchantment");
            /* Tooltip.SetDefault(
@"Drastically increases projectile and attack speed
Reduces damage to compensate for increased speed
Increases armor pen by 15
'Attack faster than the eye can see'"); */
        }

        protected override Color nameColor => new(48, 49, 52);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.NinjaEnchantItem = Item;
        }

        public static void NinjaSpeedSetup(FargoSoulsPlayer modPlayer, Projectile projectile, FargoSoulsGlobalProjectile globalProj)
        {
            Player player = modPlayer.Player;

            if (player.GetToggleValue("NinjaSpeed"))
            {
                /*
                float maxSpeedIncrease = modPlayer.ShadowForce ? 0.75f : 0.5f;
                float maxSpeedScaling = modPlayer.ShadowForce ? 20 : 15; //the highest velocity at which your projectile speed is increased
                float speedIncrease = maxSpeedIncrease - Math.Min(player.velocity.Length() / maxSpeedScaling, maxSpeedIncrease);
                globalProj.NinjaSpeedup = 1 + speedIncrease;
                */
                float maxSpeedRequired = modPlayer.ForceEffect(modPlayer.NinjaEnchantItem.type) ? 7 : 4; //the highest velocity at which your projectile speed is increased
                if (player.velocity.Length() < maxSpeedRequired)
                {
                    const int speedIncrease = 1;
                    globalProj.NinjaSpeedup = projectile.extraUpdates + speedIncrease;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaHood)
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient(ItemID.NinjaPants)
                .AddIngredient(ItemID.Rally)
                .AddIngredient(ItemID.Shuriken, 100)
                .AddIngredient(ItemID.ThrowingKnife, 100)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
