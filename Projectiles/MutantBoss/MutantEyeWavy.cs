using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantEyeWavy : MutantEye
    {
        public override string Texture => "Terraria/Projectile_452";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 120;
            cooldownSlot = 0;
        }

        private float Amplitude => projectile.ai[0];
        private float Period => projectile.ai[1];
        private float Counter => projectile.localAI[1] * 4;

        public float oldRot;

        public override void AI()
        {
            NPC mutant = FargoSoulsUtil.NPCExists(NPCs.EModeGlobalNPC.mutantBoss);
            if (mutant != null && (mutant.ai[0] == -5f || mutant.ai[0] == -7f))
            {
                float targetRotation = mutant.ai[3];

                float speed = projectile.velocity.Length();
                float rotation = targetRotation + (float)Math.PI / 4 * (float)Math.Sin(2 * (float)Math.PI * Counter / Period) * Amplitude;
                projectile.velocity = speed * rotation.ToRotationVector2();

                if (oldRot != 0)
                    projectile.Center = mutant.Center + (projectile.Center - mutant.Center).RotatedBy(targetRotation - oldRot);

                oldRot = targetRotation;
            }
            else
            {
                projectile.Kill();
                return;
            }

            projectile.localAI[0] += 0.1f;

            base.AI();
        }
    }
}