//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria;
//using Terraria.ID;

//namespace FargowiltasSouls.Patreon.Northstrider
//{
//    public class _1001Nights : PatreonModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            base.SetStaticDefaults();
//            DisplayName.SetDefault("1001 Nights");
//            Tooltip.SetDefault(
//@"
//''");
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 20;
//            Item.height = 20;
//            Item.accessory = true;
//            Item.rare = 1;
//            Item.value = 100;
//        }

//        public override void UpdateAccessory(Player player, bool hideVisual)
//        {
//            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();

//        }

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//                .AddIngredient(ItemID.NinjaShirt)
//                .AddIngredient(ItemID.Dynamite, 50)

//                .AddTile(TileID.WorkBenches)

//                .Register();
//        }
//    }
//}
