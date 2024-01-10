using FargowiltasSouls.Core.AccessoryEffectSystem;
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
        public AccessoryEffect Effect;
        public Header Header;
        public bool ToggleBool;
        public bool DisplayToggle;

        public Toggle(AccessoryEffect effect, string mod, string category, Header header)
        {
            Effect = effect;
            Mod = mod;
            Category = category;
            Header = header;

            ToggleBool = true;
            DisplayToggle = true;
        }

        public override string ToString() => $"Mod: {Mod}, Category: {Category}, Effect: {Effect.Name}, Toggled: {ToggleBool}";

        public string GetRawToggleName()
        {
            string baseText = Effect.ToggleDescription;
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
