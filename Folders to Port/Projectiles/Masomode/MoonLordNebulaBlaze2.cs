using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordNebulaBlaze2 : MoonLordNebulaBlaze
    {
        public override string Texture => "Terraria/Images/Projectile_634";

        public override void SetDefaults()
        {
            base.SetDefaults();

            projectile.timeLeft = 2400 * 3;
        }

        public override bool? CanDamage()
        {
            return base.CanDamage() && counter > 30 * projectile.MaxUpdates;
        }

        public int counter;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.MoonLordCore);
            if (npc == null || npc.ai[0] == 2f)
            {
                projectile.Kill();
                return;
            }

            counter++;

            if (projectile.ai[1] == 0) //identify the ritual CLIENT SIDE
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<LunarRitual>() && Main.projectile[i].ai[1] == npc.whoAmI)
                    {
                        projectile.localAI[1] = i;
                        break;
                    }
                }
            }

            Projectile ritual = FargoSoulsUtil.ProjectileExists(projectile.localAI[1], ModContent.ProjectileType<LunarRitual>());
            if (ritual != null && ritual.ai[1] == npc.whoAmI)
            {
                if (projectile.Distance(ritual.Center) > 1600f) //bounce off arena walls
                {
                    if (npc.GetEModeNPCMod<MoonLordCore>().VulnerabilityState != 2)
                    {
                        projectile.Kill();
                        return;
                    }

                    projectile.velocity = -projectile.velocity;
                    Vector2 toCenter = ritual.Center - projectile.Center;
                    float rotationDifference = toCenter.ToRotation() - projectile.velocity.ToRotation();
                    projectile.velocity = projectile.velocity.RotatedBy(MathHelper.WrapAngle(2 * rotationDifference * Main.rand.NextFloat(0.8f, 1.2f)));

                    projectile.position -= ritual.velocity;

                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}