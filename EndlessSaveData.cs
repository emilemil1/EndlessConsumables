
namespace EndlessConsumables
{
    public abstract class EndlessSaveData<T>
    {
        public string Serialize()
        {
            return Utils.Serialize(this);
        }

        public void Deserialize(string data)
        {
            if (data == null || data == "") return;
            try
            {
                Utils.Deserialize<T>(data, this);
            }
            catch
            {
                Utils.Log("Deserialization failed for data:");
                Utils.Log(data);
            }
        }
    }
}