using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class EModeDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && FargoSoulsWorld.EternityMode;

        public bool CanShowItemDropInUI() => FargoSoulsWorld.EternityMode;

        public string GetConditionDescription() => $"[i:{ModContent.ItemType<Items.Masochist>()}]Eternity Mode drop rate";
    }
}
