using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SummonVariants
{
    public abstract class BaseSummonItem : SoulsItem
    {
        public abstract int Type
        {
            get;
        }

        public override string Texture => $"Terraria/Images/Item_{Type}";

        public override void SetDefaults()
        {
            item.CloneDefaults(Type);
            item.melee = false;
            item.ranged = false;
            item.magic = false;
            Item.DamageType = DamageClass.Summon;

            //item.mana = 0;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int num = Item.NewItem(player.getRect(), Type, prefixGiven: item.prefix);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, number: num, number2: 1f);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].minion = true;
            Main.projectile[proj].melee = false;
            Main.projectile[proj].ranged = false;
            Main.projectile[proj].magic = false;
            return false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "help", "Right click to convert");
            tooltips.Add(line);
        }
    }
}