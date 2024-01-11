using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class BorealWoodEnchant : BaseEnchant
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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<BorealEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BorealWoodHelmet)
            .AddIngredient(ItemID.BorealWoodBreastplate)
            .AddIngredient(ItemID.BorealWoodGreaves)
            .AddIngredient(ItemID.Shiverthorn)
            .AddIngredient(ItemID.Plum)
            .AddIngredient(ItemID.Snowball, 300)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class BorealEffect : AccessoryEffect
    {
        public override bool HasToggle => true;
        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();

        public override void PostUpdateEquips(Player player)
        {
            BorealEffectInstance effectFields = player.GetEffectInstance<BorealEffectInstance>();
            if (effectFields.BorealCD > 0)
                effectFields.BorealCD--;
        }
        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            BorealEffectInstance effectFields = player.GetEffectInstance<BorealEffectInstance>();
            if (effectFields.BorealCD <= 0)
            {
                Item item = EffectItem(player);
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                bool forceEffect = modPlayer.ForceEffect(item.type);
                effectFields.BorealCD = forceEffect ? 30 : 60;

                Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
                int snowballDamage = damage / 2;
                if (!modPlayer.TerrariaSoul)
                    snowballDamage = Math.Min(snowballDamage, FargoSoulsUtil.HighestDamageTypeScaling(player, forceEffect ? 300 : 30));
                int p = Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, vel, ProjectileID.SnowBallFriendly, snowballDamage, 1, Main.myPlayer);

                int numSnowballs = forceEffect ? 7 : 3;
                if (p != Main.maxProjectiles)
                    FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, MathHelper.Pi / 10, 1);
            }
        }
    }
    public class BorealEffectInstance : AccessoryEffectInstance
    {
        public int BorealCD;
    }
}
