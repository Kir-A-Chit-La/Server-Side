using System;
using System.IO;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;

[Serializable]
public class ItemSlotNetworkedData
{
    public string itemId;
    public int itemAmount;

    public ItemSlotNetworkedData(string id, int amount)
    {
        itemId = id;
        itemAmount = amount;
    }
    public ItemSlotNetworkedData() : this(string.Empty, 0) {}
    public static void RegisterSerialization()
    {
        SerializationManager.RegisterSerializationHandlers<ItemSlotNetworkedData>((Stream stream, ItemSlotNetworkedData instance) =>
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteStringPacked(instance.itemId);
                writer.WriteInt32(instance.itemAmount);
            }
        }, (Stream stream) =>
        {
            using(PooledBitReader reader = PooledBitReader.Get(stream))
            {
                return new ItemSlotNetworkedData(reader.ReadStringPacked().ToString(), reader.ReadInt32());
            }
        });
    }
}

[Serializable]
public class ItemContainerNetworkedData
{
    public ItemSlotNetworkedData[] networkedSlots;

    public ItemContainerNetworkedData(int numItems)
    {
        networkedSlots = new ItemSlotNetworkedData[numItems];
    }
}
