using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.IO;

namespace FargowiltasSouls.Toggler
{
    public class ToggleBackend
    {
        public string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "FargowiltasSouls_Toggles.json");
        public Preferences Config;
        public Dictionary<string, bool> RawToggles = new Dictionary<string, bool>();
        public Dictionary<string, Toggle> Toggles = new Dictionary<string, Toggle>();
        public Point TogglerPosition;

        public void Load()
        {
            Config = new Preferences(ConfigPath);

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
            Toggles[toggle].ToggleBool.Value = value;
            RawToggles[toggle] = value;
        }

        public KeyValuePair<string, bool> UnpackToggle(Toggle toggle) => new KeyValuePair<string, bool>(toggle.InternalName, toggle.ToggleBool.Value);

        // Fill in whether or not the toggle is enabled
        public void ParseUnpackedToggles()
        {
            foreach (KeyValuePair<string, bool> unpackedToggle in RawToggles)
            {
                Toggles[unpackedToggle.Key].ToggleBool.Value = unpackedToggle.Value;
            }
        }

        public Dictionary<string, bool> ParsePackedToggles()
        {
            Dictionary<string, bool> unpackedToggles = new Dictionary<string, bool>();

            foreach (KeyValuePair<string, Toggle> packedToggle in Toggles)
            {
                unpackedToggles[packedToggle.Key] = Toggles[packedToggle.Key].ToggleBool.Value;
            }

            return unpackedToggles;
        }
    }
}
