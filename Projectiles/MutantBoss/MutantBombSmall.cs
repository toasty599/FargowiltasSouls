using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantBombSmall : MutantBomb
    {
        public override string Texture => "Terraria/Projectile_645";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 275;
            projectile.height = 275;
            projectile.scale = 0.75f;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = false;
        }

        public override bool CanDamage()
        {
            if (projectile.frame > 2 && projectile.frame <= 4)
            {
                projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCD = 1;
                return false;
            }
            return true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Main.PlaySound(SoundID.Item, projectile.Center, 14);
            }

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame--;
                    projectile.Kill();
                }
            }
        }
    }
}