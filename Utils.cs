using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using log4net;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace EndlessConsumables
{

    public class Utils
    {
        public static Mod mod;
        public static EndlessWorld endlessWorld;

        private static ILog Logger = LogManager.GetLogger("EndlessConsumables");

        private static JsonSerializerSettings settings;

        public static Dictionary<PacketType, Dictionary<string, Action<string>>> packetSubscribers = InitPacketSubscribers();

        static Utils()
        {
            settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static Item FindItemFromNameAndArray(string itemName, Item[] arr)
        {
            string itemNameLower = itemName.ToLower();
            foreach (Item item in arr)
            {
                if (item.Name.ToLower() == itemNameLower)
                {
                    return item;
                }
            }
            return null;
        }

        public static bool IsAtMaxStacks(Item item)
        {
            if (!item.consumable) return true;

            if (endlessWorld.data.overriddenEndlessItems.ContainsKey(item.type) && item.stack >= endlessWorld.data.overriddenEndlessItems[item.type])
            {
                return true;
            }
            return item.stack >= item.maxStack;
        }

        public static void Log(object msg)
        {
            Logger.Debug(msg);
        }

        public static string Serialize<T>(T o)
        {
            DataVersion dataVersion = (DataVersion)Attribute.GetCustomAttribute(o.GetType(), typeof(DataVersion));
            SerializedObject<T> wrapper = new SerializedObject<T>
            {
                version = dataVersion?.version ?? 1,
                data = o
            };
            return JsonConvert.SerializeObject(wrapper);
        }

        public static void Deserialize<T>(string data, object o)
        {
            SerializedObject<T> wrapper = JsonConvert.DeserializeObject<SerializedObject<T>>(data);
            Type dataType = wrapper.data.GetType();
            Type objectType = o.GetType();
            foreach (FieldInfo field in dataType.GetFields())
            {
                objectType.GetField(field.Name).SetValue(o, field.GetValue(wrapper.data));
            }
        }

        public static T Deserialize<T>(string data)
        {
            SerializedObject<T> wrapper = JsonConvert.DeserializeObject<SerializedObject<T>>(data);
            return wrapper.data;
        }

        private struct SerializedObject<T>
        {
            public int version;

            public T data;
        }

        private static Dictionary<PacketType, Dictionary<string, Action<string>>> InitPacketSubscribers()
        {
            Dictionary<PacketType, Dictionary<string, Action<string>>> packetSubscribers = new Dictionary<PacketType, Dictionary<string, Action<string>>>();
            foreach (PacketType packetType in Enum.GetValues(typeof(PacketType)))
            {
                packetSubscribers.Add(packetType, new Dictionary<string, Action<string>>());
            }
            return packetSubscribers;
        }
        public static void PacketSubscribe(string callerID, PacketType type, Action<string> callback)
        {
            Dictionary<string, Action<string>> subscribers = packetSubscribers[type];
            subscribers[callerID] = callback;
        }

        public static void PacketUnsubscribe(string callerID, PacketType type)
        {
            Dictionary<string, Action<string>> subscribers = packetSubscribers[type];
            subscribers.Remove(callerID);
        }

        public static void PacketSend(string data, PacketType type)
        {
            Log(Main.netMode);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)type);
                packet.Write(data);
                packet.Send();
                Log("sent!");
                Log(data);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                PacketCallSubscribers(data, type);
            }
        }

        public static void PacketReceive(BinaryReader reader)
        {
            PacketType type = (PacketType)reader.ReadByte();
            string data = reader.ReadString();
            Log("received!");
            Log(data);
            PacketCallSubscribers(data, type);
        }

        private static void PacketCallSubscribers(string data, PacketType type)
        {
            foreach (Action<string> action in packetSubscribers[type].Values)
            {
                action.Invoke(data);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DataVersion : System.Attribute
    {
        public readonly int version;
        public DataVersion(int version)
        {
            this.version = version;
        }
    }

    public enum PacketType : byte
    {
        WORLD_TOGGLE_ENDLESS_ITEM = 0
    }
}