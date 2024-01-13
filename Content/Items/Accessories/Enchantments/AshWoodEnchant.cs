using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class AshWoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        protected override Color nameColor => new(139, 116, 100);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }
        public static void PassiveEffect(Player player)
        {
            player.FargoSouls().fireNoDamage = true;
        }
        public override void UpdateInventory(Player player) => PassiveEffect(player);
        public override void UpdateVanity(Player player) => PassiveEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AshWoodEffect>(Item);
            player.AddEffect<AshWoodFireballs>(Item);
        }



        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.AshWoodHelmet)
            .AddIngredient(ItemID.AshWoodBreastplate)
            .AddIngredient(ItemID.AshWoodGreaves)
            .AddIngredient(ItemID.Fireblossom)
            .AddIngredient(ItemID.SpicyPepper)
            .AddIngredient(ItemID.Gel, 50)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class AshWoodEffect : AccessoryEffect
    {
        
        public override Header ToggleHeader => null;
        
        public override void PostUpdateEquips(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            player.buffImmune[ModContent.BuffType<OiledBuff>()] = true;
            player.ashWoodBonus = true;
        }
    }
    public class AshWoodFireballs : AccessoryEffect
    {
        
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.AshwoodCD > 0)
                modPlayer.AshwoodCD--;
        }
        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.AshwoodCD <= 0 && ((player.onFire || player.onFire2 || player.onFire3) || player.HasEffect<ObsidianProcEffect>()))
            {
                modPlayer.AshwoodCD = modPlayer.ForceEffect<AshWoodEnchant>() ? 15 : player.HasEffect<ObsidianProcEffect>() ? 20 : 30;

                Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
                vel = vel.RotatedByRandom(Math.PI / 10);
                int fireballDamage = damage;
                if (!modPlayer.TerrariaSoul)
                    fireballDamage = Math.Min(fireballDamage, FargoSoulsUtil.HighestDamageTypeScaling(player, 60));
                Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, vel, ProjectileID.BallofFire, fireballDamage, 1, Main.myPlayer);
            }
        }
    }
}
