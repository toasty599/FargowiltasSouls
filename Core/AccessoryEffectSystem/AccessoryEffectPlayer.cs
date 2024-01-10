using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    public class AccessoryEffectPlayer : ModPlayer
    {
        public HashSet<AccessoryEffect> ActiveEffects = new();
        internal AccessoryEffectInstance[] EffectInstances;
        public Dictionary<AccessoryEffect, bool> EffectToggle; // TODO: rework to be implemented properly with toggle-side of rework
        public override void Initialize()
        {
            int instanceCount = AccessoryEffectLoader.AccessoryEffectInstances.Count;
            EffectInstances = new AccessoryEffectInstance[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                EffectInstances[i] = AccessoryEffectLoader.AccessoryEffectInstances[i];
            }
        }
        
        public override void ResetEffects()
        {
            ActiveEffects.Clear();
            foreach (AccessoryEffectInstance effectInstance in EffectInstances)
            {
                effectInstance.ResetEffects();
            }
        }

        public override void PostUpdateEquips()
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.PostUpdateEquips(Player);
            }
        }
        // etc etc
    }
}
