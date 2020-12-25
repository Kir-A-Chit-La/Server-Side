using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MLAPI;

public class PlayerTriggersHandler : MonoBehaviour
{
    public Action<NetworkedObject> OnFocusChange;
    public List<NetworkedObject> availableInteractables;
    private int _interactableA;
    private int _interactableB;
    private int _lootableA;
    private int _lootableB;
    public void Init()
    {
        availableInteractables = new List<NetworkedObject>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        var interactable = collider.GetComponent<IInteractable>();
        if(interactable != null)
        {
            availableInteractables.Add(collider.GetComponent<NetworkedObject>());
            availableInteractables.Sort(CompareInteractablesOrder);
            OnFocusChange?.Invoke(availableInteractables.First());
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        var interactable = collider.GetComponent<IInteractable>();
        if(interactable != null)
        {
            RemoveFromList(collider.GetComponent<NetworkedObject>());
        }
    }
    private void RemoveFromList(NetworkedObject interactable)
    {
        availableInteractables.Remove(interactable);
        OnFocusChange?.Invoke(availableInteractables.Count > 0 ? availableInteractables.First() : null);
    }
    private int CompareInteractablesOrder(NetworkedObject a, NetworkedObject b)
    {
        _interactableA = (int)a.GetComponent<IInteractable>().InteractableType;
        _interactableB = (int)b.GetComponent<IInteractable>().InteractableType;

        if(_interactableA < _interactableB)
        {
            return -1;
        }   
        else if(_interactableA > _interactableB)
        {
            return 1;
        }
        else
        {
            if(_interactableA != 1 && _interactableB != 1)
            {
                _lootableA = (int)a.GetComponent<ILootable>().LootType;
                _lootableB = (int)b.GetComponent<ILootable>().LootType;

                if(_lootableA < _lootableB)
                    return -1;
                else if(_lootableA > _lootableB)
                    return 1;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
