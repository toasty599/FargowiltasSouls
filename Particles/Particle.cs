using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace FargowiltasSouls.Particles
{
    // THESE DO NOT CURRENTLY WORK, DO NOT USE.
    public abstract class Particle
    {
        #region Fields/Properties

        public float RotationAmount;

        public float Rotation;

        public float Scale;

        public float MaxScale;

        public float Opacity;

        public Vector2 Position;

        public Vector2 Velocity;

        public Rectangle? Frame;

        public int TimeLeft;

        public int Lifetime;

        public int Width => Texture.Width;

        public int Height => Texture.Height;

        public int Timer => Lifetime - TimeLeft;

        public int LifetimeInterpolant => Timer / Lifetime;

        public virtual Texture2D Texture => ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value;

        public Vector2 Origin
        {
            get
            {
                if (Frame.HasValue)
                    return Frame.Value.Size() * 0.5f;
                return Texture.Size() * 0.5f;
            }
        }

        #endregion

        #region Abstracts/Virtuals
        /// <summary>
        /// This is just the name of the particle in the same namespace normally, but can be overriden.
        /// </summary>
        public virtual string TexturePath => "";

        /// <summary>
        /// Override to use additive drawing in PreDraw().
        /// </summary>
        /// <returns></returns>
        public virtual bool UseAdditiveDrawing => false;

        /// <summary>
        /// Ran every frame.
        /// </summary>
        public virtual void AI()
        {

        }

        /// <summary>
        /// Ran when the particle is killed.
        /// </summary>
        public virtual void OnKill()
        {

        }

        /// <summary>
        /// Override and return false to stop the base drawing from running. Returns true by default.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch, ref Color lightColor)
        {

        }
       
        #endregion
    }
}