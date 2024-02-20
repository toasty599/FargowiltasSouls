using FargowiltasSouls.Common.StateMachines;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
	public partial class CursedCoffin
	{
		private FiniteStateMachine<AIState<BehaviorStates>, BehaviorStates> stateMachine;

		/// <summary>
		/// The state machine that controls the behavior of this NPC.
		/// </summary>
		public FiniteStateMachine<AIState<BehaviorStates>, BehaviorStates> StateMachine
		{
			get
			{
				if (stateMachine == null)
					LoadStateMachine();

				return stateMachine;
			}
			set => stateMachine = value;
		}

		private void LoadStateMachine()
		{
			StateMachine = new(new(BehaviorStates.Opening));

			for (int i = 0; i < (int)BehaviorStates.Count; i++)
				StateMachine.RegisterState(new((BehaviorStates)i));

			StateMachine.OnStateTransition += OnStateTransition;

			// Autoload the state behaviors.
			AutoloadAsBehavior<BehaviorStates>.FillStateMachineBehaviors<ModNPC>(StateMachine, GetType(), this);

			// Load the attack cycle resetter and the phase transition.
			LoadTransition_ResetCycle();
			LoadTransition_PhaseTwoTransition();

			// Register each attack transition.
			#region Transition Registering
			// A basic example of a transition that doesnt go to anything specific here, it requires the current state to be the opening one, and the timer to be -1 to occur.
			StateMachine.RegisterTransition(BehaviorStates.Opening, null, false, () => Timer == -1);

			// An example of a more complex transition, where it goes from the phase 2 transition to the slam shockwave if the condition is met, and also performs additional stuff on occuring.
			StateMachine.RegisterTransition(BehaviorStates.PhaseTransition, BehaviorStates.SlamWShockwave, false, () => Timer >= 60, () =>
			{
				SoundEngine.PlaySound(PhaseTransitionSFX, NPC.Center);
				PhaseTwo = true;
				NPC.netUpdate = true;
				if (FargoSoulsUtil.HostCheck)
				{
					Vector2 maskCenter = MaskCenter();
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)maskCenter.X, (int)maskCenter.Y, ModContent.NPCType<CursedSpirit>(), ai0: NPC.whoAmI);
				}
				NPC.velocity = Vector2.UnitY * 0.1f;
				LockVector1 = Player.Top - Vector2.UnitY * 250;
				AI2 = 2; // only bounce once
			});

			// An example of using this function to apply a transition to a bunch of states at once, in this case for an interrupting attack.
			StateMachine.ApplyToAllStatesExcept((state) =>
			{
				StateMachine.RegisterTransition(state, BehaviorStates.StunPunish, false, () => Player.HasBuff<StunnedBuff>() && !Main.projectile.Any(p => p.TypeAlive<CoffinHand>()));
			}, BehaviorStates.StunPunish, BehaviorStates.PhaseTransition);

			StateMachine.RegisterTransition(BehaviorStates.StunPunish, null, false, () => Timer > 20 && Frame <= 0, () =>
			{
				NPC.frameCounter = 0;
				Frame = 0;
			});

			StateMachine.RegisterTransition(BehaviorStates.HoveringForSlam, BehaviorStates.SlamWShockwave, false, () => Timer == AI3, () =>
			{
				NPC.velocity.Y = -5;
				NPC.velocity.X /= 2;
				LockVector1 = Player.Top - Vector2.UnitY * 250;
				AI2 = 0;
			});

			StateMachine.RegisterTransition(BehaviorStates.SlamWShockwave, null, false, () => Timer == -1);

			StateMachine.RegisterTransition(BehaviorStates.WavyShotCircle, null, false, () =>
			{
				int telegraphTime = WorldSavingSystem.MasochistModeReal ? 60 : 70;
				//bool phase1Condition = Timer == telegraphTime && (WorldSavingSystem.MasochistModeReal || !PhaseTwo);
				bool phase2InitialCondition = Timer > telegraphTime + (WorldSavingSystem.MasochistModeReal || AI3 < 1 ? 20 : 50);
				bool phase2SecondaryCondition = PhaseTwo && AI3 < 1 && WorldSavingSystem.EternityMode;
				return/* phase1Condition || */(phase2InitialCondition && !phase2SecondaryCondition);
			});

			StateMachine.RegisterTransition(BehaviorStates.WavyShotFlight, BehaviorStates.SlamWShockwave, false, () => Timer >= WavyShotFlightPrepTime + WavyShotFlightCirclingTime + WavyShotFlightEndTime, () =>
			{
				Frame = 0;
				NPC.velocity.X /= 2;
				NPC.velocity.Y = -5;
				if (NPC.velocity.Y == 0)
					NPC.velocity.Y = -0.1f;
				LockVector1 = Player.Top - Vector2.UnitY * 250;
				AI2 = 2; // only bounce once
			});

			StateMachine.RegisterTransition(BehaviorStates.GrabbyHands, BehaviorStates.SlamWShockwave, false, () => Timer > 40 && Frame <= 0 && Timer > AI3 + 10 && NPC.Center.Y < Player.Center.Y, () =>
			{
				NPC.noTileCollide = true;
				LockVector1 = Player.Top - Vector2.UnitY * 250;
				NPC.velocity.Y -= 5;
				NPC.velocity.X /= 2;
			});

			StateMachine.RegisterTransition(BehaviorStates.GrabbyHands, BehaviorStates.WavyShotFlight, false, () => Timer > 40 && Frame <= 0 && Timer > AI3 + 10 && NPC.Center.Y >= Player.Center.Y, () =>
			{
				NPC.noTileCollide = true;
				LockVector1 = Player.Top - Vector2.UnitY * 250;
				NPC.velocity.Y -= 5;
				NPC.velocity.X /= 2;
			});

			StateMachine.RegisterTransition(BehaviorStates.RandomStuff, BehaviorStates.SlamWShockwave, false, () => Timer > RandomStuffOpenTime + 310 && Frame <= 0 && NPC.Center.Y < Player.Center.Y - 100, () =>
			{
				NPC.noTileCollide = true;
				LockVector1 = Player.Top - Vector2.UnitY * 250;
				NPC.velocity.Y -= 5;
				NPC.velocity.X /= 2;
				NPC.velocity = Vector2.Zero;
				NPC.rotation = 0;
				NPC.frameCounter = 0;
				Frame = 0;
			});

			StateMachine.RegisterTransition(BehaviorStates.RandomStuff, BehaviorStates.WavyShotFlight, false, () => Timer > RandomStuffOpenTime + 310 && Frame <= 0 && NPC.Center.Y >= Player.Center.Y - 100, () =>
			{
				NPC.velocity = Vector2.Zero;
				NPC.rotation = 0;
				NPC.frameCounter = 0;
				Frame = 0;
			});
			#endregion
		}

		// This is ran whenever a state transition occures and is very useful for resetting variables.
		public void OnStateTransition(bool stateWasPopped, AIState<BehaviorStates> oldState)
		{
			NPC.netUpdate = true;
			NPC.TargetClosest(false);
			AI2 = 0;
			AI3 = 0;

			if (oldState != null)
				LastAttackChoice = (int)oldState.ID;
		}

		public void LoadTransition_ResetCycle()
		{
			StateMachine.RegisterTransition(BehaviorStates.RefillAttacks, null, false, () => true, () =>
			{
				NPC.netUpdate = true;

				if (!FargoSoulsUtil.HostCheck)
					return;

				StateMachine.StateStack.Clear();

				// Get the correct attack list, and remove the last attack to avoid repeating it.
				var attackList = PhaseTwo ? P2Attacks : P1Attacks;
				attackList.Remove((BehaviorStates)LastAttackChoice);

				// Fill a list of indices.
				var indices = new List<int>();
				for (int i = 0; i < attackList.Count; i++)
					indices.Add(i);

				// Randomly push the attack list using the indices list accessed with a random index.
				for (int i = 0; i < attackList.Count; i++)
				{
					var currentIndex = indices[Main.rand.Next(0, indices.Count)];
					StateMachine.StateStack.Push(StateMachine.StateRegistry[attackList[currentIndex]]);
					indices.Remove(currentIndex);
				}
			});
		}

		public void LoadTransition_PhaseTwoTransition()
		{
			// Example of a transition hijack, which is checked for any possible starting state when a transition occurs and will replace any normal transition
			// if it returns anything other than the provided state.
			StateMachine.AddTransitionStateHijack(originalState =>
			{
				// Transition to phase 2 if required.
				if (!PhaseTwo && NPC.GetLifePercent() <= 0.8f)
				{
					// Clear the stack to ensure states do not linger.
					StateMachine.StateStack.Clear();
					return BehaviorStates.PhaseTransition;
				}

				return originalState;
			});
		}
	}
}