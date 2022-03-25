using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Corruption
{
    public class VileSpit : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.VileSpit);

        public int SuicideCounter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale *= 2; //THIS DOESNT WORK ANYMORE? WHY??? WHATS GOING ON????? I HATE THIS GAME

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.eaterBoss, NPCID.EaterofWorldsHead))
            {
                if (FargoSoulsWorld.MasochistModeReal)
                    npc.dontTakeDamage = true;
                else
                    npc.damage = (int)(npc.damage * 0.75);
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++SuicideCounter > 600)
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Rotting>(), 240);
        }
    }
}
