using System;
using LiteNetLib.Utils;
using TerrainBattlesCore.Math;
using TerrainBattlesCore.Net;

namespace TerrainBattlesCore.Core
{
    [Serializable]
    public struct PlayerState
    {
        public string UserName;
        public Point3 WorldPosition;
        
        public static bool operator ==(PlayerState c1, PlayerState c2)
        {
            return c1.UserName == c2.UserName && c1.WorldPosition == c2.WorldPosition;
        }

        public static bool operator !=(PlayerState c1, PlayerState c2) 
        {
            return !(c1.UserName == c2.UserName && c1.WorldPosition == c2.WorldPosition);
        }

        public static void Serialize(NetDataWriter writer, PlayerState state)
        {
            writer.Put((int)PacketType.PlayerState);
            writer.Put(state.UserName);
            writer.Put(state.WorldPosition.x);
            writer.Put(state.WorldPosition.y);
            writer.Put(state.WorldPosition.z);
        }

        public static PlayerState Deserialize(NetDataReader reader)
        {
            PlayerState state = new PlayerState();
            state.UserName = reader.GetString();
            state.WorldPosition.x = reader.GetFloat();
            state.WorldPosition.y = reader.GetFloat();
            state.WorldPosition.z = reader.GetFloat();
            return state;
        }
    }
}