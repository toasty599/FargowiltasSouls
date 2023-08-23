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

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 4f);
            npc.damage = (int)Math.Round(npc.damage * 0.6f);
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
        public List<NPC> Cells = new List<NPC> { };
        public const int CellAmount = 20;
        public override void ShieldsDownAI(NPC npc)
        {
            if (!gotBossBar)
            {
                npc.BossBar = ModContent.GetInstance<CompositeBossBar>();
                gotBossBar = true;
            }
            SpawnMinions(npc);
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
            const int WindupDuration = 60 * 1;
            const int AttackDuration = 60 * 4;
            const int EndlagDuration = 60 * 2;
            void Windup()
            {
                CellState((int)States.PrepareExpand);
            }
            void Attack()
            {
                CellState((int)States.Expand);
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
            foreach (NPC cell in Cells)
            {
                if (cell.active)
                {
                    cell.ai[1] = state;
                }
            }
        }
        private void SpawnMinions(NPC npc)
        {
            if (Cells.Count < CellAmount)
            {
                NPC spawn = NPC.NewNPCDirect(npc.GetSource_FromThis(), npc.Center + Main.rand.Next(-20, 20) * Vector2.UnitX + Main.rand.Next(-20, 20) * Vector2.UnitY, ModContent.NPCType<StardustMinion>());
                spawn.ai[1] = 1;
                spawn.ai[2] = npc.whoAmI;
                spawn.lifeMax = spawn.life = spawn.lifeMax * 10;
                Cells.Append(spawn);
                spawn.ai[3] = Cells.IndexOf(spawn);
                return;
            }
        }
        const int IdleTime = 90;
        private void Idle(NPC npc, Player player)
        {
            if (AttackTimer > IdleTime)
            {
                RandomAttack(npc);
            }
        }
        #endregion
    }
}
