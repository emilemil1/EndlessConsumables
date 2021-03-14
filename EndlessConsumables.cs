using System.IO;
using Terraria.ModLoader;

namespace EndlessConsumables
{

    public class EndlessConsumables : Mod
    {
        public EndlessConsumables()
        {
            Utils.mod = this;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Utils.PacketReceive(reader);
        }
    }
}