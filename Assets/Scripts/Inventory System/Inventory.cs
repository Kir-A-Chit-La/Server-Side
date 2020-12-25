using System;
using UnityEngine;

public class Inventory : ItemContainer
{
    public event Action<int, ItemSlotNetworkedData> OnItemSlotChangeEvent;
    [SerializeField, Range(7, 14)] private int _maxSlots = 7;
    public int MaxSlots
    {
        get => _maxSlots;
        set
        {
            SetMaxSlots(value);
        }
    }
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private Transform _itemSlotsParent;
    [SerializeField] private Item[] _startingItems;

    public override void Init()
    {
        SetMaxSlots(_maxSlots);
        base.Init();
        for(int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].OnAmountChange += slot => OnItemSlotChange(slot);
        }
        SetStartingItems();
    }
    private void OnItemSlotChange(BaseItemSlot itemSlot)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i] == itemSlot)
                OnItemSlotChangeEvent?.Invoke(i, itemSlot.NetworkedItemSlot);
        }
    }
    private void SetMaxSlots(int value)
    {
        if(value <= 7)
            _maxSlots = 7;
        else
            _maxSlots = value;
        
        if(_maxSlots < itemSlots.Count)
        {
            for(int i = _maxSlots; i < itemSlots.Count; i++)
            {
                Destroy(itemSlots[i].gameObject);
            }
            int diff = itemSlots.Count - _maxSlots;
            itemSlots.RemoveRange(_maxSlots, diff);
        }
        else if(_maxSlots > itemSlots.Count)
        {
            int diff = _maxSlots - itemSlots.Count;

            for(int i = 0; i < diff; i++)
            {
                GameObject itemSlotGameObj = Instantiate(_itemSlotPrefab);
                itemSlotGameObj.transform.SetParent(_itemSlotsParent, false);
                ItemSlot itemSlot = itemSlotGameObj.GetComponentInChildren<ItemSlot>();
                itemSlot.OnAmountChange += slot => OnItemSlotChange(slot);
                itemSlots.Add(itemSlot);
            }
        }
    }
    private void SetStartingItems()
    {
        Clear();
        foreach(Item item in _startingItems)
        {
            AddItem(item.GetCopy());
        }
    }
    private void OnValidate()
    {
        if(_itemSlotsParent != null)
            _itemSlotsParent.GetComponentsInChildren<ItemSlot>(true, itemSlots);
        
        if (!Application.isPlaying) {
			SetStartingItems();
		}
    }
}
