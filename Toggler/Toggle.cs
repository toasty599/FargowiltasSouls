using System;
using Terraria;

namespace FargowiltasSouls.Toggler
{
    public class Toggle
    {
        public string InternalName;
        public Ref<bool> ToggleBool;
        public Ref<bool> PlayerBool;

        public Toggle(string internalName, Ref<bool> toggleBool, Ref<bool> playerBool)
        {
            InternalName = internalName;
            ToggleBool = toggleBool;
            PlayerBool = playerBool;
        }
    }
}
