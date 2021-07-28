using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomFlocko3 : AbomFlocko
    {
        public override string Texture => "Terraria/NPC_352";

        public override void AI()
        {
            if (projectile.ai[0] < 0 || projectile.ai[0] >= Main.maxNPCs)
            {
                projectile.Kill();
                return;
            }

            NPC npc = Main.npc[(int)projectile.ai[0]];

            Vector2 target = npc.Center;
            target.X += projectile.ai[1];
            target.Y -= 1100;

            Vector2 distance = target - projectile.Center;
            float length = distance.Length();
            if (length > 10f)
            {
                distance /= 8f;
                projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (projectile.velocity.Length() < 12f)
                    projectile.velocity *= 1.05f;
            }

            if (++projectile.localAI[0] > 90 && ++projectile.localAI[1] > (npc.localAI[3] > 1 ? 4 : 2)) //spray shards
            {
                Main.PlaySound(SoundID.Item27, projectile.position);
                projectile.localAI[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                        speed.Normalize();
                        speed *= 8f;
                        speed.X /= 2;
                        Projectile.NewProjectile(projectile.Center + speed * 4f, speed, ModContent.ProjectileType<AbomFrostShard>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }

                    if (Main.player[npc.target].active && !Main.player[npc.target].dead && Main.player[npc.target].Center.Y < projectile.Center.Y)
                    {
                        Main.PlaySound(SoundID.Item120, projectile.position);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = projectile.DirectionTo(Main.player[npc.target].Center + new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 201))) * 12f;
                            Projectile.NewProjectile(projectile.Center, vel, ModContent.ProjectileType<AbomFrostWave>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
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