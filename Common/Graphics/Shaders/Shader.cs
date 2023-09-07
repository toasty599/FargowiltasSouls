using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace FargowiltasSouls.Common.Graphics.Shaders
{
    /// <summary>
    /// A wrapper class for <see cref="Effect"/> that is less restrictive than Terraria's <see cref="MiscShaderData"/>. Supports both pixel and vertex shaders.
    /// </summary>
    public class Shader : IDisposable
    {
        /// <summary>
        /// The <see cref="Effect"/> that the wrapper contains.
        /// </summary>
        public Effect WrappedEffect
        {
            get;
            internal set;
        }

        /// <summary>
        /// A wrapper class for <see cref="Effect"/> that is less restrictive than Terraria's <see cref="MiscShaderData"/>.
        /// </summary>
        public Shader(Effect effect) => WrappedEffect = effect;

        /// <summary>
        /// Sets "mainColor" to the provided value, if it exists.
        /// </summary>
        /// <param name="color"></param>
        public Shader SetMainColor(Color color)
        {
			WrappedEffect.Parameters["mainColor"]?.SetValue(color.ToVector3());
			return this;
		}

        /// <summary>
        /// Sets "secondaryColor" to the provided value, if it exists.
        /// </summary>
        /// <param name="color"></param>
        public Shader SetSecondaryColor(Color color)
        {
			WrappedEffect.Parameters["secondaryColor"]?.SetValue(color.ToVector3());
			return this;
		}

		/// <summary>
		/// Sets "bloomColor" to the provided value, if it exists.
		/// </summary>
		/// <param name="color"></param>
		public Shader SetBloomColor(Color color)
		{
			WrappedEffect.Parameters["bloomColor"]?.SetValue(color.ToVector3());
			return this;
		}

		/// <summary>
		/// Sets "frame" to the provided value, if it exists.
		/// </summary>
		/// <param name="rectangle"></param>
		public Shader SetFrame(Rectangle rectangle)
        {
            WrappedEffect.Parameters["frame"]?.SetValue(rectangle.ToVector4());
			return this;
		}

		/// <summary>
		/// Sets "opacity" to the provided value, if it exists.
		/// </summary>
		/// <param name="opacity"></param>
		public Shader SetOpacity(float opacity)
        {
            WrappedEffect.Parameters["opacity"]?.SetValue(opacity);
            return this;
        }

        /// <summary>
        /// Apply the shader.
        /// </summary>
        /// <param name="setCommonParams"> By default, it will set the "time" and "uWorldViewProjection" parameter if it exists.</param>
        /// <param name="pass">Specify a specific pass to use, if the shader has multiple.</param>
        public Shader Apply(bool setCommonParams = true, string pass = null)
        {
            // Apply commonly used parameters.
            if (setCommonParams)
                ApplyParams();

            WrappedEffect.CurrentTechnique.Passes[pass ?? ShaderManager.AutoloadPassName]?.Apply();
            return this;
        }

        /// <summary>
        /// This is automatically called when <see cref="Apply(bool, string)"/> is.
        /// Only manually call if you are passing the effect into <see cref="SpriteBatch.Begin()"/>.
        /// </summary>
        public void ApplyParams()
        {
            WrappedEffect.Parameters["time"]?.SetValue(Main.GlobalTimeWrappedHourly);

            // Don't waste time doing matrix calculations if not needed.
            var vertexParam = WrappedEffect.Parameters["worldViewProjection"];
            if (vertexParam == null)
                return;

            FargoSoulsUtil.CreatePerspectiveMatrixes(out var view, out var projection);
            vertexParam.SetValue(view * projection);
        }

        /// <summary>
        /// Called during unloading to dispose of the effect the class contains. Do not manually call.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (WrappedEffect != null && !WrappedEffect.IsDisposed)
                WrappedEffect.Dispose();

            WrappedEffect = null;
        }
    }
}
