using System;
using System.Reflection;

namespace FargowiltasSouls.Toggler
{
    public abstract class ToggleCollection
    {
        public abstract string Mod { get; }
        public abstract string SortCatagory { get; }
        public abstract int Priority { get; }

        public Toggle[] Load()
        {
            FieldInfo[] fields = GetType().GetFields();
            Toggle[] ret = new Toggle[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                ret[i] = new Toggle(fields[i].Name, Mod, SortCatagory);
            }

            return ret;
        }
    }
}
