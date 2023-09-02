using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class DecrepitAirstrikeRemote : SoulsItem
    {

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Summon;
            Item.width = 82;
            Item.height = 24;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 15;
            Item.value = Item.sellPrice(0, 5);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item66;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DecrepitAirstrike>();
            Item.shootSpeed = 1f;
            Item.sentry = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-0, -0);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            int CurrentSentries = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.sentry)
                {
                    CurrentSentries++;
                }
            }
            if (CurrentSentries >= player.maxTurrets)
            {
                return false;
            }
            for (int i = 0; i < player.maxTurrets - CurrentSentries; i++)
            {
                Projectile.NewProjectile(source, mouse.X, mouse.Y, 0f, 0f, type, damage, knockback, player.whoAmI, i == 0 ? 0 : 1, ai2: player.maxTurrets - CurrentSentries); //ai0 sets the first spawned turret as the original
            }
            Projectile.NewProjectile(source, mouse.X, mouse.Y, 0f, 0f, ModContent.ProjectileType<GlowRingHollow>(), damage, knockback, player.whoAmI, 14, 60 * 4 + 30); 
            player.UpdateMaxTurrets();
            return false;
        }
        public override bool? UseItem(Player player)
        {
            
            return base.UseItem(player);
        }
    }
}