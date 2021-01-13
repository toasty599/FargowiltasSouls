using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

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
            List<ToggleCollection> collections = new List<ToggleCollection>();

            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (typeof(ToggleCollection).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    ToggleCollection toggles = (ToggleCollection)Activator.CreateInstance(type);
                    collections.Add(toggles);
                }
            }

            IEnumerable<ToggleCollection> orderedCollections = collections.OrderBy((collection) => collection.Priority);
            Fargowiltas.Instance.Logger.Info($"ToggleCollections found: {orderedCollections.Count()}");

            foreach (ToggleCollection collection in orderedCollections)
            {
                Toggle[] toggleCollectionChildren = collection.Load();

                foreach (Toggle toggle in toggleCollectionChildren)
                {
                    RegisterToggle(toggle);
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
