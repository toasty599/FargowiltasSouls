using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SouloftheMasochist : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Soul of the Siblings");
            Description.SetDefault("The power of Eternity Mode is with you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "受虐之魂");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "受虐模式的力量与你同在");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderBuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

                if (player.GetToggleValue("MasoSkele"))
                {
                    fargoPlayer.SkeletronArms = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmL>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmL>(), 0, 8f, player.whoAmI);
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmR>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmR>(), 0, 8f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoPugent"))
                {
                    fargoPlayer.PungentEyeballMinion = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<PungentEyeball>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<PungentEyeball>(), 0, 0f, player.whoAmI);
                }

                if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoRainbow"))
                {
                    fargoPlayer.RainbowSlime = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<RainbowSlime>()] < 1)
                    {
                        Projectile pro = Main.projectile[Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<RainbowSlime>(), 0, 3f, player.whoAmI)];
                        pro.netUpdate = true;
                    }
                }

                if (player.GetToggleValue("MasoProbe"))
                {
                    fargoPlayer.Probes = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe1>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Probe1>(), 0, 9f, player.whoAmI);
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe2>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Probe2>(), 0, 9f, player.whoAmI, 0f, -1f);
                }

                if (player.GetToggleValue("MasoPlant"))
                {
                    fargoPlayer.MagicalBulb = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<PlanterasChild>()] < 1)
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, -0.15f, -0.1f, ModContent.ProjectileType<PlanterasChild>(), 0, 3f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoFlocko"))
                {
                    fargoPlayer.SuperFlocko = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SuperFlocko>()] < 1)
                        Projectile.NewProjectile(player.Center, new Vector2(0f, -10f), ModContent.ProjectileType<SuperFlocko>(), 0, 4f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoUfo"))
                {
                    fargoPlayer.MiniSaucer = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniSaucer>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<MiniSaucer>(), 0, 3f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoCultist"))
                {
                    fargoPlayer.LunarCultist = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<LunarCultist>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<LunarCultist>(), 0, 2f, player.whoAmI, -1f);
                }

                if (player.GetToggleValue("MasoTrueEye"))
                {
                    fargoPlayer.TrueEyes = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeL>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeL>(), 0, 3f, player.whoAmI, -1f);

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeR>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeR>(), 0, 3f, player.whoAmI, -1f);

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeS>()] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeS>(), 0, 3f, player.whoAmI, -1f);
                }
            }
        }
    }
}