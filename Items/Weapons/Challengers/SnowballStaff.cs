using FargowiltasSouls.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.Challengers
{
    public class SnowballStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Creates a snowball that grows as you roll it\nMust use continuously to sustain snowball\nRight click to recall the snowball to yourself");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 0, 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RollingSnowball>();
            Item.shootSpeed = 6f;

            Item.noMelee = true;
            Item.channel = true;
            Item.mana = 5;
        }

        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}