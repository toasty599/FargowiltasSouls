using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.NPCs.AbomBoss;
using FargowiltasSouls.Core;
using FargowiltasSouls.Patreon.DemonKing;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class AbomBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override int BossBagNPC => ModContent.NPCType<AbomBoss>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<AbomEnergy>(), Main.rand.Next(16) + 15); // 15-30

            float chance = 3f;
            for (int i = 0; i < FargoSoulsWorld.downedBoss.Length; i++)
                if (FargoSoulsWorld.downedBoss[i])
                    chance += 0.5f;

            if (SoulConfig.Instance.PatreonFishron && Main.rand.NextFloat(100) < chance)
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<StaffOfUnleashedOcean>());
        }
    }
}