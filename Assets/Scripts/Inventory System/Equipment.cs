using System;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public event Action<int, ItemSlotNetworkedData> OnItemSlotChangeEvent;
    [SerializeField] protected Transform _equipmentSlotsParent;
    public EquipmentSlot[] equipmentSlots;

    private void OnValidate()
    {
        if(_equipmentSlotsParent != null)
            equipmentSlots = _equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>();
    }
    public void Init() 
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].OnAmountChange += slot => OnItemSlotChange(slot);
        }
    }
    private void OnItemSlotChange(BaseItemSlot itemSlot)
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            if(equipmentSlots[i] == itemSlot)
            {
                if(itemSlot.Item != null)
                    OnItemSlotChangeEvent?.Invoke(i, itemSlot.NetworkedItemSlot);
                else
                    OnItemSlotChangeEvent?.Invoke(i, itemSlot.NetworkedItemSlot);
            }
        }
    }
    public bool AddItem(EquippableItem item, out EquippableItem previousItem)
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            if(equipmentSlots[i].equipmentType == item.EquipmentType)
            {
                previousItem = (EquippableItem)equipmentSlots[i].Item;
                equipmentSlots[i].Item = item;
                equipmentSlots[i].Amount++;
                return true;
            }
        }
        previousItem = null;
        return false;
    }
    public bool AddItem(EquippableItem item)
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            if(equipmentSlots[i].equipmentType == item.EquipmentType && equipmentSlots[i].Item == null)
            {
                equipmentSlots[i].Item = item;
                equipmentSlots[i].Amount = 1;
                return true;
            }
        }
        return false;
    }
    public bool RemoveItem(EquippableItem item)
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            if(equipmentSlots[i].Item == item)
            {
                equipmentSlots[i].Item = null;
                equipmentSlots[i].Amount = 0;
                return true;
            }
        }
        return false;
    }
}
