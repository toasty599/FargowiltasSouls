using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using FargowiltasSouls.ItemDropRules.Conditions;

namespace FargowiltasSouls.EternityMode
{
    public static class EModeUtils
    {
        public static void DropSummon(NPC npc, int itemType, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (prerequisite && !downed && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(npc.GetItemSource_Loot(), player.Hitbox, itemType);
                droppedSummonFlag = true;
            }
        }

        public static void DropSummon(NPC npc, string itemName, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (prerequisite && !downed && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                if (ModContent.TryFind("Fargowiltas", itemName, out ModItem modItem))
                    Item.NewItem(npc.GetItemSource_Loot(), player.Hitbox, modItem.Type);
                droppedSummonFlag = true;
            }
        }

        public static T GetEModeNPCMod<T>(this NPC npc) where T : EModeNPCBehaviour
            => npc.GetGlobalNPC<NewEModeGlobalNPC>().EModeNpcBehaviours.FirstOrDefault(m => m is T) as T;

        public static bool LockEarlyBirdDrop(NPCLoot npcLoot, IItemDropRule rule)
        {
            EModeEarlyBirdLockDropCondition lockCondition = new EModeEarlyBirdLockDropCondition();
            IItemDropRule conditionalRule = new LeadingConditionRule(lockCondition);
            conditionalRule.OnSuccess(rule);
            npcLoot.Add(conditionalRule);
            return true;
        }

        public static void AddEarlyBirdDrop(NPCLoot npcLoot, IItemDropRule rule)
        {
            EModeEarlyBirdRewardDropCondition dropCondition = new EModeEarlyBirdRewardDropCondition();
            IItemDropRule conditionalRule = new LeadingConditionRule(dropCondition);
            conditionalRule.OnSuccess(rule);
            npcLoot.Add(conditionalRule);
        }

        public static void EModeDrop(NPCLoot npcLoot, IItemDropRule rule)
        {
            EModeDropCondition dropCondition = new EModeDropCondition();
            IItemDropRule conditionalRule = new LeadingConditionRule(dropCondition);
            conditionalRule.OnSuccess(rule);
            npcLoot.Add(conditionalRule);
        }
    }
}
