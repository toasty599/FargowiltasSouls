using System.Collections.Generic;
using Terraria;

namespace FargowiltasSouls.Toggler
{
    public static class ToggleLoader
    {
        public static Dictionary<string, bool> LoadedRawToggles = new Dictionary<string, bool>();
        public static Dictionary<string, Toggle> LoadedToggles = new Dictionary<string, Toggle>();

        public static Ref<bool> Test = new Ref<bool>();
        public static Ref<bool> TestToggle = new Ref<bool>();

        public static void Load()
        {
            RegisterToggle(new Toggle("Test", TestToggle, Test));
        }

        public static void RegisterToggle(Toggle toggle)
        {
            if (LoadedToggles.ContainsKey(toggle.InternalName) || LoadedRawToggles.ContainsKey(toggle.InternalName)) throw new System.Exception("Toggle with internal name " + toggle.InternalName + " is already registered");

            LoadedToggles.Add(toggle.InternalName, toggle);
            LoadedRawToggles.Add(toggle.InternalName, toggle.ToggleBool.Value);
        }
    }
}
