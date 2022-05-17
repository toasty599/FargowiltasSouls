using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Placeables.Trophies
{
    public class DeviTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.DeviTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Deviantt Trophy");
        }
    }
}