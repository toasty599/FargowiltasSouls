using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Toggler
{
    public static class SoulCheck
    {
        public static Toggle GetToggle<T>(this Player player) where T : AccessoryEffect => player.GetToggle(ModContent.GetInstance<T>());
        public static Toggle GetToggle(this Player player, AccessoryEffect effect)
        {
            return player.FargoSouls().Toggler.Toggles[effect];
        }
        public static bool GetToggleValue<T>(this Player player) where T : AccessoryEffect => player.GetToggleValue(ModContent.GetInstance<T>());
        public static bool GetToggleValue(this Player player, AccessoryEffect effect)
        {
            Toggle toggle = player.GetToggle(effect);
            toggle.DisplayToggle = true;
            return toggle.ToggleBool;
        }

        public static bool GetPlayerBoolValue(this Player player, AccessoryEffect effect)
        {
            Toggle toggle = player.GetToggle(effect);
            return toggle.ToggleBool;
        }

        public static void SetToggleValue<T>(this Player player, bool value) where T : AccessoryEffect => player.SetToggleValue(ModContent.GetInstance<T>(), value);

        public static void SetToggleValue(this Player player, AccessoryEffect effect, bool value)
        {
            if (player.FargoSouls().Toggler.Toggles.ContainsKey(effect))
                player.FargoSouls().Toggler.Toggles[effect].ToggleBool = value;
            else
                FargowiltasSouls.Instance.Logger.Warn($"Expected toggle not found: {effect.Name}");
        }
        public static void DisplayToggle<T>(this Player player) where T : AccessoryEffect => player.DisplayToggle(ModContent.GetInstance<T>());
        public static void DisplayToggle(this Player player, AccessoryEffect effect)
        {
            if (player.FargoSouls().Toggler.Toggles.ContainsKey(effect))
                player.FargoSouls().Toggler.Toggles[effect].DisplayToggle = true;
            else
                FargowiltasSouls.Instance.Logger.Warn($"Expected toggle not found: {effect.Name}");
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
