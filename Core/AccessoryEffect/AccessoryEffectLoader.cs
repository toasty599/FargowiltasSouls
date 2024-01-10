using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.AccessoryEffect
{
    public static class AccessoryEffectLoader
    {
        public static List<AccessoryEffect> AccessoryEffectTypes = new();
        public static List<AccessoryEffectInstance> AccessoryEffectInstances = new();
        internal static void Register(AccessoryEffect effect)
        {
            AccessoryEffectTypes.Add(effect);
        }
        internal static void Register(AccessoryEffectInstance effect)
        {
            AccessoryEffectInstances.Add(effect);
        }
        
        public static void EnableEffect<T>(this Player player, Item item) where T : AccessoryEffect
        {
            // TODO: field for items calling the effect, probably a lookup/dictionary-like structure in AccessoryEffectPlayer
            AccessoryEffect effect = ModContent.GetInstance<T>();
            AccessoryEffectPlayer effectPlayer = player.GetModPlayer<AccessoryEffectPlayer>();
            if (effectPlayer.EffectToggle[effect]) // TODO: implement properly with eventual toggle-side of rework
            {
                effectPlayer.ActiveEffects.Add(effect);
            }
        }
        
    }
}
