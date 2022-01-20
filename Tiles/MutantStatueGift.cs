using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Tiles
{
    public class MutantStatueGift : MutantStatue
    {
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            base.KillMultiTile(i, j, frameX, frameY);

            Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Masochist>());
        }
    }
}