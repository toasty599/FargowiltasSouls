using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SkeletronArms : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Skeletron Arms");
            Description.SetDefault("The Skeletron arms will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "骷髅王之手");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "骷髅王之手将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().SkeletronArms = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmL>()] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmL>(), 0, 8f, player.whoAmI);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmR>()] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmR>(), 0, 8f, player.whoAmI);
            }
        }
    }
}