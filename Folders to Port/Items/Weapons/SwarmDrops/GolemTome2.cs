using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class GolemTome2 : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Landslide");
            Tooltip.SetDefault("'The reward for slaughtering many...'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "山崩 EX");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励'");
        }

        public override void SetDefaults()
        {
            item.damage = 220;
            Item.DamageType = DamageClass.Magic;
            item.width = 24;
            item.height = 28;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            item.knockBack = 2;
            item.value = Item.sellPrice(0, 25);
            item.rare = ItemRarityID.Purple;
            item.mana = 24;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("GolemHeadProj");
            item.shootSpeed = 20f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "RockSlide");
            .AddIngredient(null, "AbomEnergy", 10);
            .AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerGolem"));
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            .Register();
        }
    }
}