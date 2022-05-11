using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs.Challengers;

namespace FargowiltasSouls.Items.Summons
{
    public class SquirrelCoatofArms : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squirrel Coat of Arms");
            Tooltip.SetDefault("Summons squirrelly wrath");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 20;
            Item.useAnimation = 46;
            Item.useTime = 46;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            int squrrl = ModContent.TryFind("Fargowiltas", "Squirrel", out ModNPC modNPC) ? NPC.FindFirstNPC(modNPC.Type) : -1;
            if (squrrl > -1 && Main.npc[squrrl].active)
            {
                Main.npc[squrrl].Transform(ModContent.NPCType<TrojanSquirrel>());
                FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}", new Color(175, 75, 255));
            }
            else
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<TrojanSquirrel>());
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("Wood", 20)
                .AddRecipeGroup("FargowiltasSouls:AnySquirrel")
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}