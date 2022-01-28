using System.IO;

namespace FargowiltasSouls.EternityMode.Net
{
    public interface IRecieveStrategy
    {
        void Recieve(ref object value, BinaryReader reader);
    }
}
