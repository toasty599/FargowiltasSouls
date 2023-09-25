using System.Linq;
using Terraria;

namespace FargowiltasSouls.Core.Toggler
{
    public static class SoulCheck
    {
        public static Toggle GetToggle(this Player player, string name)
        {
            return player.FargoSouls().Toggler.Toggles[name];
        }

        public static bool GetToggleValue(this Player player, string name, bool checkForMutantPresence = true)
        {
            Toggle toggle = player.GetToggle(name);
            toggle.DisplayToggle = true;
            return (!checkForMutantPresence || !Main.player[Main.myPlayer].FargoSouls().MutantPresence) && toggle.ToggleBool;
        }

        public static bool GetPlayerBoolValue(this Player player, string name)
        {
            Toggle toggle = player.GetToggle(name);
            return toggle.ToggleBool;
        }

        public static void SetToggleValue(this Player player, string name, bool value)
        {
            if (player.FargoSouls().Toggler.Toggles.ContainsKey(name))
                player.FargoSouls().Toggler.Toggles[name].ToggleBool = value;
            else
                FargowiltasSouls.Instance.Logger.Warn($"Expected toggle not found: {name}");
        }
        public static void DisplayToggle(this Player player, string name)
        {
            if (player.FargoSouls().Toggler.Toggles.ContainsKey(name))
                player.FargoSouls().Toggler.Toggles[name].DisplayToggle = true;
            else
                FargowiltasSouls.Instance.Logger.Warn($"Expected toggle not found: {name}");
        }

        public static void ReloadToggles(this Player player)
        {
            //disable toggles that should be disabled
            foreach (Toggle toggle in player.FargoSouls().Toggler.Toggles.Values)//.Where(t => t.Category == "Enchantments" || t.Category == "Maso"))
            {
                //force them enabled if the config is on, otherwise force them disabled
                toggle.DisplayToggle = SoulConfig.Instance.DisplayTogglesRegardless;
            }
        }
        /*public static void SetPlayerBoolValue(this Player player, string name, bool value)
        {
            player.FargoSouls().Toggler.Toggles[name].PlayerBool = value;
        }*/
    }
}
