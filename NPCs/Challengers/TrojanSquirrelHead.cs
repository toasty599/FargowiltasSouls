using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace FargowiltasSouls.NPCs.Challengers
{
    public class TrojanSquirrelHead : TrojanSquirrelLimb
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.lifeMax = 400;

            NPC.width = 80;
            NPC.height = 76;
        }

        public override void AI()
        {
            base.AI();

            if (body == null)
                return;

            NPC.velocity = Vector2.Zero;
            NPC.target = body.target;
            NPC.direction = NPC.spriteDirection = body.direction;
            NPC.Center = body.Bottom + new Vector2(42f * NPC.direction, -153f) * body.scale;

            switch ((int)NPC.ai[0])
            {
                case 0:
                    if (body.ai[0] == 0 && body.localAI[0] <= 0)
                    {
                        NPC.ai[1] += FargoSoulsWorld.EternityMode ? 1.5f : 1f;

                        if (body.dontTakeDamage)
                            NPC.ai[1] += 1f;

                        int threshold = 240;

                        //structured like this so body gets priority first
                        int stallPoint = threshold - 30;
                        if (NPC.ai[1] > stallPoint)
                        {
                            TrojanSquirrel squirrel = body.ModNPC as TrojanSquirrel;
                            if (squirrel.arms != null && squirrel.arms.ai[0] != 0f) //wait if other part is attacking
                                NPC.ai[1] = stallPoint;
                        }

                        if (NPC.ai[1] > threshold && Math.Abs(body.velocity.Y) < 0.05f)
                        {
                            NPC.ai[0] = 1;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 1:
                    if (++NPC.ai[1] % (body.dontTakeDamage || FargoSoulsWorld.MasochistModeReal ? 30 : 45) == 0)
                    {
                        const float gravity = 0.2f;
                        float time = FargoSoulsWorld.MasochistModeReal ? 50f : 80f;
                        Vector2 distance = Main.player[NPC.target].Center - NPC.Center;// + player.velocity * 30f;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 10; i++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f),
                                    ModContent.ProjectileType<Projectiles.Champions.TimberAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (NPC.ai[1] > 210)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                Vector2 pos = NPC.Center;
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TrojanSquirrelGore1").Type, NPC.scale);
            }
        }
    }
}
