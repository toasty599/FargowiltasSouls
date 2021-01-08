using Terraria;

namespace FargowiltasSouls.Toggler
{
    // TODO: static class that contains methods for checking souls for a player
    public static class SoulCheck
    {
        public static Toggle GetToggle(this Player player, string name)
        {
            return player.GetModPlayer<FargoPlayer>().Toggler.Toggles[name];
        }

        public static bool GetToggleValue(this Player player, string name, bool checkForMutantPresence = true)
        {
            Toggle toggle = player.GetToggle(name);
            return checkForMutantPresence && Main.player[Main.myPlayer].GetModPlayer<FargoPlayer>().MutantPresence ? false : (toggle.ToggleBool.Value && toggle.PlayerBool.Value);
        }
    }
}
