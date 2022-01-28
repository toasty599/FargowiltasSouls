using System.IO;

namespace FargowiltasSouls.EternityMode.Net
{
    public interface ISendStrategy
    {
        void Send(object value, BinaryWriter writer);
    }
}
