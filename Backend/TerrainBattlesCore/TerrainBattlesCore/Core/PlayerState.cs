using System;
using LiteNetLib.Utils;
using TerrainBattlesCore.Math;
using TerrainBattlesCore.Net;

namespace TerrainBattlesCore.Core
{
    [Serializable]
    public class PlayerState
    {
        public string UserName;
        public Point3 WorldPosition;
        public Point3 WorldRotation;

        private int _health, _stamina, _hex;
        
        //max stats
        public int MaxHealth;
        public int MaxStamina;
        public int MaxHex;

        //tick multipliers
        public int Hunger;
        public int Thirst;

        public PlayerState(){}

        public PlayerState(string userName, Point3 worldPosition)
        {
            MaxHealth = MaxStamina = MaxHex = CurrentHealth = CurrentStamina = CurrentHex = Hunger = Thirst = 100;
            UserName = userName;
            WorldPosition = worldPosition;
        }

        public int CurrentHealth
        {
            get => _health;
            set
            {
                _health = value >= MaxHealth ? MaxHealth : value;
                if (value <= 0)
                {
                    //DEAD
                }
            }
        }
        
        public int CurrentStamina
        {
            get => _stamina;
            set => _stamina = value >= MaxStamina ? MaxStamina : value;
        }
        
        public int CurrentHex
        {
            get => _hex;
            set => _hex = value >= MaxHex ? MaxHex : value;
        }

        public void TickHealth()
        {
            var multiplier = ((int)(Hunger + Thirst) / 2) / 100;
            if (multiplier == 0) multiplier = 1;
            if (Hunger == 0 || Thirst == 0) multiplier *= -1;
            CurrentHealth += multiplier * 5;
        }

        public void TickHungerAndThirst()
        {
            var multiplier = 1; //I want to do something with this for temperature of biome etc
            Hunger -= multiplier * 2;
            Thirst -= multiplier;
        }

        public void EatFood(int value)
        {
            Hunger += value;
            if (Hunger > 100) Hunger = 100;
        }

        public void Drink(int value)
        {
            Thirst += value;
            if (Thirst > 100) Thirst = 100;
        }
        

        public static bool operator ==(PlayerState c1, PlayerState c2)
        {
            return c1.UserName == c2.UserName;
        }

        public static bool operator !=(PlayerState c1, PlayerState c2) 
        {
            return !(c1.UserName == c2.UserName);
        }

        public static void Serialize(NetDataWriter writer, PlayerState state)
        {
            writer.Put((int)PacketType.PlayerState); //header
            
            writer.Put(state.UserName);
            writer.Put(state.WorldPosition.x);
            writer.Put(state.WorldPosition.y);
            writer.Put(state.WorldPosition.z);
            writer.Put(state.WorldRotation.x);
            writer.Put(state.WorldRotation.y);
            writer.Put(state.WorldRotation.z);
            writer.Put(state.MaxHealth);
            writer.Put(state.MaxStamina);
            writer.Put(state.MaxHex);
            writer.Put(state.CurrentHealth);
            writer.Put(state.CurrentStamina);
            writer.Put(state.CurrentHex);
            writer.Put(state.Hunger);
            writer.Put(state.Thirst);
        }

        public static PlayerState Deserialize(NetDataReader reader)
        {
            PlayerState state = new PlayerState();
            state.UserName = reader.GetString();
            state.WorldPosition.x = reader.GetFloat();
            state.WorldPosition.y = reader.GetFloat();
            state.WorldPosition.z = reader.GetFloat();
            state.WorldRotation.x = reader.GetFloat();
            state.WorldRotation.y = reader.GetFloat();
            state.WorldRotation.z = reader.GetFloat();
            state.MaxHealth = reader.GetInt();
            state.MaxStamina = reader.GetInt();
            state.MaxHex = reader.GetInt();
            state.CurrentHealth = reader.GetInt();
            state.CurrentStamina = reader.GetInt();
            state.CurrentHex = reader.GetInt();
            state.Hunger = reader.GetInt();
            state.Thirst = reader.GetInt();
            return state;
        }
        
        protected bool Equals(PlayerState other)
        {
            return UserName == other.UserName && WorldPosition.Equals(other.WorldPosition) && WorldRotation.Equals(other.WorldRotation) && _health == other._health && _stamina == other._stamina && _hex == other._hex && MaxHealth == other.MaxHealth && MaxStamina == other.MaxStamina && MaxHex == other.MaxHex && Hunger == other.Hunger && Thirst == other.Thirst;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayerState)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ WorldPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ WorldRotation.GetHashCode();
                hashCode = (hashCode * 397) ^ _health;
                hashCode = (hashCode * 397) ^ _stamina;
                hashCode = (hashCode * 397) ^ _hex;
                hashCode = (hashCode * 397) ^ MaxHealth;
                hashCode = (hashCode * 397) ^ MaxStamina;
                hashCode = (hashCode * 397) ^ MaxHex;
                hashCode = (hashCode * 397) ^ Hunger;
                hashCode = (hashCode * 397) ^ Thirst;
                return hashCode;
            }
        }
    }
}