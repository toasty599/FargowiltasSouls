using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class DownedEvilBossDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && NPC.downedBoss2;

        public bool CanShowItemDropInUI() => NPC.downedBoss2;

        public string GetConditionDescription() => $"[i:After any Evil Boss has been defeated";
    }
}
