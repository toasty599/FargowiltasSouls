using FargowiltasSouls.Items;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Accessories.Souls;
using FargowiltasSouls.Items.Armor;
using FargowiltasSouls.Items.Consumables;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Placeables;
using FargowiltasSouls.Items.Summons;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
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

            void AddToggle(string toggle, string name, int item, string color = "ffffff")
            {
                ModTranslation text = LocalizationLoader.CreateTranslation(Instance, toggle);
                text.SetDefault($"[i:{item}] [c/{color}:{name}]");
                LocalizationLoader.AddTranslation(text);
            }

            #endregion helpers


            Add("Message.MasochistExtraTooltip", $"[i:{ModContent.ItemType<MutantsPact>()}]Enables Masochist Mode when used in Master Mode");


            #region boss spawn info

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

            #endregion boss spawn info


            #region Toggles

            //AddToggle("PresetHeader", "Preset Configurations", ModContent.ItemType<Masochist>());

            #region enchants

            AddToggle("WoodHeader", "Force of Timber", ModContent.ItemType<TimberForce>());
            AddToggle("BorealConfig", "Boreal Snowballs", ModContent.ItemType<BorealWoodEnchant>(), "8B7464");
            AddToggle("MahoganyConfig", "Mahogany Hook Speed", ModContent.ItemType<RichMahoganyEnchant>(), "b56c64");
            AddToggle("EbonConfig", "Ebonwood Shadowflame", ModContent.ItemType<EbonwoodEnchant>(), "645a8d");
            AddToggle("ShadeConfig", "Blood Geyser On Hit", ModContent.ItemType<ShadewoodEnchant>(), "586876");
            AddToggle("ShadeOnHitConfig", "Proximity Triggers On Hit Effects", ModContent.ItemType<ShadewoodEnchant>(), "586876");
            AddToggle("PalmConfig", "Palmwood Sentry", ModContent.ItemType<PalmWoodEnchant>(), "b78d56");
            AddToggle("PearlConfig", "Pearlwood Rain", ModContent.ItemType<PearlwoodEnchant>(), "ad9a5f");

            AddToggle("EarthHeader", "Force of Earth", ModContent.ItemType<EarthForce>());
            AddToggle("AdamantiteConfig", "Adamantite Projectile Splitting", ModContent.ItemType<AdamantiteEnchant>(), "dd557d");
            AddToggle("CobaltConfig", "Cobalt Shards", ModContent.ItemType<CobaltEnchant>(), "3da4c4");
            AddToggle("AncientCobaltConfig", "Ancient Cobalt Stingers", ModContent.ItemType<AncientCobaltEnchant>(), "354c74");
            AddToggle("MythrilConfig", "Mythril Weapon Speed", ModContent.ItemType<MythrilEnchant>(), "9dd290");
            AddToggle("OrichalcumConfig", "Orichalcum Petals", ModContent.ItemType<OrichalcumEnchant>(), "eb3291");
            AddToggle("PalladiumConfig", "Palladium Healing", ModContent.ItemType<PalladiumEnchant>(), "f5ac28");
            AddToggle("PalladiumOrbConfig", "Palladium Orbs", ModContent.ItemType<PalladiumEnchant>(), "f5ac28");
            AddToggle("TitaniumConfig", "Titanium Shadow Dodge", ModContent.ItemType<TitaniumEnchant>(), "828c88");

            AddToggle("TerraHeader", "Terra Force", ModContent.ItemType<TerraForce>());
            AddToggle("CopperConfig", "Copper Lightning", ModContent.ItemType<CopperEnchant>(), "d56617");
            AddToggle("IronMConfig", "Iron Magnet", ModContent.ItemType<IronEnchant>(), "988e83");
            AddToggle("IronSConfig", "Iron Parry", ModContent.ItemType<IronEnchant>(), "988e83");
            AddToggle("TinConfig", "Tin Crits", ModContent.ItemType<TinEnchant>(), "a28b4e");
            AddToggle("TungstenConfig", "Tungsten Item Effect", ModContent.ItemType<TungstenEnchant>(), "b0d2b2");
            AddToggle("TungstenProjConfig", "Tungsten Projectile Effect", ModContent.ItemType<TungstenEnchant>(), "b0d2b2");
            AddToggle("ObsidianConfig", "Obsidian Explosions", ModContent.ItemType<ObsidianEnchant>(), "453e73");

            AddToggle("WillHeader", "Force of Will", ModContent.ItemType<WillForce>());
            AddToggle("GladiatorConfig", "Gladiator Rain", ModContent.ItemType<GladiatorEnchant>(), "9c924e");
            AddToggle("GoldConfig", "Gold Lucky Coin", ModContent.ItemType<GoldEnchant>(), "e7b21c");
            AddToggle("HuntressConfig", "Huntress Ability", ModContent.ItemType<HuntressEnchant>(), "7ac04c");
            AddToggle("ValhallaConfig", "Squire/Valhalla Healing", ModContent.ItemType<ValhallaKnightEnchant>(), "93651e");
            AddToggle("SquirePanicConfig", "Ballista Panic On Hit", ModContent.ItemType<SquireEnchant>(), "948f8c");

            AddToggle("LifeHeader", "Force of Life", ModContent.ItemType<LifeForce>());
            AddToggle("BeeConfig", "Bees", ModContent.ItemType<BeeEnchant>(), "FEF625");
            AddToggle("BeetleConfig", "Beetles", ModContent.ItemType<BeetleEnchant>(), "6D5C85");
            AddToggle("CactusConfig", "Cactus Needles", ModContent.ItemType<CactusEnchant>(), "799e1d");
            AddToggle("PumpkinConfig", "Grow Pumpkins", ModContent.ItemType<PumpkinEnchant>(), "e3651c");
            AddToggle("SpiderConfig", "Spider Crits", ModContent.ItemType<SpiderEnchant>(), "6d4e45");
            AddToggle("TurtleConfig", "Turtle Shell Buff", ModContent.ItemType<TurtleEnchant>(), "f89c5c");

            AddToggle("NatureHeader", "Force of Nature", ModContent.ItemType<NatureForce>());
            AddToggle("ChlorophyteConfig", "Chlorophyte Leaf Crystal", ModContent.ItemType<ChlorophyteEnchant>(), "248900");
            AddToggle("CrimsonConfig", "Crimson Regen", ModContent.ItemType<CrimsonEnchant>(), "C8364B");
            AddToggle("RainConfig", "Rain Clouds", ModContent.ItemType<RainEnchant>(), "ffec00");
            AddToggle("FrostConfig", "Frost Icicles", ModContent.ItemType<FrostEnchant>(), "7abdb9");
            AddToggle("SnowConfig", "Snowstorm", ModContent.ItemType<SnowEnchant>(), "25c3f2");
            AddToggle("JungleConfig", "Jungle Spores", ModContent.ItemType<JungleEnchant>(), "71971f");
            AddToggle("JungleDashConfig", "Jungle Dash", ModContent.ItemType<JungleEnchant>(), "71971f");
            AddToggle("MoltenConfig", "Molten Inferno Buff", ModContent.ItemType<MoltenEnchant>(), "c12b2b");
            AddToggle("MoltenEConfig", "Molten Explosion On Hit", ModContent.ItemType<MoltenEnchant>(), "c12b2b");
            AddToggle("ShroomiteConfig", "Shroomite Stealth", ModContent.ItemType<ShroomiteEnchant>(), "008cf4");
            AddToggle("ShroomiteShroomConfig", "Shroomite Mushrooms", ModContent.ItemType<ShroomiteEnchant>(), "008cf4");

            AddToggle("ShadowHeader", "Shadow Force", ModContent.ItemType<ShadowForce>());
            AddToggle("DarkArtConfig", "Flameburst Minion", ModContent.ItemType<DarkArtistEnchant>(), "9b5cb0");
            AddToggle("ApprenticeConfig", "Apprentice Effect", ModContent.ItemType<ApprenticeEnchant>(), "5d86a6");
            AddToggle("NecroConfig", "Necro Graves", ModContent.ItemType<NecroEnchant>(), "565643");
            AddToggle("NecroGloveConfig", "Necro Bone Glove", ModContent.ItemType<NecroEnchant>(), "565643");
            AddToggle("ShadowConfig", "Shadow Orbs", ModContent.ItemType<ShadowEnchant>(), "42356f");
            AddToggle("AncientShadowConfig", "Ancient Shadow Darkness", ModContent.ItemType<AncientShadowEnchant>(), "42356f");
            AddToggle("MonkConfig", "Monk Dash", ModContent.ItemType<MonkEnchant>(), "920520");
            AddToggle("ShinobiDashConfig", "Shinobi Teleport Dash", ModContent.ItemType<ShinobiEnchant>(), "935b18");
            AddToggle("ShinobiConfig", "Shinobi Through Walls", ModContent.ItemType<ShinobiEnchant>(), "935b18");
            AddToggle("SpookyConfig", "Spooky Scythes", ModContent.ItemType<SpookyEnchant>(), "644e74");

            AddToggle("SpiritHeader", "Force of Spirit", ModContent.ItemType<SpiritForce>());
            AddToggle("FossilConfig", "Fossil Bones On Hit", ModContent.ItemType<FossilEnchant>(), "8c5c3b");
            AddToggle("ForbiddenConfig", "Forbidden Storm", ModContent.ItemType<ForbiddenEnchant>(), "e7b21c");
            AddToggle("HallowDodgeConfig", "Hallowed Dodge", ModContent.ItemType<HallowEnchant>(), "968564");
            AddToggle("HallowedConfig", "Ancient Hallowed Enchanted Sword Familiar", ModContent.ItemType<AncientHallowEnchant>(), "968564");
            AddToggle("HallowSConfig", "Ancient Hallowed Shield", ModContent.ItemType<AncientHallowEnchant>(), "968564");
            AddToggle("SilverConfig", "Silver Sword Familiar", ModContent.ItemType<SilverEnchant>(), "b4b4cc");
            AddToggle("SilverSpeedConfig", "Silver Minion Speed", ModContent.ItemType<SilverEnchant>(), "b4b4cc");
            AddToggle("SpectreConfig", "Spectre Orbs", ModContent.ItemType<SpectreEnchant>(), "accdfc");
            AddToggle("TikiConfig", "Tiki Minions", ModContent.ItemType<TikiEnchant>(), "56A52B");

            AddToggle("CosmoHeader", "Force of Cosmos", ModContent.ItemType<CosmoForce>());
            AddToggle("MeteorConfig", "Meteor Shower", ModContent.ItemType<MeteorEnchant>(), "5f4752");
            AddToggle("NebulaConfig", "Nebula Boosters", ModContent.ItemType<NebulaEnchant>(), "fe7ee5");
            AddToggle("SolarConfig", "Solar Shield", ModContent.ItemType<SolarEnchant>(), "fe9e23");
            AddToggle("SolarFlareConfig", "Inflict Solar Flare", ModContent.ItemType<SolarEnchant>(), "fe9e23");
            AddToggle("StardustConfig", "Stardust Guardian", ModContent.ItemType<StardustEnchant>(), "00aeee");
            AddToggle("VortexSConfig", "Vortex Stealth", ModContent.ItemType<VortexEnchant>(), "00f2aa");
            AddToggle("VortexVConfig", "Vortex Voids", ModContent.ItemType<VortexEnchant>(), "00f2aa");

            #endregion enchants

            #region masomode toggles

            //Masomode Header
            AddToggle("MasoHeader", "Eternity Mode", ModContent.ItemType<MutantStatue>());
            AddToggle("MasoBossRecolors", "Boss Recolors", ModContent.ItemType<Masochist>());
            AddToggle("MasoCanPlay", "Masochist Mode Available", ModContent.ItemType<Masochist>(), "ff0000");

            AddToggle("MasoHeader2", "Eternity Mode Accessories", ModContent.ItemType<DeviatingEnergy>());
            AddToggle("MasoAeolusConfig", "Aeolus Jump", ModContent.ItemType<AeolusBoots>());
            AddToggle("MasoAeolusFlowerConfig", "Flower Boots", ModContent.ItemType<AeolusBoots>());
            AddToggle("MasoIconConfig", "Sinister Icon Spawn Rates", ModContent.ItemType<SinisterIcon>());
            AddToggle("MasoIconDropsConfig", "Sinister Icon Drops", ModContent.ItemType<SinisterIcon>());
            AddToggle("MasoGrazeConfig", "Graze", ModContent.ItemType<SparklingAdoration>());
            AddToggle("MasoGrazeRingConfig", "Graze Radius Visual", ModContent.ItemType<SparklingAdoration>());
            AddToggle("MasoDevianttHeartsConfig", "Homing Hearts On Hit", ModContent.ItemType<SparklingAdoration>());
            AddToggle("DreadShellParryConfig", "Dread Shell Parry", ModContent.ItemType<DreadShell>());
            AddToggle("PrecisionSealHurtboxConfig", "Reduced Hurtbox Size", ModContent.ItemType<PrecisionSeal>());

            //supreme death fairy header
            AddToggle("SupremeFairyHeader", "Supreme Deathbringer Fairy", ModContent.ItemType<SupremeDeathbringerFairy>());
            AddToggle("MasoSlimeConfig", "Slimy Balls", ModContent.ItemType<SlimyShield>());
            AddToggle("SlimeFallingConfig", "Increased Fall Speed", ModContent.ItemType<SlimyShield>());
            AddToggle("MasoEyeConfig", "Scythes When Dashing", ModContent.ItemType<AgitatingLens>());
            AddToggle("MasoHoneyConfig", "Honey When Hitting Enemies", ModContent.ItemType<QueenStinger>());
            AddToggle("MasoSkeleConfig", "Skeletron Arms Minion", ModContent.ItemType<NecromanticBrew>());

            //bionomic
            AddToggle("BionomicHeader", "Bionomic Cluster", ModContent.ItemType<BionomicCluster>());
            AddToggle("MasoConcoctionConfig", "Tim's Concoction", ModContent.ItemType<TimsConcoction>());
            AddToggle("MasoCarrotConfig", "Carrot View", ModContent.ItemType<OrdinaryCarrot>());
            AddToggle("MasoRainbowConfig", "Rainbow Slime Minion", ModContent.ItemType<ConcentratedRainbowMatter>());
            AddToggle("MasoFrigidConfig", "Frostfireballs", ModContent.ItemType<FrigidGemstone>());
            AddToggle("MasoNymphConfig", "Attacks Spawn Hearts", ModContent.ItemType<NymphsPerfume>());
            AddToggle("MasoSqueakConfig", "Squeaky Toy On Hit", ModContent.ItemType<Items.Accessories.Masomode.SqueakyToy>());
            AddToggle("MasoPouchConfig", "Shadowflame Tentacles", ModContent.ItemType<WretchedPouch>());
            AddToggle("MasoClippedConfig", "Inflict Clipped Wings", ModContent.ItemType<WyvernFeather>());
            AddToggle("TribalCharmConfig", "Tribal Charm Auto Swing", ModContent.ItemType<TribalCharm>());
            //AddToggle("WalletHeader", "Security Wallet", ModContent.ItemType<SecurityWallet>());

            //dubious
            AddToggle("DubiousHeader", "Dubious Circuitry", ModContent.ItemType<DubiousCircuitry>());
            AddToggle("MasoLightningConfig", "Inflict Lightning Rod", ModContent.ItemType<GroundStick>());
            AddToggle("MasoProbeConfig", "Probes Minion", ModContent.ItemType<GroundStick>());

            //pure heart
            AddToggle("PureHeartHeader", "Pure Heart", ModContent.ItemType<PureHeart>());
            AddToggle("MasoEaterConfig", "Tiny Eaters", ModContent.ItemType<CorruptHeart>());
            AddToggle("MasoBrainConfig", "Creeper Shield", ModContent.ItemType<GuttedHeart>());

            //lump of flesh
            AddToggle("LumpofFleshHeader", "Lump of Flesh", ModContent.ItemType<LumpOfFlesh>());
            AddToggle("MasoPugentConfig", "Pungent Eye Minion", ModContent.ItemType<LumpOfFlesh>());

            //chalice
            AddToggle("ChaliceHeader", "Chalice of the Moon", ModContent.ItemType<ChaliceoftheMoon>());
            AddToggle("MasoCultistConfig", "Cultist Minion", ModContent.ItemType<ChaliceoftheMoon>());
            AddToggle("MasoPlantConfig", "Plantera Minion", ModContent.ItemType<MagicalBulb>());
            AddToggle("MasoGolemConfig", "Lihzahrd Ground Pound", ModContent.ItemType<LihzahrdTreasureBox>());
            AddToggle("MasoBoulderConfig", "Boulder Spray", ModContent.ItemType<LihzahrdTreasureBox>());
            AddToggle("MasoCelestConfig", "Celestial Rune Support", ModContent.ItemType<CelestialRune>());
            AddToggle("MasoVisionConfig", "Ancient Visions On Hit", ModContent.ItemType<CelestialRune>());

            //heart of the masochist
            AddToggle("HeartHeader", "Heart of the Eternal", ModContent.ItemType<HeartoftheMasochist>());
            AddToggle("MasoPumpConfig", "Pumpking's Cape Support", ModContent.ItemType<PumpkingsCape>());
            AddToggle("IceQueensCrownConfig", "Freeze On Hit", ModContent.ItemType<IceQueensCrown>());
            AddToggle("MasoFlockoConfig", "Flocko Minion", ModContent.ItemType<IceQueensCrown>());
            AddToggle("MasoUfoConfig", "Saucer Minion", ModContent.ItemType<SaucerControlConsole>());
            AddToggle("MasoGravConfig", "Gravity Control", ModContent.ItemType<GalacticGlobe>());
            AddToggle("MasoGrav2Config", "Stabilized Gravity", ModContent.ItemType<GalacticGlobe>());
            AddToggle("MasoTrueEyeConfig", "True Eyes Minion", ModContent.ItemType<GalacticGlobe>());

            //cyclonic fin
            AddToggle("CyclonicHeader", "Abominable Wand", ModContent.ItemType<AbominableWand>());
            AddToggle("MasoFishronConfig", "Spectral Abominationn", ModContent.ItemType<AbominableWand>());

            //mutant armor
            AddToggle("MutantArmorHeader", "True Mutant Armor", ModContent.ItemType<HeartoftheMasochist>());
            AddToggle("MasoAbomConfig", "Abominationn Minion", ModContent.ItemType<MutantMask>());
            AddToggle("MasoRingConfig", "Phantasmal Ring Minion", ModContent.ItemType<MutantMask>());
            AddToggle("MasoReviveDeathrayConfig", "Deathray When Revived", ModContent.ItemType<MutantMask>());

            #endregion masomode toggles

            #region soul toggles

            AddToggle("UniverseHeader", "Soul of the Universe", ModContent.ItemType<UniverseSoul>());
            AddToggle("MeleeConfig", "Melee Speed", ModContent.ItemType<BerserkerSoul>());
            AddToggle("MagmaStoneConfig", "Magma Stone", ModContent.ItemType<BerserkerSoul>());
            AddToggle("YoyoBagConfig", "Yoyo Bag", ModContent.ItemType<BerserkerSoul>());
            AddToggle("MoonCharmConfig", "Moon Charm", ModContent.ItemType<BerserkerSoul>());
            AddToggle("NeptuneShellConfig", "Neptune's Shell", ModContent.ItemType<BerserkerSoul>());
            AddToggle("SniperConfig", "Sniper Scope", ModContent.ItemType<SnipersSoul>());
            AddToggle("UniverseConfig", "Universe Attack Speed", ModContent.ItemType<UniverseSoul>());

            AddToggle("WorldShaperHeader", "World Shaper Soul", ModContent.ItemType<WorldShaperSoul>());
            AddToggle("MiningHuntConfig", "Mining Hunter Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningDangerConfig", "Mining Dangersense Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningSpelunkConfig", "Mining Spelunker Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningShineConfig", "Mining Shine Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("BuilderConfig", "Builder Mode", ModContent.ItemType<WorldShaperSoul>());

            AddToggle("ColossusHeader", "Colossus Soul", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseStarConfig", "Stars On Hit", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseBrainConfig", "Brain of Confusion", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseBeeConfig", "Bees On Hit", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefensePanicConfig", "Panic On Hit", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseFleshKnuckleConfig", "Flesh Knuckles Aggro", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefensePaladinConfig", "Paladin's Shield", ModContent.ItemType<ColossusSoul>());

            AddToggle("FlightMasteryHeader", "Flight Mastery Soul", ModContent.ItemType<FlightMasterySoul>());
            AddToggle("FlightMasteryInsigniaConfig", "Soaring Insignia Acceleration", ModContent.ItemType<FlightMasterySoul>());
            AddToggle("FlightMasteryGravityConfig", "Amplified Gravity", ModContent.ItemType<FlightMasterySoul>());

            AddToggle("SupersonicHeader", "Supersonic Soul", ModContent.ItemType<SupersonicSoul>());
            AddToggle("RunSpeedConfig", "Higher Base Run Speed", ModContent.ItemType<SupersonicSoul>());
            AddToggle("MomentumConfig", "No Momentum", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicTabiConfig", "Tabi Dash", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicClimbingConfig", "Tiger Climbing Gear", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicConfig", "Supersonic Speed Boosts", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicJumpsConfig", "Supersonic Jumps", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicRocketBootsConfig", "Supersonic Rocket Boots", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicCarpetConfig", "Supersonic Carpet", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicFlowerConfig", "Supersonic Flower Boots", ModContent.ItemType<SupersonicSoul>());
            AddToggle("CthulhuShieldConfig", "Shield of Cthulhu", ModContent.ItemType<SupersonicSoul>());
            AddToggle("BlackBeltConfig", "Black Belt", ModContent.ItemType<SupersonicSoul>());

            AddToggle("TrawlerHeader", "Trawler Soul", ModContent.ItemType<TrawlerSoul>());
            AddToggle("TrawlerSporeConfig", "Spore Sac", ModContent.ItemType<TrawlerSoul>());
            AddToggle("TrawlerConfig", "Trawler Extra Lures", ModContent.ItemType<TrawlerSoul>());
            AddToggle("TrawlerJumpConfig", "Trawler Jump", ModContent.ItemType<TrawlerSoul>());

            AddToggle("EternityHeader", "Soul of Eternity", ModContent.ItemType<EternitySoul>());
            AddToggle("EternityConfig", "Eternity Stacking", ModContent.ItemType<EternitySoul>());

            #endregion soul toggles

            #region pet toggles

            AddToggle("PetHeader", "Pets", ItemID.ZephyrFish);
            AddToggle("PetBlackCatConfig", "Black Cat Pet", ItemID.UnluckyYarn);
            AddToggle("PetCompanionCubeConfig", "Companion Cube Pet", ItemID.CompanionCube);
            AddToggle("PetCursedSaplingConfig", "Cursed Sapling Pet", ItemID.CursedSapling);
            AddToggle("PetDinoConfig", "Dino Pet", ItemID.AmberMosquito);
            AddToggle("PetDragonConfig", "Dragon Pet", ItemID.DD2PetDragon);
            AddToggle("PetEaterConfig", "Eater Pet", ItemID.EatersBone);
            AddToggle("PetEyeSpringConfig", "Eye Spring Pet", ItemID.EyeSpring);
            AddToggle("PetFaceMonsterConfig", "Face Monster Pet", ItemID.BoneRattle);
            AddToggle("PetGatoConfig", "Gato Pet", ItemID.DD2PetGato);
            AddToggle("PetHornetConfig", "Hornet Pet", ItemID.Nectar);
            AddToggle("PetLizardConfig", "Lizard Pet", ItemID.LizardEgg);
            AddToggle("PetMinitaurConfig", "Mini Minotaur Pet", ItemID.TartarSauce);
            AddToggle("PetParrotConfig", "Parrot Pet", ItemID.ParrotCracker);
            AddToggle("PetPenguinConfig", "Penguin Pet", ItemID.Fish);
            AddToggle("PetPupConfig", "Puppy Pet", ItemID.DogWhistle);
            AddToggle("PetSeedConfig", "Seedling Pet", ItemID.Seedling);
            AddToggle("PetDGConfig", "Skeletron Pet", ItemID.BoneKey);
            AddToggle("PetSnowmanConfig", "Snowman Pet", ItemID.ToySled);
            AddToggle("PetGrinchConfig", "Grinch Pet", ItemID.BabyGrinchMischiefWhistle);
            AddToggle("PetSpiderConfig", "Spider Pet", ItemID.SpiderEgg);
            AddToggle("PetSquashConfig", "Squashling Pet", ItemID.MagicalPumpkinSeed);
            AddToggle("PetTikiConfig", "Tiki Pet", ItemID.TikiTotem);
            AddToggle("PetShroomConfig", "Truffle Pet", ItemID.StrangeGlowingMushroom);
            AddToggle("PetTurtleConfig", "Turtle Pet", ItemID.Seaweed);
            AddToggle("PetZephyrConfig", "Zephyr Fish Pet", ItemID.ZephyrFish);
            AddToggle("PetHeartConfig", "Crimson Heart Pet", ItemID.CrimsonHeart);
            AddToggle("PetNaviConfig", "Fairy Pet", ItemID.FairyBell);
            AddToggle("PetFlickerConfig", "Flickerwick Pet", ItemID.DD2OgrePetItem);
            AddToggle("PetLanternConfig", "Magic Lantern Pet", ItemID.MagicLantern);
            AddToggle("PetOrbConfig", "Shadow Orb Pet", ItemID.ShadowOrb);
            AddToggle("PetSuspEyeConfig", "Suspicious Eye Pet", ItemID.SuspiciousLookingTentacle);
            AddToggle("PetWispConfig", "Wisp Pet", ItemID.WispinaBottle);

            #endregion pet toggles

            #region patreon toggles
            AddToggle("PatreonHeader", "Patreon Items", ModContent.ItemType<Patreon.Gittle.RoombaPet>());
            AddToggle("PatreonRoomba", "Roomba", ModContent.ItemType<Patreon.Gittle.RoombaPet>());
            AddToggle("PatreonOrb", "Computation Orb", ModContent.ItemType<Patreon.Daawnz.ComputationOrb>());
            AddToggle("PatreonFishingRod", "Miss Drakovi's Fishing Pole", ModContent.ItemType<Patreon.Sasha.MissDrakovisFishingPole>());
            AddToggle("PatreonDoor", "Squidward Door", ModContent.ItemType<Patreon.Sam.SquidwardDoor>());
            AddToggle("PatreonWolf", "Paradox Wolf Soul", ModContent.ItemType<Patreon.ParadoxWolf.ParadoxWolfSoul>());
            AddToggle("PatreonDove", "Fig Branch", ModContent.ItemType<Patreon.ManliestDove.FigBranch>());
            AddToggle("PatreonKingSlime", "Medallion of the Fallen King", ModContent.ItemType<Patreon.Catsounds.MedallionoftheFallenKing>());
            AddToggle("PatreonFishron", "Staff Of Unleashed Ocean", ModContent.ItemType<Patreon.DemonKing.StaffOfUnleashedOcean>());
            AddToggle("PatreonPlant", "Piranha Plant Voodoo Doll", ModContent.ItemType<Patreon.LaBonez.PiranhaPlantVoodooDoll>());
            AddToggle("PatreonDevious", "Devious Aestheticus", ModContent.ItemType<Patreon.DevAesthetic.DeviousAestheticus>());
            AddToggle("PatreonVortex", "Vortex Ritual", ModContent.ItemType<Patreon.GreatestKraken.VortexMagnetRitual>());
            AddToggle("PatreonPrime", "Prime Staff", ModContent.ItemType<Patreon.Purified.PrimeStaff>());
            AddToggle("PatreonCrimetroid", "Crimetroid", ModContent.ItemType<Patreon.Shucks.CrimetroidEgg>());
            #endregion patreon toggles

            #endregion Toggles


        }
    }
}
