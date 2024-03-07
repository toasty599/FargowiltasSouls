using FargowiltasSouls.Common.StateMachines;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
	public partial class ShadowChallenger
	{
		public const int FightIntro_AttackLength = 120;

		[AutoloadMethod]
		public void LoadTransitions_FightIntro()
		{
			StateMachine.RegisterTransition(BehaviorStates.FightIntro, BehaviorStates.FogCharges, false, () => Timer > FightIntro_AttackLength);
		}

		[AutoloadAsBehavior<BehaviorStates>(BehaviorStates.FightIntro)]
		public void DoBehavior_FightIntro()
		{
			// TODO: Program attack.
		}
	}
}
