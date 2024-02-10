using System;
using System.Collections.Generic;
using System.Linq;

namespace FargowiltasSouls.Common.StateMachines
{
	/// <summary>
	/// Represents a finite state machine, that handles behavior states and processing them.
	/// </summary>
	/// <typeparam name="TState">The state type that this machine is set up with.</typeparam>
	/// <typeparam name="TStateID">The ID of the state type that the machine is set up with. Should be an enum in 99% of cases.</typeparam>
	public sealed class FiniteStateMachine<TState, TStateID> where TState : State<TStateID> where TStateID : struct
	{
		private sealed class StateTransitionInfo
		{
			/// <summary>
			/// The state to transition to.
			/// </summary>
			public TStateID? NewState;

			/// <summary>
			/// Whether the previous state should be retained when the transition happens. If true, this ensures that the previous state is not popped from the stack.
			/// </summary>
			public bool ShouldRememberPreviousState;

			/// <summary>
			/// Whether the transition is ready to happen.
			/// </summary>
			public Func<bool> TransitionCondition;

			/// <summary>
			/// An action that determines any special things that happen after a transition.
			/// </summary>
			public Action TransitionCallback;


			public StateTransitionInfo(TStateID? newState, bool shouldRememberPreviousState, Func<bool> transitionCondition, Action transitionCallback = null)
			{
				NewState = newState;
				ShouldRememberPreviousState = shouldRememberPreviousState;
				TransitionCondition = transitionCondition;
				TransitionCallback = transitionCallback;
			}
		}

		/// <summary>
		/// A lookup table of states, accessed by their ID.
		/// </summary>
		public Dictionary<TStateID, TState> StateRegistry = new();

		/// <summary>
		/// A table of states, and their behaviors.
		/// </summary>
		public Dictionary<TStateID, Action> StateBehaviors = new();

		private readonly Dictionary<TStateID, List<StateTransitionInfo>> TransitionTable = new();

		/// <summary>
		/// An ordered stack of all of the current states to execute.
		/// </summary>
		public Stack<TState> StateStack = new();

		/// <summary>
		/// The current state on top of the stack.
		/// </summary>
		public TState CurrentState => StateStack.Peek();

		/// <summary>
		/// Delegate for actions that run when <see cref="OnStateTransition"/> is fired.
		/// </summary>
		/// <param name="stateWasPopped"></param>
		public delegate void OnStateTransitionDelegate(bool stateWasPopped);

		/// <summary>
		/// Fired when a transition occures.
		/// </summary>
		public event OnStateTransitionDelegate OnStateTransition;

		/// <summary>
		/// Represents a finite state machine, that handles behavior states and managing them.
		/// </summary>
		/// <param name="initialState">The state to start with.</param>
		public FiniteStateMachine(TState initialState)
		{
			StateStack.Push(initialState);
			RegisterState(initialState);
		}

		/// <summary>
		/// Registers a state in the states lookup table.
		/// </summary>
		/// <param name="state"></param>
		public void RegisterState(TState state) => StateRegistry[state.ID] = state;

		/// <summary>
		/// Registers a state with its assosiated behavior.
		/// </summary>
		/// <param name="id">The state ID to assosiate the behavior with.</param>
		/// <param name="behavior">The behavior action for the provided state.</param>
		public void RegisterStateBehavior(TStateID id, Action behavior) => StateBehaviors[id] = behavior;

		/// <summary>
		/// Registers a transition between two states.
		/// </summary>
		/// <param name="initialState">The state to transition from.</param>
		/// <param name="newState">The state to transition to.</param>
		/// <param name="shouldRememberPreviousState">Whether the current state should be popped from the stack, or remembered.</param>
		/// <param name="transitionCondition">The condition needed to be fulfilled to perform the transition.</param>
		/// <param name="transitionCallback">An optional action ran when this transition occures.</param>
		public void RegisterTransition(TStateID initialState, TStateID? newState, bool shouldRememberPreviousState, Func<bool> transitionCondition, Action transitionCallback = null)
		{
			// Initialize the list if needed.
			if (!TransitionTable.ContainsKey(initialState))
				TransitionTable[initialState] = new();

			TransitionTable[initialState].Add(new(newState, shouldRememberPreviousState, transitionCondition, transitionCallback));
		}

		/// <summary>
		/// Call to process the state behaviors for the current state in the machine.
		/// </summary>
		public void ProcessBehavior()
		{
			if (StateBehaviors.TryGetValue(CurrentState.ID, out var value))
				value();
		}

		/// <summary>
		/// Call to process any available transitions for the current state in the machine.
		/// </summary>
		public void ProcessTransitions()
		{
			if (!StateStack.Any() || !TransitionTable.ContainsKey(CurrentState.ID))
				return;

			// Get all transitionable states.
			var transitionalableStates = TransitionTable[CurrentState.ID].Where(stateTransition => stateTransition.TransitionCondition()).ToList();
			if (!transitionalableStates.Any())
				return;

			// Use the first one.
			var transition = transitionalableStates.First();

			// Pop the previous state if it shouldn't be remembered.
			if (!transition.ShouldRememberPreviousState && StateStack.TryPop(out var oldState))
				oldState.OnPop();

			var newState = transition.NewState;
			if (newState is not null)
				StateStack.Push(StateRegistry[newState.Value]);

			// Access the optional callback.
			transition.TransitionCallback?.Invoke();

			OnStateTransition?.Invoke(!transition.ShouldRememberPreviousState);

			// Recursively call this to allow for all transitions with met conditions to be processed.
			ProcessTransitions();
		}
	}
}
