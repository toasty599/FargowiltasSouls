using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class CosmosBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override int BossBagNPC => ModContent.NPCType<CosmosChampion>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<Eridanium>(), 10);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<UniverseCore>());
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.Find<ModItem>("Fargowiltas", "CrucibleCosmos").Type);
        }
    }
}