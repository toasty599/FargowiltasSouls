using System;
using Terraria;

namespace FargowiltasSouls.Toggler
{
    public class Toggle
    {
        public string InternalName;
        public bool ToggleBool;
        public bool PlayerBool;

        public Toggle(string internalName)
        {
            InternalName = internalName;
            ToggleBool = false;
            PlayerBool = false;
        }
    }
}
