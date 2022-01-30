using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Weapons.BossDrops;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class MechanicalLeashOfCthulhu : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechanical Leash of Cthulhu");
            Tooltip.SetDefault("'The reward for slaughtering many..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "机械克苏鲁连枷");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            item.damage = 220;
            item.width = 30;
            item.height = 10;
            item.value = Item.sellPrice(0, 10);
            item.rare = ItemRarityID.Purple;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.autoReuse = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.knockBack = 6f;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.MechFlail>();
            item.shootSpeed = 50f;
            item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LeashOfCthulhu>())
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerEye"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}