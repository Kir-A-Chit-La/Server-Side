
public enum LootType
{
    Drop = 100,
    Ore = 200,
    Stone = 300,
    Wood = 400
}

public interface ILootable : IInteractable
{
    LootType LootType { get; }
    Item Item { get; }
    float MaxLootSpawnDistance { get; }
    void GetLoot(ulong clientId, InventoryManager inventoryManager);
}
