using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.Patreon.Potato
{
    public class RazorContainer : PatreonModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();
            modPlayer.RazorContainer = true;


            //spawn 2 blades if none exist
            //if (player.ownedProjectileCounts[ModContent.ProjectileType<RazorBlade>()] < 2)
            //{
            //    Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, new Vector2(Main.rand.Next(-2, 2), -2), ModContent.ProjectileType<RazorBlade>(), 20, 2f, player.whoAmI);
            //}
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                .AddIngredient(ItemID.Chain, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    /*
     * 

Effect: Spawns 2 spinning sawblades in front of the player that are slightly bigger than half the player's size. These sawblades are connected to the player with a chain
each. They deal X melee damage to any enemies that touch them. Hitting the sawblades with a sword causes them to fly in the cursor's direction and return on enemy/tile
hit (boomerang).*/
}
