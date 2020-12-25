using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ItemSlot : BaseItemSlot, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public event Action<BaseItemSlot> OnBeginDragEvent;
    public event Action<BaseItemSlot> OnDragEvent;
    public event Action<BaseItemSlot> OnEndDragEvent;
    public event Action<BaseItemSlot> OnDropEvent;

    public override bool CanReceiveItem(Item item) => true;
    public override bool CanAddStack(Item item, int amount = 1) => base.CanAddStack(item, amount) && Amount + amount <= item.MaximumStackSize;
    public void OnBeginDrag(PointerEventData eventData) => OnBeginDragEvent?.Invoke(this);
    public void OnDrag(PointerEventData eventData) => OnDragEvent?.Invoke(this);
    public void OnEndDrag(PointerEventData eventData) => OnEndDragEvent?.Invoke(this);
    public void OnDrop(PointerEventData eventData) => OnDropEvent?.Invoke(this);
}
