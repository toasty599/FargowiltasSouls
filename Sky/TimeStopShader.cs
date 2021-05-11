using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Souls;

namespace FargowiltasSouls.Sky
{
    public class TimeStopShader : ScreenShaderData
    {
        public TimeStopShader(Ref<Effect> shader, string passName) : base(shader, passName)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            if (Filters.Scene["FargowiltasSouls:Invert"].IsActive()
                && !Main.LocalPlayer.GetModPlayer<FargoPlayer>().FreezeTime && !Main.LocalPlayer.HasBuff(ModContent.BuffType<TimeFrozen>()))
            {
                Filters.Scene.Deactivate("FargowiltasSouls:Invert");
            }
        }

        public override void Apply()
        {
            base.Apply();
        }
    }
}