using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Minions
{
    public class PlanterasChild : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Plantera's Child");
            Description.SetDefault("The child of Plantera will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "世纪之花的孩子");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "世纪之花的孩子将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MagicalBulb = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<PlanterasChild>()] < 1)
                Projectile.NewProjectile(player.Center.X, player.Center.Y, -0.15f, -0.1f, ModContent.ProjectileType<PlanterasChild>(), 0, 3f, player.whoAmI);
        }
    }
}