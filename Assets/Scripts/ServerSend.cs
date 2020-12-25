using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for(int i = 1; i <= Server.MaxClients; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    private static void SendTCPDataToAllEO(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for(int i = 1; i <= Server.MaxClients; i++)
        {
            if(i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    public static void Welcome(int _toClient, string msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void RegistrationResult(int _toClient, bool isRegistered, int id = 0, string username = "", int avatar = 0)
    {
        using(Packet _packet = new Packet((int)ServerPackets.registrationResult))
        {
            _packet.Write(isRegistered);
            if(id != 0)
            {
                _packet.Write(id);
                _packet.Write(username);
                _packet.Write(avatar);
            }

            SendTCPData(_toClient, _packet);
        }
    }

    public static void VerificationResult(int _toClient, bool isVerificated)
    {
        using (Packet _packet = new Packet((int)ServerPackets.verificationResult))
        {
            _packet.Write(isVerificated);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void CharacterCreationResult(int _toClient, bool isCreated, int id, string name, string gender, Vector3 spawn_position, Quaternion spawn_rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.characterCreationResult))
        {
            _packet.Write(isCreated);
            if(isCreated)
            {
                _packet.Write(id);
                _packet.Write(name);
                _packet.Write(gender);
                _packet.Write(spawn_position);
                _packet.Write(spawn_rotation);
            }

            SendTCPData(_toClient, _packet);
        }
    }

    public static void AccountData(int _toClient, bool success, int avatar, bool isVerificated, int charCount, Dictionary<int, Account.Character> characters = null)
    {
        using (Packet _packet = new Packet((int)ServerPackets.accountData))
        {
            _packet.Write(success);
            if(success)
            {
                _packet.Write(avatar);
                _packet.Write(isVerificated);
                _packet.Write(charCount);
                if(charCount > 0)
                {
                    _packet.Write(characters);
                }
            }

            SendTCPData(_toClient, _packet);
        }
    }

    public static void AccountSalt(int _toClient, bool isExists, string salt = "")
    {
        using (Packet _packet = new Packet((int)ServerPackets.accountSalt))
        {
            _packet.Write(isExists);
            if(isExists)
            {
                _packet.Write(salt);
            }

            SendTCPData(_toClient, _packet);
        }
    }

    public static void LogInResult(int _toClient, bool success, int id, int avatar, int charCount, Dictionary<int, Account.Character> characters = null)
    {
        using (Packet _packet = new Packet((int)ServerPackets.logInResult))
        {
            _packet.Write(success);
            if(success)
            {
                _packet.Write(id);
                _packet.Write(avatar);
                _packet.Write(charCount);
                if(charCount > 0)
                {
                    _packet.Write(characters);
                }
            }

            SendTCPData(_toClient, _packet);
        }
    }
}
