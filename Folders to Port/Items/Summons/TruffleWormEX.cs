using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Summons
{
    public class TruffleWormEX : SoulsItem
    {
        public override string Texture => "Terraria/Images/Item_2673";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Truffle Worm EX");
            Tooltip.SetDefault("Only usable in Masochist Mode\nThe tides surge in its presence\nYou probably shouldn't be reading this...");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "松露虫 EX");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "只能在受虐模式使用\n它出现时潮水汹涌澎湃");
        }

        public override void SetDefaults()
        {
            item.maxStack = 20;
            item.rare = ItemRarityID.Purple;
            item.width = 12;
            item.height = 12;
            item.consumable = true;
            item.bait = 777;
            /*item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = 4;*/
            item.value = Item.sellPrice(0, 17, 0, 0);
        }

        /*public override bool CanUseItem(Player player)
        {
            return FargoSoulsWorld.MasochistMode;
        }

        public override bool UseItem(Player player)
        {
            FargoSoulsWorld.FishronEX = !FargoSoulsWorld.FishronEX;
            string text = FargoSoulsWorld.FishronEX ? "The ocean stirs..." : "The ocean settles.";
            Color color = new Color(0, 100, 200);
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, color);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
                NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }*/

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        /*public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.TruffleWorm, 3)
            .AddIngredient(ItemID.ShrimpyTruffle)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            recipe.SetResult(this, 3);
            .Register();
        }*/
    }
}