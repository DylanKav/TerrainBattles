using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using TerrainBattlesCore.Core;
using TerrainBattlesCore.Net;
using Unity.VisualScripting;
using UnityEngine;
using Point3 = TerrainBattlesCore.Math.Point3;

public class NetClient : MonoBehaviour
{
    static EventBasedNetListener listener = new EventBasedNetListener();
    NetManager client = new NetManager(listener);
    private NetPeer server;
    [SerializeField] private Transform player;
    
    //events and delegates!
    public delegate void ReceivePlayerPosition(PlayerPosition packet);
    public delegate void UpdatePlayerState(PlayerState packet);
    
    public event ReceivePlayerPosition PlayerPositionChanged;
    public event UpdatePlayerState PlayerStateChanged;

    private bool _isConnected;
    
    void Start()
    {
        client.Start();
        client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
        listener.NetworkReceiveEvent += OnEventReceive;
        StartCoroutine(PollEvents());

        
    }

    private void OnDestroy()
    {
        client.Stop();
    }

    private void OnEventReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
    {
        var header = reader.GetInt();

        if (header == 999)
        {
            server = peer;
            OnHandshakeReceived();
            Debug.Log("Received handshake from server");
            
        }
        if ((PacketType)header == PacketType.PlayerState)
        {
            var state = PlayerState.Deserialize(reader);
            Debug.Log("Got player state for: " + state.UserName);
            PlayerStateChanged?.Invoke(state);
        }
        if ((PacketType)header == PacketType.PlayerPosition)
        {
            var position = PlayerPosition.Deserialize(reader);
            Debug.Log("Got player position for: " + position.UserName + " their new position is " + position.WorldPosition.x + ", " + position.WorldPosition.y + ", " + position.WorldPosition.z);
            PlayerPositionChanged?.Invoke(position);
        }
        
        reader.Recycle();
    }

    private void OnHandshakeReceived()
    {
        var playerState = new PlayerState();
        playerState.UserName = "Dylan";
        playerState.WorldPosition = new TerrainBattlesCore.Math.Point3();
        playerState.WorldPosition.x = player.position.x;
        playerState.WorldPosition.x = player.position.y;
        playerState.WorldPosition.x = player.position.z;
        playerState.MaxHealth = playerState.MaxStamina = playerState.MaxHex = 100;
        playerState.Hunger = 100;
        playerState.Thirst = 100;
        playerState.CurrentHealth = playerState.CurrentStamina = playerState.CurrentHex = 100;
        NetDataWriter writer = new NetDataWriter();
        PlayerState.Serialize(writer, playerState);
        SendReliableDataToServer(writer);
        StartCoroutine(SendPlayerData());
    }

    private void SendReliableDataToServer(NetDataWriter writer)
    {
        server.Send(writer, DeliveryMethod.ReliableOrdered);
    }
    
    private void SendUnreliableDataToServer(NetDataWriter writer)
    {
        server.Send(writer, DeliveryMethod.Sequenced);
    }

    private IEnumerator PollEvents()
    {
        while (true)
        {
            client.PollEvents();
            yield return new WaitForSeconds(.015f);
        }
    }

    private IEnumerator SendPlayerData()
    {
        while (true)
        {
            var position = new PlayerPosition();
            position.UserName = "Dylan";
            var playerPos = player.position;
            var playerRot = player.rotation;
            position.WorldPosition.x = playerPos.x;
            position.WorldPosition.y = playerPos.y;
            position.WorldPosition.z = playerPos.z;
            position.WorldRotation.x = playerRot.eulerAngles.x;
            position.WorldRotation.y = playerRot.eulerAngles.y;
            position.WorldRotation.z = playerRot.eulerAngles.z;
            NetDataWriter writer = new NetDataWriter();
            PlayerPosition.Serialize(writer, position);
            SendUnreliableDataToServer(writer);
            yield return new WaitForSeconds(.015f);
        }
    }
}
