using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace FargowiltasSouls.Particles
{
    // THESE DO NOT CURRENTLY WORK, DO NOT USE.
    public class ParticleManager : ModSystem
    {
        private const int MaxParticles = 500;
        public static List<Particle> ActiveParticles { get; private set; }
        private static List<string> ParticleTypes;
        private static List<Particle> NormalDrawParticles;
        private static List<Particle> AdditiveDrawParticles;


        /// <summary>
        /// Initializes the lists and loads the particles.
        /// </summary>
        public static void LoadParticles()
        {
            ActiveParticles = new();
            ParticleTypes = new();
            AdditiveDrawParticles = new();
            NormalDrawParticles = new();

            // Get every particle, and add it to the end of a list. This list is used to get the type of each particle for spawning them.
            Type particle = typeof(Particle);
            foreach (Type type in AssemblyManager.GetLoadableTypes(FargowiltasSouls.Instance.Code))
            {
                if (type.IsSubclassOf(particle) && !type.IsAbstract)
                {
                    ParticleTypes.Add(type.FullName);
                }
            }
        }

        public override void Load()
        {
            On.Terraria.Main.DrawInfernoRings += DrawParticleCall;
        }

        private void DrawParticleCall(On.Terraria.Main.orig_DrawInfernoRings orig, Main self)
        {
            orig.Invoke(self);
            DrawParticles(Main.spriteBatch);
        }

        public override void Unload()
        {
            ActiveParticles = null;
            ParticleTypes = null;
            NormalDrawParticles = null;
            AdditiveDrawParticles = null;

            On.Terraria.Main.DrawInfernoRings -= DrawParticleCall;
        }

        /// <summary>
        /// Spawns a particle.
        /// </summary>
        /// <param name="particle"></param>
        public static void SpawnParticle(Particle particle)
        {
            if (Main.dedServ || Main.gamePaused)
                return;

            //ActiveParticles.Add(particle);
            return;
        }

        /// <summary>
        /// Called from ModPlayer.PreUpdate. Do not call.
        /// </summary>
        internal static void UpdateParticles()
        {
            foreach (Particle particle in ActiveParticles)
            {
                // Handle movement.
                particle.Position += particle.Velocity;
                particle.AI();
                particle.TimeLeft--;
                if (particle.TimeLeft <= 0)
                {
                    particle.OnKill();
                    ActiveParticles.Remove(particle);
                }
            }
        }

        internal static void DrawParticles(SpriteBatch spriteBatch)
        {
            // End the current spritebatch.
            spriteBatch.End();

            // Split the particles into lists depending on what spritebatch type they need.
            foreach (Particle particle in ActiveParticles)
            {
                if (particle.UseAdditiveDrawing)
                    AdditiveDrawParticles.Add(particle);
                else
                    NormalDrawParticles.Add(particle);
            }

            // Draw all additive particles.
            if (AdditiveDrawParticles.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Particle particle in AdditiveDrawParticles)
                {
                    Color lightColor = Lighting.GetColor((int)particle.Position.X, (int)particle.Position.Y);
                    particle.Draw(spriteBatch, ref lightColor);
                }
                spriteBatch.End();
            }

            // Draw all normal particles.
            if (NormalDrawParticles.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Particle particle in NormalDrawParticles)
                {
                    Color lightColor = Lighting.GetColor((int)particle.Position.X, (int)particle.Position.Y);
                    particle.Draw(spriteBatch, ref lightColor);
                }
                spriteBatch.End();
            }

            AdditiveDrawParticles.Clear();
            NormalDrawParticles.Clear();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
}
