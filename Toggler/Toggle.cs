using System;
using Terraria;

namespace FargowiltasSouls.Toggler
{
    public class Toggle
    {
        public string Mod;
        public string Catagory;
        public string InternalName;
        public bool ToggleBool;
        public bool PlayerBool;

        public Toggle(string internalName, string mod, string catagory)
        {
            InternalName = internalName;
            Mod = mod;
            Catagory = catagory;

            ToggleBool = true;
            PlayerBool = false;
        }
    }
}
