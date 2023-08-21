using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust
{
    public class LunarTowerStardust : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerStardust;
            set => NPC.ShieldStrengthTowerStardust = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerStardust);

        public LunarTowerStardust() : base(ModContent.BuffType<AntisocialBuff>(), 20) { }

        public override void ShieldsDownAI(NPC npc)
        {
            const int attackTime = 420;

            //if (AttackTimer > attackTime / 2)
            //{
            //    float scale = 4f * (attackTime - attackTime / 2) / (attackTime / 2);
            //    int d = Dust.NewDust(npc.Center, 0, 0, AuraDust, Scale: scale);
            //    Main.dust[d].velocity *= 12f;
            //    Main.dust[d].noGravity = true;
            //}

            if (++AttackTimer > attackTime)
            {
                AttackTimer = 0;

                npc.TargetClosest(false);
                NetSync(npc);

                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const float rotate = (float)Math.PI / 12f;
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Normalize();
                    speed *= 8f;
                    for (int i = 0; i < 24; i++)
                    {
                        Vector2 vel = speed.RotatedBy(rotate * i);
                        int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.AncientLight, 0,
                            0f, (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f, vel.X, vel.Y);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].velocity = vel;
                            Main.npc[n].netUpdate = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                }
            }
        }
    }
}
