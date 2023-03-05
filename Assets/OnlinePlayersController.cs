using System;
using System.Collections;
using System.Collections.Generic;
using TerrainBattlesCore.Core;
using UnityEngine;

public class OnlinePlayersController : MonoBehaviour
{
    [SerializeField] private GameObject OnlinePlayer;
    [SerializeField]List<PlayerNode> players = new List<PlayerNode>();
    [SerializeField] private NetClient client;


    private void Start()
    {
        client.PlayerPositionChanged += OnPlayerPositionChanged;
        client.PlayerStateChanged += OnPlayerStateChanged;
    }

    private void CheckPlayerExists(string userName)
    {
        bool found = false;
        foreach (var player in players)
        {
            if (player.UserName == userName) found = true;
        }

        if (!found)
        {
            var newNode = new PlayerNode();
            var newPlayerObj = Instantiate(OnlinePlayer, this.transform);
            newNode.State = new PlayerState();
            newNode.UserName = userName;
            newNode.Player = newPlayerObj;
            players.Add(newNode);
        }
    }

    private void OnPlayerPositionChanged(PlayerPosition packet)
    {
        CheckPlayerExists(packet.UserName);
        foreach (var player in players)
        {
            if (player.UserName == packet.UserName)
            {
                player.State.WorldPosition = packet.WorldPosition;
                player.State.WorldRotation = packet.WorldRotation;
            }
        }
    }

    private void OnPlayerStateChanged(PlayerState packet)
    {
        for (var index = 0; index < players.Count; index++)
        {
            var player = players[index];
            if (player.UserName == packet.UserName)
            {
                player.State = packet;
                players[index] = player;
            }
        }
    }

    private void Update()
    {
        foreach (var player in players)
        {
            var state = player.State;
            player.Player.transform.position = Vector3.Lerp(player.Player.transform.position, new Vector3(state.WorldPosition.x, state.WorldPosition.y,
                state.WorldPosition.z), .9f);
            player.Player.transform.rotation = Quaternion.Lerp(player.Player.transform.rotation, Quaternion.Euler(state.WorldRotation.x, state.WorldRotation.y,
                state.WorldRotation.z), .9f);
        }
    }
}

[Serializable]
struct PlayerNode
{
    public string UserName;
    public GameObject Player;
    public PlayerState State;
}
