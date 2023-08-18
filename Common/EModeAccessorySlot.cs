using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common
{
    public abstract class EModeAccessorySlot : ModAccessorySlot
    {
        public abstract int Loadout { get; }
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            if ((context == AccessorySlotType.FunctionalSlot || context == AccessorySlotType.VanitySlot) && base.CanAcceptItem(checkItem, context))
            {
                if (checkItem.ModItem is BaseEnchant || checkItem.ModItem is BaseForce || checkItem.ModItem is BaseSoul)
                {
                    return true;
                }
                return false;
            }
            return base.CanAcceptItem(checkItem, context);
        }
        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
        {
            if (item.ModItem is BaseEnchant || item.ModItem is BaseForce || item.ModItem is BaseSoul)
            {
                return true;
            }
            return false;
        }
        public override bool IsVisibleWhenNotEnabled() => !(WorldSavingSystem.EternityMode && Player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot);
        public override bool IsEnabled()
        {
            return WorldSavingSystem.EternityMode && Player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot && Player.CurrentLoadoutIndex == Loadout;
        }
        public override string FunctionalTexture => "FargowiltasSouls/Assets/UI/EnchantSlotIcon";
        //public override string FunctionalBackgroundTexture => "FargowiltasSouls/Assets/UI/EnchantSlotBackground";
        //public override string VanityBackgroundTexture => "FargowiltasSouls/Assets/UI/EnchantSlotBackground";
        //public override string DyeBackgroundTexture => "FargowiltasSouls/Assets/UI/EnchantSlotBackground";

        public override void OnMouseHover(AccessorySlotType context)
        {
            switch (context)
            {
                case AccessorySlotType.FunctionalSlot:
                    Main.hoverItemName = "Enchantment, Force or Soul";
                    break;
                case AccessorySlotType.VanitySlot:
                    Main.hoverItemName = "Social Enchantment, Force or Soul";
                    break;
            }
        }
    }
    public class EModeAccessorySlot0 : EModeAccessorySlot
    {
        public override int Loadout => 0;
    }
    public class EModeAccessorySlot1 : EModeAccessorySlot
    {
        public override int Loadout => 1;
    }
    public class EModeAccessorySlot2 : EModeAccessorySlot
    {
        public override int Loadout => 2;
    }
}
