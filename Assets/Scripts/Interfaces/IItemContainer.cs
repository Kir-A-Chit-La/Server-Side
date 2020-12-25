
public interface IItemContainer
{
    bool CanAddItem(Item item, int amount = 1);
    bool AddItem(Item item);

    Item RemoveItem(string itemId);
    bool RemoveItem(Item item);

    int ItemCount(string itemId);
    
    void Clear();
}
