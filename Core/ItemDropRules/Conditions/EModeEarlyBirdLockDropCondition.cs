using FargowiltasSouls.Content.Items;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class EModeEarlyBirdLockDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && (Main.hardMode || !FargoSoulsWorld.EternityMode);

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => $"[i:{ModContent.ItemType<Masochist>()}]Hardmode Eternity Mode drop rate";
    }
}
