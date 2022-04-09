using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class TheBigSting : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("The Big Sting");
            Tooltip.SetDefault("Uses darts for ammo" +
                "\n66% chance to not consume ammo" +
                "\n'The reward for slaughtering many..'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "大螫刺");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 266;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.2f;
            Item.value = 500000;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.BigStinger>();
            Item.useAmmo = AmmoID.Dart;
            Item.UseSound = SoundID.Item97;
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Item.shoot;

            //tsunami code
            /*Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            float num = 0.314159274f;
            int numShots = 3;
            Vector2 vel = new Vector2(speedX, speedY);
            vel.Normalize();
            vel *= 40f;
            bool collide = Collision.CanHit(vector, 0, 0, vector + vel, 0, 0);

            float rotation = MathHelper.ToRadians(Main.rand.NextFloat(0, 10));

            for (int i = 0; i < numShots; i++)
            {
                float num3 = i - (numShots - 1f) / 2f;
                Vector2 value = Utils.RotatedBy(vel, num * num3, default(Vector2));

                if (!collide)
                {
                    value -= vel;
                }

                Vector2 speed = new Vector2(speedX, speedY).RotatedBy(rotation * num3);
                Projectile.NewProjectile(vector.X + value.X, vector.Y + value.Y, speed.X, speed.Y, type, damage, knockBack, player.whoAmI);
            }*/

            return true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool CanConsumeAmmo(Player player) => Main.rand.NextBool(3);

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "TheSmallSting")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerBee"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}