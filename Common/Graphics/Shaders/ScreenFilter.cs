using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Shaders
{
	public class ScreenFilter
	{
		public Ref<Effect> Effect
		{
			get;
			internal set;
		}

		public Effect WrappedEffect => Effect.Value;

		/// <summary>
		/// A wrapper class for <see cref="Effect"/> that is focused around screen filter effects.
		/// </summary>
		public ScreenFilter(Ref<Effect> effect) => Effect = effect;

		public bool IsActive
		{
			get;
			private set;
		}

		/// <summary>
		/// The opacity of the filter. As long as this is above 0, the filter will be applied for that frame.
		/// </summary>
		public float Opacity
		{
			get;
			private set;
		}

		public Vector2 FocusPosition
		{
			get;
			private set;
		}

		/// <summary>
		/// Sets "mainColor" to the provided value, if it exists.
		/// </summary>
		/// <param name="color"></param>
		public ScreenFilter SetMainColor(Color color)
		{
			WrappedEffect.Parameters["mainColor"]?.SetValue(color.ToVector3());
			return this;
		}

		/// <summary>
		/// Sets "secondaryColor" to the provided value, if it exists.
		/// </summary>
		/// <param name="color"></param>
		public ScreenFilter SetSecondaryColor(Color color)
		{
			WrappedEffect.Parameters["secondaryColor"]?.SetValue(color.ToVector3());
			return this;
		}

		public ScreenFilter SetFocusPosition(Vector2 worldPosition)
		{
			FocusPosition = worldPosition;
			return this;
		}

		/// <summary>
		/// Call to indicate that the filter should be active. This needs to happen each frame it should be active for.
		/// </summary>
		public void Activate() => IsActive = true;

		/// <summary>
		/// Automatically called at the end of each update, after updating the filter.
		/// </summary>
		public void Deactivate() => IsActive = false;

		public void Update()
		{
			if (IsActive)
				Opacity = MathHelper.Clamp(Opacity + 0.015f, 0f, 1f);
			else
				Opacity = MathHelper.Clamp(Opacity - 0.015f, 0f, 1f);
		}

		/// <summary>
		/// Apply the filter.
		/// </summary>
		/// <param name="setCommonParams"> By default, it will set the "time" and "uWorldViewProjection" parameter if it exists.</param>
		/// <param name="pass">Specify a specific pass to use, if the shader has multiple.</param>
		public void Apply(bool setCommonParams = true, string pass = null)
		{
			// Apply commonly used parameters.
			if (setCommonParams)
				ApplyParams();

			WrappedEffect.CurrentTechnique.Passes[pass ?? ShaderManager.AutoloadPassName].Apply();
		}

		private void ApplyParams()
		{
			WrappedEffect.Parameters["time"]?.SetValue(Main.GlobalTimeWrappedHourly);
			WrappedEffect.Parameters["opacity"]?.SetValue(Opacity);
			WrappedEffect.Parameters["focusPosition"]?.SetValue(FocusPosition);
			WrappedEffect.Parameters["screenPosition"]?.SetValue(Main.screenPosition);
			WrappedEffect.Parameters["screenSize"]?.SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
		}
	}
}
