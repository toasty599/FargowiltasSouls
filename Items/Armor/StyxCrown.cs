using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class StyxCrown : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Styx Crown");
            Tooltip.SetDefault(@"10% increased damage
10% increased critical strike chance
Increases max number of minions and sentries by 3");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 20);
            item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(10);

            player.maxMinions += 3;
            player.maxTurrets += 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StyxChestplate>() && legs.type == ModContent.ItemType<StyxLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlinesForbidden = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = @"20% increased damage
Attack enemies to charge energy
Reduces damage taken at the cost of some energy
Double tap up to release energy as homing shots
Brandish a blade of infernal magic when fully charged";

            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.2f);

            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.StyxSet = true;

            int scytheType = ModContent.ProjectileType<Projectiles.StyxArmorScythe>();

            const int maxProjs = 12;
            const int threshold = 1500000 / maxProjs; //based off mutant hp
            if (fargoPlayer.StyxMeter > threshold)
            {
                fargoPlayer.StyxMeter -= threshold;
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[scytheType] < maxProjs)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, scytheType, 0, 10f, player.whoAmI, player.ownedProjectileCounts[scytheType], -1f);
                }
            }

            if (player.whoAmI == Main.myPlayer && player.controlUp && player.releaseUp
                && player.doubleTapCardinalTimer[1] > 0 && player.doubleTapCardinalTimer[1] != 15)
            {
                bool superAttack = player.ownedProjectileCounts[scytheType] >= maxProjs;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == scytheType && Main.projectile[i].owner == player.whoAmI)
                    {
                        if (!superAttack)
                        {
                            Projectile.NewProjectile(Main.projectile[i].Center, Vector2.Normalize(Main.projectile[i].velocity) * 24f, ModContent.ProjectileType<Projectiles.StyxArmorScythe2>(),
                                Main.projectile[i].damage, Main.projectile[i].knockBack, player.whoAmI, -1, -1);
                        }

                        Main.projectile[i].Kill();
                    }
                }

                if (superAttack)
                {
                    Vector2 speed = Vector2.Normalize(Main.MouseWorld - player.Center);
                    bool flip = speed.X < 0;
                    speed = speed.RotatedBy(MathHelper.PiOver2 * (flip ? 1 : -1 ));
                    Projectile.NewProjectile(player.Center, speed, ModContent.ProjectileType<Projectiles.StyxGazerArmor>(), 0, 14f, player.whoAmI, MathHelper.Pi / 120 * (flip ? -1 : 1));
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SoulofSight, 15);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<Misc.MutantScale>(), 10);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}