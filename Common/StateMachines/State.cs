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
