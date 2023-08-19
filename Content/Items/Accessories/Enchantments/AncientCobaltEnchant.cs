﻿using FargowiltasSouls.Content.Projectiles.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AncientCobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Ancient Cobalt Enchantment");
            /* Tooltip.SetDefault(
@"Grants an explosion jump that inflicts Oiled
'Ancient Kobold'"); */
        }

        protected override Color nameColor => new(53, 76, 116);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AncientCobaltEffect(player, Item, 100);
        }

        public static void AncientCobaltEffect(Player player, Item item, int damage)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.CobaltImmuneTimer > 0)
            {
                player.immune = true;
                modPlayer.CobaltImmuneTimer--;
            }
            if (modPlayer.CobaltCooldownTimer > 0)
            {
                modPlayer.CobaltCooldownTimer--;
            }


            if (player.jump <= 0 && player.velocity.Y == 0f)
            {
                modPlayer.CanCobaltJump = true;
                modPlayer.JustCobaltJumped = false;
            }
            else
            {
                modPlayer.CanCobaltJump = false;
            }

            if (player.controlJump && player.releaseJump && player.GetToggleValue("AncientCobalt") && modPlayer.CanCobaltJump && !modPlayer.JustCobaltJumped && modPlayer.CobaltCooldownTimer <= 0)
            {
                int projType = ModContent.ProjectileType<CobaltExplosion>();

                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, projType, damage, 0, player.whoAmI);

                modPlayer.JustCobaltJumped = true;

                if (modPlayer.CobaltImmuneTimer <= 0)
                    modPlayer.CobaltImmuneTimer = 15;

                if (modPlayer.CobaltCooldownTimer <= 10)
                    modPlayer.CobaltCooldownTimer = 10;
            }

            if (modPlayer.CanCobaltJump || modPlayer.JustCobaltJumped && !player.GetJumpState<CloudInABottleJump>().Active && !player.GetJumpState<BlizzardInABottleJump>().Active && !player.GetJumpState<FartInAJarJump>().Active && !player.GetJumpState<TsunamiInABottleJump>().Active && !player.GetJumpState<SandstormInABottleJump>().Active && !modPlayer.JungleJumping)
            {
                if (modPlayer.CobaltEnchantItem != null)
                {
                    player.jumpSpeedBoost += 10f;
                }
                else
                {
                    player.jumpSpeedBoost += 5f;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.AncientCobaltHelmet)
            .AddIngredient(ItemID.AncientCobaltBreastplate)
            .AddIngredient(ItemID.AncientCobaltLeggings)
            .AddIngredient(ItemID.Bomb, 10)
            .AddIngredient(ItemID.Dynamite, 10)
            .AddIngredient(ItemID.Grenade, 10)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
