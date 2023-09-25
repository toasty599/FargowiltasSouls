using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Northstrider
{
    public class EulogistsDoomsdayScenario : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Eulogists Doomsday Scenario");
            /* Tooltip.SetDefault(
@"Destroys an area around you including yourself
''"); */
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            //Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = 100;

            Item.useAnimation = ItemHoldStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            //spawn proj
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<Explosion>(), 0, 5, player.whoAmI);


            //destroy blocks
            int radius = 50;
            Vector2 position = player.Center;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Math.Sqrt(x * x + y * y) <= radius)   //circle
                    {
                        int xPosition = (int)(x + position.X / 16.0f);
                        int yPosition = (int)(y + position.Y / 16.0f);
                        if (xPosition < 0 || xPosition >= Main.maxTilesX || yPosition < 0 || yPosition >= Main.maxTilesY)
                            continue;

                        Tile tile = Main.tile[xPosition, yPosition];

                        if (tile == null) continue;

                        if (WorldGen.InWorld(xPosition, yPosition))
                        {
                            tile.ClearEverything();
                            Main.Map.Update(xPosition, yPosition, 255);
                        }
                    }
                }
            }

            Main.refreshMap = true;
            // Play explosion sound
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.Item15, position);
                SoundEngine.PlaySound(SoundID.Item14, position);
            }

            return true;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient(ItemID.Dynamite, 50)

                .AddTile(TileID.WorkBenches)

                .Register();
        }
    }
}
