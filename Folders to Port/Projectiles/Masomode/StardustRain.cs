using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class StardustRain : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_539";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Invader");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;

            projectile.hide = true;

            projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (++projectile.ai[0] > 2)
            {
                projectile.ai[0] = 0;

                const int spacing = 160;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        if (i == 0)
                            continue;
                        Vector2 spawnPos = projectile.Center;
                        spawnPos.X += spacing * projectile.ai[1] * i;
                        Projectile.NewProjectile(spawnPos, Vector2.UnitY * 7f, ProjectileID.StardustJellyfishSmall, projectile.damage, 0f, Main.myPlayer, 210);
                    }
                }

                if (++projectile.ai[1] > 1600 / spacing)
                {
                    projectile.Kill();
                }
            }
        }
    }
}