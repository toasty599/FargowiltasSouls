using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class BigBrainBuster : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Brain Buster");
            Tooltip.SetDefault(
"Repeated summons increase the size and damage of the minion\n" +
$"This caps at {Projectiles.Minions.BigBrainProj.MaxMinionSlots} slots\n" +
"'The reward for slaughtering many...'");
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 1;
        }

        public override void SetDefaults()
        {
            item.damage = 222;
            Item.DamageType = DamageClass.Summon;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.Swing;
            item.noMelee = true;
            item.knockBack = 3;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<Projectiles.Minions.BigBrainProj>();
            item.shootSpeed = 10f;
            //item.buffType = ModContent.BuffType<BigBrainMinion>();
            //item.buffTime = 3600;
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 10);
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<BigBrainMinion>(), 2);
            Vector2 spawnPos = player.Center - Main.MouseWorld;
            if (player.ownedProjectileCounts[type] == 0)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0, spawnPos.ToRotation());
            }
            else
            {
                float usedslots = 0;
                int brain = -1;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.owner == player.whoAmI && proj.minionSlots > 0 && proj.active)
                    {
                        usedslots += proj.minionSlots;
                        if (proj.type == type)
                        {
                            brain = i;
                            if (usedslots < player.maxMinions)
                                proj.minionSlots++;
                        }
                    }
                }

                if (player.GetModPlayer<FargoSoulsPlayer>().TikiMinion && usedslots > player.GetModPlayer<FargoSoulsPlayer>().actualMinions && FargoSoulsUtil.ProjectileExists(brain, type) != null)
                {
                    Main.projectile[brain].GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().tikiMinion = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "BrainStaff");
            .AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerBrain"));
            .AddIngredient(ItemID.LunarBar, 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            .Register();
        }
    }
}