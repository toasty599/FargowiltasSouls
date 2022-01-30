using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Minions
{
    public class LunarCultist : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Lunar Cultist");
            Description.SetDefault("The Lunar Cultist will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "拜月教徒");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "拜月教徒将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().LunarCultist = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<LunarCultist>()] < 1)
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<LunarCultist>(), 0, 2f, player.whoAmI, -1f);
        }
    }
}