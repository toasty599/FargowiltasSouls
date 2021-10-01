using System.Linq;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode
{
    public static class EModeUtils
    {
        public static void DropSummon(NPC npc, int itemType, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (prerequisite && !downed && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, itemType);
                droppedSummonFlag = true;
            }
        }

        public static T GetEModeNPCMod<T>(this NPC npc) where T : EModeNPCBehaviour
            => npc.GetGlobalNPC<NewEModeGlobalNPC>().EModeNpcBehaviours.FirstOrDefault(m => m is T) as T;
    }
}
