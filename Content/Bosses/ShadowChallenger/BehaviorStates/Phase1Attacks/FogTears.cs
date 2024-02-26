using FargowiltasSouls.Common.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
	public partial class ShadowChallenger
	{
		public const int FogTears_AttackLength = 240;

		[AutoloadMethod]
		public void LoadTransition_FogTears()
		{
			StateMachine.RegisterTransition(BehaviorStates.FogTears, null, false, () => Timer > FogTears_AttackLength);
		}

		[AutoloadAsBehavior<BehaviorStates>(BehaviorStates.FogTears)]
		public void DoBehavior_FogTears()
		{

		}
	}
}
