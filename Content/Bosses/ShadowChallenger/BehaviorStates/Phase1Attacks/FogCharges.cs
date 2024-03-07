using FargowiltasSouls.Common.StateMachines;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
	public partial class ShadowChallenger
	{
		public const int FogCharges_AttackLength = 360;

		[AutoloadMethod]
		public void LoadTransition_FogCharges()
		{
			StateMachine.RegisterTransition(BehaviorStates.FogCharges, BehaviorStates.FogTears, false, () => Timer > FogCharges_AttackLength);
		}

		[AutoloadAsBehavior<BehaviorStates>(BehaviorStates.FogCharges)]
		public void DoBehavior_FogCharges()
		{
			// TODO: Program attack.
		}
	}
}