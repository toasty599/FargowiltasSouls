using FargowiltasSouls.Items.Summons;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls
    {
        private void AddLocalizations()
        {
            #region helpers

            void Add(string key, string message)
            {
                ModTranslation text = LocalizationLoader.CreateTranslation(Instance, key);
                text.SetDefault(message);
                LocalizationLoader.AddTranslation(text);
            }

            void AddBossSpawnInfo(string bossName, string spawnInfo)
            {
                Add($"BossChecklist.{bossName}SpawnInfo", spawnInfo);
            }

            #endregion helpers


            AddBossSpawnInfo("DeviBoss", $"Spawn by using [i:{ModContent.ItemType<DevisCurse>()}]");
            AddBossSpawnInfo("AbomBoss", $"Spawn by using [i:{ModContent.ItemType<AbomsCurse>()}]");
            AddBossSpawnInfo("MutantBoss", $"Throw [i:{ModContent.ItemType<AbominationnVoodooDoll>()}] into a pool of lava while Abominationn is alive in Mutant's presence.");

            AddBossSpawnInfo("TimberChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] on the surface during day.");
            AddBossSpawnInfo("TerraChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] underground.");
            AddBossSpawnInfo("EarthChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the underworld.");
            AddBossSpawnInfo("NatureChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in underground snow.");
            AddBossSpawnInfo("LifeChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the Hallow at day.");
            AddBossSpawnInfo("ShadowChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the Corruption or Crimson at night.");
            AddBossSpawnInfo("SpiritChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the underground desert.");
            AddBossSpawnInfo("WillChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] at the ocean.");
            AddBossSpawnInfo("CosmosChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in space.");
        }
    }
}
