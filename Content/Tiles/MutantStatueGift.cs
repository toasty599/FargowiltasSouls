using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Content.Tiles
{
    public class MutantStatueGift : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = true;
            //TileObjectData.newTile.Origin = new Point16(0, 1);
            //TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Mutant Statue");
            AddMapEntry(new Color(144, 144, 144), name);

            AnimationFrameHeight = 54;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Masochist>();
        }
        public override bool RightClick(int i, int j)
        {
                
            Player player = Main.LocalPlayer;

            Tile tile = Framing.GetTileSafely(i, j);
            //account for possibly clicking on any part of the multi-tile, negate it to have coords of top left corner
            i -= tile.TileFrameX / 18;
            j -= tile.TileFrameY / 18;
            //add offset to get the coord right below the middle-bottom of this multi-tile
            i += 1;
            j += 3;

            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Masochist>());
            for (int x = -1; x < 2; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    WorldGen.KillTile(i - x, j - 1 - y, noItem: true);
                }
            }
            
            WorldGen.PlaceTile(i, j - 1, ModContent.TileType<MutantStatue>());

            return true;
        }
        public override bool CanDrop(int i, int j) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

        /*
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Masochist>());
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placables.MutantStatueGift>());
        }
        */

    }
}