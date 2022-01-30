using FargowiltasSouls.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexMagnetRitual : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortex Ritual");
            Tooltip.SetDefault("'Power surges in your hand'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> Patreon Item <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            item.damage = 280;
            Item.DamageType = DamageClass.Magic;
            item.useTime = 16;
            item.useAnimation = 16;
            item.knockBack = 4f;
            item.mana = 15;
            item.useStyle = ItemUseStyleID.Shoot;
            item.autoReuse = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<VortexRitualProj>();
            item.shootSpeed = 12f;
            item.channel = true;

            item.width = 28;
            item.height = 30;
            item.value = Item.sellPrice(0, 12);
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item21;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //initial spawn
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VortexRitualProj>()] <= 0)
            {
                Vector2 mouse = Main.MouseWorld;
                Projectile.NewProjectile(mouse, Vector2.Zero, ModContent.ProjectileType<VortexRitualProj>(), damage, knockBack, player.whoAmI, 0, 300);

                //some funny dust
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MagnetSphere)
            .AddIngredient(ItemID.FragmentVortex, 35)
            .AddIngredient(ItemID.LunarBar, 5)
            .AddIngredient(ModContent.ItemType<Items.Accessories.Masomode.CelestialRune>())
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerCultist"))

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}
