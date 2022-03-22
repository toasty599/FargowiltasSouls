using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Projectiles.Minions;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class DestroyerGun2 : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Destruction Cannon");
            Tooltip.SetDefault("Becomes longer and faster with up to 5 empty minion slots\n'The reward for slaughtering many...'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "毁灭者之枪 EX");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励...'");
        }

        public override void SetDefaults()
        {
            Item.damage = 275;
            Item.mana = 30;
            Item.DamageType = DamageClass.Summon;
            Item.width = 126;
            Item.height = 38;
            Item.useAnimation = 70;
            Item.useTime = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = new LegacySoundStyle(4, 13);
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DestroyerHead2>();
            Item.shootSpeed = 18f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "DestroyerGun")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerDestroy"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}