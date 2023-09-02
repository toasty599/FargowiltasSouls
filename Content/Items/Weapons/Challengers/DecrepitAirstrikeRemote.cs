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
            Item.damage = 200;
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
            Item.shoot = ProjectileID.PurificationPowder; 
            Item.shootSpeed = 1f;

            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-0, -0);
        }

        public override bool CanUseItem(Player player)
        {
            
            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override bool? UseItem(Player player)
        {
            
            return base.UseItem(player);
        }
    }
}