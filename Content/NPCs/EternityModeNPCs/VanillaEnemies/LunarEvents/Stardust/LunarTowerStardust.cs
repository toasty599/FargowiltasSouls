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
using FargowiltasSouls.Content.BossBars;
using static FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust.StardustMinion;
using Terraria.Audio;

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
        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

        }
        public enum Attacks
        {
            Idle,
            CellExpandContract,
            CellRush,
            CellRings,
            CellBallCharge
        }
        public override List<int> RandomAttacks => new List<int>() //these are randomly chosen attacks in p1
        {
            (int)Attacks.CellExpandContract,
            (int)Attacks.CellRush,
            (int)Attacks.CellRings,
            (int)Attacks.CellBallCharge
        };
        private bool gotBossBar = false;
        public const int CellAmount = 20;
        public override void ShieldsDownAI(NPC npc)
        {
            if (!gotBossBar)
            {
                npc.BossBar = ModContent.GetInstance<CompositeBossBar>();
                gotBossBar = true;
            }
            int cells = 0;
            int bigCells = 0;
            foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<StardustMinion>() && n.ai[0] == npc.whoAmI)) 
            {
                if (n.frame.Y == 0) //frame check is to check if big
                {
                    bigCells++;
                }
                cells++;
            }
            //cells are sorted by a unique key, stored in their NPC.ai[3], that determines their behavior during attacks, for example spot in a circle. 
            //here it only spawns new cells if there's missing keys, and missing cells.
            //yes this will probably lag some pcs a bit, it's a lot to check in one frame
            if (cells < CellAmount)
            {
                for (int i = 0; i < CellAmount; i++)
                {
                    bool foundCell = false;
                    foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<StardustMinion>() && n.ai[0] == npc.whoAmI && n.ai[3] == i)) //frame check is to check if big
                    {
                        foundCell = true;
                        break;
                    }
                    if (!foundCell)
                    {
                        SpawnMinion(npc, i);
                        bigCells++;
                    }
                }
                
            }
            if (bigCells > 0)
            {
                npc.defense = 99999999;
                npc.life = npc.lifeMax;
            }
            else
            {
                if (npc.defense != 0) //trigger shield going down animation
                {
                    CellState((int)States.Idle);
                    npc.defense = 0;
                    npc.ai[3] = 1f;
                    npc.netUpdate = true;
                    NetSync(npc);
                }
                return;
            }
            Player target = Main.player[npc.target];
            if (npc.HasPlayerTarget && target.active)
            {
                switch (Attack)
                {
                    case (int)Attacks.CellExpandContract:
                        CellExpandContract(npc, target);
                        break;
                    case (int)Attacks.CellRush:
                        CellRush(npc, target);
                        break;
                    case (int)Attacks.CellRings:
                        CellRings(npc, target);
                        break;
                    case (int)Attacks.CellBallCharge:
                        CellBallCharge(npc, target);
                        break;
                    case (int)Attacks.Idle:
                        Idle(npc, target);
                        break;
                }
            }
        }
        #region Attacks
        private void CellExpandContract(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 2;
            const int AttackDuration = 60 * 4;
            const int EndlagDuration = 60 * 2;
            void Windup()
            {
                if (AttackTimer == 1)
                {
                    CellState((int)States.PrepareExpand);
                }
            }
            void Attack()
            {
                if (AttackTimer - WindupDuration == 1)
                {
                    CellState((int)States.Expand);
                }
            }
            void Endlag()
            {

            }
            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private void CellRush(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 2;
            const int AttackDuration = 60 * 2;
            const int EndlagDuration = 60 * 2;
            void Windup()
            {

            }
            void Attack()
            {

            }
            void Endlag()
            {

            }

            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private void CellRings(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 2;
            const int AttackDuration = 60 * 2;
            const int EndlagDuration = 60 * 2;
            void Windup()
            {

            }
            void Attack()
            {

            }
            void Endlag()
            {

            }

            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private void CellBallCharge(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 2;
            const int AttackDuration = 60 * 2;
            const int EndlagDuration = 60 * 2;
            void Windup()
            {

            }
            void Attack()
            {

            }
            void Endlag()
            {

            }

            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private void CellState(int state)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<StardustMinion>())
                {
                    Main.npc[i].ai[1] = state;
                }
            }
        }
        private void SpawnMinion(NPC npc, int cell)
        {
            NPC spawn = NPC.NewNPCDirect(npc.GetSource_FromThis(), npc.Center + Main.rand.Next(-20, 20) * Vector2.UnitX + Main.rand.Next(-20, 20) * Vector2.UnitY, ModContent.NPCType<StardustMinion>());
            spawn.ai[1] = 1;
            spawn.ai[2] = npc.whoAmI;
            spawn.ai[3] = cell;
            return;
        }
        const int IdleTime = 90;
        private void Idle(NPC npc, Player player)
        {
            if (AttackTimer == 1)
            {
                CellState((int)States.Idle);
            }
            if (AttackTimer > IdleTime)
            {
                RandomAttack(npc);
            }
        }
        #endregion
    }
}
