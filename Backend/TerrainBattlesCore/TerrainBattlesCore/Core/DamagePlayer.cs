using LiteNetLib.Utils;
using TerrainBattlesCore.Net;

namespace TerrainBattlesCore.Core
{
    public struct DamagePlayer
    {
        public string UserName;
        public bool IsAccumulativeDamage;
        public bool StopPlayerRegenStamina;
        public int DamageIterations;
        public int DamageValue;

        DamagePlayer(string userName, int damageValue)
        {
            UserName = userName;
            DamageValue = damageValue;
            IsAccumulativeDamage = false;
            StopPlayerRegenStamina = false;
            DamageIterations = 0;
        }

        DamagePlayer(string userName, int damageValue, bool isAccumulativeDamage, bool stopPlayerRegenStamina,
            int damageIterations)
        {
            UserName = userName;
            DamageValue = damageValue;
            IsAccumulativeDamage = isAccumulativeDamage;
            StopPlayerRegenStamina = stopPlayerRegenStamina;
            DamageIterations = damageIterations;
        }
        
        public static void Serialize(NetDataWriter writer, DamagePlayer state)
        {
            writer.Put((int)PacketType.DamagePlayer); //header
            writer.Put(state.UserName);
            writer.Put(state.IsAccumulativeDamage);
            writer.Put(state.StopPlayerRegenStamina);
            writer.Put(state.DamageIterations);
            writer.Put(state.DamageValue);

        }

        public static DamagePlayer Deserialize(NetDataReader reader)
        {
            DamagePlayer state = new DamagePlayer();
            state.UserName = reader.GetString();
            state.IsAccumulativeDamage = reader.GetBool();
            state.StopPlayerRegenStamina = reader.GetBool();
            state.DamageIterations = reader.GetInt();
            state.DamageValue = reader.GetInt();
            return state;
        }
    }
}