using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class EModeNotMasterDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && !info.IsMasterMode && FargoSoulsWorld.EternityMode;

        public bool CanShowItemDropInUI() => !Main.masterMode && FargoSoulsWorld.EternityMode;

        public string GetConditionDescription() => "This is a Master/Eternity Mode drop rate";
    }
}
