using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FargowiltasSouls.Toggler.Content
{
    public class PetToggles : ToggleCollection
    {
        public override string Mod => "Terraria";
        public override string SortCatagory => "Pets";
        public override int Priority => 2;

        public string PetLantern;
    }
}
