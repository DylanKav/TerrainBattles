using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ConsoleServer.Properties.Managers;
using LiteNetLib;
using LiteNetLib.Utils;
using TerrainBattlesCore.Core;
using TerrainBattlesCore.Net;

namespace ConsoleServer
{
    internal class Program
    {
        static EventBasedNetListener listener = new EventBasedNetListener();
        static NetManager server = new NetManager(listener);

        private static PlayerManager _playerManager = new PlayerManager(30000); //make this int value public so that people can change the tick seconds...;
        

        private static void Main(string[] args)
        {
            server.Start(7777);
            Console.WriteLine("Server started...");
            listener.ConnectionRequestEvent += request =>
            {
                Console.WriteLine("Received Request");
                if(server.ConnectedPeersCount < 10)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += OnPlayerConnected;
            listener.PeerDisconnectedEvent += OnPeerDisconnected;
            listener.NetworkReceiveEvent += OnEventReceive;
            
            _playerManager.PlayerStateChanged += OnPlayerStateChanged;
            _playerManager.PlayerPositionChanged += OnPlayerPositionChanged;
            
            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();
        }

        private static void OnPlayerStateChanged(PlayerState packet)
        {
            NetDataWriter writer = new NetDataWriter();
            PlayerState.Serialize(writer, packet);
            foreach (var player in _playerManager.Players)
            {
                player.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
        
        private static void OnPlayerPositionChanged(PlayerPosition packet)
        {
            NetDataWriter writer = new NetDataWriter();
            PlayerPosition.Serialize(writer, packet);
            foreach (var player in _playerManager.Players)
            {
                player.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        private static void OnPlayerConnected(NetPeer peer)
        {

            Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put((int)999); //handshake code!
            peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
            _playerManager.PlayerConnected(peer);
           
        }

        private static void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            Console.WriteLine("Player disconnected : {0}", peer.EndPoint);
            _playerManager.PlayerDisconnected(peer);
        }

        private static void OnEventReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
        {
            ProcessPacket(reader, peer);
            reader.Recycle();
        }

        private static void ProcessPacket(NetDataReader reader, NetPeer peer)
        {
            var header = (PacketType)reader.GetInt();
            
            if (header == PacketType.PlayerState)
            {
                var state = PlayerState.Deserialize(reader);
                _playerManager.ReceivePlayerState(state, peer);
            }
            else if (header == PacketType.PlayerPosition)
            {
                var state = PlayerPosition.Deserialize(reader);
                _playerManager.ReceivePlayerPosition(state);
            }
            else if (header == PacketType.DebugMessage)
            {
                var msg = DebugMessage.Deserialize(reader);
                if (msg.MessageType == DebugMessageType.Error) Console.ForegroundColor = ConsoleColor.Red;
                if (msg.MessageType == DebugMessageType.Warning) Console.ForegroundColor = ConsoleColor.Yellow;
                if (msg.MessageType == DebugMessageType.Log) Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(msg.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}