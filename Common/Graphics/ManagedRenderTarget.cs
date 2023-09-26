using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasSouls.Common.Graphics
{
    /// <summary>
    /// Wrapper class for <see cref="RenderTarget2D"/> that automatically handles resizing, cleanup, and optionally auto-disposing for performance boosts.
    /// </summary>
    public class ManagedRenderTarget : IDisposable
    {
        private RenderTarget2D target;

        internal bool WaitingForFirstInitialization
        {
            get;
            private set;
        } = true;

        internal RenderTargetCreationCondition CreationCondition
        {
            get;
            private set;
        }

        public bool IsUninitialized => target is null || target.IsDisposed;

        public bool IsDisposed
        {
            get;
            private set;
        }

        public bool ShouldResetUponScreenResize
        {
            get;
            private set;
        }

        public RenderTarget2D Target
        {
            get
            {
                if (IsUninitialized)
                {
                    target = CreationCondition(Main.screenWidth, Main.screenHeight);
                    WaitingForFirstInitialization = false;
                    IsDisposed = false;
                }

                TimeSinceLastAccessed = 0;
                return target;
            }
            private set => target = value;
        }

        public int Width => Target.Width;

        public int Height => Target.Height;

        public readonly bool ShouldAutoDispose;

        /// <summary>
        /// How many frames since this target has been gotten. Used to dispose of unused targets for the sake of performance.
        /// </summary>
        public int TimeSinceLastAccessed;

        public delegate RenderTarget2D RenderTargetCreationCondition(int screenWidth, int screenHeight);

        public ManagedRenderTarget(bool shouldResetUponScreenResize, RenderTargetCreationCondition creationCondition, bool shouldAutoDispose = true)
        {
            ShouldResetUponScreenResize = shouldResetUponScreenResize;
            CreationCondition = creationCondition;
            ShouldAutoDispose = shouldAutoDispose;
            RenderTargetManager.ManagedTargets.Add(this);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            target?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Recreate(int screenWidth, int screenHeight)
        {
            Dispose();
            IsDisposed = false;

            target = CreationCondition(screenWidth, screenHeight);
            TimeSinceLastAccessed = 0;
        }

        public static implicit operator RenderTarget2D(ManagedRenderTarget target) => target.Target;

        public static bool operator ==(ManagedRenderTarget value1, ManagedRenderTarget value2) => value1.target == value2.target;

        public static bool operator !=(ManagedRenderTarget value1, ManagedRenderTarget value2) => value1.target != value2.target;

        public override bool Equals(object obj) => this == (ManagedRenderTarget)obj;

		public override int GetHashCode() => Width * Height;
	}
}
