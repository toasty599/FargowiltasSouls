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

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player == null || !player.active || player.dead || player.ghost || player.whoAmI != Main.myPlayer || drawInfo.shadow != 0)
            {
                return false;
            }
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (modPlayer == null)
            {
                return false;
            }
            if (modPlayer.NinjaEnchantItem == null)
            {
                return false;
            }
            if (!player.GetToggleValue("NinjaSpeed"))
            {
                return false;
            }
            float maxSpeed = modPlayer.ForceEffect(modPlayer.NinjaEnchantItem.type) ? 7 : 4;

            return player.velocity.Length() < maxSpeed;
        }

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
