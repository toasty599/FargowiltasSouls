using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class FrostEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Frost Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰霜魔石");

/*            string tooltip =
@"Icicles will start to appear around you
Attacking will launch them towards the cursor
When they hit an enemy they are inflicted with Frostbite and frozen solid
Press the Freeze Key to chill everything for 20 seconds
There is a 60 second cooldown for this effect
'Let's coat the world in a deep freeze'";*/
            // Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"你的周围会出现冰锥
            // 攻击时会将冰锥发射至光标位置
            // 冰锥击中敌人时会使其短暂冻结并受到25%额外伤害5秒
            // 敌对弹幕飞行速度减半
            // '让我们给这个世界披上一层厚厚的冰衣'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override Color nameColor => new(122, 189, 185);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FrostEffect>(Item);
            player.AddEffect<SnowEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FrostHelmet)
            .AddIngredient(ItemID.FrostBreastplate)
            .AddIngredient(ItemID.FrostLeggings)
            .AddIngredient(ModContent.ItemType<SnowEnchant>())
            .AddIngredient(ItemID.Frostbrand)
            .AddIngredient(ItemID.FrostStaff)
            //frost staff
            //coolwhip
            //.AddIngredient(ItemID.BlizzardStaff);
            //.AddIngredient(ItemID.ToySled);
            //.AddIngredient(ItemID.BabyGrinchMischiefWhistle);

            .AddTile(TileID.CrystalBall)
            .Register();

        }
    }
    public class FrostEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<FrostEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                if (modPlayer.icicleCD <= 0 && modPlayer.IcicleCount < 10 && player.ownedProjectileCounts[ModContent.ProjectileType<FrostIcicle>()] < 10)
                {
                    modPlayer.IcicleCount++;

                    //kill all current ones
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == player.whoAmI)
                        {
                            proj.active = false;
                            proj.netUpdate = true;
                        }
                    }

                    //respawn in formation
                    for (int i = 0; i < modPlayer.IcicleCount; i++)
                    {
                        float radians = 360f / modPlayer.IcicleCount * i * (float)(Math.PI / 180);
                        Projectile frost = FargoSoulsUtil.NewProjectileDirectSafe(GetSource_EffectItem(player), player.Center, Vector2.Zero, ModContent.ProjectileType<FrostIcicle>(), 0, 0f, player.whoAmI, 5, radians);
                        frost.netUpdate = true;
                    }

                    float dustScale = 1.5f;

                    if (modPlayer.IcicleCount % 10 == 0)
                    {
                        dustScale = 3f;
                    }

                    //dust
                    for (int j = 0; j < 20; j++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 5f;
                        vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.MagicMirror);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = dustScale;
                        Main.dust[d].velocity = vector7;

                        if (modPlayer.IcicleCount % 10 == 0)
                        {
                            Main.dust[d].velocity *= 2;
                        }
                    }

                    modPlayer.icicleCD = 30;
                }

                if (modPlayer.icicleCD > 0)
                    modPlayer.icicleCD--;

                if (modPlayer.IcicleCount >= 1 && player.controlUseItem && player.HeldItem.IsWeapon() && player.HeldItem.createTile == -1 && player.HeldItem.createWall == -1 && player.HeldItem.ammo == AmmoID.None)
                {

                    int dmg = modPlayer.ForceEffect<FrostEnchant>() ? 100 : 50;

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == player.whoAmI)
                        {
                            Vector2 vel = (Main.MouseWorld - proj.Center).SafeNormalize(-Vector2.UnitY) * 20f;

                            int p = Projectile.NewProjectile(GetSource_EffectItem(player), proj.Center, vel, ProjectileID.Blizzard, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 1f, player.whoAmI);
                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].FargoSouls().CanSplit = false;
                                Main.projectile[p].FargoSouls().FrostFreeze = true;
                            }

                            proj.Kill();
                        }
                    }

                    modPlayer.IcicleCount = 0;
                    modPlayer.icicleCD = 120;
                }
            }
        }
    }
}
