using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static Terraria.GameContent.UI.EmoteID;

namespace FargowiltasSouls.Core.Toggler
{
    public abstract class Header : ModType
    {
        public abstract string SortCategory { get; }
        public abstract int Priority { get; }
        public abstract int Item { get; }
        public string HeaderDescription => Language.GetTextValue($"Mods.{Mod}.Toggler.{Name}");

        protected override void Register()
        {
            ToggleLoader.RegisterHeader(this);
            ModTypeLookup<Header>.Register(this);
        }

        public override string ToString() => $"Mod: {Mod}, Item: {Item}, Category: {SortCategory}, Priority: {Priority}";
        public string GetRawToggleName()
        {
            string baseText = HeaderDescription;
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

        public static T GetHeader<T>() where T : Header => ModContent.GetInstance<T>();
        public static Header GetHeaderFromItem<T>() where T : ModItem => ToggleLoader.LoadedHeaders.FirstOrDefault(h => h.Item == ModContent.ItemType<T>());
        public static Header GetHeaderFromItemType(int type) => ToggleLoader.LoadedHeaders.FirstOrDefault(h => h.Item == type);
    }
}
