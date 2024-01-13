using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Expert
{
    public class PrimeSoul : SoulsItem
    {
        //yet to be added
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 1);

            Item.expert = true;
        }

        void PrimeSoulEffect(Player player)
        {
            if (Item.favorited)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                modPlayer.PrimeSoulItem = Item;
            }
        }

        public override void UpdateInventory(Player player) => PrimeSoulEffect(player);
        public override void UpdateVanity(Player player) => PrimeSoulEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual) => PrimeSoulEffect(player);

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<BoxofGizmos>())
            .AddIngredient(ModContent.ItemType<RustedOxygenTank>())
            .AddIngredient(ModContent.ItemType<LifeRevitalizer>())
            .AddIngredient(ItemID.SoulofFlight, 3)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddIngredient(ItemID.SoulofNight, 3)
            .AddIngredient(ItemID.SoulofMight, 3)
            .AddIngredient(ItemID.SoulofFright, 3)
            .AddIngredient(ItemID.SoulofSight, 3)
            //add each other challenger expert drop when they're made

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}