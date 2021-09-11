using System.IO;

namespace FargowiltasSouls.EternityMode.Net.Strategies
{
    public static class BoolStrategies
    {
        public static CompoundStrategy CompoundStrategy = new CompoundStrategy(new SendBool(), new RecieveBool());

        public class SendBool : ISendStrategy
        {
            public void Send(object value, BinaryWriter writer)
            {
                writer.Write((bool)value);
            }
        }

        public class RecieveBool : IRecieveStrategy
        {
            public void Recieve(ref object value, BinaryReader writer)
            {
                value = writer.ReadBoolean();
            }
        }
    }
}
