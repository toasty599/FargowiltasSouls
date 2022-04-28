using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs.Champions;

namespace FargowiltasSouls.Items.Summons
{
    public class SigilOfChampions : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sigil of Champions");
            Tooltip.SetDefault(@"Summons the Champions
Summons vary depending on time and biome
Right click to check for possible summons
Not consumed on use");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 1;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(1);
        }

        public override bool CanUseItem(Player player)
        {
            List<int> bosses = new List<int>(new int[] {
                ModContent.NPCType<CosmosChampion>(),
                ModContent.NPCType<EarthChampion>(),
                ModContent.NPCType<LifeChampion>(),
                ModContent.NPCType<NatureChampion>(),
                ModContent.NPCType<ShadowChampion>(),
                ModContent.NPCType<SpiritChampion>(),
                ModContent.NPCType<TerraChampion>(),
                ModContent.NPCType<TimberChampion>(),
                ModContent.NPCType<WillChampion>()
            });

            for (int i = 0; i < Main.maxNPCs; i++) //no using during another champ fight
            {
                if (Main.npc[i].active && i == NPCs.EModeGlobalNPC.championBoss && bosses.Contains(Main.npc[i].type))
                    return false;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            Color color = new Color(175, 75, 255);

            //using the raw name SigilOfChampions here because ChampionySigil runs this code too, can't use "Name"
            if (player.ZoneUndergroundDesert)
            {
                if (player.altFunctionUse == 2)
                    Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Spirit", color);
                else
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SpiritChampion>());
            }
            else if (player.ZoneUnderworldHeight)
            {
                if (player.altFunctionUse == 2)
                    Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Earth", color);
                else
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<EarthChampion>());
            }
            else if (player.Center.Y >= Main.worldSurface * 16) //is underground
            {
                if (player.ZoneSnow)
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Nature", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NatureChampion>());
                }
                else
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Terra", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<TerraChampion>());
                }
            }
            else //above ground
            {
                if (player.ZoneSkyHeight)
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Cosmos", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CosmosChampion>());
                }
                else if (player.ZoneBeach)
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Will", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<WillChampion>());
                }
                else if (player.ZoneHallow && Main.dayTime)
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Life", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<LifeChampion>());
                }
                else if ((player.ZoneCorrupt || player.ZoneCrimson) && !Main.dayTime) //night
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Shadow", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ShadowChampion>());
                }
                else if (!player.ZoneHallow && !player.ZoneCorrupt && !player.ZoneCrimson
                    && !player.ZoneDesert && !player.ZoneSnow && !player.ZoneJungle && Main.dayTime) //purity day
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Timber", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<TimberChampion>());
                }
                else //nothing to summon
                {
                    if (player.altFunctionUse == 2)
                        Main.NewText($"$Mods.{Mod.Name}.Message.SigilOfChampions.Nothing", color);
                }
            }

            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = Main.DiscoColor;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Acorn, 5)
            .AddRecipeGroup("IronBar", 5)
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient(ItemID.FrostCore, 5)
            .AddIngredient(ItemID.SoulofLight, 5)
            .AddIngredient(ItemID.SoulofNight, 5)
            .AddIngredient(ItemID.AncientBattleArmorMaterial, 5)
            .AddIngredient(ItemID.Coral, 5)
            .AddIngredient(ItemID.LunarBar, 5)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}