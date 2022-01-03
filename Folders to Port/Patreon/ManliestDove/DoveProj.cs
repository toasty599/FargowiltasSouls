using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.ManliestDove
{
    public class DoveProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 5;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Parrot);
            aiType = ProjectileID.Parrot;
            projectile.height = 22;
        }

        public override bool PreAI()
        {
            Player player = Main.player[projectile.owner];
            player.parrot = false; // Relic from aiType
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead)
            {
                modPlayer.DovePet = false;
            }
            if (modPlayer.DovePet)
            {
                projectile.timeLeft = 2;
            }

            /*int num113 = Dust.NewDust(new Vector2(projectile.Center.X - projectile.direction * (projectile.width / 2), projectile.Center.Y + projectile.height / 2), projectile.width, 6, 76, 0f, 0f, 0, default(Color), 1f);
            Main.dust[num113].noGravity = true;
            Main.dust[num113].velocity *= 0.3f;
            Main.dust[num113].noLight = true;*/
        }
    }
}