using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.Sasha
{
    public class MissDrakovisFishingPole : PatreonModItem
    {
        private int mode = 1;

        public override string Texture => "Terraria/Images/Item_2296";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Miss Drakovi's Fishing Pole");
            Tooltip.SetDefault("Right click to cycle through options of attack\nEvery damage type has one");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "Drakovi小姐的钓竿");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "右键循环切换攻击模式\n每种伤害类型对应一种模式");
        }

        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 15);
            Item.rare = 10;
            Item.autoReuse = true;

            SetUpItem();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //right click
            if (player.altFunctionUse == 2)
            {
                mode++;

                if (mode > 5)
                {
                    mode = 1;
                }

                SetUpItem();

                return false;
            }

            switch (mode)
            {
                //melee
                case 1:
                    Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PufferRang>(), damage, knockBack, player.whoAmI);
                    break;

                //range
                case 2:
                    {
                        Vector2 speed = new Vector2(speedX, speedY);

                        int numBullets = 4;
                        for (int num130 = 0; num130 < numBullets; num130++) //shotgun blast
                        {
                            Vector2 bulletSpeed = speed;
                            bulletSpeed.X += Main.rand.NextFloat(-1f, 1f);
                            bulletSpeed.Y += Main.rand.NextFloat(-1f, 1f);
                            Projectile.NewProjectile(position, bulletSpeed, type, damage, knockBack, player.whoAmI);
                        }

                        for (int i = 0; i < Main.maxNPCs; i++) //shoot an extra bullet at every nearby enemy
                        {
                            if (Main.npc[i].active && Main.npc[i].CanBeChasedBy() && player.Distance(Main.npc[i].Center) < 1000f)
                            {
                                Vector2 bulletSpeed = 2f * speed.Length() * player.DirectionTo(Main.npc[i].Center);
                                Projectile.NewProjectile(position, bulletSpeed, type, damage / 2, knockBack, player.whoAmI);
                            }
                        }
                    }
                    break;

                //magic
                case 3:
                    {
                        Vector2 speed = new Vector2(speedX, speedY);
                        for (int i = -2; i <= 2; i++)
                        {
                            float modifier = 1f - 0.75f / 2f * Math.Abs(i);
                            Projectile.NewProjectile(position, modifier * speed.RotatedBy(MathHelper.ToRadians(9) * i),
                                ModContent.ProjectileType<Bubble>(), damage, knockBack, player.whoAmI);
                        }
                    }
                    break;

                //summon
                case 4:
                    Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<FishMinion>(), damage, knockBack, player.whoAmI);
                    break;

                //throwing
                default:
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectile(position, 4f * new Vector2(speedX + Main.rand.Next(-2, 2), speedY + Main.rand.Next(-2, 2)), ModContent.ProjectileType<SpikyLure>(), damage, knockBack, player.whoAmI);
                    }
                    break;
            }

            return false;
        }

        private void SetUpItem()
        {
            ResetDamageType();

            switch (mode)
            {
                //melee
                case 1:
                    Item.DamageType = DamageClass.Melee;
                    Item.shoot = ModContent.ProjectileType<PufferRang>();

                    Item.useStyle = 1;
                    Item.useTime = 12;
                    Item.useAnimation = 12;
                    Item.UseSound = SoundID.Item1;
                    Item.knockBack = 6;
                    Item.noMelee = false;
                    Item.shootSpeed = 15f;
                    break;

                //range
                case 2:
                    Item.DamageType = DamageClass.Ranged;
                    Item.shoot = ProjectileID.Bullet;

                    Item.knockBack = 6.5f;
                    Item.useStyle = 5;
                    Item.useAnimation = 50;
                    Item.useTime = 50;
                    Item.useAmmo = AmmoID.Bullet;
                    Item.UseSound = SoundID.Item36;
                    Item.shootSpeed = 12f;
                    Item.noMelee = true;
                    break;

                //magic
                case 3:
                    Item.DamageType = DamageClass.Magic;
                    Item.mana = 15;
                    Item.shoot = ModContent.ProjectileType<Bubble>();

                    Item.knockBack = 3f;
                    Item.useStyle = 5;
                    Item.useAnimation = 25;
                    Item.useTime = 25;
                    Item.UseSound = SoundID.Item85;
                    Item.shootSpeed = 30f;
                    Item.noMelee = true;
                    break;

                //minion
                case 4:
                    Item.DamageType = DamageClass.Summon;
                    Item.mana = 10;
                    Item.shoot = ModContent.ProjectileType<FishMinion>();

                    Item.useTime = 36;
                    Item.useAnimation = 36;
                    Item.useStyle = 1;
                    Item.noMelee = true;
                    Item.knockBack = 3;
                    Item.UseSound = SoundID.Item44;
                    Item.shootSpeed = 10f;
                    Item.buffType = ModContent.BuffType<FishMinionBuff>();
                    Item.buffTime = 3600;
                    Item.autoReuse = true;
                    break;

                //throwing
                case 5:
                    Item.thrown = true;
                    Item.shoot = ModContent.ProjectileType<SpikyLure>();

                    Item.useStyle = 1;
                    Item.shootSpeed = 5f;
                    Item.knockBack = 1f;
                    Item.UseSound = SoundID.Item1;
                    Item.useAnimation = 15;
                    Item.useTime = 15;
                    Item.noMelee = true;
                    break;
            }
        }

        private void ResetDamageType()
        {
            Item.melee = false;
            Item.ranged = false;
            Item.magic = false;
            Item.summon = false;
            Item.ranged = false;
            Item.mana = 0;
            Item.useAmmo = AmmoID.None;
        }

        public override void AddRecipes()
        {
            if (SoulConfig.Instance.PatreonFishingRod)
            {
                CreateRecipe()
                .AddIngredient(ItemID.GoldenFishingRod)
                .AddIngredient(ItemID.BalloonPufferfish)
                .AddIngredient(ItemID.PurpleClubberfish)
                .AddIngredient(ItemID.FrostDaggerfish, 500)
                .AddIngredient(ItemID.ZephyrFish)
                .AddIngredient(ItemID.Toxikarp)
                .AddIngredient(ItemID.Bladetongue)
                .AddIngredient(ItemID.CrystalSerpent)
                .AddIngredient(ItemID.ScalyTruffle)
                .AddIngredient(ItemID.ObsidianSwordfish)

                .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

                
                .Register();
            }
        }
    }
}