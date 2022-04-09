using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public Point TogglerPosition;
        public bool CanPlayMaso;

        public bool Initialized;

        public void Load(FargoSoulsPlayer player)
        {
            if (Initialized)
                return;

            //Main.NewText("OOBA");
            Config = new Preferences(ConfigPath);

            RawToggles = ToggleLoader.LoadedRawToggles;
            Toggles = ToggleLoader.LoadedToggles;
            TogglerPosition = new Point(0, 0);

            if (!Main.dedServ)
            {
                if (!Config.Load())
                    Save();
            }

            Dictionary<string, int> togglerPositionUnpack = Config.Get("TogglerPosition", new Dictionary<string, int>() { { "X", Main.screenWidth / 2 - 300 }, { "Y", Main.screenHeight / 2 - 200 } });
            TogglerPosition = new Point(togglerPositionUnpack["X"], togglerPositionUnpack["Y"]);

            if (!Main.dedServ)
                FargowiltasSouls.UserInterfaceManager.SoulToggler.SetPositionToPoint(TogglerPosition);

            CanPlayMaso = Config.Get("CanPlayMaso", false);

            RawToggles = Config.Get("Toggles", ToggleLoader.LoadedRawToggles);
            Toggles = ToggleLoader.LoadedToggles;

            if (RawToggles != ToggleLoader.LoadedRawToggles) // Version mismatch, rebuild RawToggles without loosing data
            {
                string[] missingKeys = ToggleLoader.LoadedRawToggles.Keys.Except(RawToggles.Keys).ToArray();
                foreach (string key in missingKeys)
                {
                    if (!Main.dedServ)
                        Config.Put($"Toggles.{key}", ToggleLoader.LoadedRawToggles[key]);
                }
            }

            ParseUnpackedToggles();
            player.TogglesToSync = RawToggles;
            RawToggles = null;

            Initialized = true;
        }

        public void Save()
        {
            if (!Main.dedServ)
            {
                Config.Put("CanPlayMaso", CanPlayMaso);
                Config.Put("Toggles", ParsePackedToggles());

                TogglerPosition = FargowiltasSouls.UserInterfaceManager.SoulToggler.GetPositionAsPoint();
                Config.Put("TogglerPosition", UnpackPosition());
                Config.Save();
            }
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
                if (!Toggles.ContainsKey(unpackedToggle.Key))
                {
                    continue;
                }

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

        public Dictionary<string, int> UnpackPosition() => new Dictionary<string, int>() {
            { "X", TogglerPosition.X },
            { "Y", TogglerPosition.Y }
        };

        public void SetAll(bool value)
        {
            foreach (Toggle toggle in Toggles.Values)
            {
                Main.LocalPlayer.SetToggleValue(toggle.InternalName, value);
            }
        }

        public void MinimalEffects()
        {
            Player player = Main.LocalPlayer;

            SetAll(false);
            player.SetToggleValue("Mythril", true);
            player.SetToggleValue("Palladium", true);
            player.SetToggleValue("IronM", true);
            player.SetToggleValue("CthulhuShield", true);
            player.SetToggleValue("Tin", true);
            player.SetToggleValue("Beetle", true);
            player.SetToggleValue("Spider", true);
            player.SetToggleValue("JungleDash", true);
            player.SetToggleValue("SupersonicTabi", true);
            player.SetToggleValue("Valhalla", true);
            player.SetToggleValue("Nebula", true);
            player.SetToggleValue("Solar", true);

            player.SetToggleValue("MasoGraze", true);
            player.SetToggleValue("MasoGrazeRing", true);
            player.SetToggleValue("MasoIconDrops", true);
            player.SetToggleValue("MasoNymph", true);
            player.SetToggleValue("TribalCharm", true);
            player.SetToggleValue("MasoGrav2", true);
            player.SetToggleValue("MasoSqueak", true);

            player.SetToggleValue("YoyoBag", true);
            player.SetToggleValue("MiningHunt", true);
            player.SetToggleValue("MiningDanger", true);
            player.SetToggleValue("MiningSpelunk", true);
            player.SetToggleValue("MiningShine", true);
            player.SetToggleValue("Trawler", true);
            player.SetToggleValue("RunSpeed", true);
            player.SetToggleValue("SupersonicRocketBoots", true);
            player.SetToggleValue("Momentum", true);
            player.SetToggleValue("FlightMasteryInsignia", true);
            player.SetToggleValue("FlightMasteryGravity", true);
            player.SetToggleValue("Universe", true);
            player.SetToggleValue("DefensePaladin", true);
            player.SetToggleValue("MasoAeolus", true);
            player.SetToggleValue("MasoConcoction", true);
        }
    }
}
