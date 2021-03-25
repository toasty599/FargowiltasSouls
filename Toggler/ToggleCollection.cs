using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FargowiltasSouls.Toggler
{
    public abstract class ToggleCollection
    {
        public abstract string Mod { get; }
        public abstract string SortCatagory { get; }
        public abstract int Priority { get; }

        public List<Toggle> Load()
        {
            // All string (toggles) and int (header) fields
            FieldInfo[] fields = GetType().GetFields().Where((f) => f.FieldType == typeof(string) || f.FieldType == typeof(int)).ToArray();
            // The amount of int fields, ie toggles
            List<Toggle> ret = new List<Toggle>();

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType != typeof(int)) // Register as toggle if it's a string
                    ret.Add(new Toggle(fields[i].Name, Mod, SortCatagory));
                else // ...or as a header if it's an int
                    ToggleLoader.LoadedHeaders.Add(fields[i + 1].Name, (fields[i].Name, (int)fields[i].GetValue(this)));
            }

            // Return the toggles (strings)
            return ret;
        }
    }
}
