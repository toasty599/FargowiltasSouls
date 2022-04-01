using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrystalAssassinEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Crystal Assassin Enchantment");
            Tooltip.SetDefault(@"Allows the ability to dash
Use Ninja hotkey to throw a smoke bomb, use it again to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
When you teleport, you also spawn several homing blades
Effects of Volatile Gel''");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "水晶刺客魔石");
            string tooltip_ch =
@"拥有挥发明胶效果
''";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(231, 178, 28); //change e
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().ForbiddenEffect(); //effect tele on party girl bathwater, when tele slashes through enemies
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalNinjaHelmet)
                .AddIngredient(ItemID.CrystalNinjaChestplate)
                .AddIngredient(ItemID.CrystalNinjaLeggings) 
                .AddIngredient(ModContent.ItemType<NinjaEnchant>())
                .AddIngredient(ItemID.VolatileGelatin)
                .AddIngredient(ItemID.FlyingKnife)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
