using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;

public class NetworkManager : NetworkedBehaviour
{
    public static NetworkManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 20;

        Server.Start(100, 26950);
        NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkingManager.Singleton.OnClientDisconnectCallback += GetLastPlayerState;
        NetworkingManager.Singleton.StartServer();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
    {
        string[] strConnetionData = Encoding.ASCII.GetString(connectionData).Split(' ');
        int _clientId = Int32.Parse(strConnetionData[0]);
        int charId = Int32.Parse(strConnetionData[1]);
        
        bool approve = true;
        bool createPlayerObject = false;
        
        ulong? prefabHash = null;
        callback(createPlayerObject, prefabHash, approve, null, null);
        CreatePlayerObject(clientId, _clientId, charId);
    }

    private void CreatePlayerObject(ulong playerID, int clientID, int charID)
    {
        GameObject player = Instantiate(NetworkingManager.Singleton.NetworkConfig.NetworkedPrefabs[0].Prefab, Server.clients[clientID].account.Characters[charID].Position, Server.clients[clientID].account.Characters[charID].Rotation);
        player.GetComponent<NetworkedObject>().SpawnAsPlayerObject(playerID);
    }

    private void GetLastPlayerState(ulong cliendId)
    {
        // TODO: get from here last player data and send to DBManager
        Debug.Log("Disconnected");
        //Debug.Log(GetNetworkedObject(cliendId).NetworkId);
        //Debug.Log(GetNetworkedObject(cliendId).transform.position);
    }

    private void OnApplicationQuit()
    {
        NetworkingManager.Singleton.StopServer();
        Server.Stop();
    }
}
