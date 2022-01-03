using System.IO;
namespace FargowiltasSouls.EternityMode.Net
{
    public struct CompoundStrategy
    {
        public ISendStrategy SendStrategy;
        public IRecieveStrategy RecieveStrategy;

        public CompoundStrategy(ISendStrategy sendStrategy, IRecieveStrategy recieveStrategy)
        {
            SendStrategy = sendStrategy;
            RecieveStrategy = recieveStrategy;
        }

        public void Send(object value, BinaryWriter writer) => SendStrategy.Send(value, writer);

        public void Recieve(ref object value, BinaryReader reader) => RecieveStrategy.Recieve(ref value, reader);
    }
}
