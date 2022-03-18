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
    public class DD2Wyvern : Shooters
    {
        public DD2Wyvern() : base(300, ProjectileID.Fireball, 6f) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2WyvernT1,
            NPCID.DD2WyvernT2,
            NPCID.DD2WyvernT3
        );

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
            target.AddBuff(BuffID.Rabies, 3600);
        }
    }
}
