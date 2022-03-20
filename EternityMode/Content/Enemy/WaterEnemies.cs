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

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class WaterEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.BlueJellyfish,
            NPCID.Crab,
            NPCID.PinkJellyfish,
            NPCID.Piranha,
            NPCID.SeaSnail,
            NPCID.Shark,
            NPCID.Squid,
            NPCID.AnglerFish,
            NPCID.Arapaima,
            NPCID.BloodFeeder,
            NPCID.BloodJelly,
            NPCID.FungoFish,
            NPCID.GreenJellyfish,
            NPCID.Goldfish,
            NPCID.CorruptGoldfish,
            NPCID.CrimsonGoldfish,
            NPCID.WaterSphere,
            NPCID.Frog,
            NPCID.GoldFrog,
            NPCID.Grubby,
            NPCID.Sluggy,
            NPCID.Buggy
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.GetGlobalNPC<EModeGlobalNPC>().isWaterEnemy = true;
        }
    }
}
