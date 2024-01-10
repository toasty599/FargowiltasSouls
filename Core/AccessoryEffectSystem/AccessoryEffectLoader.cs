using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    public static class AccessoryEffectLoader
    {
        public static List<AccessoryEffect> AccessoryEffectTypes = new();
        public static List<AccessoryEffectInstance> AccessoryEffectInstances = new();
        internal static void Register(AccessoryEffect effect)
        {
            AccessoryEffectTypes.Add(effect);
            if (effect.HasToggle)
                ToggleLoader.RegisterToggle(new Toggle(effect, effect.Mod.Name, effect.ToggleHeader.SortCategory, effect.ToggleHeader));
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
            if (player.GetToggleValue<T>())
            {
                effectPlayer.ActiveEffects.Add(effect);
            }
        }
        public static T EffectType<T>() where T : AccessoryEffect => ModContent.GetInstance<T>();
        public static T EffectType(string internalName) => ModContent.Find<AccessoryEffect>(internalName);
        public static T GetEffectInstance<T>(this Player player) where T : AccessoryEffectInstance =>
            GetEffectInstance(ModContent.GetInstance<T>(), player);
        public static T GetEffectInstance<T>(T baseInstance, Player player) where T : AccessoryEffectInstance
        => player.GetModPlayer<AccessoryEffectPlayer>().EffectInstances[baseInstance.Index] as T ?? throw new KeyNotFoundException($"Instance of '{typeof(T).Name}' does not exist on the current player.");
    }
}
