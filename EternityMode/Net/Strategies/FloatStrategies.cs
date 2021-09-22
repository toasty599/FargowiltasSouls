using System.IO;

namespace FargowiltasSouls.EternityMode.Net.Strategies
{
    public static class FloatStrategies
    {
        public static CompoundStrategy CompoundStrategy = new CompoundStrategy(new SendFloat(), new RecieveFloat());

        public class SendFloat : ISendStrategy
        {
            public void Send(object value, BinaryWriter writer)
            {
                writer.Write((float)value);
            }
        }

        public class RecieveFloat : IRecieveStrategy
        {
            public void Recieve(ref object value, BinaryReader writer)
            {
                value = writer.ReadSingle();
            }
        }
    }
}
