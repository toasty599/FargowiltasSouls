using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public class EternityAdvisor : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eternity Advisor");
            // Tooltip.SetDefault("Suggests loadouts for Eternity Mode based on progression");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(0, 1);
        }

        public override bool CanUseItem(Player player) => WorldSavingSystem.EternityMode;

        private static string GetBuildText(params int[] args)
        {
            string text = "";
            foreach (int itemType in args)
            {
                if (itemType != -1)
                {
                    text += $"[i:{itemType}]";
                }
                
            }
            return text;
        }

        private static string GetBuildTextRandom(params int[] args) //takes number of accs to use as first param and list of accs as the rest
        {
            List<int> choices = new();
            int maxSize = args.Length - 1;
            for (int i = 0; i < args[0]; i++)
            {
                int attempt = Main.rand.Next(maxSize) + 1; //skip the first number
                if (choices.Contains(args[attempt]) || args[attempt] == -1) //if already chose this acc or is -1, try to choose the next in line
                {
                    for (int j = 0; j < maxSize; j++)
                    {
                        if (++attempt >= maxSize) //wrap around at end of array
                            attempt = 1;
                        if (!choices.Contains(args[attempt]))
                            break;
                    }
                }
                choices.Add(args[attempt]);
            }
            return GetBuildText(choices.ToArray());
        }

        private int GetBossHelp(ref string build, Player player)
        {
            int summonType = -1;
            bool playerMelee = HighestDamageClass(player) == DamageClass.Melee;
            bool playerRanged = HighestDamageClass(player) == DamageClass.Melee;
            bool playerMage = HighestDamageClass(player) == DamageClass.Melee;
            bool playerSummoner = HighestDamageClass(player) == DamageClass.Melee;

            if (!WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TrojanSquirrel])
            {
                summonType = ModContent.ItemType<SquirrelCoatofArms>();
                build = GetBuildText(
                    ModContent.ItemType<EurusSock>(),
                    ModContent.ItemType<PuffInABottle>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    //ModContent.ItemType<PumpkinEnchant>(),
                    //ModContent.ItemType<PalmWoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>()
                );
            }
            else if (!NPC.downedSlimeKing)
            {
                summonType = ItemID.SlimeCrown;
                build = GetBuildText(
                    ModContent.ItemType<EurusSock>(),
                    ModContent.ItemType<PuffInABottle>()
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    ModContent.ItemType<PumpkinEnchant>(),
                    ModContent.ItemType<ShadewoodEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<CactusEnchant>()
                );
            }
            else if (!NPC.downedBoss1)
            {
                summonType = ItemID.SuspiciousLookingEye;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ItemID.HermesBoots, ItemID.SailfishBoots, ItemID.FlurryBoots, ItemID.RocketBoots, ItemID.SpectreBoots }),
                    Main.rand.Next(new int[] { ItemID.CloudinaBalloon, ItemID.SharkronBalloon, ItemID.SandstorminaBalloon, ItemID.BlizzardinaBalloon })
                ) + GetBuildTextRandom(
                    3,
                    ItemID.CharmofMyths,
                    ModContent.ItemType<NinjaEnchant>(),
                    ModContent.ItemType<LeadEnchant>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    ModContent.ItemType<ShadewoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>(),
                    ModContent.ItemType<PalmWoodEnchant>(),
                    ModContent.ItemType<TungstenEnchant>()
                );
            }
            else if (!NPC.downedBoss2)
            {
                summonType = WorldGen.crimson ? ItemID.BloodySpine : ItemID.WormFood;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ItemID.SpectreBoots, ItemID.LightningBoots, ItemID.FrostsparkBoots }),
                    Main.rand.Next(new int[] { ItemID.BalloonHorseshoeFart, ItemID.BalloonHorseshoeSharkron, ItemID.WhiteHorseshoeBalloon }),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<MeteorEnchant>() })
                ) + GetBuildTextRandom(
                    2,
                    ModContent.ItemType<LeadEnchant>(),
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<CactusEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<CopperEnchant>()
                );
            }
            else if (!NPC.downedQueenBee)
            {
                summonType = ItemID.Abeemination;
                build = GetBuildText(
                    Main.rand.NextBool() ? ItemID.FrostsparkBoots : ItemID.LightningBoots,
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<MeteorEnchant>() }),
                    ItemID.Bezoar,
                    Main.rand.Next(new int[] { ItemID.BalloonHorseshoeFart, ItemID.BalloonHorseshoeSharkron, ItemID.WhiteHorseshoeBalloon }),
                    Main.rand.Next(new int[] {
                                ModContent.ItemType<RainEnchant>(),
                                ModContent.ItemType<TungstenEnchant>(),
                                ModContent.ItemType<ShadowEnchant>(),
                                ModContent.ItemType<ShadewoodEnchant>(),
                                ModContent.ItemType<NinjaEnchant>()
                    })
                );
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "CityBuster").Type}]";
            }
            else if (!NPC.downedBoss3)
            {
                summonType = ModContent.TryFind("Fargowiltas", "SuspiciousSkull", out ModItem modItem) ? modItem.Type : ItemID.SkeletronMask;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ItemID.FrostsparkBoots, ItemID.TerrasparkBoots }),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<QueenStinger>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.BundleofBalloons, ItemID.HorseshoeBundle, ModContent.ItemType<BeeEnchant>() })
                    ) + GetBuildTextRandom(
                    2,
                    ModContent.ItemType<SkullCharm>(),
                    ModContent.ItemType<ShadowEnchant>(),
                    ModContent.ItemType<TinEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<NinjaEnchant>(),
                    ModContent.ItemType<CrimsonEnchant>()
                );
            }
            else if (!NPC.downedDeerclops)
            {
                summonType = ModContent.TryFind("Fargowiltas", "DeerThing2", out ModItem modItem) ? modItem.Type : ItemID.DeerThing;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ItemID.FrostsparkBoots, ItemID.TerrasparkBoots,  }),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<QueenStinger>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.BundleofBalloons, ItemID.HorseshoeBundle, ModContent.ItemType<BeeEnchant>() })
                    ) + GetBuildTextRandom(
                    2,
                    ItemID.HandWarmer,
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<ShadowEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<TinEnchant>(),
                    ModContent.ItemType<NinjaEnchant>()
                );
            }
            else if (!WorldSavingSystem.DownedDevi)
            {
                summonType = ModContent.ItemType<DevisCurse>();
                build = GetBuildText(
                    Main.rand.Next(new int[] { ItemID.FrostsparkBoots, ItemID.TerrasparkBoots, }),
                    Main.rand.Next(new int[] { ItemID.EoCShield, ModContent.ItemType<JungleEnchant>(), ModContent.ItemType<QueenStinger>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.BundleofBalloons, ItemID.HorseshoeBundle, ModContent.ItemType<BeeEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<NymphsPerfume>(), ModContent.ItemType<ShadowEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<TinEnchant>(), ModContent.ItemType<NinjaEnchant>(), ModContent.ItemType<TungstenEnchant>() })
                    );
            }
            else if (!Main.hardMode)
            {
                summonType = ModContent.TryFind("Fargowiltas", "FleshyDoll", out ModItem modItem) ? modItem.Type : ItemID.GuideVoodooDoll;
                build = GetBuildText(
                    ModContent.ItemType<ZephyrBoots>(),
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<SparklingAdoration>() })
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<TinEnchant>(),
                    playerMelee ? ModContent.ItemType<TungstenEnchant>() : -1,
                    ModContent.ItemType<SkullCharm>(),
                    ModContent.ItemType<CopperEnchant>(),
                    ModContent.ItemType<NinjaEnchant>()
                );
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "DoubleObsidianInstabridge").Type}]";
            }
            else if (!NPC.downedQueenSlime)
            {
                summonType = ModContent.TryFind("Fargowiltas", "JellyCrystal", out ModItem modItem) ? modItem.Type : ItemID.QueenSlimeCrystal;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FrozenWings, ItemID.AngelWings, ModContent.ItemType<BeeEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<SparklingAdoration>() })
                    ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<AdamantiteEnchant>(),
                    playerMelee ? ModContent.ItemType<TungstenEnchant>() : -1,
                    ModContent.ItemType<PalladiumEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.BanishedBaron])
            {
                summonType = ModContent.ItemType<MechLure>();
                build = GetBuildText(
                    ModContent.ItemType<ZephyrBoots>(),
                    Main.rand.Next(new int[] { ItemID.FrozenWings, ItemID.AngelWings, ModContent.ItemType<GelicWings>(), ModContent.ItemType<BeeEnchant>() }),
                    Main.rand.Next(new int[] { ModContent.ItemType<CrystalAssassinEnchant>(), ModContent.ItemType<MeteorEnchant>() })
                    ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    HighestDamageClass(player) == DamageClass.Melee ? ModContent.ItemType<TungstenEnchant>() : -1,
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!NPC.downedMechBoss1)
            {
                summonType = ItemID.MechanicalWorm;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    playerMelee ? ModContent.ItemType<TungstenEnchant>() : -1,
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!NPC.downedMechBoss2)
            {
                summonType = ItemID.MechanicalEye;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    playerSummoner ? ModContent.ItemType<AncientHallowEnchant>() : ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!NPC.downedMechBoss3)
            {
                summonType = ItemID.MechanicalSkull;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<ZephyrBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    playerSummoner ? ModContent.ItemType<AncientHallowEnchant>() : ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>(),
                    ModContent.ItemType<FrostEnchant>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.Lifelight])
            {
                summonType = ModContent.ItemType<FragilePixieLamp>();
                build = GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<AeolusBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ItemID.BeeWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<ChlorophyteEnchant>(),
                    ModContent.ItemType<SquireEnchant>(),
                    ModContent.ItemType<SupremeDeathbringerFairy>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    playerSummoner ? ModContent.ItemType<AncientHallowEnchant>() : ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<DubiousCircuitry>(),
                    Main.rand.Next(new int[] { ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem })
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!NPC.downedPlantBoss)
            {
                summonType = ModContent.TryFind("Fargowiltas", "PlanterasFruit", out ModItem modItem) ? modItem.Type : ItemID.PlanteraMask;
                build = GetBuildText(
                    Main.rand.Next(new int[] { ModContent.ItemType<AeolusBoots>(), ModContent.ItemType<MeteorEnchant>() }),
                    Main.rand.Next(new int[] { ItemID.FlameWings, ItemID.FrozenWings, ItemID.BeeWings, ModContent.ItemType<GelicWings>() })
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<ForbiddenEnchant>(),
                    ModContent.ItemType<ChlorophyteEnchant>(),
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<AncientShadowEnchant>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<ApprenticeEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ModContent.ItemType<TitaniumEnchant>()
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "CityBuster").Type}]";
            }
            else if (!NPC.downedGolemBoss)
            {
                summonType = ItemID.LihzahrdPowerCell;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    Main.rand.Next(new int[] { ItemID.SpookyWings })
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.Next(new int[] { ItemID.MasterNinjaGear, ModContent.ItemType<MonkEnchant>(), ModContent.ItemType<ChlorophyteEnchant>(), ModContent.ItemType<MeteorEnchant>() }),
                    playerSummoner ? ModContent.ItemType<SpookyEnchant>() : -1,
                    playerSummoner ? ModContent.ItemType<TikiEnchant>() : -1,
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<CrimsonEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ModContent.ItemType<AncientHallowEnchant>(),
                    ModContent.ItemType<ForbiddenEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>()
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
                build += $"[i:{ModContent.Find<ModItem>("Fargowiltas", "LihzahrdInstactuationBomb").Type}]";
            }
            else if (!WorldSavingSystem.DownedBetsy)
            {
                summonType = ModContent.TryFind("Fargowiltas", "BetsyEgg", out ModItem modItem) ? modItem.Type : ItemID.BossMaskBetsy;
                build = GetBuildText(
                    ModContent.ItemType<AeolusBoots>(),
                    ItemID.BeetleWings,
                    ModContent.ItemType<LihzahrdTreasureBox>()
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.Next(new int[] { ItemID.MasterNinjaGear, ModContent.ItemType<MonkEnchant>(), ModContent.ItemType<ChlorophyteEnchant>(), ModContent.ItemType<MeteorEnchant>() }),
                    playerSummoner ? ModContent.ItemType<SpookyEnchant>() : ModContent.ItemType<DubiousCircuitry>(),
                    playerSummoner ? ModContent.ItemType<TikiEnchant>() : ModContent.ItemType<BeetleEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>(),
                    ModContent.ItemType<CrimsonEnchant>(),
                    ModContent.ItemType<HallowEnchant>()
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!NPC.downedFishron)
            {
                summonType = ModContent.TryFind("Fargowiltas", "TruffleWorm2", out ModItem modItem) ? modItem.Type : ItemID.TruffleWorm;
                build = GetBuildText(
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    ItemID.BetsyWings,
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<LihzahrdTreasureBox>(), ModContent.ItemType<BetsysHeart>(), ModContent.ItemType<MeteorEnchant>() })
                ) + GetBuildTextRandom(
                    2,
                    playerSummoner ? ModContent.ItemType<SpookyEnchant>() : ModContent.ItemType<ForbiddenEnchant>(),
                    playerSummoner ? ModContent.ItemType<TikiEnchant>() : -1,
                    ModContent.ItemType<DarkArtistEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>(),
                    ModContent.ItemType<PumpkingsCape>(),
                    ModContent.ItemType<BeetleEnchant>()
                ) 
                + GetBuildText(ModContent.ItemType<WizardEnchant>())
                + GetBuildText(ModContent.ItemType<RabiesVaccine>())
                + GetBuildText(ModContent.ItemType<BionomicCluster>());
            }
            else if (!NPC.downedEmpressOfLight)
            {
                summonType = ModContent.TryFind("Fargowiltas", "PrismaticPrimrose", out ModItem modItem) ? modItem.Type : ItemID.EmpressButterfly;
                build = GetBuildText(
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    Main.rand.Next(new int[] { ItemID.BetsyWings, ItemID.FishronWings }),
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<LihzahrdTreasureBox>(), ModContent.ItemType<BetsysHeart>(), ModContent.ItemType<MeteorEnchant>() })
                ) + GetBuildTextRandom(
                    4,
                    playerSummoner ? ModContent.ItemType<SpookyEnchant>() : ModContent.ItemType<ForbiddenEnchant>(),
                    playerSummoner ? ModContent.ItemType<TikiEnchant>() : -1,
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<DarkArtistEnchant>(),
                    ModContent.ItemType<BeetleEnchant>(),
                    ModContent.ItemType<SpectreEnchant>(),
                    ModContent.ItemType<RainEnchant>()
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!NPC.downedAncientCultist)
            {
                summonType = ModContent.TryFind("Fargowiltas", "CultistSummon", out ModItem modItem) ? modItem.Type : ItemID.BossMaskCultist;
                build = GetBuildText(
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    Main.rand.NextBool() ? ItemID.BetsyWings : ItemID.FishronWings,
                    ItemID.EmpressFlightBooster,
                    Main.rand.Next(new int[] { ModContent.ItemType<SupremeDeathbringerFairy>(), ModContent.ItemType<LihzahrdTreasureBox>(), ModContent.ItemType<BetsysHeart>(), ModContent.ItemType<MeteorEnchant>() })
                ) + GetBuildTextRandom(
                    3,
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<DarkArtistEnchant>(),
                    ModContent.ItemType<LumpOfFlesh>(),
                    ModContent.ItemType<BeetleEnchant>(),
                    ModContent.ItemType<SpectreEnchant>()
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!NPC.downedMoonlord)
            {
                summonType = ModContent.TryFind("Fargowiltas", "CelestialSigil2", out ModItem modItem) ? modItem.Type : ItemID.CelestialSigil;
                build = GetBuildText(
                    ModContent.ItemType<GaiaHelmet>(),
                    ModContent.ItemType<GaiaPlate>(),
                    ModContent.ItemType<GaiaGreaves>()
                ) + " " + GetBuildText(

                    Main.rand.NextBool() ? ItemID.BetsyWings : ItemID.FishronWings,
                    ItemID.EmpressFlightBooster,
                    ModContent.ItemType<ChaliceoftheMoon>()
                ) + GetBuildTextRandom(
                    3,
                    Main.rand.NextBool() ? ModContent.ItemType<AeolusBoots>() : ModContent.ItemType<ValhallaKnightEnchant>(),
                    playerMelee ? ModContent.ItemType<TungstenEnchant>() : -1,
                    ModContent.ItemType<DubiousCircuitry>(),
                    ModContent.ItemType<PrecisionSeal>(),
                    ModContent.ItemType<MutantAntibodies>(),
                    ModContent.ItemType<BeetleEnchant>()
                ) + GetBuildText(ModContent.ItemType<WizardEnchant>());
            }
            else if (!WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CosmosChampion])
            {
                summonType = ModContent.ItemType<SigilOfChampions>();
                build = GetBuildText(
                    ModContent.ItemType<FlightMasterySoul>(),
                    Main.rand.NextBool() ? ModContent.ItemType<SupersonicSoul>() : ModContent.ItemType<ColossusSoul>(),
                    playerMelee ? ModContent.ItemType<BerserkerSoul>() : -1,
                    playerRanged ? ModContent.ItemType<SnipersSoul>() : -1,
                    playerMage ? ModContent.ItemType<ArchWizardsSoul>() : -1,
                    playerSummoner ? ModContent.ItemType<ConjuristsSoul>() : -1
                    ) + GetBuildTextRandom(
                    4,
                    ModContent.ItemType<NebulaEnchant>(),
                    ModContent.ItemType<TerraForce>(),
                    playerSummoner ? ModContent.ItemType<SpiritForce>() : ModContent.ItemType<EarthForce>(),
                    ModContent.ItemType<ShadowForce>(),
                    ModContent.ItemType<NatureForce>()
                );
            }
            else if (!WorldSavingSystem.DownedAbom)
            {
                summonType = ModContent.ItemType<AbomsCurse>();
                build = GetBuildText(
                    ModContent.ItemType<FlightMasterySoul>(),
                    ModContent.ItemType<UniverseCore>(),
                    playerMelee ? ModContent.ItemType<BerserkerSoul>() : -1,
                    playerRanged ? ModContent.ItemType<SnipersSoul>() : -1,
                    playerMage ? ModContent.ItemType<ArchWizardsSoul>() : -1,
                    playerSummoner ? ModContent.ItemType<ConjuristsSoul>() : -1,
                    ModContent.ItemType<UniverseCore>(),
                    ModContent.ItemType<ColossusSoul>(),
                    playerSummoner ? ModContent.ItemType<SpiritForce>() : ModContent.ItemType<EarthForce>()
                ) + GetBuildTextRandom(
                    1,
                    ModContent.ItemType<SpiritForce>(),
                    ModContent.ItemType<NatureForce>()
                );
            }
            else if (!WorldSavingSystem.DownedMutant)
            {
                summonType = ModContent.ItemType<AbominationnVoodooDoll>();
                build = GetBuildText(
                    ModContent.ItemType<TerrariaSoul>(),
                    ModContent.ItemType<MasochistSoul>(),
                    ModContent.ItemType<UniverseSoul>(),
                    ModContent.ItemType<DimensionSoul>(),
                    playerMelee ? ModContent.ItemType<BerserkerSoul>() : -1,
                    playerRanged ? ModContent.ItemType<SnipersSoul>() : -1,
                    playerMage ? ModContent.ItemType<ArchWizardsSoul>() : -1,
                    playerSummoner ? ModContent.ItemType<ConjuristsSoul>() : -1,
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<AbominableWand>()
                );
            }
            else
            {
                summonType = ModContent.ItemType<MutantsCurse>();
                build = GetBuildText(
                    ModContent.ItemType<MutantMask>(),
                    ModContent.ItemType<MutantBody>(),
                    ModContent.ItemType<MutantPants>()
                ) + " " + GetBuildText(
                    ModContent.ItemType<EternitySoul>(),
                    ModContent.ItemType<MasochistSoul>(),
                    ModContent.ItemType<UniverseSoul>(),
                    Main.rand.Next(new int[] { ModContent.ItemType<BerserkerSoul>(), ModContent.ItemType<SnipersSoul>(), ModContent.ItemType<ArchWizardsSoul>(), ModContent.ItemType<ConjuristsSoul>() }),
                    ModContent.ItemType<SparklingAdoration>(),
                    ModContent.ItemType<AbominableWand>(),
                    ModContent.ItemType<MutantEye>()
                );
            }

            build += $" [i:{summonType}]";

            if (Main.hardMode)
            {
                if (!player.inventory.Any(i => !i.IsAir && i.type == ModContent.ItemType<BionomicCluster>())
                    && !player.armor.Any(i => !i.IsAir && i.type == ModContent.ItemType<BionomicCluster>())
                    && !player.armor.Any(i => !i.IsAir && i.type == ModContent.ItemType<MasochistSoul>()))
                {
                    build += $" [i:{ModContent.ItemType<BionomicCluster>()}]";
                }

                if (ModContent.TryFind("Fargowiltas", "Omnistation", out ModItem omni1) && ModContent.TryFind("Fargowiltas", "Omnistation2", out ModItem omni2))
                {
                    bool hasOmni = false;
                    if (player.inventory.Any(i => !i.IsAir && (i.type == omni1.Type || i.type == omni2.Type)))
                        hasOmni = true;
                    else if (ModContent.TryFind("Fargowiltas", "Omnistation", out ModBuff omnibuff) && player.HasBuff(omnibuff.Type))
                        hasOmni = true;

                    if (!hasOmni)
                    {
                        build += Main.rand.NextBool()
                            ? $" [i:{omni1.Type}]"
                            : $" [i:{omni2.Type}]";
                    }
                }
            }

            return summonType;
        }

        public override bool? UseItem(Player player)
        {
            if (player.ItemTimeIsZero)
            {
                string dialogue = "";
                GetBossHelp(ref dialogue, player);
                if (player.whoAmI == Main.myPlayer)
                    Main.NewText(dialogue);

                SoundEngine.PlaySound(SoundID.Meowmere, player.Center);
            }
            return true;
        }
        public DamageClass HighestDamageClass(Player player)
        {
            float melee = player.GetDamage(DamageClass.Melee).ApplyTo(100); 
            float ranged = player.GetDamage(DamageClass.Ranged).ApplyTo(100);
            float mage = player.GetDamage(DamageClass.Magic).ApplyTo(100);
            float summon = player.GetDamage(DamageClass.Summon).ApplyTo(100);
            if (melee > Math.Max(ranged, Math.Max(mage, summon)))
            {
                return DamageClass.Melee;
            }
            else if (ranged > Math.Max(mage, summon))
            {
                return DamageClass.Ranged;
            }
            else if (mage > summon)
            {
                return DamageClass.Magic;
            }
            else return DamageClass.Summon;
        }
    }
}