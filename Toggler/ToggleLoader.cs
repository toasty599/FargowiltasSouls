using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Toggler
{
    public static class ToggleLoader
    {
        public static Dictionary<string, bool> LoadedRawToggles;
        public static Dictionary<string, Toggle> LoadedToggles;

        public static void Load()
        {
            LoadedRawToggles = new Dictionary<string, bool>();
            LoadedToggles = new Dictionary<string, Toggle>();
            LoadTogglesFromAssembly(Fargowiltas.Instance.Code);
        }

        public static void LoadTogglesFromAssembly(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (typeof(ToggleCollection).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    ToggleCollection toggles = (ToggleCollection)Activator.CreateInstance(type);
                    Toggle[] toggleCollectionChildren = toggles.Load();

                    foreach (Toggle toggle in toggleCollectionChildren)
                    {
                        RegisterToggle(toggle);
                    }
                }
            }
        }

        public static void Unload()
        {
            LoadedRawToggles.Clear();
            LoadedToggles.Clear();
            LoadedRawToggles = null;
            LoadedToggles = null;
        }

        public static void RegisterToggle(Toggle toggle)
        {
            if (LoadedToggles.ContainsKey(toggle.InternalName) || LoadedRawToggles.ContainsKey(toggle.InternalName)) throw new System.Exception("Toggle with internal name " + toggle.InternalName + " is already registered");

            LoadedToggles.Add(toggle.InternalName, toggle);
            LoadedRawToggles.Add(toggle.InternalName, toggle.ToggleBool);
        }
    }
}
