using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Projectiles.Ammos;

namespace FargowiltasSouls.Items.Ammos
{
    public class FargoBullet : SoulsItem
    {
        private Mod fargos = ModLoader.GetMod("Fargowiltas");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amalgamated Bullet Pouch");
            Tooltip.SetDefault("Chases after your enemy\n" +
                               "Bounces several times\n" +
                               "Each impact causes an explosion of crystal shards\n" +
                               "Inflicts several debuffs");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "混合子弹袋");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
                               "追踪敌人\n" +
                               "弹跳多次\n" +
                               "每次撞击都会造成魔晶碎片爆炸\n" +
                               "造成多种Debuff");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.knockBack = 4f; //same as explosive
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<FargoBulletProj>();
            Item.shootSpeed = 15f; // same as high velocity bullets
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            //.AddIngredient(ItemID.EndlessMusketPouch);
            .AddIngredient(fargos, "SilverPouch")
            .AddIngredient(fargos, "MeteorPouch")
            .AddIngredient(fargos, "CursedPouch")
            .AddIngredient(fargos, "IchorPouch")
            .AddIngredient(fargos, "CrystalPouch")
            .AddIngredient(fargos, "VelocityPouch")
            .AddIngredient(fargos, "VenomPouch")
            .AddIngredient(fargos, "ExplosivePouch")
            .AddIngredient(fargos, "GoldenPouch")
            .AddIngredient(fargos, "PartyPouch")
            .AddIngredient(fargos, "ChlorophytePouch")
            .AddIngredient(fargos, "NanoPouch")
            .AddIngredient(fargos, "LuminitePouch")
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 15)
            .AddTile(fargos, "CrucibleCosmosSheet")
            .Register();
        }
    }
}