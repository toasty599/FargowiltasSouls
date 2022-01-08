using Microsoft.Xna.Framework;
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
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
            if (npc == null)
            {
                projectile.Kill();
                return;
            }

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

            if (++projectile.localAI[0] > 180 && ++projectile.localAI[1] > (npc.localAI[3] > 1 ? 4 : 2)) //spray shards
            {
                SoundEngine.PlaySound(SoundID.Item27, projectile.position);
                projectile.localAI[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                    speed.Normalize();
                    speed *= 6f;
                    speed.X /= 2;
                    Projectile.NewProjectile(projectile.Center + speed * 4f, speed, ModContent.ProjectileType<AbomFrostShard>(), projectile.damage, projectile.knockBack, projectile.owner);
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