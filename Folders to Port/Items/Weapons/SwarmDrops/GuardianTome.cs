using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Linq;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class GuardianTome : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Guardian");
            Tooltip.SetDefault("'It's their turn to run'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "守卫者");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "现在轮到他们跑了");
        }

        public override void SetDefaults()
        {
            item.damage = 1499;
            Item.DamageType = DamageClass.Magic;
            item.width = 24;
            item.height = 28;
            item.useTime = 50;
            item.useAnimation = 50;
            item.useStyle = ItemUseStyleID.Shoot;
            item.useTurn = true;
            item.noMelee = true;
            item.knockBack = 2;
            item.value = Item.sellPrice(0, 70);
            item.rare = ItemRarityID.Purple;
            item.mana = 100;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("DungeonGuardian");
            item.shootSpeed = 18f;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipItemNameLine = tooltips.FirstOrDefault(line => line.Name == "ItemName" && line.mod == "Terraria");
            tooltipItemNameLine.overrideColor = new Color(255, Main.DiscoG, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerDG"));
            .AddIngredient(mod.ItemType("Sadism"), 15);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            .Register();
        }
    }
}