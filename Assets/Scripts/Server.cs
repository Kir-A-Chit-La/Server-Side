using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class Server
{
    public static int MaxClients { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;

    public static void Start(int maxClients, int port)
    {
        MaxClients = maxClients;
        Port = port;

        Debug.Log("Starting server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptSocket(new AsyncCallback(TCPConnectCallback), null);

        Debug.Log($"Server started on {Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for(int i = 1; i <= MaxClients; i++)
        {
            if(clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full");
    }

    private static void InitializeServerData()
    {
        for(int i = 1; i <= MaxClients; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
            { (int)ClientPackets.registerRequest, ServerHandle.RegisterRequest },
            { (int)ClientPackets.verificationRequest, ServerHandle.VerificationRequest },
            { (int)ClientPackets.accountDataRequest, ServerHandle.AccountDataRequest },
            { (int)ClientPackets.avatarRequest, ServerHandle.AvatarRequest },
            { (int)ClientPackets.characterCreationRequest, ServerHandle.CharacterCreationRequest },
            { (int)ClientPackets.logInRequst, ServerHandle.LogInRequest },
            { (int)ClientPackets.logIn, ServerHandle.LogIn }
        };
        Debug.Log("Initialized packets");
    }

    public static void Stop()
    {
        tcpListener.Stop();
    }
}
