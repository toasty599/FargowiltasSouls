using Terraria.Audio;

namespace FargowiltasSouls
{
    public static class SoundHelper
    {
        public static SoundStyle LegacySoundStyle(string sound, int style, float volume = 1, float pitch = 0) => new SoundStyle($"Terraria/Sounds/{sound}_{style}") { Volume = volume, Pitch = pitch };

        public static readonly SoundStyle ZaWarudo = new SoundStyle("FargowiltasSouls/Sounds/ZaWarudo")
        {
            PitchVariance = 0.5f,
        };
        public static readonly SoundStyle ZaWarudoResume = new SoundStyle("FargowiltasSouls/Sounds/ZaWarudoResume")
        {
            PitchVariance = 0.5f,
        };
        public static readonly SoundStyle Zhonyas = new SoundStyle("FargowiltasSouls/Sounds/Zhonyas");
        public static readonly SoundStyle Zombie_104 = new SoundStyle("Terraria/Sounds/Zombie_104");

        public static SoundStyle FargoSound(string sound, float volume = 1, float pitchVar = 0) => new SoundStyle($"FargowiltasSouls/Sounds/{sound}") { Volume = volume, PitchVariance = pitchVar };
    }
}
