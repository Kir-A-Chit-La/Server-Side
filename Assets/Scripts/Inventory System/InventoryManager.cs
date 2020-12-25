using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar.Collections;
using MLAPI.NetworkedVar;
using System.Collections.Generic;
using MLAPI.Messaging;
using System;

public class InventoryManager : NetworkedBehaviour
{
    private PlayerStats _stats;
    [SerializeField] private ItemDatabase _itemDatabase;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Equipment _equipment;
    private NetworkedList<ItemSlotNetworkedData> _networkedInventoryData = new NetworkedList<ItemSlotNetworkedData>();
    private NetworkedList<ItemSlotNetworkedData> _networkedEquipmentData = new NetworkedList<ItemSlotNetworkedData>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone });

    public void Init(PlayerStats stats)
    {
        _stats = stats;
        ItemSlotNetworkedData.RegisterSerialization();

        _inventory.Init();
        _equipment.Init();
        UpdateInventoryMaxSlots();
        SetNetworkedListData(_networkedInventoryData, _inventory.itemSlots);
        SetNetworkedListData(_networkedEquipmentData, _equipment.equipmentSlots);

        _inventory.OnItemSlotChangeEvent += (index, networkedItemSlot) => OnListChange(index, networkedItemSlot, _networkedInventoryData);
        _equipment.OnItemSlotChangeEvent += (index, networkedItemSlot) => OnListChange(index, networkedItemSlot, _networkedEquipmentData);
    }
    private void SetNetworkedListData(NetworkedList<ItemSlotNetworkedData> networkedList, IList<ItemSlot> sourceList)
    {
        for(int i = 0; i < sourceList.Count; i++)
        {
            ItemSlot itemSlot = sourceList[i];
            if(itemSlot.Item == null)
            {
                networkedList.Insert(i, null);
            }
            else
            {
                networkedList.Insert(i, new ItemSlotNetworkedData(itemSlot.Item.Id, itemSlot.Amount));
            }
        }
    }
    private void OnListChange(int index, ItemSlotNetworkedData networkedItemSlot, NetworkedList<ItemSlotNetworkedData> networkedList)
    {
        networkedList.Insert(index, networkedItemSlot);
        // Debug.LogError(networkedItemSlot.itemAmount);
        // networkedList[index] = networkedItemSlot;
    }
    [ServerRPC] private void ChangeItemSlots(int draggedItemSlotIndex, string draggedItemSlotType, int dropItemSlotIndex, string dropItemSlotType)
    {
        if(draggedItemSlotType != typeof(EquipmentSlot).ToString() && dropItemSlotType == typeof(EquipmentSlot).ToString())
        {
            ItemSlot draggedItemSlot = _inventory.itemSlots[draggedItemSlotIndex];
            EquipmentSlot dropItemSlot = _equipment.equipmentSlots[dropItemSlotIndex];
            HandleUserInput(draggedItemSlot, dropItemSlot);
        }
        else if(draggedItemSlotType == typeof(EquipmentSlot).ToString() && dropItemSlotType != typeof(EquipmentSlot).ToString())
        {
            EquipmentSlot draggedItemSlot = _equipment.equipmentSlots[draggedItemSlotIndex];
            ItemSlot dropItemSlot = _inventory.itemSlots[dropItemSlotIndex];
            HandleUserInput(draggedItemSlot, dropItemSlot);
        }
        else
        {
            ItemSlot draggedItemSlot = _inventory.itemSlots[draggedItemSlotIndex];
            ItemSlot dropItemSlot = _inventory.itemSlots[dropItemSlotIndex];
            HandleUserInput(draggedItemSlot, dropItemSlot);
        }
    }
    private void HandleUserInput(ItemSlot draggedItemSlot, ItemSlot dropItemSlot)
    {
        if(draggedItemSlot.Item != null)
        {
            if(dropItemSlot.CanAddStack(draggedItemSlot.Item))
                AddStacks(draggedItemSlot, dropItemSlot);
            else if(dropItemSlot.CanReceiveItem(draggedItemSlot.Item) && draggedItemSlot.CanReceiveItem(dropItemSlot.Item))
                SwapItems(draggedItemSlot, dropItemSlot);
        }
    }
    private void SwapItems(BaseItemSlot draggedItemSlot, BaseItemSlot dropItemSlot)
    {
        EquippableItem dragEquipItem = draggedItemSlot.Item as EquippableItem;
        EquippableItem dropEquipItem = dropItemSlot.Item as EquippableItem;

        if (dropItemSlot is EquipmentSlot)
        {
            if (dragEquipItem != null)
                dragEquipItem.Equip(_stats);
            if (dropEquipItem != null)
                dropEquipItem.Unequip(_stats);
        }
        if (draggedItemSlot is EquipmentSlot)
        {
            if (dragEquipItem != null)
                dragEquipItem.Unequip(_stats);
            if (dropEquipItem != null)
                dropEquipItem.Equip(_stats);
        }
        UpdateInventoryMaxSlots();
       
        Item draggedItem = draggedItemSlot.Item;
        int draggedItemAmount = draggedItemSlot.Amount;

        draggedItemSlot.Item = dropItemSlot.Item;
        draggedItemSlot.Amount = dropItemSlot.Amount;

        dropItemSlot.Item = draggedItem;
        dropItemSlot.Amount = draggedItemAmount;
    }
    private void AddStacks(BaseItemSlot draggedItemSlot, BaseItemSlot dropItemSlot)
    {
        int numAddableStacks = dropItemSlot.Item.MaximumStackSize - dropItemSlot.Amount;
        int stacksToAdd = Mathf.Min(numAddableStacks, draggedItemSlot.Amount);

        dropItemSlot.Amount += stacksToAdd;
        draggedItemSlot.Amount -= stacksToAdd;
    }
    public bool AddItem(Item item)
    {
        EquippableItem equippableItem = item as EquippableItem;
        if(equippableItem != null)
        {
            if(_equipment.AddItem(equippableItem))
            {
                equippableItem.Equip(_stats);
                UpdateInventoryMaxSlots();
                return true;
            }
            else
            {
                return _inventory.AddItem(item);
            }
        }
        else
        {
            return _inventory.AddItem(item);
        }
    }
    public void Equip(EquippableItem item)
    {
        if(_inventory.RemoveItem(item))
        {
            EquippableItem previousItem;
            if(_equipment.AddItem(item, out previousItem))
            {
                if(previousItem != null)
                {
                    _inventory.AddItem(previousItem);
                    previousItem.Unequip(_stats);
                    UpdateInventoryMaxSlots();
                }
                item.Equip(_stats);
                UpdateInventoryMaxSlots();
            }
            else
            {
                _inventory.AddItem(item);
            }
        }
    }
    public void Unequip(EquippableItem item)
    {
        if(_inventory.CanAddItem(item) && _equipment.RemoveItem(item))
        {
            item.Unequip(_stats);
            UpdateInventoryMaxSlots();
            _inventory.AddItem(item);
        }
    }
    private void Equip(BaseItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if(equippableItem != null)
            Equip(equippableItem);
    }
    private void Unequip(BaseItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if(equippableItem != null)
            Unequip(equippableItem);
    }
    private void UpdateInventoryMaxSlots() => _inventory.MaxSlots = (int)_stats.BackpackSize.Value;
}
