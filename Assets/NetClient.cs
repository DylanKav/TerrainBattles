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
    
    void Start()
    {
        client.Start();
        client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
        listener.NetworkReceiveEvent += OnEventReceive;
        StartCoroutine(PollEvents());

        StartCoroutine(SendPlayerData());
    }

    private void OnEventReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
    {
            Debug.Log(reader.GetString(100 /* max length of string */));
            server = peer;
            reader.Recycle();
    }

    private void SendLogToServer(NetDataWriter writer)
    {
        server.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    private IEnumerator PollEvents()
    {
        while (true)
        {
            client.PollEvents();
            yield return new WaitForSeconds(.015f);
        }
        
        client.Stop();
    }

    private IEnumerator SendPlayerData()
    {
        yield return new WaitForSeconds(3);
        var playerState = new PlayerState();
        playerState.UserName = "Dylan";
        playerState.WorldPosition = new TerrainBattlesCore.Math.Point3();
        playerState.WorldPosition.x = 1f;
        playerState.WorldPosition.y = 5f;
        playerState.WorldPosition.z = 7f;
        NetDataWriter writer = new NetDataWriter();
        PlayerState.Serialize(writer, playerState);
        SendLogToServer(writer);

    }
}
