using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.IO;

namespace EndlessConsumables
{
    public class EndlessWorld : ModWorld
    {
        public EndlessWorldSaveData data;
        public override void Initialize()
        {
            Utils.endlessWorld = this;
            data = new EndlessWorldSaveData();
        }

        public override TagCompound Save()
        {
            return new TagCompound() { { "data", data.Serialize() } };
        }

        public override void Load(TagCompound tag)
        {
            data.Deserialize(tag.GetString("data"));
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(data.Serialize());
        }

        public override void NetReceive(BinaryReader reader)
        {
            this.data.Deserialize(reader.ReadString());
            PostSync();
        }

        public void ToggleCustomEndlessItem(Item item, int amount)
        {

            int type = item.type;
            if (data.overriddenEndlessItems.ContainsKey(type))
            {
                data.overriddenEndlessItems.Remove(type);
                data.announcements.Add(item.Name + " is no longer endless.");
            }
            else
            {
                data.overriddenEndlessItems.Add(type, amount);
                data.announcements.Add(item.Name + " is now endless at " + amount + " stacks.");
            }
            SyncClients();
        }

        private struct ToggleCustomEndlessItemPacket
        {
            public int type;
            public string name;
            public int amount;
        }

        public void SyncClients()
        {
            NetMessage.SendData(MessageID.WorldData);
            PostSync();
        }

        private void Announce()
        {
            foreach (string announcement in data.announcements)
            {
                Main.NewText(announcement);
            }
            data.announcements.Clear();
        }

        private void PostSync()
        {
            Announce();
        }

        [DataVersion(1)]
        public class EndlessWorldSaveData : EndlessSaveData<EndlessWorldSaveData>
        {
            public Dictionary<int, int> overriddenEndlessItems = new Dictionary<int, int>();
            public List<string> announcements = new List<string>();
        }
    }
}