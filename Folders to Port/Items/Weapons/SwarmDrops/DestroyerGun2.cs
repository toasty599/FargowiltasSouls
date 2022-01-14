using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;

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
            item.damage = 275;
            item.mana = 30;
            Item.DamageType = DamageClass.Summon;
            item.width = 126;
            item.height = 38;
            item.useAnimation = 70;
            item.useTime = 70;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.UseSound = new LegacySoundStyle(4, 13);
            item.value = Item.sellPrice(0, 25);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("DestroyerHead2");
            item.shootSpeed = 18f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "DestroyerGun");
            .AddIngredient(null, "AbomEnergy", 10);
            .AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerDestroy"));
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            .Register();
        }
    }
}