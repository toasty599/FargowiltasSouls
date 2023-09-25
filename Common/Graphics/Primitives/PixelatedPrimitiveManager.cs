using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Graphics.Primitives
{
    public class PixelatedPrimitiveManager : ModSystem
    {
		#region Fields And Properities
		public static ManagedRenderTarget PixelRenderTargetAfterProjectiles
        {
            get;
            private set;
        }

		public static ManagedRenderTarget PixelRenderTargetBeforeNPCs
        {
            get;
            private set;
        }
        #endregion

        #region Overrides
        public override void Load()
        {
			PixelRenderTargetAfterProjectiles = new(true, PixelCreationContext, true);
			PixelRenderTargetBeforeNPCs = new(true, PixelCreationContext, true);

			On_Main.CheckMonoliths += DrawToCustomRenderTargets;
			On_Main.DoDraw_DrawNPCsOverTiles += DrawPixelRenderTargetNPCs;
			On_Main.DrawDust += DrawPixelRenderTargetProjectiles;
        }

        public override void Unload()
        {
			On_Main.CheckMonoliths -= DrawToCustomRenderTargets;
			On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRenderTargetNPCs;
			On_Main.DrawDust -= DrawPixelRenderTargetProjectiles;
        }
        #endregion

        #region Methods
        private static RenderTarget2D PixelCreationContext(int width, int height) => new(Main.instance.GraphicsDevice, width / 2, height / 2);

        private void DrawPixelRenderTargetNPCs(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            // Draw our RT. The scale is important, it is 2 here as this RT is 0.5x the main screen size.
            Main.spriteBatch.Draw(PixelRenderTargetBeforeNPCs, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }

		private void DrawPixelRenderTargetProjectiles(On_Main.orig_DrawDust orig, Main self)
		{
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			// Draw our RT. The scale is important, it is 2 here as this RT is 0.5x the main screen size.
			Main.spriteBatch.Draw(PixelRenderTargetAfterProjectiles, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
			orig(self);
		}

		private void DrawToCustomRenderTargets(On_Main.orig_CheckMonoliths orig)
        {
            // Clear our render target from the previous frame.
            List<IPixelPrimitiveDrawer> drawersBeforeNPCs = new();
			List<IPixelPrimitiveDrawer> drawersAfterProjectiles = new();

			// Check every active projectile.
			for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];
                // If the projectile is active, a mod projectile, and uses our interface,
                if (projectile.active && projectile.ModProjectile != null && projectile.ModProjectile is IPixelPrimitiveDrawer pixelPrimitiveProjectile)
                {
                    if (pixelPrimitiveProjectile.RenderOverProjectiles)
                        drawersAfterProjectiles.Add(pixelPrimitiveProjectile);// Add it to the list of prims to draw this frame.
                    else
						drawersBeforeNPCs.Add(pixelPrimitiveProjectile);
                }
            }

            // Draw the prims. The render target gets set here.
            DrawPrimsToRenderTarget(PixelRenderTargetBeforeNPCs, drawersBeforeNPCs);
			DrawPrimsToRenderTarget(PixelRenderTargetAfterProjectiles, drawersAfterProjectiles);

			// Clear the current render target.
			Main.graphics.GraphicsDevice.SetRenderTarget(null);

            // Call orig.
            orig();
        }

        private static void DrawPrimsToRenderTarget(RenderTarget2D renderTarget, List<IPixelPrimitiveDrawer> pixelPrimitives)
        {
			// Swap to our custom render target.
			renderTarget.SwapTo();

            // If the list has any entries.
			if (pixelPrimitives.Any())
            {
				// Start a spritebatch, as one does not exist in the method we're detouring.
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

                // Loop through the list and call each draw function.
                foreach (IPixelPrimitiveDrawer pixelPrimitiveDrawer in pixelPrimitives)
                    pixelPrimitiveDrawer.DrawPixelPrimitives(Main.spriteBatch);

                // End the spritebatch we started.
                Main.spriteBatch.End();
            }
        }
        #endregion
    }
}
