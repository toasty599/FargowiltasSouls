using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Patreon.DemonKing;

namespace FargowiltasSouls.Items.Misc
{
    public class AbomBag : SoulsItem
    {
        public override int BossBagNPC => ModContent.NPCType<NPCs.AbomBoss.AbomBoss>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("Right click to open");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变体的摸彩袋");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "右键打开");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Purple;
        }

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetItemSource_OpenItem(Item.type), ModContent.ItemType<AbomEnergy>(), Main.rand.Next(16) + 15); // 15-30

            float chance = 3f;
            for (int i = 0; i < FargoSoulsWorld.downedChampions.Length; i++)
                if (FargoSoulsWorld.downedChampions[i])
                    chance += 0.5f;

            if (SoulConfig.Instance.PatreonFishron && Main.rand.NextFloat(100) < chance)
                player.QuickSpawnItem(player.GetItemSource_OpenItem(Item.type), ModContent.ItemType<StaffOfUnleashedOcean>());
        }
    }
}