namespace FargowiltasSouls.Toggler
{
    public class Toggle
    {
        public string Mod;
        public string Catagory;
        public string InternalName;
        public bool ToggleBool;

        public bool BlehIsHeader;

        public Toggle(string internalName, string mod, string catagory, bool blehIsHeader = false)
        {
            InternalName = internalName;
            Mod = mod;
            Catagory = catagory;

            ToggleBool = true;
            BlehIsHeader = blehIsHeader;
        }

        public override string ToString() => $"Mod: {Mod}, Catagory: {Catagory}, InternalName: {InternalName}, Toggled: {ToggleBool}";
    }
}
