using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using FargowiltasSouls.Items;
using System.Linq;
namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prime Staff");
            Tooltip.SetDefault("Summons Skeletron to fight for you");
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 1;
        }

        public override void SetDefaults()
        {
            item.damage = 100;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4f;
            item.rare = 11;
            item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Zombie, 20);
            item.shoot = ModContent.ProjectileType<PrimeMinionProj>();
            item.shootSpeed = 10f;
            item.buffType = mod.BuffType("PrimeMinionBuff");
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 25);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(ModContent.BuffType<PrimeMinionBuff>(), 2);
            Vector2 spawnPos = Main.MouseWorld;
            float usedminionslots = 0;
            var minions = Main.projectile.Where(x => x.minionSlots > 0 && x.owner == player.whoAmI && x.active);
            foreach (Projectile minion in minions)
                usedminionslots += minion.minionSlots;
            if (player.ownedProjectileCounts[type] == 0 && usedminionslots != player.maxMinions) //only spawn brain minion itself when the player doesnt have any, and if minion slots aren't maxxed out
            {
                Projectile.NewProjectile(spawnPos, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                Main.NewText("spawned");
            }
            if (Main.rand.Next(1, 3) == 1)
            {
                Projectile.NewProjectile(spawnPos, Main.rand.NextVector2Circular(10, 10), mod.ProjectileType("PrimeMinionVice"), damage, knockBack, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(spawnPos, Main.rand.NextVector2Circular(10, 10), mod.ProjectileType("PrimeMinionSaw"), (int)(damage * 1.2), knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
