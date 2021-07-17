using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MothDust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moth Dust");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = -1;
            //projectile.hide = true;
            projectile.hostile = true;
            projectile.timeLeft = 180;

            projectile.scale = 0.5f;
        }

        public override void AI()
        {
            projectile.velocity *= .96f;

            if (Main.rand.NextBool())
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 70);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2.5f;
            }

            Lighting.AddLight(projectile.position, .3f, .1f, .3f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
            {
                target.AddBuff(mod.BuffType("Berserked"), 240);
                target.AddBuff(mod.BuffType("MutantNibble"), 240);
                target.AddBuff(mod.BuffType("Guilty"), 240);
                target.AddBuff(mod.BuffType("Lovestruck"), 240);
                target.AddBuff(mod.BuffType("Rotting"), 240);
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int d = Main.rand.Next(Fargowiltas.DebuffIDs.Count);
                    target.AddBuff(Fargowiltas.DebuffIDs[d], 240);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = lightColor;
            color.A = 0;
            return color;
        }
    }
}