using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ServerHandle : MonoBehaviour
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}");
        if(_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player (ID: {_fromClient} has assumed the wrong client ID ({_clientIdCheck})!");
        }
    }

    public static void RegisterRequest(int _fromClient, Packet _packet)
    {
        string username = _packet.ReadString();
        DBManager.instance.StartCoroutine(DBManager.instance.Register(_fromClient, username));
    }

    public static void VerificationRequest(int _fromClient, Packet _packet)
    {
        int id = _packet.ReadInt();
        string salt = _packet.ReadString();
        string hash = _packet.ReadString();

        DBManager.instance.StartCoroutine(DBManager.instance.Verificate(_fromClient, id, salt, hash));
    }

    public static void AvatarRequest(int _fromClient, Packet _packet)
    {
        int id = _packet.ReadInt();
        int avatar = _packet.ReadInt();

        DBManager.instance.StartCoroutine(DBManager.instance.ChangeAvatar(_fromClient, id, avatar));
    }

    public static void CharacterCreationRequest(int _fromClient, Packet _packet)
    {
        int id = _packet.ReadInt();
        string name = _packet.ReadString();
        string gender = _packet.ReadString();
        string _spawnPos = JsonUtility.ToJson(Client.spawnPosition);
        string _spawnRot = JsonUtility.ToJson(Quaternion.Euler(Client.spawnRotation));

        DBManager.instance.StartCoroutine(DBManager.instance.CreateCharacter(_fromClient, id, name, gender, _spawnPos, _spawnRot));
    }

    public static void AccountDataRequest(int _fromClient, Packet _packet)
    {
        int id = _packet.ReadInt();
        string username = _packet.ReadString();

        DBManager.instance.StartCoroutine(DBManager.instance.GetAccountData(_fromClient, id, username));
    }

    public static void LogInRequest(int _fromClient, Packet _packet)
    {
        string username = _packet.ReadString();

        DBManager.instance.StartCoroutine(DBManager.instance.LogInRequest(_fromClient, username));
    }

    public static void LogIn(int _fromClient, Packet _packet)
    {
        string username = _packet.ReadString();
        string hash = _packet.ReadString();

        DBManager.instance.StartCoroutine(DBManager.instance.LogIn(_fromClient, username, hash));
    }
}
