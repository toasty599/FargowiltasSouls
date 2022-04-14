using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasSouls
{
    class SoulConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static SoulConfig Instance => ModContent.GetInstance<SoulConfig>();

        [Label("Only show effect toggler when inventory is open")]
        [Description("If true, the effect toggler is automatically hidden when your inventory is closed.")]
        [DefaultValue(true)]
        public bool HideTogglerWhenInventoryIsClosed;

        [Label("Mutant boss music effect")]
        [DefaultValue(true)]
        public bool MutantMusicIsRePrologue;

        private const float max4kX = 3840f;
        private const float max4kY = 2160f;

        [Label("Inventory icon X position")]
        [Increment(1f)]
        [Range(0f, max4kX)]
        [DefaultValue(610f)]
        public float OncomingMutantX;

        [Label("Inventory icon Y position")]
        [Increment(1f)]
        [Range(0f, max4kY)]
        [DefaultValue(250f)]
        public float OncomingMutantY;

        #region maso

        [Header("$Mods.FargowiltasSouls.MasoHeader")]

        [Label("$Mods.FargowiltasSouls.MasoBossRecolors")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool BossRecolors;
        
        #endregion


        #region patreon

        [Header("$Mods.FargowiltasSouls.PatreonHeader")]
        [Label("$Mods.FargowiltasSouls.PatreonRoomba")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonRoomba;

        [Label("$Mods.FargowiltasSouls.PatreonOrb")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonOrb;

        [Label("$Mods.FargowiltasSouls.PatreonFishingRod")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishingRod;

        [Label("$Mods.FargowiltasSouls.PatreonDoor")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDoor;

        [Label("$Mods.FargowiltasSouls.PatreonWolf")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonWolf;

        [Label("$Mods.FargowiltasSouls.PatreonDove")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDove;

        [Label("$Mods.FargowiltasSouls.PatreonKingSlime")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonKingSlime;

        [Label("$Mods.FargowiltasSouls.PatreonFishron")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishron;

        [Label("$Mods.FargowiltasSouls.PatreonPlant")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPlant;

        [Label("$Mods.FargowiltasSouls.PatreonDevious")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDevious;

        [Label("$Mods.FargowiltasSouls.PatreonVortex")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonVortex;

        [Label("$Mods.FargowiltasSouls.PatreonPrime")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPrime;

        [Label("$Mods.FargowiltasSouls.PatreonCrimetroid")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonCrimetroid;

        #endregion


        /*[Label("$Mods.FargowiltasSouls.ThoriumHeader")]
        public ThoriumToggles thoriumToggles = new ThoriumToggles();

        [Label("$Mods.FargowiltasSouls.CalamityHeader")]
        public CalamityToggles calamityToggles = new CalamityToggles();*/


        //soa soon tm

        // Proper cloning of reference types is required because behind the scenes many instances of ModConfig classes co-exist.
        /*public override ModConfig Clone()
        {
            var clone = (SoulConfig)base.Clone();

            clone.walletToggles = walletToggles == null ? null : new WalletToggles();
            clone.thoriumToggles = thoriumToggles == null ? null : new ThoriumToggles();
            clone.calamityToggles = calamityToggles == null ? null : new CalamityToggles();

            return clone;
        }*/

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            OncomingMutantX = Utils.Clamp(OncomingMutantX, 0, max4kX);
            OncomingMutantY = Utils.Clamp(OncomingMutantY, 0, max4kY);
        }
    }
}
