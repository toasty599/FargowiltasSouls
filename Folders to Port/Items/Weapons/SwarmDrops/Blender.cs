using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class Blender : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Blender");
            Tooltip.SetDefault("'The reward for slaughtering many...'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "绞肉机");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励...'");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.Shoot;
            item.width = 24;
            item.height = 24;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            item.channel = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<BlenderYoyoProj>();
            item.useAnimation = 25;
            item.useTime = 25;
            item.shootSpeed = 16f;
            item.knockBack = 2.5f;
            item.damage = 377;

            item.value = Item.sellPrice(0, 25);
            item.rare = ItemRarityID.Purple;
        }

        public override void HoldItem(Player player)
        {
            //player.counterWeight = 556 + Main.rand.Next(6);
            player.stringColor = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "Dicer")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerPlant"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}