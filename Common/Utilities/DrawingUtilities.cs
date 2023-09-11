using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;

namespace FargowiltasSouls
{
	public static partial class FargoSoulsUtil
	{
		public static void CreatePerspectiveMatrixes(out Matrix view, out Matrix projection)
		{
			int height = Main.instance.GraphicsDevice.Viewport.Height;

			Vector2 zoom = Main.GameViewMatrix.Zoom;
			Matrix zoomScaleMatrix = Matrix.CreateScale(zoom.X, zoom.Y, 1f);

			// Get a matrix that aims towards the Z axis (these calculations are relative to a 2D world).
			view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

			// Offset the matrix to the appropriate position.
			view *= Matrix.CreateTranslation(0f, -height, 0f);

			// Flip the matrix around 180 degrees.
			view *= Matrix.CreateRotationZ(MathHelper.Pi);

			if (Main.LocalPlayer.gravDir == -1f)
				view *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, height, 0f);

			// Account for the current zoom.
			view *= zoomScaleMatrix;

			projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth * zoom.X, 0f, Main.screenHeight * zoom.Y, 0f, 1f) * zoomScaleMatrix;
		}

		private static readonly FieldInfo shaderTextureField = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly FieldInfo shaderTextureField2 = typeof(MiscShaderData).GetField("_uImage2", BindingFlags.NonPublic | BindingFlags.Instance);

		/// <summary>
		/// Uses reflection to set uImage1. Its underlying data is private and the only way to change it publicly
		/// is via a method that only accepts paths to vanilla textures.
		/// </summary>
		/// <param name="shader">The shader</param>
		/// <param name="texture">The texture to set</param>
		public static void SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture) => shaderTextureField.SetValue(shader, texture);

		/// <summary>
		/// Uses reflection to set uImage2. Its underlying data is private and the only way to change it publicly
		/// is via a method that only accepts paths to vanilla textures.
		/// </summary>
		/// <param name="shader">The shader</param>
		/// <param name="texture">The texture to set</param>
		public static void SetShaderTexture2(this MiscShaderData shader, Asset<Texture2D> texture) => shaderTextureField2.SetValue(shader, texture);

		/// <summary>
		/// Prepares a <see cref="SpriteBatch"/> for shader-based drawing.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch.</param>
		public static void EnterShaderRegion(this SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

		/// <summary>
		/// Ends changes to a <see cref="SpriteBatch"/> based on shader-based drawing in favor of typical draw begin states.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch.</param>
		public static void ExitShaderRegion(this SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public static void SetTexture1(this Texture2D texture) => Main.instance.GraphicsDevice.Textures[1] = texture;

		public static void SetTexture2(this Texture2D texture) => Main.instance.GraphicsDevice.Textures[2] = texture;

		public static Vector4 ToVector4(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
	}
}
