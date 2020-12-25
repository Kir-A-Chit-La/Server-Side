using System;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar.Collections;

public abstract class ItemContainer : MonoBehaviour, IItemContainer
{
    public List<ItemSlot> itemSlots;

    public virtual void Init()
    {
    }
    public virtual int ItemCount(string itemId)
    {
        int number = 0;
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i].Item.Id == itemId)
            {
                number += itemSlots[i].Amount;
            }
        }
        return number;
    }
    public virtual bool ContainsItem(Item item)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i].Item == item)
            {
                return true;
            }
        }
        return false;
    }
    public virtual bool CanAddItem(Item item, int amount = 1)
    {
        int freeSpaces = 0;
        foreach(ItemSlot itemSlot in itemSlots)
        {
            if(itemSlot.Item != null || itemSlot.Item.Id == item.Id)
            {
                freeSpaces += item.MaximumStackSize - itemSlot.Amount;
            }
        }
        return freeSpaces >= amount;
    }
    public virtual bool AddItem(Item item)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i].CanAddStack(item))
            {
                itemSlots[i].Item = item;
                itemSlots[i].Amount++;
                return true;
            }
        }
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i].Item == null || itemSlots[i].CanAddStack(item))
            {
                itemSlots[i].Item = item;
                itemSlots[i].Amount++;
                return true;
            }
        }
        return false;
    }
    public virtual Item RemoveItem(string itemId)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            Item item = itemSlots[i].Item;
            if(item != null && item.Id == itemId)
            {
                itemSlots[i].Amount--;
                
                return item;
            }
        }
        return null;
    }
    public virtual bool RemoveItem(Item item)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(itemSlots[i].Item == item)
            {
                itemSlots[i].Amount--;
                
                return true;
            }
        }
        return false;
    }
    public virtual void Clear()
	{
		for (int i = 0; i < itemSlots.Count; i++)
		{
			if (itemSlots[i].Item != null && Application.isPlaying) {
				itemSlots[i].Item.Destroy();
			}
			itemSlots[i].Item = null;
			itemSlots[i].Amount = 0;
		}
	}
}
