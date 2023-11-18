using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Buffs.Souls;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class ObsidianEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        protected override Color nameColor => new(69, 62, 115);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateInventory(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ObsidianEffect(player, Item);
            
        }

        public static void ObsidianEffect(Player player, Item item)
        {
            player.DisplayToggle("Obsidian");
            AshWoodEnchant.AshwoodEffect(player, item);
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            player.lavaImmune = true;
            player.fireWalk = true;
            //player.buffImmune[BuffID.OnFire] = true;

            //in lava effects
            if (player.lavaWet)
            {
                player.gravity = Player.defaultGravity;
                player.ignoreWater = true;
                player.accFlipper = true;

                if (player.GetToggleValue("Obsidian"))
                {
                    player.AddBuff(ModContent.BuffType<ObsidianLavaWetBuff>(), 600);
                }
            }

            if (modPlayer.ObsidianCD > 0)
                modPlayer.ObsidianCD--;

            if (modPlayer.ForceEffect(ModContent.ItemType<ObsidianEnchant>()) || player.lavaWet || modPlayer.LavaWet)
            {
                modPlayer.ObsidianEnchantItem = item;
            }
        }

        public static void ObsidianProc(FargoSoulsPlayer modPlayer, NPC target, int damage)
        {
            damage = Math.Min(damage, FargoSoulsUtil.HighestDamageTypeScaling(modPlayer.Player, 300));

            Player player = modPlayer.Player;
            Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.ObsidianEnchantItem), target.Center, Vector2.Zero, ModContent.ProjectileType<ObsidianExplosion>(), damage, 0, player.whoAmI);

            if (modPlayer.ForceEffect(modPlayer.ObsidianEnchantItem.type))
            {
                modPlayer.ObsidianCD = 20;
            }
            else
            {
                modPlayer.ObsidianCD = 40;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ObsidianHelm)
            .AddIngredient(ItemID.ObsidianShirt)
            .AddIngredient(ItemID.ObsidianPants)
            .AddIngredient(ItemID.MoltenSkullRose) //molten skull rose
            //.AddIngredient(ItemID.Cascade)
            .AddIngredient(null, "AshWoodEnchant")

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
