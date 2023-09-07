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

        public static RenderTarget2D MainTarget
        {
            get;
            private set;
        }

        public static RenderTarget2D AuxilaryTarget
        {
            get;
            private set;
        }

        public override void Load()
        {
			On_FilterManager.EndCapture += ApplyScreenFilters;
			Main.OnResolutionChanged += ResizeTargets;

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
                    ShaderLookupTable.Add(name, new Shader(effect));
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
					FilterLookupTable.Add(name, new ScreenFilter(effect));
				else
					Mod.Logger.Warn($"ShaderManager loading error: A filer with name {name} has already been registered!");
			}
		}

		public override void Unload()
        {
            Main.QueueMainThreadAction(() =>
            {
				if (Main.netMode is NetmodeID.Server)
					return;
				
                foreach (var keyValuePair in ShaderLookupTable)
                    keyValuePair.Value.Dispose();

                ShaderLookupTable = null;

                foreach (var keyValuePair in FilterLookupTable)
                    keyValuePair.Value.Dispose();

                FilterLookupTable = null;
            });
        }

		private void ResizeTargets(Vector2 _)
		{
			MainTarget?.Dispose();
            MainTarget = new(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            AuxilaryTarget?.Dispose();
            AuxilaryTarget = new(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);
		}

		private void ApplyScreenFilters(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
		{
            if (MainTarget == null || AuxilaryTarget == null)
                ResizeTargets(default);

            RenderTarget2D target1 = null;
            RenderTarget2D target2 = screenTarget1;

			if (Main.player[Main.myPlayer].gravDir == -1f)
			{
				target1 = screenTarget2;
				Main.instance.GraphicsDevice.SetRenderTarget(target1);
				Main.instance.GraphicsDevice.Clear(clearColor);
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.EffectMatrix));
				Main.spriteBatch.Draw(target2, Vector2.Zero, Color.White);
				Main.spriteBatch.End();
				target2 = screenTarget2;
			}

			foreach (var filter in FilterLookupTable.Values.Where(filter => filter.Opacity > 0))
            {
				target1 = ((target2 != screenTarget1) ? screenTarget1 : screenTarget2);
				Main.instance.GraphicsDevice.SetRenderTarget(target1);
				Main.instance.GraphicsDevice.Clear(clearColor);
				Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.AlphaBlend);
				filter.Apply();
				Main.spriteBatch.Draw(target2, Vector2.Zero, Main.ColorOfTheSkies);
				Main.spriteBatch.End();
				target2 = (target2 != screenTarget1) ? screenTarget1 : screenTarget2;
			}


			// Apply vanilla ones.
            orig(self, finalTexture, target1 ?? screenTarget1, target2 ?? screenTarget2, clearColor);
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
