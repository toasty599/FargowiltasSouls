using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class RustrifleReload : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<FargoSoulsPlayer>().RustRifleReloading && drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<NavalRustrifle>();
        public override Position GetDefaultPosition() => new Between();
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                Player player = drawInfo.drawPlayer;
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

                Vector2 barPos = (player.gravDir > 0 ? player.Bottom : player.Top) - Main.screenPosition;

                Texture2D barTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/RustrifleReloadBar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle barRectangle = barTexture.Bounds;

                
                barPos.Y += 40 * player.gravDir;

                int barWidth = barRectangle.Width - 16;
                Vector2 barPos0 = barPos - Vector2.UnitX * barWidth / 2;
                

                DrawData bar = new(barTexture, barPos, barRectangle, Color.White, player.gravDir < 0 ? MathHelper.Pi : 0f, barRectangle.Size() / 2, 1f, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(bar);

                Texture2D zoneTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/RustrifleReloadZone", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle zoneRectangle = zoneTexture.Bounds;


                Vector2 zonePos = barPos0 + Vector2.UnitX * barWidth * modPlayer.RustRifleReloadZonePos;
                DrawData zone = new(zoneTexture, zonePos, zoneRectangle, Color.White, player.gravDir < 0 ? MathHelper.Pi : 0f, zoneRectangle.Size() / 2, 1f, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(zone);

                Texture2D sliderTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/RustrifleReloadSlider", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle sliderRectangle = sliderTexture.Bounds;


                Vector2 sliderPos = barPos0 + Vector2.UnitX * barWidth * modPlayer.RustRifleReloadProgress;
                DrawData slider = new(sliderTexture, sliderPos, sliderRectangle, Color.White, player.gravDir < 0 ? MathHelper.Pi : 0f, sliderRectangle.Size() / 2, 1f, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(slider);
            }
        }
    }
}