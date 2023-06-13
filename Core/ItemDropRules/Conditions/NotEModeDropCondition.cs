using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class NotEModeDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && !FargoSoulsWorld.EternityMode;

        public bool CanShowItemDropInUI() => !FargoSoulsWorld.EternityMode;

        public string GetConditionDescription() => $"[i:{ModContent.ItemType<Items.Masochist>()}]Non-Eternity Mode drop rate";
    }
}
