using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon
{
    public static class PatreonMiscMethods
    {
        public static void Load(Mod mod)
        {
            mod.AddEquipTexture(null, EquipType.Legs, "BetaLeg", "FargowiltasSouls/Patreon/JojoTheGamer/Beta_Legs");
            mod.AddEquipTexture(null, EquipType.Body, "BetaBody", "FargowiltasSouls/Patreon/JojoTheGamer/Beta_Body");
            mod.AddEquipTexture(null, EquipType.Head, "BetaHead", "FargowiltasSouls/Patreon/JojoTheGamer/Beta_Head");
        }
    }
}
