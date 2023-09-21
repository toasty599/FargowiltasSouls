namespace FargowiltasSouls.Core.Toggler
{
    public class Toggle
    {
        public string Mod;
        public string Category;
        public string InternalName;
        public bool ToggleBool;
        public bool DisplayToggle;

        public Toggle(string internalName, string mod, string category)
        {
            InternalName = internalName;
            Mod = mod;
            Category = category;

            ToggleBool = true;
            DisplayToggle = true;
        }

        public override string ToString() => $"Mod: {Mod}, Category: {Category}, InternalName: {InternalName}, Toggled: {ToggleBool}";
    }
}
