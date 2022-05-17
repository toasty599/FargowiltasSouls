using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Placeables.Trophies
{
    public class MutantTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.MutantTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Mutant Trophy");
        }
    }
}