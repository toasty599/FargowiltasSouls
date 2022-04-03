using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class EmpressofLight : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.HallowBoss);

        public bool DroppedSummon;

        //public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
        //    new Dictionary<Ref<object>, CompoundStrategy> {
        //        { new Ref<object>(FuryRingTimer), IntStrategies.CompoundStrategy },
        //    };

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.empressBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            EModeUtils.DropSummon(npc, "PrismaticPrimrose", NPC.downedEmpressOfLight, ref DroppedSummon, Main.hardMode);

            return true;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            //LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<EMPRESSACCESSORY>()));
            //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
            //npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            //LoadNPCSprite(recolor, npc.type);
            //LoadBossHeadSprite(recolor, 37);
            //LoadGoreRange(recolor, 1262, 1268);
            //extra_188?
        }
    }
}
