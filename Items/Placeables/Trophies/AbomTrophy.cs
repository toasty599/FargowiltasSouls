using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Placeables.Trophies
{
    public class AbomTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.AbomTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Abominationn Trophy");
        }
    }
}