using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles;

namespace FargowiltasSouls.Items.Misc
{
    public class Nuke : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Reformer");
            Tooltip.SetDefault("Destroys an incredibly massive area\n" +
                                "Use at your own risk");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "银河重构器");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "破坏一片难以置信的巨大区域\n" +
                                                        "风险自负");
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 32;
            item.maxStack = 99;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.Swing;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item1;
            item.useAnimation = 20;
            item.useTime = 20;
            item.value = Item.buyPrice(0, 0, 3);
            item.noUseGraphic = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<NukeProj>();
            item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Dynamite, 500);
            recipe.AddTile(TileID.Hellforge);
            recipe.SetResult(this);
            .Register();
        }
    }
}