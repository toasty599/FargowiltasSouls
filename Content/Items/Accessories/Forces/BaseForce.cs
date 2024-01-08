using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public abstract class BaseForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.ItemNoGravity[Type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = 600000;
        }

        
    }

    public class ForceSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            SetFactory factory = new(ContentSamples.ItemsByType.Count);
            CosmoForce.ContainsEnchant = factory.CreateBoolSet(CosmoForce.Enchants);
            EarthForce.ContainsEnchant = factory.CreateBoolSet(EarthForce.Enchants);
            LifeForce.ContainsEnchant = factory.CreateBoolSet(LifeForce.Enchants);
            NatureForce.ContainsEnchant = factory.CreateBoolSet(NatureForce.Enchants);
            ShadowForce.ContainsEnchant = factory.CreateBoolSet(ShadowForce.Enchants);
            SpiritForce.ContainsEnchant = factory.CreateBoolSet(SpiritForce.Enchants);
            TerraForce.ContainsEnchant = factory.CreateBoolSet(TerraForce.Enchants);
            TimberForce.ContainsEnchant = factory.CreateBoolSet(TimberForce.Enchants);
            WillForce.ContainsEnchant = factory.CreateBoolSet(WillForce.Enchants);
        }
    }
}
