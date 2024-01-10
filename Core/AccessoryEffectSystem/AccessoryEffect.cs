using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    //Proof of concept file for an accessory effect rework

    /// <summary>
    /// Contains the behavior for an accessory effect. <para/>
    /// Each accessory effect needs a localized toggle description as Mods.YourMod.Toggler.YourAccessoryEffectName.<para/>
    /// This type is not instanced per player. Put instanced things (such as fields) in an AccessoryEffectInstance.
    /// </summary>
    public abstract class AccessoryEffect : ModType
    {
        /// <summary>
        /// Whether the effect has a toggle. <para\>
        /// ToggleHeader is unused if this is false.
        /// </summary>
        public abstract bool HasToggle { get; }
        public string ToggleDescription => Language.GetTextValue($"Mods.{Mod}.Toggler.{Name}");
        public Header ToggleHeader { get; }

        public bool MinionEffect = false;
        public bool ExtraAttackEffect = false;
        public virtual void PostUpdateEquips(Player player) { }

        // Add more methods as needed here
        protected sealed override void Register()
        {
            AccessoryEffectLoader.Register(this);
            ModTypeLookup<AccessoryEffect>.Register(this);
        }
    }
    /// <summary>
    /// Contains the parts of an accessory effect that should be instanced - for example fields.
    /// </summary>
    public abstract class AccessoryEffectInstance : ModType<ModPlayer, AccessoryEffectInstance>, IIndexed
    {
        public ModPlayer ModPlayer => Entity;
        public Player Player => Entity.Player;
        public ushort Index { get; internal set; }
        protected override ModPlayer CreateTemplateEntity() => null;
        public override AccessoryEffectInstance NewInstance(ModPlayer entity)
        {
            var inst = base.NewInstance(entity);

            inst.Index = Index;
            return inst;
        }

        /// <summary>
        /// Runs in ModPlayer ResetEffects (once per frame). See ModPlayer.ResetEffects for more info.
        /// </summary>
        public virtual void ResetEffects() { }
        protected sealed override void Register()
        {
            AccessoryEffectLoader.Register(this);
            ModTypeLookup<AccessoryEffectInstance>.Register(this);
        }
    }
}
