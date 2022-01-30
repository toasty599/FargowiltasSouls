using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;
using FargowiltasSouls.Items.Weapons.SwarmDrops;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Weapons.FinalUpgrades
{
    public class PhantasmalLeashOfCthulhu : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Leash of Cthulhu");
            Tooltip.SetDefault("'The True Eye's soul trapped for eternity..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "幻影克苏鲁连枷");
        }

        public override void SetDefaults()
        {
            item.damage = 2800; //eoc base life funny
            item.width = 30;
            item.height = 10;
            item.value = Item.sellPrice(1);
            item.rare = ItemRarityID.Purple;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.autoReuse = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.knockBack = 6f;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<PhantasmalFlail>();
            item.shootSpeed = 45f;
            item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, Main.DiscoG, 255);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<MechanicalLeashOfCthulhu>(), 1)
            .AddIngredient(ModContent.ItemType<Sadism>(), 15)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            
            .Register();
        }
    }
}