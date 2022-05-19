using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    //weird name so it goes before the other globalprojectiles
    //TODO: make this a proper inherited class and a modproj equivalent, axe the dictionary
    public class a_SourceNPCGlobalProjectile : GlobalProjectile
    {
        public static Dictionary<int, bool> NeedsSyncByType = new Dictionary<int, bool>();

        public override bool InstancePerEntity => true;

        public NPC sourceNPC;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC)
                {
                    projectile.SetSourceNPC(parent.Entity as NPC);
                }
                else if (parent.Entity is Projectile)
                {
                    Projectile sourceProj = parent.Entity as Projectile;
                    projectile.SetSourceNPC(sourceProj.GetSourceNPC());
                }
            }
        }

        private bool NeedsSync(Projectile projectile)
            => NeedsSyncByType.TryGetValue(projectile.type, out bool value) && value;

        public override void SendExtraAI(Projectile projectile, BinaryWriter writer)
        {
            if (NeedsSync(projectile))
            writer.Write(sourceNPC is NPC ? sourceNPC.whoAmI : -1);
        }

        public override void ReceiveExtraAI(Projectile projectile, BinaryReader reader)
        {
            if (NeedsSync(projectile))
                sourceNPC = FargoSoulsUtil.NPCExists(reader.ReadInt32());
        }

        public override bool PreAI(Projectile projectile)
        {
            if (sourceNPC is NPC && !sourceNPC.active)
                sourceNPC = null;

            return base.PreAI(projectile);
        }
    }
}
