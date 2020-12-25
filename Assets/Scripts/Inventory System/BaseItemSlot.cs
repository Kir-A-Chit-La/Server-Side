using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseItemSlot : MonoBehaviour, IPointerClickHandler
{
    public event Action<BaseItemSlot> OnAmountChange;
    public event Action<BaseItemSlot> OnClickEvent;
    public Image slotImage;
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TMP_Text amountText;
    protected Color normalColor = Color.white;
    protected Color disabledColor = new Color(1, 1, 1, 0);
    [SerializeField] private Item _item;
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
			if (_item == null && Amount != 0) 
                Amount = 0;

			if (_item == null) {
				itemImage.sprite = null;
				itemImage.color = disabledColor;
			} else {
				itemImage.sprite = _item.Preview;
				itemImage.color = normalColor;
			}
        }
    }
    private int _amount;
    public int Amount
    {
        get => _amount;
        set
        { 
            _amount = value;
            if(_amount < 0)
                _amount = 0;
            if(_amount == 0 && Item != null)
                Item = null;
            
            OnAmountChange?.Invoke(this);
            
            if(amountText != null)
            {
                amountText.enabled = _item != null && (_amount > 1 || _item.MaximumStackSize > 1);
                if(amountText.enabled)
                    amountText.text = _amount.ToString();
            }
        }
    }
    private ItemSlotNetworkedData _networkedItemSlot = new ItemSlotNetworkedData();
    public ItemSlotNetworkedData NetworkedItemSlot
    {
        get
        {
            if(Item != null)
            {
                _networkedItemSlot.itemId = Item.Id;
                _networkedItemSlot.itemAmount = Amount;
                return _networkedItemSlot;
            }
            else
            {
                return null;
            }
        }
    }

    protected virtual void OnValidate()
    {
        if(slotImage == null)
            slotImage = GetComponent<Image>();

        if(itemImage == null)
            itemImage = GetComponentsInChildren<Image>()[1];
        
        if(amountText == null)
            amountText = GetComponentInChildren<TMP_Text>();
        
        Item = _item;
        Amount = _amount;
    }
    public virtual bool CanReceiveItem(Item item) => false;
    public virtual bool CanAddStack(Item item, int amount = 1) => Item != null && Item.Id == item.Id;
    public void OnPointerClick(PointerEventData eventData) => OnClickEvent?.Invoke(this);
}
