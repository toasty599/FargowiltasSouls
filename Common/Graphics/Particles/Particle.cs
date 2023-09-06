using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	/// <summary>
	/// Represents a particle instance.
	/// </summary>
	public abstract class Particle
	{
		#region Statics
		internal static List<Particle> ActiveParticles = new();

		public static int AnimationRate => 6;

		public static int DefaultFadeTime => 20;

		public static int MaxParticleCount => 1000;

		/// <summary>
		/// A bloom texture commonly used by many particles.
		/// </summary>
		public static Texture2D CommonBloomTexture => ModContent.Request<Texture2D>(ParticleTexturePath + "Bloom").Value;

		public const string ParticleTexturePath = "FargowiltasSouls/Common/Graphics/Particles/ParticleTextures/";

		/// <summary>
		/// Do not call.
		/// </summary>
		internal static void UpdateParticles()
		{
			// Particles are client side.
			if (Main.netMode == NetmodeID.Server)
				return;

			if (!ActiveParticles.Any())
				return;

			foreach (var particle in ActiveParticles)
			{
				particle.Update();
				particle.Position += particle.Velocity;
				particle.Rotation += particle.RotationSpeed;

				if (particle.UsesFrames && particle.IsAnimated)
				{
					if (particle.FrameTimer > AnimationRate)
					{
						particle.CurrentFrame = (particle.CurrentFrame + 1) % particle.MaxFrames;
						particle.FrameTimer = 0;
					}
					else
						particle.FrameTimer++;
				}

				particle.Timer++;
			}

			ActiveParticles.RemoveAll(p => p.Timer > p.MaxLifetime);
		}

		/// <summary>
		/// Do not call.
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="layerToDraw"></param>
		internal static void DrawParticles(SpriteBatch spriteBatch, ParticleLayers layerToDraw)
		{
			if (!ActiveParticles.Any(particle => particle.Layer == layerToDraw))
				return;

			List<Particle> additiveParticles = new();
			List<Particle> nonPreMultipliedparticles = new();
			List<Particle> alphaParticles = new();

			// Only draw the correct layer.
			foreach (var particle in ActiveParticles.Where(p => p.Layer == layerToDraw))
			{
				if (particle.UseAdditiveBlend)
					additiveParticles.Add(particle);
				else if (particle.UseNonPreMultipliedBlend)
					nonPreMultipliedparticles.Add(particle);
				else
					alphaParticles.Add(particle);
			}

			if (additiveParticles.Any())
			{
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				foreach (var particle in additiveParticles)
					particle.Draw(spriteBatch);

				spriteBatch.End();
			}

			if (nonPreMultipliedparticles.Any())
			{
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				foreach (var particle in nonPreMultipliedparticles)
					particle.Draw(spriteBatch);

				spriteBatch.End();
			}

			if (alphaParticles.Any())
			{
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				foreach (var particle in alphaParticles)
					particle.Draw(spriteBatch);

				spriteBatch.End();
			}
		}
		#endregion

		#region Fields/Properties
		public int Timer;
		public int MaxLifetime;
		public int Variant;
		public int CurrentFrame;
		public int FrameTimer;
		public int Width;
		public int Height;

		/// <summary>
		/// The draw rotation of the particle.
		/// </summary>
		public float Rotation;

		/// <summary>
		/// How much the particle's rotation should change per frame.
		/// </summary>
		public float RotationSpeed;

		/// <summary>
		/// How opaque the particle is.
		/// </summary>
		public float Opacity;

		/// <summary>
		/// Whether the particle fades in.
		/// </summary>
		public bool FadeIn;

		/// <summary>
		/// Whether the particle should not despawn if the particle limit is reached. Should be set
		/// true for particles being used as telegraphs.
		/// </summary>
		public bool Important;

		/// <summary>
		/// The world position of the particle.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// How much the particle's position should move each frame.
		/// </summary>
		public Vector2 Velocity;

		/// <summary>
		/// The draw scale of the particle.
		/// </summary>
		public Vector2 Scale;

		/// <summary>
		/// The color to draw the particle with.
		/// </summary>
		public Color DrawColor;

		/// <summary>
		/// The draw layer for the particle. Is the dust layer by default.
		/// </summary>
		public ParticleLayers Layer = ParticleLayers.Dust;

		/// <summary>
		/// The texture of the particle. By default, this is the file path of the particle plus the type name.
		/// </summary>
		public virtual Texture2D MainTexture => ModContent.Request<Texture2D>(ParticleTexturePath + GetType().Name).Value;

		/// <summary>
		/// Whether the particle should use additive blend. This takes priority over NonPreMultiplied.
		/// </summary>
		public virtual bool UseAdditiveBlend => false;

		/// <summary>
		/// Whether the particle should use Non Pre-Multiplied blend.
		/// </summary>
		public virtual bool UseNonPreMultipliedBlend => false;

		/// <summary>
		/// Whether the particle should have its framing automatically animated.
		/// </summary>
		public virtual bool IsAnimated => false;

		/// <summary>
		/// How many frames the <see cref="MainTexture"/> has.
		/// </summary>
		public virtual int MaxFrames => 1;

		/// <summary>
		/// Whether the particle uses frames.
		/// </summary>
		public bool UsesFrames => MaxFrames > 1;

		/// <summary>
		/// How many frames are left before the particle will despawn.
		/// </summary>
		public int TimeLeft => MaxLifetime - Timer;

		/// <summary>
		/// A 0-1 value from just spawned, to despawned over the particles lifetime.
		/// </summary>
		public float LifetimeRatio => (float)Timer / MaxLifetime;
		#endregion

		#region Methods
		public virtual void Update() { }

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			Rectangle? frame = null;
			Vector2 origin = MainTexture.Size() * 0.5f;
			if (UsesFrames)
			{
				frame = new(Variant * Width, CurrentFrame * Height, Width, Height);
				origin = frame.Value.Size() * 0.5f;
			}

			Vector2 screenPos = Position - Main.screenPosition;

			spriteBatch.Draw(MainTexture, screenPos, frame, DrawColor * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
		}

		/// <summary>
		/// Call to set whether the particle fades in or not.
		/// </summary>
		/// <param name="fadeIn"></param>
		/// <returns></returns>
		public Particle SetFadeIn(bool fadeIn = true)
		{
			FadeIn = fadeIn;
			return this;
		}

		/// <summary>
		/// Call to specifiy the draw layer of the particle to be non default.
		/// </summary>
		/// <param name="layer"></param>
		/// <returns></returns>
		public Particle SpecifyLayer(ParticleLayers layer)
		{
			Layer = layer;
			return this;
		}

		/// <summary>
		/// Call to actually spawn the particle in the world.
		/// </summary>
		/// <returns></returns>
		public void Spawn()
		{
			if (Main.netMode is NetmodeID.Server)
				return;

			// If the particle is abstract, do not spawn it.
			if (GetType().IsAbstract)
				return;

			// Stay within the chosen particle limit by removing the earliest, non important particle.
			if (ActiveParticles.Count > MaxParticleCount)
			{
				Particle particleToRemove = ActiveParticles.FirstOrDefault(particle => !particle.Important);
				if (particleToRemove != null)
					ActiveParticles.Remove(particleToRemove);
			}

			ActiveParticles.Add(this);
			return;
		}
		#endregion
	}
}