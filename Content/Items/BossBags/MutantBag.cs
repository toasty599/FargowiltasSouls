using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class MutantBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override int BossBagNPC => ModContent.NPCType<NPCs.MutantBoss.MutantBoss>();

        public override void OpenBossBag(Player player)
        {
            if (WorldSavingSystem.EternityMode)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<EternalEnergy>(), Main.rand.Next(6) + 15);
            }

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<Masochist>());
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<MutantsFury>());
        }
    }
}