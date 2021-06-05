using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Ammos
{
    public class BrittleBone : SoulsItem
    {
        public override string Texture => "Terraria/Item_154";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brittle Bone");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Bone);
            item.shoot = 0;
            item.useAnimation = 0;
            item.useTime = 0;
            item.useStyle = 0;
            item.notAmmo = true;
        }
    }
}