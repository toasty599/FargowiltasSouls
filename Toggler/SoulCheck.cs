using Terraria;

namespace FargowiltasSouls.Toggler
{
    public static class SoulCheck
    {
        public static Toggle GetToggle(this Player player, string name)
        {
            return player.GetModPlayer<FargoPlayer>().Toggler.Toggles[name];
        }

        public static bool GetToggleValue(this Player player, string name, bool checkForMutantPresence = true, bool checkForPlayerBool = true)
        {
            Toggle toggle = player.GetToggle(name);
            return checkForMutantPresence && Main.player[Main.myPlayer].GetModPlayer<FargoPlayer>().MutantPresence ? false : (toggle.ToggleBool && (checkForPlayerBool ? toggle.PlayerBool : true));
        }

        public static void SetToggleValue(this Player player, string name, bool value)
        {
            player.GetModPlayer<FargoPlayer>().Toggler.Toggles[name].ToggleBool = value;
        }
    }
}
