using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class FishStick : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Fish Stick");
            // Tooltip.SetDefault("Right click to throw a sharknado stick\n'The carcass of a defeated foe shoved violently on a stick..'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "鱼杖");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'一个被打败的敌人的尸体,用棍子粗暴地串起来..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Ranged;
            //Item.mana = 10;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 6);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FishStickProj>();
            Item.shootSpeed = 35f;
            Item.noUseGraphic = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.shoot = player.altFunctionUse == 2
                ? ModContent.ProjectileType<FishStickProjTornado>()
                : ModContent.ProjectileType<FishStickProj>();

            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                foreach (Projectile tornado in Main.projectile.Where(p => p.active && p.type == ModContent.ProjectileType<FishStickProjTornado>()))
                {
                    if (player.altFunctionUse == 2)
                    {
                        tornado.Kill();
                    }
                    else
                    {
                        float shootSpeed = velocity.Length();
                        Vector2 vel = Vector2.Normalize(Main.MouseWorld - tornado.Center) * shootSpeed;
                        Projectile.NewProjectile(source, tornado.Center, vel, ModContent.ProjectileType<FishStickShark>(), damage, knockback, player.whoAmI);
                    }
                }
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override bool? UseItem(Player player)
        {
            
            
            return base.UseItem(player);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
                velocity = Vector2.Normalize(velocity) * (Main.MouseWorld - position).Length() / FishStickProjTornado.TravelTime;
        }
    }
}