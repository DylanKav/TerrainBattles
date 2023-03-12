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
    
    [Range(.1f, 1f)]
    [SerializeField] private float lerpAmount = .5f;

    [SerializeField] private bool viewOwnPlayer = true;


    private void Awake()
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
            newNode.AnimationController = newPlayerObj.GetComponent<AnimationController>();
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
                player.State.AnimationState = packet.AnimationState;
            }
        }
    }

    private void OnPlayerStateChanged(PlayerState packet)
    {
        CheckPlayerExists(packet.UserName);
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

    private void FixedUpdate()
    {
        foreach (var player in players)
        {
            if (player.UserName == client.PlayerUserName && !viewOwnPlayer) continue;
            var state = player.State;
            player.Player.transform.position = Vector3.Lerp(player.Player.transform.position, new Vector3(state.WorldPosition.x, state.WorldPosition.y,
                state.WorldPosition.z), lerpAmount);
            player.Player.transform.rotation = Quaternion.Lerp(player.Player.transform.rotation, Quaternion.Euler(state.WorldRotation.x, state.WorldRotation.y,
                state.WorldRotation.z), lerpAmount);
            player.AnimationController.SetAnimationState(player.State.AnimationState);
        }
    }
}

[Serializable]
struct PlayerNode
{
    public string UserName;
    public GameObject Player;
    public PlayerState State;
    public AnimationController AnimationController;
}
