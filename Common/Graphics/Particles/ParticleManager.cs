using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class ParticleManager : ModSystem
	{
		public override void Load()
		{
			On_Main.DrawProjectiles += DrawParticles_Projectiles;
			On_Main.DrawDust += DrawParticles_Dust;
			On_Main.DrawInfernoRings += DrawParticles_AfterEverything;
			Particle.ActiveParticles = new();

		}

		public override void Unload()
		{
			On_Main.DrawProjectiles -= DrawParticles_Projectiles;
			On_Main.DrawDust -= DrawParticles_Dust;
			On_Main.DrawInfernoRings -= DrawParticles_AfterEverything;
			Particle.ActiveParticles.Clear();
			Particle.ActiveParticles = null;
		}

		private void DrawParticles_Projectiles(On_Main.orig_DrawProjectiles orig, Main self)
		{
			Particle.DrawParticles(Main.spriteBatch, ParticleLayers.BeforeProjectiles);
			orig(self);
			Particle.DrawParticles(Main.spriteBatch, ParticleLayers.AfterProjectiles);
		}

		private void DrawParticles_Dust(On_Main.orig_DrawDust orig, Main self)
		{
			orig(self);
			Particle.DrawParticles(Main.spriteBatch, ParticleLayers.Dust);
		}

		private void DrawParticles_AfterEverything(On_Main.orig_DrawInfernoRings orig, Main self)
		{
			orig(self);
			Particle.DrawParticles(Main.spriteBatch, ParticleLayers.AfterEverything);
		}

		public override void PostUpdateDusts() => Particle.UpdateParticles();

		public override void ClearWorld() => Particle.ActiveParticles?.Clear();
	}
}
