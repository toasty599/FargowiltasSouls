using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Graphics.Shaders
{
    public class ShaderRecompilerSystem : ModSystem
    {
		public static Queue<string> CompilingFiles
		{
			get;
			private set;
		}

		public FileSystemWatcher ShaderWatcher
		{
			get;
			private set;
		}

		public FileSystemWatcher FilterWatcher
		{
			get;
			private set;
		}

		public static string EffectsPath
		{
			get;
			private set;
		}

		public static string CompilerPath
		{
			get;
			private set;
		}

		public static bool UseCompiler
		{
			get;
			private set;
		}

		public override void OnModLoad()
		{
			UseCompiler = false;

			if (Main.netMode != NetmodeID.SinglePlayer)
				return;

			// Don't use this system if the mod is not present in the ModSources folder.
			string modSourcesPath = $"{Path.Combine(Program.SavePathShared, "ModSources")}\\{Mod.Name}".Replace("\\..\\tModLoader", string.Empty);
			if (!Directory.Exists(modSourcesPath))
				return;

			// Verify that the Assets/Effects directory exists.
			EffectsPath = $"{modSourcesPath}\\Assets\\Effects";
			if (!Directory.Exists(EffectsPath))
				return;

			// Verify that the compiler directory exists.
			CompilerPath = $"{modSourcesPath}\\Common\\Graphics\\Shaders\\Compiler";
			if (!Directory.Exists(CompilerPath))
				return;

			CompilingFiles = new();

			// Two watchers are used here, to ensure that the recompiler doesnt attempt to recompile shaders in the Armor folder, as those still use the vanilla
			// armor shader system.

			string shaderPath = EffectsPath + "\\Shaders";
			// If the Assets/Effects/Shaders directory exists, watch over it.
			ShaderWatcher = new(shaderPath)
			{
				Filter = "*.fx",
				IncludeSubdirectories = true,
				EnableRaisingEvents = true,
				NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security
			};
			ShaderWatcher.Changed += RecompileShader;

			string filterPath = EffectsPath + "\\Filters";

			// If the Assets/Effects/Filters directory exists, watch over it.
			FilterWatcher = new(filterPath)
			{
				Filter = "*.fx",
				IncludeSubdirectories = true,
				EnableRaisingEvents = true,
				NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security
			};
			FilterWatcher.Changed += RecompileShader;

			// Mark the compiler as valid to use.
			UseCompiler = true;
		}

		public override void PostUpdateEverything()
		{
			// Don't do anything if not valid.
			if (!UseCompiler)
				return;

			bool shaderIsCompiling = false;
			List<string> compiledFiles = new();
			string compilerDirectory = CompilerPath + "\\";
			while (CompilingFiles.TryDequeue(out string shaderPath))
			{
				// Take the contents of the new shader and copy them over to the compiler folder so that the XNB can be regenerated.
				string shaderPathInCompilerDirectory = compilerDirectory + Path.GetFileName(shaderPath);
				File.Delete(shaderPathInCompilerDirectory);
				File.WriteAllText(shaderPathInCompilerDirectory, File.ReadAllText(shaderPath));
				shaderIsCompiling = true;
				compiledFiles.Add(shaderPath);
			}

			if (shaderIsCompiling)
			{
				Process easyXnb = new()
				{
					StartInfo = new()
					{
						FileName = CompilerPath + "\\EasyXnb.exe",
						WorkingDirectory = compilerDirectory,
						UseShellExecute = false,
						CreateNoWindow = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true
					}
				};
				easyXnb.Start();
				if (!easyXnb.WaitForExit(3000))
				{
					Main.NewText("Shader compiler timed out.");
					easyXnb.Kill();
					return;
				}

				easyXnb.Kill();
			}

			for (int i = 0; i < compiledFiles.Count; i++)
			{
				// Copy over the XNB from the compiler, and delete the copy in the Compiler folder.
				string shaderPath = compiledFiles[i];
				string compiledXnbPath = compilerDirectory + Path.GetFileNameWithoutExtension(shaderPath) + ".xnb";
				string originalXnbPath = shaderPath.Replace(".fx", ".xnb");
				File.Delete(originalXnbPath);
				File.Copy(compiledXnbPath, originalXnbPath);
				File.Delete(compiledXnbPath);

				// Finally, load the new XNB's shader data into the game's managed wrappers that reference it.
				string shaderPathInCompilerDirectory = compilerDirectory + Path.GetFileName(shaderPath);
				File.Delete(shaderPathInCompilerDirectory);
				Main.QueueMainThreadAction(() =>
				{
					ContentManager tempManager = new(Main.instance.Content.ServiceProvider, EffectsPath);
					// Get everything after "Effects".
					string toBeSearched = "\\Effects\\";
					string subfolders = originalXnbPath[(originalXnbPath.IndexOf(toBeSearched) + toBeSearched.Length)..];
					string assetName = Path.GetFileNameWithoutExtension(originalXnbPath);
					string assetPath = subfolders.Split(Path.GetFileName(originalXnbPath))[0];
					Effect recompiledEffect = tempManager.Load<Effect>(assetPath + assetName);

					// Replace any shaders that exist already.
					if (ShaderManager.ShaderLookupTable.ContainsKey(compiledXnbPath))
					{
						//ShaderManager.ShaderLookupTable[Path.GetFileNameWithoutExtension(compiledXnbPath)].Dispose();
						ShaderManager.ShaderLookupTable[Path.GetFileNameWithoutExtension(compiledXnbPath)] = new Shader(new Ref<Effect>(recompiledEffect));
					}
					// Replace any filters that exist already.
					else if (ShaderManager.FilterLookupTable.ContainsKey(compiledXnbPath))
					{
						//ShaderManager.FilterLookupTable[Path.GetFileNameWithoutExtension(compiledXnbPath)].Dispose();
						ShaderManager.FilterLookupTable[Path.GetFileNameWithoutExtension(compiledXnbPath)] = new ScreenFilter(new Ref<Effect>(recompiledEffect));
					}
					// If it is not already registered, check whether its a shader or filter and register it with the appropriate dictonary.
					else if (originalXnbPath.Contains("\\Effects\\Shaders\\"))
						ShaderManager.ShaderLookupTable[Path.GetFileNameWithoutExtension(compiledXnbPath)] = new Shader(new Ref<Effect>(recompiledEffect));
					else if (originalXnbPath.Contains("\\Effects\\Filters\\"))
						ShaderManager.FilterLookupTable[Path.GetFileNameWithoutExtension(compiledXnbPath)] = new ScreenFilter(new Ref<Effect>(recompiledEffect));
					else
					{
						Main.NewText($"Shader failed to register, path is:{shaderPath}");
						return;
					}

					Main.NewText($"Shader with the file name '{Path.GetFileName(shaderPath)}' has been successfully recompiled.");
				});
			}
		}

		public override void OnModUnload()
		{
			Main.QueueMainThreadAction(() =>
			{
				ShaderWatcher?.Dispose();
				FilterWatcher?.Dispose();
			});
		}

		private void RecompileShader(object sender, FileSystemEventArgs e)
		{
			if (CompilingFiles.Contains(e.FullPath))
				return;

			CompilingFiles.Enqueue(e.FullPath);
		}
	}
}