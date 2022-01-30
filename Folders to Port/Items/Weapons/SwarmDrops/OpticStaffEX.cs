using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class OpticStaffEX : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omniscience Staff");
            Tooltip.SetDefault("Summons the real twins to fight for you\nNeeds 3 minion slots\n'The reward for slaughtering many...'");
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 4;
        }

        public override void SetDefaults()
        {
            item.damage = 295;
            item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            item.width = 24;
            item.height = 24;
            item.useAnimation = 37;
            item.useTime = 37;
            item.useStyle = ItemUseStyleID.Swing;
            item.noMelee = true;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item82;
            item.value = Item.sellPrice(0, 25);
            item.rare = ItemRarityID.Purple;
            item.buffType = ModContent.BuffType<TwinsEX>();
            item.shoot = ModContent.ProjectileType<OpticRetinazer>();
            item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(item.buffType, 2);
            Vector2 spawnPos = Main.MouseWorld;
            Vector2 speed = new Vector2(speedX, speedY).RotatedBy(Math.PI / 2);
            Projectile.NewProjectile(spawnPos, speed, ModContent.ProjectileType<OpticRetinazer>(), damage, knockBack, player.whoAmI, -1);
            Projectile.NewProjectile(spawnPos, -speed, ModContent.ProjectileType<OpticSpazmatism>(), damage, knockBack, player.whoAmI, -1);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.OpticStaff);
            .AddIngredient(null, "TwinRangs");
            .AddIngredient(null, "AbomEnergy", 10);
            .AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerTwins"));
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            .Register();
        }
    }
}