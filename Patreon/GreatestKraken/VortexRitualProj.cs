using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexRitualProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortex Ritual");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = -1;

            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.timeLeft = 600;
            projectile.width = 19;
            projectile.height = 19;

            

            //projectile.scale = 0.5f;
        }

        public override void AI()
        {
            //kill me if player is not holding
            Player player = Main.player[projectile.owner];
            int maxRange = (int)projectile.ai[1];
            int attackCooldown = 1;
            int maxTargets = 1;

            if (player.dead || !player.active || !(player.HeldItem.type == ModContent.ItemType<VortexMagnetRitual>() && player.channel))
                projectile.Kill();




            //drain mana
            if (player.statMana >= 2)
            {
                player.statMana -= 2;
                player.manaRegenDelay = 300;

                //grow in power over time (size, damage, range)
                if (maxRange < 1500 && ++projectile.ai[0] >= 5)
                {
                    //projectile.position = projectile.Center;
                    projectile.scale *= 1.05f;
                    //projectile.width = (int)(projectile.width * 1.05f);
                    //projectile.height = (int)(projectile.height * 1.05f);
                    //projectile.Center = projectile.position;


                    //Vector2 center = projectile.Center;

                    //float increase = (projectile.scale * 1.05f) - projectile.scale;

                    //projectile.scale *= 1.05f;

                    //projectile.Center = new Vector2(center.X, center.Y - increase); ;

                    projectile.ai[0] = 0;

                    projectile.damage = (int)(projectile.damage * 1.5f);

                    maxRange += 50;
                    projectile.ai[1] = maxRange;

                    //Main.NewText(maxRange);

                    //dust 
                    //DustRing(projectile, 10);
                }
            }
            
            //you can keep holding it with no mana but it wont grow
            //magnet sphere code real
            int[] array = new int[20];
            int index = 0;

            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.CanBeChasedBy(this, false))
                {
                    float npcX = npc.position.X + (float)(npc.width / 2);
                    float npcY = npc.position.Y + (float)(npc.height / 2);
                    float distance = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - npcX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - npcY);

                    if (distance < maxRange && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                    {
                        if (index < 20)
                        {
                            array[index] = i;
                            index++;
                        }

                        int num434 = Main.rand.Next(index);
                        num434 = array[num434];
                        float num435 = Main.npc[num434].position.X + (float)(Main.npc[num434].width / 2);
                        float num436 = Main.npc[num434].position.Y + (float)(Main.npc[num434].height / 2);

                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] > 8f)
                        {
                            projectile.localAI[0] = 0f;
                            float num437 = 6f;
                            Vector2 value11 = new Vector2(projectile.Center.X + (float)projectile.width * 0.5f, projectile.Center.Y + (float)projectile.height * 0.5f);
                            value11 += projectile.velocity * 4f;
                            float num438 = num435 - value11.X;
                            float num439 = num436 - value11.Y;
                            float num440 = (float)Math.Sqrt((double)(num438 * num438 + num439 * num439));
                            num440 = num437 / num440;
                            num438 *= num440;
                            num439 *= num440;
                            Projectile.NewProjectile(value11.X, value11.Y, num438, num439, ProjectileID.MagnetSphereBolt, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                            return;
                        }
                    }
                }
            }

            /*projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 4)
                {
                    projectile.frame = 0;
                }
            }*/
        }

        public override void Kill(int timeLeft)
        {
            DustRing(projectile, 10);
        }

        private void DustRing(Projectile proj, int max)
        {
            //dust
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + proj.Center;
                Vector2 vector7 = vector6 - proj.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Vortex, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
    }
}
