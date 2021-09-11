using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode
{
    public static class EModeUtils
    {
        public static void DropSummon(NPC npc, int itemType, bool downed, ref bool droppedSummonFlag)
        {
            if (!downed && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                Item.NewItem(player.Hitbox, itemType);
                droppedSummonFlag = true;
            }
        }
    }
}
