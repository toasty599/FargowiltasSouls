// Original implementation by Dominic Karma, used here with permission by Toasty. Do not copy elsewhere without getting permission first, or face action being taken against your mod.

namespace FargowiltasSouls.Common.StateMachines
{
	public class State<TStateEnum>
	{
		public TStateEnum ID
		{
			get;
			private set;
		}

		public State(TStateEnum id)
		{
			ID = id;
		}

		public virtual void OnPop()
		{

		}
	}
}
