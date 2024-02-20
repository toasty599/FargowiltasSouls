using System;
using System.Reflection;

namespace FargowiltasSouls.Common.StateMachines
{
	/// <summary>
	/// Marks a method as assositated with the provided <typeparamref name="TStateID"/> for the purpose of automated state machine behavior linking.
	/// </summary>
	/// <typeparam name="TStateID"></typeparam>
	[AttributeUsage(AttributeTargets.Method)]
	public class AutoloadAsBehavior<TStateID> : Attribute where TStateID : struct
	{
		/// <summary>
		/// The assosiated state of the method.
		/// </summary>
		public TStateID AssosiatedState;

		/// <summary>
		/// Marks a method as assositated with the provided <typeparamref name="TStateID"/> for the purpose of automated state machine behavior linking.
		/// </summary>
		/// <param name="assosiatedState">The state to register this method as behavior for.</param>
		public AutoloadAsBehavior(TStateID assosiatedState) => AssosiatedState = assosiatedState;

		/// <summary>
		/// Fills the <paramref name="stateMachine"/>'s behaviors with all methods in the provided <paramref name="classToCheck"/> that have this attribute.
		/// </summary>
		/// <typeparam name="TInstanceType">The type of the instance that will access the methods.</typeparam>
		/// <param name="stateMachine">The state machine to fill.</param>
		/// <param name="classToCheck">The class to check the methods of.</param>
		/// <param name="instance">The instance to access the methods with.</param>
		public static void FillStateMachineBehaviors<TInstanceType>(FiniteStateMachine<AIState<TStateID>, TStateID> stateMachine, Type classToCheck, TInstanceType instance)
		{
			var methods = classToCheck.GetMethods(BindingFlags.Instance | BindingFlags.Public);
			if (methods == null || methods.Length == 0)
				return;

			foreach (var method in methods)
			{
				var autoloadAttribute = method.GetCustomAttribute<AutoloadAsBehavior<TStateID>>();
				if (autoloadAttribute != null)
					stateMachine.RegisterStateBehavior(autoloadAttribute.AssosiatedState, () => method.Invoke(instance, null));
			}
		}
	}
}