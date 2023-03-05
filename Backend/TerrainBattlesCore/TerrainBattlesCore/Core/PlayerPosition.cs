using System;
using LiteNetLib.Utils;
using TerrainBattlesCore.Math;
using TerrainBattlesCore.Net;

namespace TerrainBattlesCore.Core
{
    [Serializable]
    public struct PlayerPosition
    {
        public string UserName;
        public Point3 WorldPosition;
        public Point3 WorldRotation;
        
        public static bool operator ==(PlayerPosition c1, PlayerPosition c2)
        {
            return c1.UserName == c2.UserName && c1.WorldPosition == c2.WorldPosition && c1.WorldRotation == c2.WorldRotation;
        }

        public static bool operator !=(PlayerPosition c1, PlayerPosition c2) 
        {
            return !(c1.UserName == c2.UserName && c1.WorldPosition == c2.WorldPosition && c1.WorldRotation == c2.WorldRotation);
        }

        public static void Serialize(NetDataWriter writer, PlayerPosition position)
        {
            writer.Put((int)PacketType.PlayerPosition);
            writer.Put(position.UserName);
            writer.Put(position.WorldPosition.x);
            writer.Put(position.WorldPosition.y);
            writer.Put(position.WorldPosition.z);
            writer.Put(position.WorldRotation.x);
            writer.Put(position.WorldRotation.y);
            writer.Put(position.WorldRotation.z);
        }

        public static PlayerPosition Deserialize(NetDataReader reader)
        {
            PlayerPosition position = new PlayerPosition();
            position.UserName = reader.GetString();
            position.WorldPosition.x = reader.GetFloat();
            position.WorldPosition.y = reader.GetFloat();
            position.WorldPosition.z = reader.GetFloat();
            position.WorldRotation.x = reader.GetFloat();
            position.WorldRotation.y = reader.GetFloat();
            position.WorldRotation.z = reader.GetFloat();
            return position;
        }
        
        public bool Equals(PlayerPosition other)
        {
            return UserName == other.UserName && WorldPosition.Equals(other.WorldPosition) && WorldRotation.Equals(other.WorldRotation);
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ WorldPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ WorldRotation.GetHashCode();
                return hashCode;
            }
        }
    }
}