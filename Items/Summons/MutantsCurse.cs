using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.NPCs.MutantBoss;

namespace FargowiltasSouls.Items.Summons
{
    public class MutantsCurse : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant's Curse");
            Tooltip.SetDefault("'At least this way, you don't need that doll'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变体的诅咒");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'至少不需要用娃娃了'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
        public override int NumFrames => 11;
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 999;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.value = Item.buyPrice(1);
        }

        public override bool? UseItem(Player player)
        {
            int mutant = ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC modNPC) ? NPC.FindFirstNPC(modNPC.Type) : -1;

            if (mutant > -1 && Main.npc[mutant].active)
            {
                Main.npc[mutant].Transform(ModContent.NPCType<MutantBoss>());
                FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}", 175, 75, 255);
            }
            else
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<MutantBoss>());
            }

            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}