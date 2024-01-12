using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public abstract class BaseForce : SoulsItem
    {
        public static Dictionary<int, int[]> ForceEnchants;
        public static int[] EnchantsIn<T>() where T : BaseForce => ForceEnchants[ModContent.ItemType<T>()];
        public void SetActive(Player player) => player.GetEffectFields<ForceFields>().ForceEffects.Add(Type);
        public virtual int[] Enchants { get; }
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
            foreach (BaseForce force in ModContent.GetContent<BaseForce>())
            {
                BaseForce.ForceEnchants[force.Type] = force.Enchants;
                foreach (int enchant in force.Enchants)
                {
                    BaseEnchant.Force[enchant] = force.Type;
                }
            }
        }
    }
    public class ForceFields : EffectFields
    {
        public HashSet<int> ForceEffects;
        public override void ResetEffects()
        {
            ForceEffects.Clear();
        }
    }
}
