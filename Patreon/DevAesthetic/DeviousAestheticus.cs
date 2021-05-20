using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles;
using System.Collections.Generic;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    public class DeviousAestheticus : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devious Aestheticus");
            Tooltip.SetDefault("'If you're seeing this, You've been in a coma for 20 years, I don't know where this message will be, but please wake up'");
        }

        public override void SetDefaults()
        {
            item.damage = 420;
            item.summon = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 1;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DevRocket>();
            item.shootSpeed = 10f;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> Patreon Item <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockback)
        {
            int p = Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), type, damage, knockback, player.whoAmI);

            float spread = MathHelper.Pi / 8;

            FargoGlobalProjectile.SplitProj(Main.projectile[p], 4, spread, 1);

            return false;
        }
    }
}
