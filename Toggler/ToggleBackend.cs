using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.IO;

namespace FargowiltasSouls.Toggler
{
    public class ToggleBackend
    {
        public static string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "FargowiltasSouls_Toggles.json");
        public Preferences Config;
        public Dictionary<string, bool> RawToggles;
        public Dictionary<string, Toggle> Toggles;

        public void Load()
        {
            Config = new Preferences(ConfigPath);

            RawToggles = ToggleLoader.LoadedRawToggles;
            Toggles = ToggleLoader.LoadedToggles;

            if (!Config.Load())
                Save();

            RawToggles = Config.Get("Toggles", ToggleLoader.LoadedRawToggles);
            Toggles = ToggleLoader.LoadedToggles;
            ParseUnpackedToggles();
            RawToggles = null;
        }

        public void Save()
        {
            Config.Put("Toggles", ParsePackedToggles());
            Config.Save();
        }

        public void UpdateToggle(string toggle, bool value)
        {
            Toggles[toggle].ToggleBool = value;
            RawToggles[toggle] = value;
        }

        public KeyValuePair<string, bool> UnpackToggle(Toggle toggle) => new KeyValuePair<string, bool>(toggle.InternalName, toggle.ToggleBool);

        // Fill in whether or not the toggle is enabled
        public void ParseUnpackedToggles()
        {
            foreach (KeyValuePair<string, bool> unpackedToggle in RawToggles)
            {
                Toggles[unpackedToggle.Key].ToggleBool = unpackedToggle.Value;
            }
        }

        public Dictionary<string, bool> ParsePackedToggles()
        {
            Dictionary<string, bool> unpackedToggles = new Dictionary<string, bool>();

            foreach (KeyValuePair<string, Toggle> packedToggle in Toggles)
            {
                unpackedToggles[packedToggle.Key] = Toggles[packedToggle.Key].ToggleBool;
            }

            return unpackedToggles;
        }
    }
}
