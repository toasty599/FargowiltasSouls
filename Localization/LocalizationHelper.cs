using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls
{
	public partial class FargowiltasSouls
    {
        /// <summary>
        /// DEPRECATED <para/>
        /// Input item as string (item class name) for FargoSouls items, and as int for vanilla items.
        /// </summary>
        public void AddToggle(string toggle, object item, string color = "ffffff") 
        {
            /*
            if (item.GetType() == typeof(int))
            {
                Language.GetOrRegister(toggle, () => $"[i:{item}] [c/{color}:{{$Mods.{Name}.Toggler.{toggle}}}]");
            }
            else if (item.GetType() == typeof(string))
            {
                Language.GetOrRegister(toggle, () => $"[i:FargowiltasSouls/{item}] [c/{color}:{{$Mods.{Name}.Toggler.{toggle}}}]");
            }
            */
        }
        private void AddLocalizations()
        {
            #region helpers

            void Add(string key, string message)
            {
                Language.GetOrRegister(key, () => message);
            }

            void AddBossSpawnInfo(string bossName, string spawnInfo)
            {
                Add($"BossChecklist.{bossName}SpawnInfo", spawnInfo);
            }
            

            #endregion helpers
            
            #region boss spawn info
            
            if (FargoSoulsUtil.IsChinese())
            {
                AddBossSpawnInfo("DeviBoss", $"使用[i:{"DevisCurse"}]召唤");
                AddBossSpawnInfo("AbomBoss", $"使用[i:{"AbomsCurse"}]召唤");
                AddBossSpawnInfo("MutantBoss", $"在突变体和憎恶存活时将[i:{"AbominationnVoodooDoll"}]投入岩浆池中。");

                AddBossSpawnInfo("TimberChampion", $"白天时在地表使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("TerraChampion", $"在地下使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("EarthChampion", $"在地狱使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("NatureChampion", $"在地下雪原使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("LifeChampion", $"白天时在神圣之地使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("ShadowChampion", $"夜晚时在腐化之地或猩红之地使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("SpiritChampion", $"在地下沙漠使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("WillChampion", $"在海洋使用[i:{"SigilOfChampions"}]召唤。");
                AddBossSpawnInfo("CosmosChampion", $"在太空使用[i:{"SigilOfChampions"}]召唤。");

                AddBossSpawnInfo("TrojanSquirrel", $"使用[i:{"SquirrelCoatofArms"}]召唤");
                AddBossSpawnInfo("Lifelight", $"Spawn by using [i:{"FragilePixieLamp"}] in the Hallow at day.");
                //JAVYZ TODO: Cursed Coffin and Banished Baron
                //AddBossSpawnInfo("CursedCoffin", $"Spawn by using [i:{"CoffinSummon"}] in the Underground Desert.");
                AddBossSpawnInfo("BanishedBaron", $"Spawn by using [i:{"BaronSummon"}] underwater at the ocean.");
            }
            else if (FargoSoulsUtil.IsPortuguese())
            {
                AddBossSpawnInfo("DeviBoss", $"Invoque usando [i:{"DevisCurse"}]");
                AddBossSpawnInfo("AbomBoss", $"Invoque usando [i:{"AbomsCurse"}]");
                AddBossSpawnInfo("MutantBoss", $"Arremesse [i:{"AbominationnVoodooDoll"}] em uma poça de lava enquanto Abominationn está vivo na presença de Mutant.");

                AddBossSpawnInfo("TimberChampion", $"Invoque usando [i:{"SigilOfChampions"}] na superfície durante o dia.");
                AddBossSpawnInfo("TerraChampion", $"Invoque usando [i:{"SigilOfChampions"}] no subterrâneo.");
                AddBossSpawnInfo("EarthChampion", $"Invoque usando [i:{"SigilOfChampions"}] no submundo.");
                AddBossSpawnInfo("NatureChampion", $"Invoque usando [i:{"SigilOfChampions"}] na neve subterrânea.");
                AddBossSpawnInfo("LifeChampion", $"Invoque usando [i:{"SigilOfChampions"}] no Sagrado de dia.");
                AddBossSpawnInfo("ShadowChampion", $"Invoque usando [i:{"SigilOfChampions"}] na Corrupção ou Carmim de noite.");
                AddBossSpawnInfo("SpiritChampion", $"Invoque usando [i:{"SigilOfChampions"}] no deserto subterrâneo.");
                AddBossSpawnInfo("WillChampion", $"Invoque usando [i:{"SigilOfChampions"}] no oceano.");
                AddBossSpawnInfo("CosmosChampion", $"Invoque usando [i:{"SigilOfChampions"}] no espaço.");

                AddBossSpawnInfo("TrojanSquirrel", $"Invoque usando [i:{"SquirrelCoatofArms"}]");
            }
            else {
                AddBossSpawnInfo("DeviBoss", $"Spawn by using [i:{"DevisCurse"}]");
                AddBossSpawnInfo("AbomBoss", $"Spawn by using [i:{"AbomsCurse"}]");
                AddBossSpawnInfo("MutantBoss", $"Throw [i:{"AbominationnVoodooDoll"}] into a pool of lava while Abominationn is alive in Mutant's presence.");

                AddBossSpawnInfo("TimberChampion", $"Spawn by using [i:{"SigilOfChampions"}] on the surface during day.");
                AddBossSpawnInfo("TerraChampion", $"Spawn by using [i:{"SigilOfChampions"}] underground.");
                AddBossSpawnInfo("EarthChampion", $"Spawn by using [i:{"SigilOfChampions"}] in the underworld.");
                AddBossSpawnInfo("NatureChampion", $"Spawn by using [i:{"SigilOfChampions"}] in underground snow.");
                AddBossSpawnInfo("LifeChampion", $"Spawn by using [i:{"SigilOfChampions"}] in the Hallow at day.");
                AddBossSpawnInfo("ShadowChampion", $"Spawn by using [i:{"SigilOfChampions"}] in the Corruption or Crimson at night.");
                AddBossSpawnInfo("SpiritChampion", $"Spawn by using [i:{"SigilOfChampions"}] in the underground desert.");
                AddBossSpawnInfo("WillChampion", $"Spawn by using [i:{"SigilOfChampions"}] at the ocean.");
                AddBossSpawnInfo("CosmosChampion", $"Spawn by using [i:{"SigilOfChampions"}] in space.");

                AddBossSpawnInfo("TrojanSquirrel", $"Spawn by using [i:{"SquirrelCoatofArms"}]");
                AddBossSpawnInfo("Lifelight", $"Spawn by using [i:{"FragilePixieLamp"}] in the Hallow at day.");
                //JAVYZ TODO: Cursed Coffin
                //AddBossSpawnInfo("CursedCoffin", $"Spawn by using [i:{"CoffinSummon"}] in the Underground Desert.");
                AddBossSpawnInfo("BanishedBaron", $"Spawn by using [i:{"BaronSummon"}] underwater at the ocean.");
            }

            #endregion boss spawn info


            #region Toggles

            #region pet toggles
            AddToggle("PetHeader", ItemID.ZephyrFish);
            AddToggle("PetDinoConfig", ItemID.AmberMosquito);
            AddToggle("PetEaterConfig", ItemID.EatersBone);
            AddToggle("PetFaceMonsterConfig", ItemID.BoneRattle);
            AddToggle("PetGrinchConfig", ItemID.BabyGrinchMischiefWhistle);
            AddToggle("PetHornetConfig", ItemID.Nectar);
            AddToggle("PetImpConfig", ItemID.HellCake);
            AddToggle("PetPenguinConfig", ItemID.Fish);
            AddToggle("PetPandaConfig", ItemID.BambooLeaf);
            AddToggle("PetDGConfig", ItemID.BoneKey);
            AddToggle("PetSnowmanConfig", ItemID.ToySled);
            AddToggle("PetShroomConfig", ItemID.StrangeGlowingMushroom);
            AddToggle("PetWerewolfConfig", ItemID.FullMoonSqueakyToy);
            AddToggle("PetBernieConfig", ItemID.BerniePetItem);
            AddToggle("PetBlackCatConfig", ItemID.UnluckyYarn);
            //AddToggle("PetBlueChickenConfig", ItemID.egg);
            //AddToggle("PetCavelingConfig", ItemID.glowt);
            AddToggle("PetChesterConfig", ItemID.ChesterPetItem);
            AddToggle("PetCompanionCubeConfig", ItemID.CompanionCube);
            AddToggle("PetCursedSaplingConfig", ItemID.CursedSapling);
            //public string PetDirt;
            AddToggle("PetKittenConfig", ItemID.BallOfFuseWire);
            AddToggle("PetEsteeConfig", ItemID.CelestialWand);
            AddToggle("PetEyeSpringConfig", ItemID.EyeSpring);
            AddToggle("PetFoxConfig", ItemID.ExoticEasternChewToy);
            AddToggle("PetButterflyConfig", ItemID.BedazzledNectar);
            AddToggle("PetGlommerConfig", ItemID.GlommerPetItem);
            AddToggle("PetDragonConfig", ItemID.DD2PetDragon);
            //AddToggle("PetJunimoConfig", ItemID.stardr);
            AddToggle("PetHarpyConfig", ItemID.BirdieRattle);
            AddToggle("PetLizardConfig", ItemID.LizardEgg);
            AddToggle("PetMinitaurConfig", ItemID.TartarSauce);
            AddToggle("PetParrotConfig", ItemID.ParrotCracker);
            AddToggle("PetPigmanConfig", ItemID.PigPetItem);
            AddToggle("PetPlanteroConfig", ItemID.MudBud);
            AddToggle("PetGatoConfig", ItemID.DD2PetGato);
            AddToggle("PetPupConfig", ItemID.DogWhistle);
            AddToggle("PetSeedConfig", ItemID.Seedling);
            AddToggle("PetSpiderConfig", ItemID.SpiderEgg);
            AddToggle("PetMimicConfig", ItemID.OrnateShadowKey);
            AddToggle("PetSharkConfig", ItemID.SharkBait);
            //AddToggle("PetSpiffoConfig", ItemID.spiff);
            AddToggle("PetSquashConfig", ItemID.MagicalPumpkinSeed);
            AddToggle("PetGliderConfig", ItemID.EucaluptusSap);
            AddToggle("PetTikiConfig", ItemID.TikiTotem);
            AddToggle("PetTurtleConfig", ItemID.Seaweed);
            AddToggle("PetVoltConfig", ItemID.LightningCarrot);
            AddToggle("PetZephyrConfig", ItemID.ZephyrFish);

            AddToggle("PetOrbConfig", ItemID.ShadowOrb);
            AddToggle("PetHeartConfig", ItemID.CrimsonHeart);
            AddToggle("PetLanternConfig", ItemID.MagicLantern);
            AddToggle("PetNaviConfig", ItemID.FairyBell);
            AddToggle("PetFlickerConfig", ItemID.DD2OgrePetItem);
            AddToggle("PetWispConfig", ItemID.WispinaBottle);
            AddToggle("PetSuspEyeConfig", ItemID.SuspiciousLookingTentacle);

            AddToggle("PetKSConfig", ItemID.KingSlimePetItem);
            AddToggle("PetEoCConfig", ItemID.EyeOfCthulhuPetItem);
            AddToggle("PetEoWConfig", ItemID.EaterOfWorldsPetItem);
            AddToggle("PetBoCConfig", ItemID.BrainOfCthulhuPetItem);
            AddToggle("PetDeerConfig", ItemID.DeerclopsPetItem);
            AddToggle("PetQBConfig", ItemID.QueenBeePetItem);
            AddToggle("PetSkeleConfig", ItemID.SkeletronPetItem);
            AddToggle("PetQSConfig", ItemID.QueenSlimePetItem);
            AddToggle("PetDestroyerConfig", ItemID.DestroyerPetItem);
            AddToggle("PetTwinsConfig", ItemID.TwinsPetItem);
            AddToggle("PetSkelePrimeConfig", ItemID.SkeletronPrimePetItem);

            AddToggle("PetOgreConfig", ItemID.DD2OgrePetItem);
            AddToggle("PetPlanteraConfig", ItemID.PlanteraPetItem);
            AddToggle("PetPumpkingConfig", ItemID.PumpkingPetItem);
            AddToggle("PetEverscreamConfig", ItemID.EverscreamPetItem);
            AddToggle("PetIceQueenConfig", ItemID.IceQueenPetItem);
            AddToggle("PetDukeConfig", ItemID.DukeFishronPetItem);
            AddToggle("PetGolemConfig", ItemID.GolemPetItem);
            AddToggle("PetEoLConfig", ItemID.FairyQueenPetItem);
            AddToggle("PetBetsyConfig", ItemID.DD2BetsyPetItem);
            AddToggle("PetMartianConfig", ItemID.MartianPetItem);
            AddToggle("PetLCConfig", ItemID.LunaticCultistPetItem);
            AddToggle("PetMLConfig", ItemID.MoonLordPetItem);


        #endregion pet toggles
        
            #endregion Toggles


        }
    }
}
