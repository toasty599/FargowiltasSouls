using FargowiltasSouls.Common.StateMachines;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
	public partial class ShadowChallenger
	{
		[AutoloadMethod]
		public void LoadTransition_RefillAttacks()
		{
			StateMachine.RegisterTransition(BehaviorStates.RefillStates, null, false, () => true, () =>
			{
				// TODO: Refill state stack with attacks.
			});
		}
	}
}
