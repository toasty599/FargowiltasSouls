
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class PearlwoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Pearlwood Enchantment");
            /* Tooltip.SetDefault(
@"Attacks may spawn a homing star when they hit something
'Too little, too late…'"); */
        }

        protected override Color nameColor => new(173, 154, 95);

        
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PearlwoodEffect(player, Item);
        }


        public static void PearlwoodEffect(Player player, Item item)
        {
            player.DisplayToggle("Pearl");
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.PearlwoodEnchantItem = item;
        }
        public static void PearlwoodStar(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.PearlwoodTrail[modPlayer.PearlwoodIndex] != Vector2.Zero && player.GetToggleValue("Pearl")) //check if trail actually exists and if its toggled on
            {
                modPlayer.PStarelinePos = modPlayer.PearlwoodTrail[modPlayer.PearlwoodIndex]; //set stareline position

                if (!modPlayer.PStarelineActive) //check if stareline is active
                {
                    if (modPlayer.PearlwoodGrace == 120) //spawn after 2 seconds 
                    {
                        Projectile.NewProjectile(player.GetSource_Accessory(item), modPlayer.PStarelinePos, Vector2.Zero, ProjectileID.FairyQueenMagicItemShot, 1000, 0f);
                        modPlayer.PStarelineActive = true;
                        modPlayer.PearlwoodGrace = 0;
                    }
                    else
                    {
                        if (player.velocity.Length() > 0)
                            modPlayer.PearlwoodGrace += 1;
                    }
                }
            }


            modPlayer.PearlwoodTrail[modPlayer.PearlwoodIndex] = player.Center; //set position of stareline next cycle

            modPlayer.PearlwoodIndex++; //read next in array
            if (modPlayer.PearlwoodIndex >= modPlayer.PearlwoodTrail.Length) { modPlayer.PearlwoodIndex = 0; } //loop around
        }
        public static void PearlwoodCritReroll(Player player, ref NPC.HitModifiers modifiers, DamageClass damageClass)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.PearlwoodEnchantItem == null)
                return;
            int rerolls = modPlayer.ForceEffect(modPlayer.PearlwoodEnchantItem.type) ? 2 : 1;
            for (int i = 0; i < rerolls; i++)
            {
                if (Main.rand.Next(0, 100) <= player.ActualClassCrit(damageClass))
                {
                    modifiers.SetCrit();
                }
            }
            
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.PearlwoodHelmet)
            .AddIngredient(ItemID.PearlwoodBreastplate)
            .AddIngredient(ItemID.PearlwoodGreaves)
            .AddIngredient(ItemID.PearlwoodSword)
            .AddIngredient(ItemID.LightningBug)
            .AddIngredient(ItemID.Starfruit)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
