//using Terraria.ID;
//using Terraria.ModLoader;

//namespace FargowiltasSouls.Items.Ammos
//{
//    public class AmalgamatedDartBox : SoulsItem
//    {
//        private Mod fargos = ModLoader.GetMod("Fargowiltas");

//        public override bool Autoload(ref string name)
//        {
//            return false;
//        }

//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("Amalgamated Dart Box");
//            Tooltip.SetDefault("");
//        }

//        public override void SetDefaults()
//        {
//            Item.damage = 60;
//            Item.DamageType = DamageClass.Ranged;
//            Item.width = 26;
//            Item.height = 26;
//            Item.knockBack = 3.5f;
//            Item.rare = ItemRarityID.Red;
//            Item.shoot = mod.ProjectileType("AmalgamatedDart");
//            Item.shootSpeed = 3f;
//            Item.ammo = AmmoID.Dart;
//            //Item.UseSound = SoundID.Item63;
//        }

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//            .AddIngredient(fargos, "PoisonDartBox");
//            .AddIngredient(fargos, "CursedDartBox");
//            .AddIngredient(fargos, "IchorDartBox");
//            .AddIngredient(fargos, "CrystalDartBox");

//            .AddIngredient(mod.ItemType("Sadism"), 15);
//            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
//            recipe.SetResult(this);
//            .Register();
//        }
//    }
//}