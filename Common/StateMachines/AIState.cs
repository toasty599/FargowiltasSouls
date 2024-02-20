 // Original implementation by Dominic Karma, used here with permission by Toasty. Do not copy elsewhere without getting permission first, or face action being taken against your mod.

namespace FargowiltasSouls.Common.StateMachines
{
	/// <summary>
	/// A <see cref="State{TStateEnum}"/> for use for basic enemy behavior. Includes a timer that resets on each state pop.
	/// </summary>
	/// <typeparam name="TStateEnum">The state enumeration.</typeparam>
	public class AIState<TStateEnum> : State<TStateEnum>
	{
		/// <summary>
		/// The timer for the current state.
		/// </summary>
		public float Timer;

		/// <summary>
		/// A <see cref="State{TStateEnum}"/> for use for basic enemy behavior. Includes a timer that resets on each state pop.
		/// </summary>
		/// <param name="id">The state enumeration to start with.</param>
		public AIState(TStateEnum id) : base(id)
		{
			Timer = 0;
		}

		public override void OnPop()
		{
			Timer = 0;
		}
	}
}
