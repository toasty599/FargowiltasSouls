using Microsoft.Xna.Framework;
using System.Diagnostics.Contracts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace FargowiltasSouls.Content.Items.Accessories.Expert
{
    public class RustedOxygenTank : SoulsItem
    {
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 4);

            Item.expert = true;
        }

        public static void PassiveEffect(Player player)
        {
            if (Collision.WetCollision(player.position - 20 * Vector2.UnitX - 20 * Vector2.UnitY, player.width + 10, player.height + 10)) //need some extra otherwise you get stuck near water
            {
                player.moveSpeed += 0.2f;
                return;
            }
            //ripped from "effects while in water" vanilla code
            if (player.merman)
            {
                player.gravity = 0.3f;
                player.maxFallSpeed = 7f;
            }
            else if (player.trident)
            {
                player.gravity = 0.25f;
                player.maxFallSpeed = 6f;
                Player.jumpHeight = 25;
                Player.jumpSpeed = 5.51f;
                if (player.controlUp)
                {
                    player.gravity = 0.1f;
                    player.maxFallSpeed = 2f;
                }
            }
            else
            {
                player.gravity = 0.2f;
                player.maxFallSpeed = 5f;
                Player.jumpHeight = 30;
                Player.jumpSpeed = 6.01f;
            }
            
            if (!player.wet)
            {
                player.wet = true;
                player.wetCount = 10;
            }
            //player.wingTime = player.wingTimeMax;

            if (player.controlJump && player.accFlipper) //simulate flipper jump
            {
                player.swimTime = 30;
                player.velocity.Y = (0f - Player.jumpSpeed) * player.gravDir;
                player.jump = Player.jumpHeight;
            }
            player.position -= player.velocity * 0.5f; //water speed is half of normal speed


        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<FargoSoulsPlayer>().OxygenTank = true;
        public override void UpdateVanity(Player player) => player.GetModPlayer<FargoSoulsPlayer>().OxygenTank = true;
        public override void UpdateInventory(Player player)
        {
            if (Item.favorited)
            {
                player.GetModPlayer<FargoSoulsPlayer>().OxygenTank = true;
            }
        }
    }
}