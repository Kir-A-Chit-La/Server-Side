using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class InteractableObject : NetworkedBehaviour, ILootable
{
    [SerializeField] private InteractableType _interactableType;
    [SerializeField] private bool _respawnable;
    [SerializeField] private float _respawnTime;
    [SerializeField] private LootType _lootType;
    [SerializeField] private Item _item;
    [SerializeField] private float _maxLootSpawnDistance;
    [SerializeField] private float _lootObjectHight;
    public InteractableType InteractableType => _interactableType;
    public float RespawnTime => _respawnTime;
    public LootType LootType => _lootType;
    public Item Item => Instantiate(_item);
    public float MaxLootSpawnDistance => _maxLootSpawnDistance;

    private NetworkedObject _networkedObj;
    private float _timeToRespawn;
    private Transform _transform;
    private Vector3 _spawnPos;

    private void Awake()
    {
        _networkedObj = GetComponent<NetworkedObject>();
        _transform = GetComponent<Transform>();
    }
    private IEnumerator RespawnCoroutine()
    {
        while(Time.time < _timeToRespawn)
        {
            yield return null;
        }
        _networkedObj.Spawn();
    }
    [ClientRPC] private void GiveLoot() {}
    public void Respawn()
    {
        if(!_respawnable)
            return;
        
        _timeToRespawn = Time.time + RespawnTime;
        StartCoroutine(RespawnCoroutine());
    }
    public void Despawn() => _networkedObj.UnSpawn();
    public void GetLoot(ulong clientId, InventoryManager targetInventory)
    {
        if(targetInventory.AddItem(Item))
        {

        }
        else
        {
            Debug.Log($"Cant add item to {clientId} player");
        }
        Despawn();
        Respawn();
    }
}
