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
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
                modPlayer.PrimeSoulItem = Item;
            }
        }

        public override void UpdateInventory(Player player) => PrimeSoulEffect(player);
        public override void UpdateVanity(Player player) => PrimeSoulEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual) => PrimeSoulEffect(player);
    }
}