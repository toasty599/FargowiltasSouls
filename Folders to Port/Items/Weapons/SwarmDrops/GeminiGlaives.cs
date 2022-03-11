using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class GeminiGlaives : SoulsItem
    {
        private int lastThrown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gemini Glaives");
            Tooltip.SetDefault("Fire different glaives depending on mouse click" +
                "\nAlternating clicks will enhance attacks" +
                "\n'The compressed forms of defeated foes..'");

            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "被打败的敌人的压缩形态..");
        }

        public override void SetDefaults()
        {
            item.damage = 340;
            Item.DamageType = DamageClass.Melee;
            item.width = 30;
            item.height = 30;
            item.useTime = 40;
            item.useAnimation = 40;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.Swing;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 25);
            item.rare = ItemRarityID.Purple;
            item.shootSpeed = 20;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Retiglaive>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<Spazmaglaive>()] > 0)
                return false;

            if (player.altFunctionUse == 2)
            {
                item.shoot = ModContent.ProjectileType<Retiglaive>();
                item.shootSpeed = 15f;
            }
            else
            {
                item.shoot = ModContent.ProjectileType<Spazmaglaive>();
                item.shootSpeed = 45f;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (lastThrown != type)
                damage = (int)(damage * 1.2); //additional damage boost for switching

            Vector2 speed = new Vector2(speedX, speedY);
            for (int i = -1; i <= 1; i++)
            {
                Projectile.NewProjectile(position, speed.RotatedBy(MathHelper.ToRadians(30) * i), type, damage, knockBack, player.whoAmI, lastThrown);
            }

            lastThrown = type;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "TwinRangs")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerTwins"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}