using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class Tim : Teleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Tim);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.OnFire] = true;
            npc.lavaImmune = true;
            npc.lifeMax *= 2;
            npc.damage /= 2;
        }

        public override void OnSpawn(NPC npc)
        {
            base.OnSpawn(npc);

            for (int i = 0; i < 6; i++)
            {
                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.FireImp, velocity: Main.rand.NextVector2Circular(8, 8));
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 450, BuffID.Silenced, true, 15);
            EModeGlobalNPC.Aura(npc, 150, BuffID.Cursed, false, 20);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            npcLoot.Add(ItemDropRule.ByCondition(new EModeDropCondition(), ModContent.ItemType<TimsConcoction>(), 5));
        }
    }
}
