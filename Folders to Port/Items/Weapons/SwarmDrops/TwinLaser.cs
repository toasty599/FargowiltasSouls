using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class TwinLaser : SoulsItem
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gemini Cannon");
            Tooltip.SetDefault("");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "双子机炮");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "");
        }

        public override void SetDefaults()
        {
            item.damage = 45; //
            Item.DamageType = DamageClass.Magic;
            item.width = 24;
            item.height = 24;
            item.channel = true;
            item.mana = 5; //
            item.useTime = 20;
            item.useAnimation = 20; //
            item.reuseDelay = 20;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            //item.UseSound = new LegacySoundStyle(4, 13);
            item.value = 50000;
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<GeminiLaser1>();
            item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Main.NewText("mouse:" + Main.MouseWorld + " pos:" + position);

            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);

            return true;
        }
    }
}