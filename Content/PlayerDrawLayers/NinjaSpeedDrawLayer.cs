using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
	public class NinjaSpeedDrawLayer : PlayerDrawLayer
    {
        public override bool IsHeadLayer => false;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            drawInfo.drawPlayer.whoAmI == Main.myPlayer
            && drawInfo.drawPlayer.active
            && !drawInfo.drawPlayer.dead
            && !drawInfo.drawPlayer.ghost
            && drawInfo.shadow == 0
            && drawInfo.drawPlayer.GetModPlayer<FargoSoulsPlayer>().NinjaEnchantItem != null
            && drawInfo.drawPlayer.GetToggleValue("NinjaSpeed")
            && drawInfo.drawPlayer.velocity.Length() < (drawInfo.drawPlayer.GetModPlayer<FargoSoulsPlayer>().ShadowForce ? 8f : 5f);

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayerLoader.Layers[0]);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            Vector2 drawPos = drawPlayer.Center - Main.screenPosition;

            const float scale = 1f;

            Texture2D texture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = new(0, 0, texture.Width, texture.Height);
            Vector2 origin2 = rectangle.Size() / 2f;

            //float opacity = 0.4f + (-0.4f * drawInfo.drawPlayer.velocity.Length() / (drawPlayer.GetModPlayer<FargoSoulsPlayer>().ShadowForce ? 8f : 5f));
            float opacity = 0.4f;
            DrawData data = new(texture, drawPos, rectangle, Color.Black * opacity, 0f, origin2, scale, SpriteEffects.None, 0);
            drawInfo.DrawDataCache.Add(data);
        }
    }
}
