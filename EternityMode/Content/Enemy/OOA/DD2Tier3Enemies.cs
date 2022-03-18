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
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.OOA
{
    public class DD2Tier3Enemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2GoblinT3,
            NPCID.DD2GoblinBomberT3,
            NPCID.DD2JavelinstT3,
            NPCID.DD2DrakinT3,
            NPCID.DD2WitherBeastT2,
            NPCID.DD2WitherBeastT3
        );

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
                npc.Transform(NPCID.DD2WyvernT3);
        }
    }
}
