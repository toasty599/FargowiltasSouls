using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.UI.Chat;

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

        public string GetRawToggleName()
        {
            string baseText = Language.GetTextValue($"Mods.{Mod}.{InternalName}Config");
            List<TextSnippet> parsedText = ChatManager.ParseMessage(baseText, Color.White);
            string rawText = "";

            foreach (TextSnippet snippet in parsedText)
            {
                if (!snippet.Text.StartsWith("["))
                {
                    rawText += snippet.Text.Trim();
                }
            }

            return rawText;
        }
    }
}
