using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SkeletronArms : ModBuff
    {
        public override void SetStaticDefaults()
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
                    FargoSoulsUtil.NewSummonProjectile(player.GetProjectileSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmL>(), 18, 8f, player.whoAmI);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmR>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetProjectileSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmR>(), 18, 8f, player.whoAmI);
            }
        }
    }
}