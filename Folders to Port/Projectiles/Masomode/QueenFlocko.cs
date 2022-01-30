using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class QueenFlocko : AbomBoss.AbomFlocko
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 150;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.IceQueen);
            if (npc == null)
            {
                projectile.Kill();
                return;
            }

            Player player = Main.player[npc.target];

            Vector2 target = player.Center;
            target.X += 700 * projectile.ai[1];

            Vector2 distance = target - projectile.Center;
            float length = distance.Length();
            if (length > 100f)
            {
                distance /= 8f;
                projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (projectile.velocity.Length() < 12f)
                    projectile.velocity *= 1.05f;
            }

            if (++projectile.localAI[1] > 120) //fire frost wave
            {
                projectile.localAI[1] = 0f;
                SoundEngine.PlaySound(SoundID.Item120, projectile.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = projectile.DirectionTo(player.Center) * 7f;
                    float xDistance = Math.Abs(player.Center.X - projectile.Center.X);
                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 velocity = vel.RotatedBy(MathHelper.ToRadians(4) * i);
                        velocity.X = (player.Center.X - projectile.Center.X) / 100f;
                        int p = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.FrostWave, projectile.damage, projectile.knockBack, projectile.owner);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 101;
                    }
                }
            }

            projectile.rotation += projectile.velocity.Length() / 12f * (projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++projectile.frameCounter > 3)
            {
                if (++projectile.frame >= 6)
                    projectile.frame = 0;
                projectile.frameCounter = 0;
            }
        }
    }
}