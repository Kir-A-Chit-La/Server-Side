using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.NetworkedVar.Collections;

public class PlayerController : NetworkedBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private NetworkedObject _currentFocus;
    private InventoryManager _inventoryManager;
    private PlayerTriggersHandler _triggersHandler;
    private IInteractable _interactable;
    private ILootable _lootable;

    private void Awake()
    {
        _inventoryManager = GetComponent<InventoryManager>();
        _triggersHandler = GetComponent<PlayerTriggersHandler>();

        _inventoryManager.Init(Instantiate(_stats));
        _triggersHandler.Init();

        _triggersHandler.OnFocusChange += SetFocus;
    }
    private void SetFocus(NetworkedObject networkedObject)
    {
        if(_currentFocus != networkedObject)
            _currentFocus = networkedObject;
    }
    [ServerRPC] private void Interuct(ulong targetNetworkId)
    {
        for(int i = 0; i < _triggersHandler.availableInteractables.Count; i++)
        {
            if(_triggersHandler.availableInteractables[i].NetworkId == targetNetworkId)
            {
                _lootable = _triggersHandler.availableInteractables[i].GetComponent<ILootable>();
                _lootable.GetLoot(OwnerClientId, _inventoryManager);
            }
        }
    }
}