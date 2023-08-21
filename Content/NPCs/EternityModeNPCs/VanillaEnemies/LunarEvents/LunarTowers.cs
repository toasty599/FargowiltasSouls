using System.IO;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System;
using FargowiltasSouls.Common.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.Linq;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents
{
    public abstract class LunarTowers : EModeNPCBehaviour
    {
        public abstract int ShieldStrength { get; set; }

        protected readonly int DebuffNotToInflict;
        protected readonly int AuraDust;
        public int AuraSize = 1500;

        protected LunarTowers(int debuffNotToInflict, int auraDust)
        {
            DebuffNotToInflict = debuffNotToInflict;
            AuraDust = auraDust;
        }
        public virtual void ShieldsUpAI(NPC npc)
        {

        }
        public abstract void ShieldsDownAI(NPC npc);

        public int AttackTimer;
        public int HealCounter;
        public int AuraSync;
        public bool SpawnedDuringLunarEvent;

        public int Phase = 0;
        public float PhaseHealthRatio = 0.5f;

        public bool spawned;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
            bitWriter.WriteBit(SpawnedDuringLunarEvent);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
            SpawnedDuringLunarEvent = bitReader.ReadBit();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);
            npc.boss = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
        }
        public bool AnyPlayerWithin(NPC npc, int range)
        {
            foreach (Player p in Main.player.Where(x => x.active && !x.dead))
            {
                if (npc.Distance(p.Center) <= range)
                {
                    return true;
                }
            }
            return false;
        }
        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (!WorldSavingSystem.EternityMode)
                return;

            if (!spawned)
            {
                spawned = true;
                SpawnedDuringLunarEvent = NPC.LunarApocalypseIsUp;
                npc.damage += 150;
                npc.defDamage += 150;
                npc.netUpdate = true;
                npc.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
            }

            if (SpawnedDuringLunarEvent && ShieldStrength > NPC.LunarShieldPowerMax)
                ShieldStrength = NPC.LunarShieldPowerMax;

            void Aura(int debuff)
            {
                if (DebuffNotToInflict != debuff)
                    EModeGlobalNPC.Aura(npc, AuraSize, debuff, dustid: AuraDust);
            }

            if (SpawnedDuringLunarEvent)
            {
                Aura(ModContent.BuffType<AtrophiedBuff>());
                Aura(ModContent.BuffType<JammedBuff>());
                Aura(ModContent.BuffType<ReverseManaFlowBuff>());
                Aura(ModContent.BuffType<AntisocialBuff>());

                if (++AuraSync > 60)
                {
                    AuraSync -= 600;
                    NetSync(npc);
                }
            }

            if (++HealCounter > 60)
            {
                HealCounter = 0;
                npc.TargetClosest(false);
                if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > AuraSize * 1.5f)
                {
                    Phase = 1;
                    PhaseHealthRatio = 1;
                    const int heal = 2000;
                    npc.life += heal;
                    if (npc.life > npc.lifeMax)
                        npc.life = npc.lifeMax;
                    CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                }
            }
            bool anyPlayersClose = AnyPlayerWithin(npc, AuraSize);
            if (anyPlayersClose)
            {
                npc.boss = !npc.dontTakeDamage;
                AttackTimer++;
            }
            if (npc.dontTakeDamage)
            {
                if (anyPlayersClose)
                {
                    ShieldsUpAI(npc);
                }
                if (npc.life < npc.lifeMax * PhaseHealthRatio)
                {
                    npc.life = (int)(npc.lifeMax * PhaseHealthRatio);
                }
            }
            else
            {
                if (anyPlayersClose)
                {
                    //when shields down, if life is lower than threshhold, put shields back up, go to next non-attacking phase, and ignore rest of AI.
                    if ((float)npc.life / npc.lifeMax < PhaseHealthRatio && Phase >= 0)
                    {
                        Phase = -(Phase + 1);
                        AttackTimer = 0;
                        SoundEngine.PlaySound(SoundID.NPCDeath58, npc.Center);
                        ShieldStrength = NPC.LunarShieldPowerMax;
                        return;
                    }
                    //when shields down, kill all pillar enemies and go to attack phase
                    if (Phase < 0)
                    {
                        foreach (NPC n in Main.npc)
                        {
                            int[] IDs;
                            switch (npc.type)
                            {
                                case NPCID.LunarTowerSolar:
                                    IDs = Solar.SolarEnemies.SolarEnemyIDs;
                                    break;
                                case NPCID.LunarTowerVortex:
                                    IDs = Vortex.VortexEnemies.VortexEnemyIDs;
                                    break;
                                case NPCID.LunarTowerNebula:
                                    IDs = Nebula.NebulaEnemies.NebulaEnemyIDs;
                                    break;
                                case NPCID.LunarTowerStardust:
                                    IDs = Stardust.StardustEnemies.StardustEnemyIDs;
                                    break;
                                default:
                                    IDs = null;
                                    Main.NewText("You shouldn't be seeing this. Tell Javyz or Terry.");
                                    break;
                            }
                            if (IDs.Contains(n.type))
                            {
                                n.StrikeInstantKill();
                            }
                        }
                        PhaseHealthRatio -= 0.5f;
                        Phase = -Phase;
                    }
                    ShieldsDownAI(npc);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (!WorldSavingSystem.EternityMode)
                return;

            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByAnything(npc, player, ref modifiers);

            if (!WorldSavingSystem.EternityMode)
                return;

            if (npc.Distance(player.Center) > AuraSize)
            {
                modifiers.Null();
            }
            else
            {
                modifiers.FinalDamage /= 2;
            }
        }
    }
}
