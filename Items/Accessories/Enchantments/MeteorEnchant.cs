using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MeteorEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Meteor Enchantment");

            string tooltip =
@"A meteor shower initiates every few seconds while attacking
'Drop a draco on 'em'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(95, 71, 82);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            MeteorEffect(modPlayer);
        }

        public static void MeteorEffect(FargoSoulsPlayer modPlayer)
        {
            Player player = modPlayer.Player;

            modPlayer.MeteorEnchantActive = true;

            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("Meteor"))
            {
                int damage = 50;

                if (modPlayer.MeteorShower)
                {
                    if (modPlayer.MeteorTimer % 2 == 0)
                    {
                        int p = Projectile.NewProjectile(player.GetSource_Misc(""), player.Center.X + Main.rand.Next(-1000, 1000), player.Center.Y - 1000, Main.rand.Next(-2, 2), 0f + Main.rand.Next(8, 12), Main.rand.Next(424, 427), FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 0f, player.whoAmI, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                            Main.projectile[p].netUpdate = true;
                            if (ModLoader.GetMod("Fargowiltas") != null)
                                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", Main.projectile[p]);
                        }
                    }

                    modPlayer.MeteorTimer--;

                    if (modPlayer.MeteorTimer <= 0)
                    {
                        modPlayer.MeteorCD = 300;

                        if (modPlayer.CosmoForce)
                        {
                            modPlayer.MeteorCD = 200;
                        }

                        modPlayer.MeteorTimer = 150;
                        modPlayer.MeteorShower = false;
                    }
                }
                else
                {
                    if (player.controlUseItem)
                    {
                        modPlayer.MeteorCD--;

                        if (modPlayer.MeteorCD == 0)
                        {
                            modPlayer.MeteorShower = true;
                        }
                    }
                    else
                    {
                        modPlayer.MeteorCD = 300;
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MeteorHelmet)
            .AddIngredient(ItemID.MeteorSuit)
            .AddIngredient(ItemID.MeteorLeggings)
            .AddIngredient(ItemID.SpaceGun)
            .AddIngredient(ItemID.StarCannon)
            .AddIngredient(ItemID.PlaceAbovetheClouds)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
