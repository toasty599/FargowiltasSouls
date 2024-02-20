using System.Runtime.CompilerServices;

namespace FargowiltasSouls
{
	public static partial class FargoSoulsUtil
	{
		/// <summary>
		/// Converts the provided seconds amount to the equivalent time in frames (60 per second).
		/// </summary>
		/// <param name="timeInSeconds">The amount to convert.</param>
		/// <returns>The converted amount of time.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SecondsToFrames(float timeInSeconds) => timeInSeconds * 60f;
	}
}
