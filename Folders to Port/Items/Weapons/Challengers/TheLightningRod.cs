using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.Challengers
{
    public class TheLightningRod : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Lightning Rod");
            Tooltip.SetDefault("Charges power as it is spun\nDamage decreases per hit when thrown");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
            item.useStyle = ItemUseStyleID.Shoot;
            item.useAnimation = 30;
            item.useTime = 30;
            item.shootSpeed = 1f;
            item.knockBack = 6f;
            item.width = 68;
            item.height = 68;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<TheLightningRodProj>();
            item.value = Item.sellPrice(0, 2);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTurn = false;
            Item.DamageType = DamageClass.Melee;
            item.autoReuse = true;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            .AddIngredient(ItemID.Diamond)
            .AddIngredient(ItemID.Topaz, 2)
            .AddIngredient(ItemID.DemoniteBar, 7)
            .AddIngredient(ItemID.ShadowScale, 7)
            .AddTile(TileID.Anvils)
            
            .Register();

            recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            .AddIngredient(ItemID.Diamond)
            .AddIngredient(ItemID.Topaz, 2)
            .AddIngredient(ItemID.CrimtaneBar, 7)
            .AddIngredient(ItemID.TissueSample, 7)
            .AddTile(TileID.Anvils)
            
            .Register();
        }
    }
}