﻿using FargowiltasSouls.Content.Items.Misc;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Systems
{
    public class RecipeSystem : ModSystem
    {
        public readonly static Recipe.ConsumeItemCallback IronBonusBars = (Recipe recipe, int type, ref int amount) =>
        {

            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.IronEnchantItem == null)
                return;

            if (recipe.requiredTile.Contains(TileID.MythrilAnvil) && !modPlayer.WizardEnchantActive)
                return;

            //calc the new amount consumed (each has 33% of not)
            int amountUsed = 0;

            for (int i = 0; i < amount; i++)
            {
                if (!Main.rand.NextBool(3))
                    amountUsed++;
            }

            amount = amountUsed;
        };

        public static string AnyItem(int id) => $"{Lang.misc[37]} {Lang.GetItemName(id)}";

        public static string AnyItem(string fargoSoulsLocalizationKey) => $"{Lang.misc[37]} {Language.GetTextValue($"Mods.FargowiltasSouls.RecipeGroups.{fargoSoulsLocalizationKey}")}";

        public static string ItemXOrY(int id1, int id2) => $"{Lang.GetItemName(id1)} {Language.GetTextValue($"Mods.FargowiltasSouls.RecipeGroups.Or")} {Lang.GetItemName(id2)}";

        public override void AddRecipeGroups()
        {
            RecipeGroup group;

            //drax
            group = new RecipeGroup(() => AnyItem(ItemID.Drax), ItemID.Drax, ItemID.PickaxeAxe);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyDrax", group);

            //dungeon enemies
            group = new RecipeGroup(() => AnyItem("BonesBanner"), ItemID.AngryBonesBanner, ItemID.BlueArmoredBonesBanner, ItemID.HellArmoredBonesBanner, ItemID.RustyArmoredBonesBanner);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBonesBanner", group);

            //cobalt
            group = new RecipeGroup(() => AnyItem(ItemID.CobaltRepeater), ItemID.CobaltRepeater, ItemID.PalladiumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltRepeater", group);

            //mythril
            group = new RecipeGroup(() => AnyItem(ItemID.MythrilRepeater), ItemID.MythrilRepeater, ItemID.OrichalcumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilRepeater", group);

            //adamantite
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteRepeater), ItemID.AdamantiteRepeater, ItemID.TitaniumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantiteRepeater", group);

            //evil wood
            group = new RecipeGroup(() => ItemXOrY(ItemID.Ebonwood, ItemID.Shadewood), ItemID.Ebonwood, ItemID.Shadewood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyEvilWood", group);

            //any adamantite
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteBar), ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantite", group);

            //shroomite head
            group = new RecipeGroup(() => AnyItem(ItemID.ShroomiteHelmet), ItemID.ShroomiteHelmet, ItemID.ShroomiteMask, ItemID.ShroomiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyShroomHead", group);

            //orichalcum head
            group = new RecipeGroup(() => AnyItem(ItemID.OrichalcumHelmet), ItemID.OrichalcumHelmet, ItemID.OrichalcumMask, ItemID.OrichalcumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyOriHead", group);

            //palladium head
            group = new RecipeGroup(() => AnyItem(ItemID.PalladiumHelmet), ItemID.PalladiumHelmet, ItemID.PalladiumMask, ItemID.PalladiumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPallaHead", group);

            //cobalt head
            group = new RecipeGroup(() => AnyItem(ItemID.CobaltHelmet), ItemID.CobaltHelmet, ItemID.CobaltHat, ItemID.CobaltMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltHead", group);

            //mythril head
            group = new RecipeGroup(() => AnyItem(ItemID.MythrilHelmet), ItemID.MythrilHelmet, ItemID.MythrilHat, ItemID.MythrilHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilHead", group);

            //titanium head
            group = new RecipeGroup(() => AnyItem(ItemID.TitaniumHelmet), ItemID.TitaniumHelmet, ItemID.TitaniumMask, ItemID.TitaniumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyTitaHead", group);

            //hallowed head
            group = new RecipeGroup(() => AnyItem(ItemID.HallowedHelmet), ItemID.HallowedHelmet, ItemID.HallowedMask, ItemID.HallowedHeadgear, ItemID.HallowedHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyHallowHead", group);

            //ancient hallow
            group = new RecipeGroup(() => AnyItem(ItemID.AncientHallowedHelmet), ItemID.AncientHallowedHelmet, ItemID.AncientHallowedHeadgear, ItemID.AncientHallowedHood, ItemID.AncientHallowedMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAncientHallowHead", group);

            //adamantite head
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteHelmet), ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamHead", group);

            //chloro head
            group = new RecipeGroup(() => AnyItem(ItemID.ChlorophyteHelmet), ItemID.ChlorophyteHelmet, ItemID.ChlorophyteMask, ItemID.ChlorophyteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyChloroHead", group);

            //spectre head
            group = new RecipeGroup(() => ItemXOrY(ItemID.SpectreHood, ItemID.SpectreMask), ItemID.SpectreHood, ItemID.SpectreMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySpectreHead", group);

            //beetle body
            group = new RecipeGroup(() => ItemXOrY(ItemID.BeetleShell, ItemID.BeetleScaleMail), ItemID.BeetleShell, ItemID.BeetleScaleMail);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBeetle", group);

            //            //phasesabers
            //            group = new RecipeGroup(() => AnyItem("Phasesaber"), ItemID.RedPhasesaber, ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.PurplePhasesaber, ItemID.WhitePhasesaber,
            //                ItemID.YellowPhasesaber);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPhasesaber", group);

            //            //vanilla butterflies
            //            group = new RecipeGroup(() => AnyItem("Butterfly"), ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly,
            //                ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyButterfly", group);

            //vanilla squirrels
            group = new RecipeGroup(() => AnyItem(ItemID.Squirrel),
                ItemID.Squirrel,
                ItemID.SquirrelRed,
                ItemID.SquirrelGold,
                ItemID.GemSquirrelAmber,
                ItemID.GemSquirrelAmethyst,
                ItemID.GemSquirrelDiamond,
                ItemID.GemSquirrelEmerald,
                ItemID.GemSquirrelRuby,
                ItemID.GemSquirrelSapphire,
                ItemID.GemSquirrelTopaz,
                ModContent.ItemType<TopHatSquirrelCaught>(),
                ModContent.Find<ModItem>("Fargowiltas", "Squirrel").Type
            );
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySquirrel", group);

            //            //vanilla fish
            //            group = new RecipeGroup(() => AnyItem("CommonFish"), ItemID.AtlanticCod, ItemID.Bass, ItemID.Trout, ItemID.RedSnapper, ItemID.Salmon, ItemID.Tuna);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCommonFish", group);

            //vanilla birds
            group = new RecipeGroup(() => AnyItem(ItemID.Bird), ItemID.Bird, ItemID.BlueJay, ItemID.Cardinal, ItemID.GoldBird, ItemID.Duck, ItemID.MallardDuck, ItemID.Grebe, ItemID.Penguin, ItemID.Seagull);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBird", group);

            //            //vanilla scorpions
            //            group = new RecipeGroup(() => AnyItem(ItemID.Scorpion), ItemID.Scorpion, ItemID.BlackScorpion);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyScorpion", group);

            //            //gold pick
            //            group = new RecipeGroup(() => AnyItem(ItemID.GoldPickaxe), ItemID.GoldPickaxe, ItemID.PlatinumPickaxe);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldPickaxe", group);

            //            //fish trash
            //            group = new RecipeGroup(() => AnyItem("FishingTrash"), ItemID.OldShoe, ItemID.TinCan, ItemID.FishingSeaweed);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyFishingTrash", group);

            //vanilla rotten chunk/vertebrae
            group = new RecipeGroup(() => ItemXOrY(ItemID.RottenChunk, ItemID.Vertebrae), ItemID.RottenChunk, ItemID.Vertebrae);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyRottenChunk", group);

            //vanilla gold and plat ore
            group = new RecipeGroup(() => ItemXOrY(ItemID.GoldOre, ItemID.PlatinumOre), ItemID.GoldOre, ItemID.PlatinumOre);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldOre", group);

            //vanilla gold and plat bar
            group = new RecipeGroup(() => ItemXOrY(ItemID.GoldBar, ItemID.PlatinumBar), ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldBar", group);

        }

        public override void PostAddRecipes()
        {
            foreach (Recipe recipe in Main.recipe.Where(recipe => recipe.requiredTile.Contains(TileID.Anvils) || recipe.requiredTile.Contains(TileID.MythrilAnvil)))
            {
                recipe.AddConsumeItemCallback(IronBonusBars);
            }
        }
    }
}
