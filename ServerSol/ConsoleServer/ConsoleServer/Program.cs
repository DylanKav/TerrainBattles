using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        

        static void Main(string[] args)
        {
            server.Start(9050 /* port */);
            listener.ConnectionRequestEvent += request =>
            {
                if(server.ConnectedPeersCount < 10 /* max connections */)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };
            
            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
                NetDataWriter writer = new NetDataWriter();                 // Create writer class
                writer.Put("Hello, you're connected to the server!");                                // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
            };

            listener.NetworkReceiveEvent += OnEventReceive;
            
            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();
        }

        private static void OnEventReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
        {
            ProcessPacket(reader);
            reader.Recycle();
        }

        private static void ProcessPacket(NetDataReader reader)
        {
            var header = (PacketType)reader.GetInt();
            
            if (header == PacketType.PlayerState)
            {
                var state = PlayerState.Deserialize(reader);
                Console.WriteLine(state.UserName);
                Console.WriteLine(state.WorldPosition.x + ", " + state.WorldPosition.y + ", " + state.WorldPosition.z);
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