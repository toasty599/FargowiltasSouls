using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class NavalRustrifle : SoulsItem
    {

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 82;
            Item.height = 24;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 15;
            Item.value = Item.sellPrice(0, 5);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder; //guns just have this, don't ask
            Item.shootSpeed = 30f;

            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-0, -0);
        }
        const int ShotType = ProjectileID.BulletHighVelocity;
        bool EmpoweredShot = false;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (EmpoweredShot)
            {
                type = ShotType;
                damage *= 2;
            }
        }
        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (EmpoweredShot)
            {
                crit = 100;
            }
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            if (EmpoweredShot)
            {
                knockback *= 3f;
            }
        }

        int Timer = 0;
        public override void HoldItem(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (modPlayer.RustRifleReloading)
            {
                modPlayer.RustRifleReloadProgress = (1 + (float)Math.Sin(MathHelper.Pi * (Timer-30) / 60f)) / 2;
                Timer++;
            }
        }
        public override bool CanUseItem(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (modPlayer.RustRifleReloading)
            {
                if (Math.Abs(modPlayer.RustRifleReloadProgress - modPlayer.RustRifleReloadZonePos) < 0.11f)
                {
                    EmpoweredShot = true;
                    SoundEngine.PlaySound(SoundID.Unlock with { Pitch = 0.5f }, player.Center);
                    Item.UseSound = SoundID.Item68;
                }
                else
                {
                    EmpoweredShot = false;
                    SoundEngine.PlaySound(SoundID.Unlock with { Pitch = -0.5f }, player.Center);
                    Item.UseSound = SoundID.Item40;
                }
                modPlayer.RustRifleReloading = false;
                modPlayer.RustRifleReloadProgress = 0;
                modPlayer.RustRifleReloadZonePos = 0;
                player.reuseDelay = 20;
                Timer = 0;
                return false;
            }
            return base.CanUseItem(player);
        }
        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().RustRifleReloading = true;
            player.GetModPlayer<FargoSoulsPlayer>().RustRifleReloadZonePos = Main.rand.NextFloat(0.4f, 0.8f);
            return base.UseItem(player);
        }
    }
}