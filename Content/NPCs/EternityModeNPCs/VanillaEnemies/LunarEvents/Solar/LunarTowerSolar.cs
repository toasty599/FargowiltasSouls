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
using Terraria.Audio;
using FargowiltasSouls.Content.Bosses.Lieflight;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Solar
{
    public class LunarTowerSolar : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerSolar;
            set => NPC.ShieldStrengthTowerSolar = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerSolar);

        public LunarTowerSolar() : base(ModContent.BuffType<AtrophiedBuff>(), DustID.SolarFlare) { }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 10f);
        }
        public override void ShieldsDownAI(NPC npc)
        {

            if (AttackTimer > 240)
            {
                AttackTimer = 0;

                npc.TargetClosest(false);
                NetSync(npc);

                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const float rotate = (float)Math.PI / 4f;
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Normalize();
                    speed *= 5f;
                    for (int i = -2; i <= 2; i++)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed.RotatedBy(i * rotate), ProjectileID.CultistBossFireBall, 40, 0f, Main.myPlayer);
                }
            }
        }
    }
}
