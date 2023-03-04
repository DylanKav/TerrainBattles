using System;
using LiteNetLib.Utils;
using TerrainBattlesCore.Net;

namespace TerrainBattlesCore.Core
{
    [Serializable]
    public struct DebugMessage
    {
        public DebugMessageType MessageType;
        public string Message;
        
        public static void Serialize(NetDataWriter writer, DebugMessage msg)
        {
            writer.Put((int)PacketType.DebugMessage);
            writer.Put((int)msg.MessageType);
            writer.Put(msg.Message);
        }

        public static DebugMessage Deserialize(NetDataReader reader)
        {
            DebugMessage msg = new DebugMessage();
            msg.MessageType = (DebugMessageType)reader.GetInt();
            msg.Message = reader.GetString();
            return msg;
        }
    }

    public enum DebugMessageType
    {
        Warning,
        Error,
        Log
    }
}