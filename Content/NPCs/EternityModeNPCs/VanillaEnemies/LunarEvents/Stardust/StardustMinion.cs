using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Golf;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust
{
    public class StardustMinion : ModNPC
    {
        public enum States 
        {
            Idle = 1,
            PrepareExpand,
            Expand,
            Contract
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Slime");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = NPCID.Sets.DebuffImmunitySets[NPCID.LunarTowerStardust].SpecificallyImmuneTo
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.StardustCellBig);

            NPC.lifeMax = 5000;
            NPC.damage = 80;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.timeLeft = 60 * 60 * 30;
            NPC.noTileCollide = true;

            NPC.scale = 1f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            StardustMinionAI();
        }
        public void StardustMinionAI()
        {
            ref float State = ref NPC.ai[1];
            ref float parentIndex = ref NPC.ai[2];
            ref float num = ref NPC.ai[3];

            ref float param1 = ref NPC.localAI[0];
            ref float param2 = ref NPC.localAI[1];

            NPC parent = Main.npc[(int)parentIndex];
            if (!parent.active || parent.type != NPCID.LunarTowerStardust)
            {
                NPC.active = false;
            }
            LunarTowerStardust parentModNPC = parent.GetGlobalNPC<LunarTowerStardust>();
            Main.NewText(State);
            switch (State)
            {
                case (int)States.Idle: //default, chill around center of pillar
                    {
                        Vector2 vectorToIdlePosition = parent.Center - NPC.Center;
                        Home(vectorToIdlePosition, 12, 6);
                        break;
                    }
                case (int)States.PrepareExpand:
                    {
                        //rotating at 1/4 rotations per second
                        float rotation = MathHelper.PiOver2 * parentModNPC.AttackTimer / 60f;
                        rotation += MathHelper.TwoPi * (num / LunarTowerStardust.CellAmount);
                        Vector2 desiredLocation = parent.Center + parent.height * 0.8f * rotation.ToRotationVector2();
                        Home(desiredLocation, 12, 6);
                        break;
                    }
                case (int)States.Expand:
                    { 
                        break;
                    }
                case (int)States.Contract: //is this one needed? just go to idle?
                    {
                        break;
                    }
            }
        }
        public void Home(Vector2 vectorToIdlePosition, float speed, float inertia)
        {
            vectorToIdlePosition.Normalize();
            vectorToIdlePosition *= speed;
            NPC.velocity = (NPC.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.velocity.X = -0.15f;
                NPC.velocity.Y = -0.05f;
            }
        }
        public override bool CheckDead()
        {
            ref float State = ref NPC.ai[1];
            ref float parentIndex = ref NPC.ai[2];
            NPC parent = Main.npc[(int)parentIndex];
            if (!parent.active)
            {
                return true;
            }
            NPC.life = NPC.lifeMax;
            NPC.dontTakeDamage = true;
            return false;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = NPC.dontTakeDamage ? frameHeight : 0;
        }
    }
}