using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class SlimeSword : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Slinging Slasher");
            Tooltip.SetDefault("'The reward for slaughtering many..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "史莱姆抛射屠戮者");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            item.damage = 295;
            Item.DamageType = DamageClass.Melee;
            item.width = 40;
            item.height = 40;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.Swing;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10);
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SlimeBallHoming>();
            item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockback)
        {
            int numberProjectiles = 9;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 velocity = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(45) * (Main.rand.NextDouble() - 0.5));
                Projectile.NewProjectile(position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "SlimeKingsSlasher")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerSlime"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}