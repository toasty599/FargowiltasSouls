using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Graphics.Shaders
{
    public class ShaderManager : ModSystem
    {
        /// <summary>
        /// The main pass name for every shader in the files. Optional ones can be used, but will need to be specified in <see cref="Shader.Apply(bool, string)"/>.
        /// </summary>
        public const string AutoloadPassName = "AutoloadPass";

        internal static Dictionary<string, Shader> ShaderLookupTable
        {
            get;
            set;
        } = null;

        internal static Dictionary<string, ScreenFilter> FilterLookupTable
        {
            get;
            set;
        } = null;

        public static ManagedRenderTarget MainTarget
        {
            get;
            private set;
        }

        public static ManagedRenderTarget AuxilaryTarget
        {
            get;
            private set;
        }

        public static bool HasLoaded
        {
            get;
            private set;
        }

        public override void Load()
        {
            HasLoaded = false;
            MainTarget = new ManagedRenderTarget(true, RenderTargetManager.CreateScreenSizedTarget, true);
			AuxilaryTarget = new ManagedRenderTarget(true, RenderTargetManager.CreateScreenSizedTarget, true);

			On_FilterManager.EndCapture += ApplyScreenFilters;

            if (Main.netMode is NetmodeID.Server)
                return;

            ShaderLookupTable = new();
            FilterLookupTable = new();

            // Loop through every file in the "Assets/Effects/Shaders" folder.
            foreach (string path in Mod.GetFileNames().Where(p => p.Contains("Assets/Effects/Shaders")))
            {
                string name = Path.GetFileNameWithoutExtension(path);
                string formattedPath = Path.Combine(Path.GetDirectoryName(path), name).Replace(@"\", @"/");

                Effect effect = Mod.Assets.Request<Effect>(formattedPath, AssetRequestMode.ImmediateLoad).Value;

                if (!ShaderLookupTable.ContainsKey(name))
                    ShaderLookupTable.Add(name, new Shader(new(effect)));
                else
                    Mod.Logger.Warn($"ShaderManager loading error: A shader with name {name} has already been registered!");
            }

			// Loop through every file in the "Assets/Effects/Filters" folder.
			foreach (string path in Mod.GetFileNames().Where(p => p.Contains("Assets/Effects/Filters")))
			{
				string name = Path.GetFileNameWithoutExtension(path);
				string formattedPath = Path.Combine(Path.GetDirectoryName(path), name).Replace(@"\", @"/");

				Effect effect = Mod.Assets.Request<Effect>(formattedPath, AssetRequestMode.ImmediateLoad).Value;

				if (!FilterLookupTable.ContainsKey(name))
					FilterLookupTable.Add(name, new ScreenFilter(new Ref<Effect>(effect)));
				else
					Mod.Logger.Warn($"ShaderManager loading error: A filer with name {name} has already been registered!");
			}

            HasLoaded = true;
		}

		public override void Unload()
        {
            On_FilterManager.EndCapture -= ApplyScreenFilters;

            Main.QueueMainThreadAction(() =>
            {
				if (Main.netMode is NetmodeID.Server)
					return;

                ShaderLookupTable = null;


                FilterLookupTable = null;
            });
        }

		private void ApplyScreenFilters(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
		{
            RenderTarget2D target1 = null;
            RenderTarget2D target2 = screenTarget1;

			if (Main.player[Main.myPlayer].gravDir == -1f)
			{
				target1 = AuxilaryTarget;
				Main.instance.GraphicsDevice.SetRenderTarget(target1);
				Main.instance.GraphicsDevice.Clear(clearColor);
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.EffectMatrix));
				Main.spriteBatch.Draw(target2, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
				Main.spriteBatch.End();
				target2 = AuxilaryTarget;
			}

			foreach (var filter in FilterLookupTable.Values.Where(filter => filter.Opacity > 0))
            {
				target1 = (target2 != MainTarget.Target) ? MainTarget : AuxilaryTarget;
				Main.instance.GraphicsDevice.SetRenderTarget(target1);
				Main.instance.GraphicsDevice.Clear(clearColor);
				Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.AlphaBlend);
				filter.Apply();
				Main.spriteBatch.Draw(target2, Vector2.Zero, Main.ColorOfTheSkies);
				Main.spriteBatch.End();
				target2 = (target2 != MainTarget.Target) ? MainTarget : AuxilaryTarget;
			}

            if (target1 != null)
            {
                Main.instance.GraphicsDevice.SetRenderTarget(screenTarget1);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(target1, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
            }

			// Apply vanilla ones.
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
		}

		public override void PostUpdateEverything()
		{
            if (Main.netMode == NetmodeID.Server)
                return;

            // Update the filters, and mark them as inactive for next frame.
            foreach (var filter in FilterLookupTable.Values)
            {
                filter.Update();
                filter.Deactivate();
            }
		}

		public static Shader GetShaderIfExists(string shaderFileName)
        {
			if (Main.netMode is NetmodeID.Server)
				return null;
			
            if (ShaderLookupTable.TryGetValue(shaderFileName, out var shader))
                return shader;

            return null;
        }

        public static ScreenFilter GetFilterIfExists(string filterFileName)
        {
			if (Main.netMode is NetmodeID.Server)
				return null;

			if (FilterLookupTable.TryGetValue(filterFileName, out var filter))
				return filter;

			return null;
		}
    }
}
