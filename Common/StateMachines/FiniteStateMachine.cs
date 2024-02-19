// Original implementation by Dominic Karma, used here with permission by Toasty. Do not copy elsewhere without getting permission first, or face action being taken against your mod.

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

		private sealed class StateTransitionHijack
		{
			public StateTransitionHijackDelegate SelectionHijackFunction;

			public Action<TStateID?> HijackAction;

			public StateTransitionHijack(StateTransitionHijackDelegate selectionHijackFunction, Action<TStateID?> hijackAction)
			{
				SelectionHijackFunction = selectionHijackFunction;
				HijackAction = hijackAction;
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
		/// The current state on top of the stack. Returns null if not present.
		/// </summary>
		public TState CurrentState
		{
			get
			{
				if (StateStack.TryPeek(out var state))
					return state;
				return null;
			}
		}

		private readonly List<StateTransitionHijack> HijackActions = new();

		/// <summary>
		/// Delegate for actions that run when <see cref="OnStateTransition"/> is fired.
		/// </summary>
		/// <param name="stateWasPopped"></param>
		public delegate void OnStateTransitionDelegate(bool stateWasPopped, TState oldState);

		/// <summary>
		/// Delegate for hijacking state transitions. Return originalState if the hijack should not occur.
		/// </summary>
		/// <param name="originalState">The original state that was going to be selected.</param>
		/// <returns>The state to select.</returns>
		public delegate TStateID? StateTransitionHijackDelegate(TStateID? originalState);

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
		/// Registers a hijack transition, that allows for hijacking a transitions final state selection. A good use example is for phase transitions that should occur after an attack has been completed.
		/// </summary>
		/// <param name="selectionHijackFunction">The function to replace the final state selection.</param>
		/// <param name="hijackAction">An optional action to perform when this hijack occurs.</param>
		public void AddTransitionStateHijack(StateTransitionHijackDelegate selectionHijackFunction, Action<TStateID?> hijackAction = null) => HijackActions.Add(new(selectionHijackFunction, hijackAction));

		/// <summary>
		/// Applies an action to every registered state in this machine, barring any provided exceptions. A good use example is for states that interrupt most other states if a certain condition is met.
		/// </summary>
		/// <param name="action">The action to perform.</param>
		/// <param name="exceptions">The list of exceptions.</param>
		public void ApplyToAllStatesExcept(Action<TStateID> action, params TStateID[] exceptions)
		{
			foreach (var pair in StateRegistry)
			{
				if (exceptions.Contains(pair.Key))
					continue;

				action(pair.Key);
			}
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
			TState oldState = null;
			if (!transition.ShouldRememberPreviousState && StateStack.TryPop(out oldState))
				oldState.OnPop();

			var newState = transition.NewState;
			// Get the first hijack transition that does *not* return the same state.
			var usedHijackAction = HijackActions.FirstOrDefault(h => !h.SelectionHijackFunction(newState).Equals(newState));
			if (usedHijackAction is not null)
			{
				newState = usedHijackAction.SelectionHijackFunction(newState);
				usedHijackAction.HijackAction?.Invoke(newState);
			}
			if (newState is not null)
				StateStack.Push(StateRegistry[newState.Value]);

			// Access the optional callback.
			transition.TransitionCallback?.Invoke();

			OnStateTransition?.Invoke(!transition.ShouldRememberPreviousState, oldState);

			// Recursively call this to allow for all transitions with met conditions to be processed.
			ProcessTransitions();
		}
	}
}
