using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using LiteNetLib;
using TerrainBattlesCore.Core;

namespace ConsoleServer.Properties.Managers
{
    public class PlayerManager
    {
        public List<NetPlayer> Players = new List<NetPlayer>();

        public delegate void UpdatePlayerPosition(PlayerPosition packet);
        public delegate void UpdatePlayerState(PlayerState packet);
        
        //events for server to update clients.
        public event UpdatePlayerState PlayerStateChanged;
        public event UpdatePlayerPosition PlayerPositionChanged;

        public PlayerManager(int tickSeconds)
        {
            
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(PlayerStatTick);
            aTimer.Interval = tickSeconds;
            aTimer.Enabled = true;
        }

        private void PlayerStatTick(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Player Statistic Tick");
            foreach (var player in Players)
            {
                player.State.TickHungerAndThirst();
                player.State.TickHealth();
                PlayerStateChanged?.Invoke(player.State);
            }
        }

        public void PlayerConnected(NetPeer peer)
        {
            var player = new NetPlayer();
            player.Peer = peer;
            Players.Add(player);
        }

        public void PlayerDisconnected(NetPeer peer)
        {
            for (var index = 0; index < Players.Count; index++)
            {
                var player = Players[index];
                if (peer == player.Peer){ Console.WriteLine("Player: " + player.State.UserName + " is disconnecting."); Players.RemoveAt(index);}
            }
        }
        
        public void ReceivePlayerState(PlayerState state, NetPeer peer)
        {
            for (var index = 0; index < Players.Count; index++)
            {
                var player = Players[index];
                if (player.Peer == peer)
                {
                    player.State = state;
                    Players[index] = player;
                    PlayerStateChanged?.Invoke(state);
                }
            }
        }

        public void ReceivePlayerPosition(PlayerPosition positionChanges)
        {
            for (var index = 0; index < Players.Count; index++)
            {
                var player = Players[index];
                try
                {
                    if (player.State.UserName == positionChanges.UserName)
                    {
                        if (player.State.WorldPosition == positionChanges.WorldPosition && player.State.WorldRotation == positionChanges.WorldRotation) return;
                        player.State.WorldPosition = positionChanges.WorldPosition;
                        player.State.AnimationState = positionChanges.AnimationState;
                        Players[index] = player;
                        PlayerPositionChanged?.Invoke(positionChanges);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }

    public struct NetPlayer
    {
        public NetPeer Peer;
        public PlayerState State;
    }
}